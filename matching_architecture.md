# JD Matching Architecture

## Inputs

The matching API accepts both the legacy flattened payload and the Line 3 payload:

```json
{
  "candidate": {},
  "repositoryAnalysis": {},
  "trustScore": {},
  "jobDescription": {}
}
```

Legacy fields such as `normalizedJd`, `candidateSkills`, `candidateResponsibilities`, `candidateLevel`, and salary fields are still supported for compatibility.

## Evidence Extraction

The deterministic matcher uses evidence only from supplied data:
- Candidate skill arrays from `candidateSkills`, `candidate.skills`, `candidate.technicalSkills`, `repositoryAnalysis.skills`, `repositoryAnalysis.detectedSkills`, `repositoryAnalysis.languages`, and `repositoryAnalysis.frameworks`.
- Project and responsibility evidence from `candidate.responsibilities`, `candidate.experience`, `candidate.projects`, `repositoryAnalysis.projectEvidence`, `repositoryAnalysis.evidence`, `repositoryAnalysis.projects`, and `repositoryAnalysis.summary`.
- Trust score from `trustScore.overallScore`, `trustScore.trustScore`, `trustScore.score`, or `trustScore.trustWeightedScore`.

No skills or experience are inferred when they are absent from the request.

## Scoring

Final match score uses the requested Line 3 weighting:

```text
Skills                35%
Projects              25%
Experience            15%
Contribution Quality  10%
Trust Score           10%
Risk Analysis          5%
```

Component mapping:
- Skills: required and preferred skill coverage, with required skills weighted higher.
- Projects: responsibility coverage from repository/candidate evidence.
- Experience: candidate level versus JD seniority.
- Contribution Quality: `repositoryAnalysis.contributionQuality`, `repositoryAnalysis.contributionQualityScore`, or `repositoryAnalysis.qualityScore`, defaulting to neutral evidence.
- Trust Score: normalized trust score from the request, defaulting to neutral evidence.
- Risk Analysis: salary hard mismatch, insufficient skills, and large seniority gaps reduce risk score.

## Outputs

The API returns the existing detailed response plus the requested Line 3 fields:
- `overallMatch`
- `skillMatch`
- `experienceMatch`
- `projectRelevance`
- `trustWeightedScore`
- `strengths`
- `weaknesses`
- `missingSkills`
- `recommendation`
- `riskLevel`
- `riskAssessment`
- `evidence`

## AI Pipeline Alignment

The Python Line 3 prompt factory now validates the expanded structured JD fields and documents the same scoring weights. The backend deterministic matcher acts as a reliable fallback and a testable implementation of the same contract.
