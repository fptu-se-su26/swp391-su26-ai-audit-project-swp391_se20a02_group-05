"""
Offline tests for Line 3 — JD Matching Pipeline.

Covers deterministic task dispatch and application quality gate behavior.
"""

import asyncio
import json
import os
import sys
import unittest
from unittest.mock import AsyncMock

sys.path.insert(0, os.path.abspath(os.path.join(os.path.dirname(__file__), "..")))

os.environ.setdefault("ANTHROPIC_API_KEY", "dummy-key")
os.environ.setdefault("SHARED_SECRET", "dummy-secret")
os.environ.setdefault("BACKEND_API_URL", "http://mock-backend:8080")


class TestLine3TaskDispatch(unittest.TestCase):
    def test_all_spreadsheet_line3_task_aliases_are_recognized(self):
        from app.pipelines.jd.orchestrator import is_line3_task

        for task_id in [f"L3-{idx:03d}" for idx in range(2, 16)]:
            with self.subTest(task_id=task_id):
                self.assertTrue(is_line3_task(task_id), f"{task_id} should be routable")

    def test_application_quality_gate_alias_executes(self):
        from app.pipelines.jd.orchestrator import JdMatchingOrchestrator

        async def run():
            orchestrator = JdMatchingOrchestrator()
            return await orchestrator.execute_task(
                task_type="L3-014",
                job_id="job-line3",
                inputs={
                    "cappedMatchScorePercent": 82.0,
                    "salaryMatchScore": 1.0,
                    "skillMatchScore": 0.8,
                    "levelGap": 0,
                    "gapSeverity": "none",
                },
            )

        result = asyncio.run(run())
        self.assertEqual(result["status"], "Completed")
        self.assertIn("ApplicationQualityGate", result["events"][0]["message"])


class TestApplicationQualityGate(unittest.TestCase):
    def test_clear_gate_requires_no_confirmation(self):
        from app.pipelines.jd.orchestrator import JdMatchingOrchestrator

        async def run():
            orchestrator = JdMatchingOrchestrator()
            return await orchestrator.execute_task(
                task_type="ApplicationQualityGate",
                job_id="job-clear",
                inputs={
                    "cappedMatchScorePercent": 88.0,
                    "salaryMatchScore": 1.0,
                    "skillMatchScore": 0.9,
                    "levelGap": 0,
                    "gapSeverity": "none",
                },
            )

        result = asyncio.run(run())
        data = json.loads(result["resultData"])
        self.assertEqual(result["status"], "Completed")
        self.assertEqual(data["qualityGateStatus"], "clear")
        self.assertFalse(data["requiresExplicitConfirmation"])
        self.assertTrue(data["canApply"])

    def test_risky_gate_collects_confirmation_reasons(self):
        from app.pipelines.jd.orchestrator import JdMatchingOrchestrator

        async def run():
            orchestrator = JdMatchingOrchestrator()
            return await orchestrator.execute_task(
                task_type="ApplicationQualityGate",
                job_id="job-risky",
                inputs={
                    "cappedMatchScorePercent": 60.0,
                    "salaryMatchScore": 0.0,
                    "skillMatchScore": 0.32,
                    "levelGap": 2,
                    "seniorityFlag": "strongly_underqualified",
                    "gapSeverity": "critical",
                    "missingRequiredSkills": ["GraphQL"],
                    "activeFlags": ["SALARY_HARD_MISMATCH: Score capped at 60%"],
                },
            )

        result = asyncio.run(run())
        data = json.loads(result["resultData"])
        self.assertEqual(result["status"], "Completed")
        self.assertEqual(data["qualityGateStatus"], "requires_confirmation")
        self.assertTrue(data["requiresExplicitConfirmation"])
        self.assertIn("salary_mismatch", data["confirmationRequiredReasons"])
        self.assertIn("skill_gap", data["confirmationRequiredReasons"])
        self.assertIn("seniority_gap", data["confirmationRequiredReasons"])
        self.assertIn("gap_analysis", data["confirmationRequiredReasons"])
        self.assertTrue(data["canApply"])


class TestDeterministicLine3Calculators(unittest.TestCase):
    def test_salary_match_calculator_covers_perfect_negotiable_and_hard_mismatch(self):
        from app.pipelines.jd.orchestrator import JdMatchingOrchestrator

        async def run(inputs):
            orchestrator = JdMatchingOrchestrator()
            result = await orchestrator.execute_task(
                task_type="SalaryMatchCalculator",
                job_id="job-salary",
                inputs=inputs,
            )
            return json.loads(result["resultData"])

        perfect = asyncio.run(run({"desiredSalary": 2500, "minAcceptableSalary": 2000, "jdSalaryMax": 3000}))
        negotiable = asyncio.run(run({"desiredSalary": 3500, "minAcceptableSalary": 2800, "jdSalaryMax": 3000}))
        hard = asyncio.run(run({"desiredSalary": 4500, "minAcceptableSalary": 4000, "jdSalaryMax": 3000}))

        self.assertEqual(perfect["salaryMatchScore"], 1.0)
        self.assertEqual(perfect["salaryMatchType"], "perfect")
        self.assertEqual(negotiable["salaryMatchScore"], 0.6)
        self.assertEqual(negotiable["salaryMatchType"], "negotiable")
        self.assertEqual(hard["salaryMatchScore"], 0.0)
        self.assertTrue(hard["isHardMismatch"])

    def test_match_score_aggregator_uses_weighted_formula(self):
        from app.pipelines.jd.orchestrator import JdMatchingOrchestrator

        async def run():
            orchestrator = JdMatchingOrchestrator()
            return await orchestrator.execute_task(
                task_type="MatchScoreAggregator",
                job_id="job-aggregate",
                inputs={
                    "skillMatchScore": 0.8,
                    "responsibilityMatchScore": 0.6,
                    "seniorityMatchScore": 1.0,
                    "salaryMatchScore": 0.6,
                    "cultureFitScore": 0.5,
                },
            )

        result = asyncio.run(run())
        data = json.loads(result["resultData"])

        self.assertEqual(data["matchScore"], 0.74)
        self.assertEqual(data["matchScorePercent"], 74.0)
        self.assertEqual(data["matchLabel"], "Good Match")

    def test_cap_rule_caps_salary_mismatch_and_flags_skill_gap(self):
        from app.pipelines.jd.orchestrator import JdMatchingOrchestrator

        async def run():
            orchestrator = JdMatchingOrchestrator()
            return await orchestrator.execute_task(
                task_type="MatchScoreCapRule",
                job_id="job-cap",
                inputs={
                    "matchScore": 0.9,
                    "matchScorePercent": 90.0,
                    "salaryMatchScore": 0.0,
                    "skillMatchScore": 0.2,
                    "levelGap": 2,
                    "seniorityFlag": "strongly_underqualified",
                },
            )

        result = asyncio.run(run())
        data = json.loads(result["resultData"])

        self.assertTrue(data["capApplied"])
        self.assertEqual(data["cappedMatchScorePercent"], 60.0)
        self.assertTrue(any("SALARY_HARD_MISMATCH" in flag for flag in data["activeFlags"]))
        self.assertTrue(any("INSUFFICIENT_SKILLS" in flag for flag in data["activeFlags"]))
        self.assertTrue(any("SENIORITY_GAP" in flag for flag in data["activeFlags"]))


class TestHiringRecommendationRules(unittest.TestCase):
    def test_hard_salary_mismatch_forces_no_verdict_even_above_50_percent(self):
        from app.pipelines.jd.orchestrator import JdMatchingOrchestrator

        async def run():
            orchestrator = JdMatchingOrchestrator()
            orchestrator.claude_service.analyze_repo_with_telemetry = AsyncMock(
                return_value=('{"verdict": "Conditional", "confidence": 0.9}', None)
            )
            return await orchestrator.execute_task(
                task_type="HiringRecommendationGenerator",
                job_id="job-hiring-rule",
                inputs={
                    "matchScorePercent": 60.0,
                    "salaryMatchScore": 0.0,
                    "gapSeverity": "critical",
                },
            )

        result = asyncio.run(run())
        data = json.loads(result["resultData"])
        self.assertEqual(result["status"], "Completed")
        self.assertEqual(data["verdict"], "No")


if __name__ == "__main__":
    unittest.main()
