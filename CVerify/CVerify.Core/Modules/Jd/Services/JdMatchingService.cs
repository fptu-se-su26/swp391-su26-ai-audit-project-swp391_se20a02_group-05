using CVerify.API.Modules.Jd.DTOs;

namespace CVerify.API.Modules.Jd.Services;

public sealed class JdMatchingService : IJdMatchingService
{
    private static readonly Dictionary<string, string> SkillAliases = new(StringComparer.OrdinalIgnoreCase)
    {
        ["reactjs"] = "react",
        ["react.js"] = "react",
        ["nodejs"] = "node.js",
        ["node"] = "node.js",
        ["postgres"] = "postgresql",
        ["postgresql"] = "postgresql",
        ["js"] = "javascript",
        ["ts"] = "typescript",
        ["dotnet"] = ".net",
        ["asp.net core"] = ".net",
        ["spring framework"] = "spring boot",
        ["spring"] = "spring boot"
    };

    private static readonly Dictionary<string, int> SeniorityLevels = new(StringComparer.OrdinalIgnoreCase)
    {
        ["Junior"] = 1,
        ["L1"] = 1,
        ["Middle"] = 2,
        ["Mid"] = 2,
        ["L2"] = 2,
        ["Senior"] = 3,
        ["L3"] = 3,
        ["Staff"] = 4,
        ["L4"] = 4,
        ["Principal"] = 5,
        ["L5"] = 5
    };

    public MatchScoreResponse CalculateMatch(JdMatchRequest request)
    {
        ArgumentNullException.ThrowIfNull(request);
        var jd = request.JobDescription ?? request.NormalizedJd;
        ArgumentNullException.ThrowIfNull(jd);

        var requiredSkills = jd.RequiredSkills ?? [];
        var preferredSkills = jd.PreferredSkills ?? [];
        var jdResponsibilities = jd.Responsibilities ?? [];
        var candidateSkills = ExtractCandidateSkills(request);
        var candidateResponsibilities = ExtractCandidateResponsibilities(request);

        var requiredMatches = MatchSkills(requiredSkills, candidateSkills);
        var preferredMatches = MatchSkills(preferredSkills, candidateSkills);
        var missingRequired = requiredMatches.Where(m => !m.Matched).Select(m => m.Skill).ToList();

        var requiredScore = ScoreSkillItems(requiredMatches);
        var preferredScore = preferredMatches.Count == 0 ? 1m : ScoreSkillItems(preferredMatches);
        var skillMatchScore = Clamp((requiredScore * 0.75m) + (preferredScore * 0.25m));

        var uncoveredResponsibilities = FindUncoveredResponsibilities(
            jdResponsibilities,
            candidateResponsibilities);
        var responsibilityMatchScore = jdResponsibilities.Count == 0
            ? 1m
            : Clamp((jdResponsibilities.Count - uncoveredResponsibilities.Count) /
                (decimal)jdResponsibilities.Count);

        var (seniorityScore, seniorityFlag, levelGap) = CalculateSeniority(request.CandidateLevel, jd.Seniority);
        var (salaryScore, salaryType) = CalculateSalary(
            request.DesiredSalary ?? 0m,
            request.MinimumAcceptableSalary ?? 0m,
            jd.SalaryMax,
            request.SalaryCurrency,
            jd.Currency);
        var cultureScore = CalculateCultureFit(request, jd);
        var trustScore = ExtractScore(request.TrustScore, ["overallScore", "trustScore", "score", "trustWeightedScore"]);
        var contributionQualityScore = ExtractScore(request.RepositoryAnalysis, ["contributionQuality", "contributionQualityScore", "qualityScore"]);
        var riskScore = CalculateRiskScore(salaryScore, skillMatchScore, levelGap);

        var matchScore = Clamp(
            skillMatchScore * 0.35m
            + responsibilityMatchScore * 0.25m
            + seniorityScore * 0.15m
            + contributionQualityScore * 0.10m
            + trustScore * 0.10m
            + riskScore * 0.05m);
        var matchScorePercent = Math.Round(matchScore * 100m, 1);

        var activeFlags = new List<string>();
        var cappedScorePercent = matchScorePercent;
        if (salaryScore == 0m)
        {
            cappedScorePercent = Math.Min(cappedScorePercent, 60m);
            activeFlags.Add("SALARY_HARD_MISMATCH");
        }

        if (salaryType == "currency_mismatch")
        {
            activeFlags.Add("CURRENCY_MISMATCH");
        }

        if (skillMatchScore < 0.4m)
        {
            activeFlags.Add("INSUFFICIENT_SKILLS");
        }

        if (Math.Abs(levelGap) >= 2)
        {
            activeFlags.Add("SENIORITY_GAP");
        }

        var gapAnalysis = BuildGapAnalysis(
            missingRequired,
            uncoveredResponsibilities,
            seniorityFlag,
            levelGap,
            salaryType,
            activeFlags);
        var qualityGate = BuildQualityGate(cappedScorePercent, activeFlags, missingRequired);
        var recommendation = BuildHiringRecommendation(cappedScorePercent, salaryScore, gapAnalysis, activeFlags);
        var strengths = BuildStrengths(requiredMatches, preferredMatches, candidateResponsibilities, jdResponsibilities, trustScore);
        var weaknesses = BuildWeaknesses(missingRequired, uncoveredResponsibilities, seniorityFlag, salaryType);
        var evidence = BuildEvidence(requiredMatches, preferredMatches, candidateResponsibilities, jdResponsibilities, request.RepositoryAnalysis);
        var riskLevel = RiskLevelFor(activeFlags, cappedScorePercent);

        return new MatchScoreResponse(
            MatchScore: matchScore,
            MatchScorePercent: matchScorePercent,
            CappedMatchScorePercent: cappedScorePercent,
            MatchLabel: LabelFor(cappedScorePercent),
            SkillMatchScore: skillMatchScore,
            ResponsibilityMatchScore: responsibilityMatchScore,
            SeniorityMatchScore: seniorityScore,
            SalaryMatchScore: salaryScore,
            CultureFitScore: cultureScore,
            RequiredSkillsMatch: requiredMatches,
            PreferredSkillsMatch: preferredMatches,
            MissingRequiredSkills: missingRequired,
            UncoveredResponsibilities: uncoveredResponsibilities,
            SeniorityFlag: seniorityFlag,
            LevelGap: levelGap,
            SalaryMatchType: salaryType,
            ActiveFlags: activeFlags,
            GapAnalysis: gapAnalysis,
            QualityGate: qualityGate,
            HiringRecommendation: recommendation,
            OverallMatch: cappedScorePercent,
            SkillMatch: Math.Round(skillMatchScore * 100m, 1),
            ExperienceMatch: Math.Round(seniorityScore * 100m, 1),
            ProjectRelevance: Math.Round(responsibilityMatchScore * 100m, 1),
            TrustWeightedScore: Math.Round(trustScore * 100m, 1),
            MissingSkills: missingRequired,
            Strengths: strengths,
            Weaknesses: weaknesses,
            Recommendation: recommendation.Verdict,
            RiskLevel: riskLevel,
            RiskAssessment: gapAnalysis.OverallGapSummary,
            Evidence: evidence);
    }

    private static List<SkillMatchItem> MatchSkills(List<string> jdSkills, List<CandidateSkillEvidence> candidateSkills)
    {
        var candidateByCanonical = candidateSkills
            .Where(s => !string.IsNullOrWhiteSpace(s.Skill))
            .GroupBy(s => CanonicalSkill(s.Skill), StringComparer.OrdinalIgnoreCase)
            .ToDictionary(g => g.Key, g => g.OrderByDescending(s => s.Proficiency).First(), StringComparer.OrdinalIgnoreCase);

        return jdSkills
            .Where(skill => !string.IsNullOrWhiteSpace(skill))
            .Select(skill =>
            {
                var canonical = CanonicalSkill(skill);
                if (candidateByCanonical.TryGetValue(canonical, out var exact))
                {
                    return new SkillMatchItem(skill, true, "exact", NormalizeProficiency(exact.Proficiency), exact.EvidenceStrength ?? "verified");
                }

                var semantic = candidateByCanonical.FirstOrDefault(kvp =>
                    canonical.Contains(kvp.Key, StringComparison.OrdinalIgnoreCase)
                    || kvp.Key.Contains(canonical, StringComparison.OrdinalIgnoreCase));

                if (!string.IsNullOrEmpty(semantic.Key))
                {
                    return new SkillMatchItem(skill, true, "semantic", NormalizeProficiency(semantic.Value.Proficiency) * 0.85m, semantic.Value.EvidenceStrength ?? "semantic");
                }

                return new SkillMatchItem(skill, false, "none", 0m, "none");
            })
            .ToList();
    }

    private static List<CandidateSkillEvidence> ExtractCandidateSkills(JdMatchRequest request)
    {
        var skills = new List<CandidateSkillEvidence>();
        if (request.CandidateSkills is { Count: > 0 })
        {
            skills.AddRange(request.CandidateSkills);
        }

        AddSkillsFromJson(skills, request.Candidate, ["skills", "candidateSkills", "technicalSkills"]);
        AddSkillsFromJson(skills, request.RepositoryAnalysis, ["skills", "detectedSkills", "topSkills", "languages", "frameworks"]);

        return skills
            .Where(skill => !string.IsNullOrWhiteSpace(skill.Skill))
            .GroupBy(skill => CanonicalSkill(skill.Skill), StringComparer.OrdinalIgnoreCase)
            .Select(group => group.OrderByDescending(skill => skill.Proficiency).First())
            .ToList();
    }

    private static List<string> ExtractCandidateResponsibilities(JdMatchRequest request)
    {
        var responsibilities = new List<string>();
        if (request.CandidateResponsibilities is { Count: > 0 })
        {
            responsibilities.AddRange(request.CandidateResponsibilities);
        }

        AddStringsFromJson(responsibilities, request.Candidate, ["responsibilities", "experience", "projects"]);
        AddStringsFromJson(responsibilities, request.RepositoryAnalysis, ["responsibilities", "projectEvidence", "evidence", "projects", "summary"]);

        return responsibilities
            .Where(value => !string.IsNullOrWhiteSpace(value))
            .Distinct(StringComparer.OrdinalIgnoreCase)
            .Take(100)
            .ToList();
    }

    private static void AddSkillsFromJson(List<CandidateSkillEvidence> target, System.Text.Json.JsonElement? source, string[] propertyNames)
    {
        if (source is null || source.Value.ValueKind != System.Text.Json.JsonValueKind.Object) return;

        foreach (var propertyName in propertyNames)
        {
            if (!source.Value.TryGetProperty(propertyName, out var property)) continue;

            if (property.ValueKind == System.Text.Json.JsonValueKind.Array)
            {
                foreach (var item in property.EnumerateArray())
                {
                    if (item.ValueKind == System.Text.Json.JsonValueKind.String)
                    {
                        var skill = item.GetString();
                        if (!string.IsNullOrWhiteSpace(skill)) target.Add(new CandidateSkillEvidence(skill, 3m, "repository"));
                    }
                    else if (item.ValueKind == System.Text.Json.JsonValueKind.Object
                        && item.TryGetProperty("skill", out var skillProp)
                        && skillProp.ValueKind == System.Text.Json.JsonValueKind.String)
                    {
                        var skill = skillProp.GetString();
                        var proficiency = item.TryGetProperty("proficiency", out var proficiencyProp)
                            && proficiencyProp.TryGetDecimal(out var rawProficiency)
                                ? rawProficiency
                                : 3m;
                        var evidence = item.TryGetProperty("evidenceStrength", out var evidenceProp)
                            && evidenceProp.ValueKind == System.Text.Json.JsonValueKind.String
                                ? evidenceProp.GetString()
                                : "repository";
                        if (!string.IsNullOrWhiteSpace(skill)) target.Add(new CandidateSkillEvidence(skill, proficiency, evidence));
                    }
                }
            }
        }
    }

    private static void AddStringsFromJson(List<string> target, System.Text.Json.JsonElement? source, string[] propertyNames)
    {
        if (source is null || source.Value.ValueKind != System.Text.Json.JsonValueKind.Object) return;

        foreach (var propertyName in propertyNames)
        {
            if (!source.Value.TryGetProperty(propertyName, out var property)) continue;

            if (property.ValueKind == System.Text.Json.JsonValueKind.String)
            {
                var value = property.GetString();
                if (!string.IsNullOrWhiteSpace(value)) target.Add(value);
            }
            else if (property.ValueKind == System.Text.Json.JsonValueKind.Array)
            {
                target.AddRange(property.EnumerateArray()
                    .Select(JsonToEvidenceString)
                    .Where(value => !string.IsNullOrWhiteSpace(value))
                    .Select(value => value!));
            }
        }
    }

    private static string? JsonToEvidenceString(System.Text.Json.JsonElement item)
    {
        if (item.ValueKind == System.Text.Json.JsonValueKind.String) return item.GetString();
        if (item.ValueKind != System.Text.Json.JsonValueKind.Object) return null;

        foreach (var propertyName in new[] { "summary", "description", "responsibility", "evidence", "name" })
        {
            if (item.TryGetProperty(propertyName, out var property)
                && property.ValueKind == System.Text.Json.JsonValueKind.String)
            {
                return property.GetString();
            }
        }

        return item.ToString();
    }

    private static decimal ScoreSkillItems(List<SkillMatchItem> items)
    {
        if (items.Count == 0) return 1m;
        return Clamp(items.Sum(i => i.Matched ? NormalizeProficiency(i.CandidateProficiency) : 0m) / items.Count);
    }

    private static List<string> FindUncoveredResponsibilities(List<string> jdResponsibilities, List<string> candidateResponsibilities)
    {
        var candidateTokens = candidateResponsibilities
            .SelectMany(Tokenize)
            .ToHashSet(StringComparer.OrdinalIgnoreCase);

        return jdResponsibilities
            .Where(resp =>
            {
                var tokens = Tokenize(resp).Where(t => t.Length > 3).ToList();
                if (tokens.Count == 0) return false;
                var covered = tokens.Count(candidateTokens.Contains);
                return covered < Math.Max(1, tokens.Count / 3);
            })
            .ToList();
    }

    private static (decimal Score, string Flag, int Gap) CalculateSeniority(string? candidateLevel, string? jdSeniority)
    {
        var candidate = string.IsNullOrWhiteSpace(candidateLevel)
            ? 2
            : SeniorityLevels.GetValueOrDefault(candidateLevel, 2);
        var jd = string.IsNullOrWhiteSpace(jdSeniority)
            ? 2
            : SeniorityLevels.GetValueOrDefault(jdSeniority, 2);
        var gap = candidate - jd;
        return gap switch
        {
            0 => (1.0m, "exact_match", gap),
            -1 => (0.7m, "underqualified", gap),
            <= -2 => (0.3m, "strongly_underqualified", gap),
            1 => (0.85m, "overqualified", gap),
            _ => (0.6m, "strongly_overqualified", gap)
        };
    }

    private static (decimal Score, string Type) CalculateSalary(
        decimal desired,
        decimal minimumAcceptable,
        decimal jdMax,
        string? candidateCurrency,
        string? jdCurrency)
    {
        if (jdMax <= 0m || desired <= 0m) return (1.0m, "no_jd_salary");
        if (!CurrenciesMatch(candidateCurrency, jdCurrency)) return (1.0m, "currency_mismatch");
        if (desired <= jdMax) return (1.0m, "perfect");
        if (minimumAcceptable <= jdMax) return (0.6m, "negotiable");
        return (0.0m, "hard_mismatch");
    }

    private static decimal CalculateCultureFit(JdMatchRequest request, JdFormRequest jd)
    {
        var role = request.CandidateRoleTendency ?? string.Empty;
        var title = jd.JobTitle ?? string.Empty;
        var roleAligned = !string.IsNullOrWhiteSpace(role)
            && title.Contains(role, StringComparison.OrdinalIgnoreCase);
        var workingStyleAligned = request.CandidateWorkingStyles?.Any(style =>
            !string.IsNullOrWhiteSpace(style)
            && !string.IsNullOrWhiteSpace(EffectiveWorkMode(jd))
            && (EffectiveWorkMode(jd).Contains(style, StringComparison.OrdinalIgnoreCase)
                || style.Contains(EffectiveWorkMode(jd), StringComparison.OrdinalIgnoreCase))) == true;

        return (roleAligned, workingStyleAligned) switch
        {
            (true, true) => 1.0m,
            (true, false) => 0.75m,
            (false, true) => 0.65m,
            _ => 0.5m
        };
    }

    private static GapAnalysisResponse BuildGapAnalysis(
        List<string> missingSkills,
        List<string> uncoveredResponsibilities,
        string seniorityFlag,
        int levelGap,
        string salaryType,
        List<string> activeFlags)
    {
        var suggestions = missingSkills
            .Select(skill => $"Build and publish evidence for {skill} through a focused project or production contribution.")
            .Concat(uncoveredResponsibilities.Select(r => $"Add measurable experience covering: {r}"))
            .ToList();

        var severity = activeFlags.Contains("INSUFFICIENT_SKILLS") || activeFlags.Contains("SALARY_HARD_MISMATCH")
            ? "critical"
            : activeFlags.Count > 0 || missingSkills.Count > 0 || uncoveredResponsibilities.Count > 0
                ? "significant"
                : "none";

        var seniorityGap = Math.Abs(levelGap) >= 1 ? $"{seniorityFlag} by {Math.Abs(levelGap)} level(s)" : null;
        var salaryMismatch = salaryType == "hard_mismatch" ? "Candidate minimum acceptable salary exceeds JD maximum." :
            salaryType == "negotiable" ? "Candidate desired salary exceeds JD maximum but minimum acceptable salary fits." :
            salaryType == "currency_mismatch" ? "Candidate salary currency differs from the JD currency, so salary was not scored." : null;

        return new GapAnalysisResponse(
            GapSeverity: severity,
            SkillGaps: missingSkills,
            ResponsibilityGaps: uncoveredResponsibilities,
            SeniorityGap: seniorityGap,
            SalaryMismatch: salaryMismatch,
            ImprovementSuggestions: suggestions,
            OverallGapSummary: severity == "none"
                ? "No material gaps were found for this candidate and JD."
                : $"Found {missingSkills.Count} skill gap(s), {uncoveredResponsibilities.Count} responsibility gap(s), and {activeFlags.Count} screening flag(s).");
    }

    private static ApplicationQualityGateResponse BuildQualityGate(decimal cappedScorePercent, List<string> activeFlags, List<string> missingSkills)
    {
        var reasons = new List<string>();
        var warnings = new List<string>();

        if (activeFlags.Contains("SALARY_HARD_MISMATCH"))
        {
            reasons.Add("salary_mismatch");
            warnings.Add("Salary expectation exceeds the JD maximum budget.");
        }

        if (activeFlags.Contains("INSUFFICIENT_SKILLS") || missingSkills.Count > 0)
        {
            reasons.Add("skill_gap");
            warnings.Add("Candidate is missing required skills or has low verified skill coverage.");
        }

        if (activeFlags.Contains("SENIORITY_GAP"))
        {
            reasons.Add("seniority_gap");
            warnings.Add("Candidate seniority differs from the JD requirement by at least two levels.");
        }

        return new ApplicationQualityGateResponse(
            QualityGateStatus: reasons.Count > 0 ? "requires_confirmation" : "clear",
            CanApply: true,
            RequiresExplicitConfirmation: reasons.Count > 0,
            ConfirmationRequiredReasons: reasons.Distinct().OrderBy(r => r).ToList(),
            Warnings: warnings);
    }

    private static HiringRecommendationResponse BuildHiringRecommendation(
        decimal cappedScorePercent,
        decimal salaryScore,
        GapAnalysisResponse gapAnalysis,
        List<string> activeFlags)
    {
        var verdict = cappedScorePercent >= 75m
            && salaryScore > 0m
            && gapAnalysis.GapSeverity is not "critical" and not "significant"
            ? "Yes"
            : salaryScore > 0m && (cappedScorePercent >= 50m || gapAnalysis.GapSeverity is "minor" or "none")
                ? "Conditional"
                : "No";

        var risk = verdict == "Yes" ? "low" : verdict == "Conditional" ? "medium" : "high";
        var reasons = activeFlags.Count == 0 ? ["Strong score with no active screening flags."] : activeFlags;

        return new HiringRecommendationResponse(
            Verdict: verdict,
            Confidence: activeFlags.Count == 0 ? 0.85m : 0.72m,
            OneParaSummary: $"{verdict} recommendation: candidate scored {cappedScorePercent:0.0}% after rule caps with {gapAnalysis.GapSeverity} gap severity.",
            KeyReasons: reasons,
            HiringRisk: risk);
    }

    private static string LabelFor(decimal pct) => pct switch
    {
        >= 80m => "Strong Match",
        >= 65m => "Good Match",
        >= 50m => "Partial Match",
        >= 35m => "Weak Match",
        _ => "Poor Match"
    };

    private static decimal ExtractScore(System.Text.Json.JsonElement? source, string[] candidatePropertyNames)
    {
        if (source is null) return 0.75m;

        foreach (var propertyName in candidatePropertyNames)
        {
            if (source.Value.ValueKind == System.Text.Json.JsonValueKind.Object
                && source.Value.TryGetProperty(propertyName, out var property)
                && property.ValueKind is System.Text.Json.JsonValueKind.Number
                && property.TryGetDecimal(out var raw))
            {
                return NormalizeProficiency(raw);
            }
        }

        return 0.75m;
    }

    private static decimal CalculateRiskScore(decimal salaryScore, decimal skillMatchScore, int levelGap)
    {
        var riskPenalty = 0m;
        if (salaryScore == 0m) riskPenalty += 0.45m;
        if (skillMatchScore < 0.4m) riskPenalty += 0.35m;
        if (Math.Abs(levelGap) >= 2) riskPenalty += 0.20m;
        return Clamp(1m - riskPenalty);
    }

    private static List<string> BuildStrengths(
        List<SkillMatchItem> requiredMatches,
        List<SkillMatchItem> preferredMatches,
        List<string> candidateResponsibilities,
        List<string> jdResponsibilities,
        decimal trustScore)
    {
        var strengths = requiredMatches
            .Concat(preferredMatches)
            .Where(m => m.Matched)
            .OrderByDescending(m => m.CandidateProficiency)
            .Take(5)
            .Select(m => $"Verified {m.MatchType} evidence for {m.Skill}.")
            .ToList();

        if (jdResponsibilities.Count > 0 && FindUncoveredResponsibilities(jdResponsibilities, candidateResponsibilities).Count == 0)
        {
            strengths.Add("Candidate evidence covers all listed JD responsibilities.");
        }

        if (trustScore >= 0.8m)
        {
            strengths.Add("Trust score is strong enough to support the match confidence.");
        }

        return strengths.Count == 0 ? ["No strong evidence-backed strengths were detected."] : strengths;
    }

    private static List<string> BuildWeaknesses(
        List<string> missingSkills,
        List<string> uncoveredResponsibilities,
        string seniorityFlag,
        string salaryType)
    {
        var weaknesses = missingSkills.Select(skill => $"Missing required skill evidence: {skill}.")
            .Concat(uncoveredResponsibilities.Select(r => $"No direct evidence for responsibility: {r}."))
            .ToList();

        if (seniorityFlag is "underqualified" or "strongly_underqualified")
        {
            weaknesses.Add("Candidate seniority is below the JD target.");
        }

        if (salaryType is "hard_mismatch" or "negotiable")
        {
            weaknesses.Add("Salary expectations may not fit the JD budget.");
        }

        return weaknesses.Count == 0 ? ["No material weaknesses were detected from supplied evidence."] : weaknesses;
    }

    private static List<string> BuildEvidence(
        List<SkillMatchItem> requiredMatches,
        List<SkillMatchItem> preferredMatches,
        List<string> candidateResponsibilities,
        List<string> jdResponsibilities,
        System.Text.Json.JsonElement? repositoryAnalysis)
    {
        var evidence = requiredMatches
            .Concat(preferredMatches)
            .Where(m => m.Matched)
            .Select(m => $"{m.Skill}: {m.EvidenceStrengthForDisplay()} evidence, proficiency {m.CandidateProficiency:0.##}.")
            .Concat(candidateResponsibilities
                .Where(r => jdResponsibilities.Any(jd => !FindUncoveredResponsibilities([jd], [r]).Contains(jd)))
                .Take(5)
                .Select(r => $"Responsibility evidence: {r}"))
            .ToList();

        if (repositoryAnalysis.HasValue
            && repositoryAnalysis.Value.ValueKind == System.Text.Json.JsonValueKind.Object
            && repositoryAnalysis.Value.TryGetProperty("evidence", out var repoEvidence)
            && repoEvidence.ValueKind == System.Text.Json.JsonValueKind.Array)
        {
            evidence.AddRange(repoEvidence.EnumerateArray()
                .Where(item => item.ValueKind == System.Text.Json.JsonValueKind.String)
                .Select(item => item.GetString())
                .Where(value => !string.IsNullOrWhiteSpace(value))
                .Select(value => value!)
                .Take(5));
        }

        return evidence.Count == 0
            ? ["No repository evidence was supplied for matched skills or responsibilities."]
            : evidence.Distinct().ToList();
    }

    private static string RiskLevelFor(List<string> activeFlags, decimal cappedScorePercent)
    {
        if (activeFlags.Contains("SALARY_HARD_MISMATCH") || cappedScorePercent < 50m) return "high";
        return activeFlags.Count > 0 || cappedScorePercent < 75m ? "medium" : "low";
    }

    private static string EffectiveWorkMode(JdFormRequest jd) =>
        string.IsNullOrWhiteSpace(jd.WorkMode) ? jd.WorkingModel : jd.WorkMode;

    private static string CanonicalSkill(string skill)
    {
        var normalized = skill.Trim().ToLowerInvariant();
        return SkillAliases.GetValueOrDefault(normalized, normalized);
    }

    private static IEnumerable<string> Tokenize(string value) =>
        value.Split([' ', ',', '.', '/', '-', '_', '(', ')'], StringSplitOptions.RemoveEmptyEntries)
            .Select(t => t.Trim().ToLowerInvariant());

    private static decimal NormalizeProficiency(decimal proficiency) =>
        proficiency > 1m ? Clamp(proficiency / 5m) : Clamp(proficiency);

    private static bool CurrenciesMatch(string? candidateCurrency, string? jdCurrency)
    {
        if (string.IsNullOrWhiteSpace(candidateCurrency) || string.IsNullOrWhiteSpace(jdCurrency))
        {
            return true;
        }

        return string.Equals(candidateCurrency.Trim(), jdCurrency.Trim(), StringComparison.OrdinalIgnoreCase);
    }

    private static decimal Clamp(decimal value) => Math.Round(Math.Min(1m, Math.Max(0m, value)), 4);
}

file static class SkillMatchItemExtensions
{
    public static string EvidenceStrengthForDisplay(this SkillMatchItem item) =>
        string.IsNullOrWhiteSpace(item.EvidenceStrength) ? "verified" : item.EvidenceStrength;
}
