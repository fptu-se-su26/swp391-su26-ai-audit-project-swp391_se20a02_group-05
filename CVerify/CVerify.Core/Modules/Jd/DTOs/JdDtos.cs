using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace CVerify.API.Modules.Jd.DTOs;

public sealed record JdFormRequest(
    [Required, MinLength(1)]
    [property: JsonPropertyName("jobTitle")] string JobTitle,
    [Required, MinLength(1)]
    [property: JsonPropertyName("seniority")] string Seniority,
    [Required, MinLength(1)]
    [property: JsonPropertyName("requiredSkills")] List<string> RequiredSkills,
    [property: JsonPropertyName("preferredSkills")] List<string>? PreferredSkills,
    [Required]
    [property: JsonPropertyName("responsibilities")] List<string> Responsibilities,
    [property: JsonPropertyName("experienceYearsMin")] int ExperienceYearsMin,
    [property: JsonPropertyName("experienceYearsMax")] int ExperienceYearsMax,
    [property: JsonPropertyName("educationRequirement")] string EducationRequirement,
    [property: JsonPropertyName("englishLevel")] string EnglishLevel,
    [property: JsonPropertyName("salaryMin")] decimal SalaryMin,
    [property: JsonPropertyName("salaryMax")] decimal SalaryMax,
    [property: JsonPropertyName("currency")] string Currency,
    [property: JsonPropertyName("location")] string Location,
    [property: JsonPropertyName("workingModel")] string WorkingModel
);

public sealed record JdCreateResponse(
    [property: JsonPropertyName("jdId")] string JdId,
    [property: JsonPropertyName("isValid")] bool IsValid,
    [property: JsonPropertyName("validationErrors")] List<string> ValidationErrors,
    [property: JsonPropertyName("normalizedJd")] object? NormalizedJd,
    [property: JsonPropertyName("generatedJdText")] string? GeneratedJdText,
    [property: JsonPropertyName("wordCount")] int WordCount,
    [property: JsonPropertyName("storedAt")] string? StoredAt
);

public sealed record JdSummaryResponse(
    [property: JsonPropertyName("jdId")] string JdId,
    [property: JsonPropertyName("jobTitle")] string JobTitle,
    [property: JsonPropertyName("seniority")] string Seniority,
    [property: JsonPropertyName("salaryMin")] decimal SalaryMin,
    [property: JsonPropertyName("salaryMax")] decimal SalaryMax,
    [property: JsonPropertyName("currency")] string Currency,
    [property: JsonPropertyName("createdAt")] string CreatedAt,
    [property: JsonPropertyName("updatedAt")] string UpdatedAt
);

public sealed record JdDetailResponse(
    [property: JsonPropertyName("jdId")] string JdId,
    [property: JsonPropertyName("normalizedJd")] JsonElement NormalizedJd,
    [property: JsonPropertyName("generatedJdText")] string GeneratedJdText,
    [property: JsonPropertyName("createdAt")] string CreatedAt,
    [property: JsonPropertyName("updatedAt")] string UpdatedAt
);

public sealed record JdUpdateRequest(
    [property: JsonPropertyName("normalizedJd")] JdFormRequest? NormalizedJd,
    [property: JsonPropertyName("generatedJdText")] string? GeneratedJdText
);

public sealed record JdMatchRequest(
    [Required]
    [property: JsonPropertyName("normalizedJd")] JdFormRequest NormalizedJd,
    [property: JsonPropertyName("candidateSkills")] List<CandidateSkillEvidence> CandidateSkills,
    [property: JsonPropertyName("candidateResponsibilities")] List<string> CandidateResponsibilities,
    [property: JsonPropertyName("candidateLevel")] string CandidateLevel,
    [property: JsonPropertyName("desiredSalary")] decimal? DesiredSalary,
    [property: JsonPropertyName("minimumAcceptableSalary")] decimal? MinimumAcceptableSalary,
    [property: JsonPropertyName("salaryCurrency")] string SalaryCurrency,
    [property: JsonPropertyName("candidateRoleTendency")] string? CandidateRoleTendency,
    [property: JsonPropertyName("candidateWorkingStyles")] List<string>? CandidateWorkingStyles
);

public sealed record CandidateSkillEvidence(
    [property: JsonPropertyName("skill")] string Skill,
    [property: JsonPropertyName("proficiency")] decimal Proficiency,
    [property: JsonPropertyName("evidenceStrength")] string? EvidenceStrength
);

public sealed record SkillMatchItem(
    [property: JsonPropertyName("skill")] string Skill,
    [property: JsonPropertyName("matched")] bool Matched,
    [property: JsonPropertyName("matchType")] string MatchType,
    [property: JsonPropertyName("candidateProficiency")] decimal CandidateProficiency,
    [property: JsonPropertyName("evidenceStrength")] string EvidenceStrength
);

public sealed record MatchScoreResponse(
    [property: JsonPropertyName("matchScore")] decimal MatchScore,
    [property: JsonPropertyName("matchScorePercent")] decimal MatchScorePercent,
    [property: JsonPropertyName("cappedMatchScorePercent")] decimal CappedMatchScorePercent,
    [property: JsonPropertyName("matchLabel")] string MatchLabel,
    [property: JsonPropertyName("skillMatchScore")] decimal SkillMatchScore,
    [property: JsonPropertyName("responsibilityMatchScore")] decimal ResponsibilityMatchScore,
    [property: JsonPropertyName("seniorityMatchScore")] decimal SeniorityMatchScore,
    [property: JsonPropertyName("salaryMatchScore")] decimal SalaryMatchScore,
    [property: JsonPropertyName("cultureFitScore")] decimal CultureFitScore,
    [property: JsonPropertyName("requiredSkillsMatch")] List<SkillMatchItem> RequiredSkillsMatch,
    [property: JsonPropertyName("preferredSkillsMatch")] List<SkillMatchItem> PreferredSkillsMatch,
    [property: JsonPropertyName("missingRequiredSkills")] List<string> MissingRequiredSkills,
    [property: JsonPropertyName("uncoveredResponsibilities")] List<string> UncoveredResponsibilities,
    [property: JsonPropertyName("seniorityFlag")] string SeniorityFlag,
    [property: JsonPropertyName("levelGap")] int LevelGap,
    [property: JsonPropertyName("salaryMatchType")] string SalaryMatchType,
    [property: JsonPropertyName("activeFlags")] List<string> ActiveFlags,
    [property: JsonPropertyName("gapAnalysis")] GapAnalysisResponse GapAnalysis,
    [property: JsonPropertyName("qualityGate")] ApplicationQualityGateResponse QualityGate,
    [property: JsonPropertyName("hiringRecommendation")] HiringRecommendationResponse HiringRecommendation
);

public sealed record GapAnalysisResponse(
    [property: JsonPropertyName("gapSeverity")] string GapSeverity,
    [property: JsonPropertyName("skillGaps")] List<string> SkillGaps,
    [property: JsonPropertyName("responsibilityGaps")] List<string> ResponsibilityGaps,
    [property: JsonPropertyName("seniorityGap")] string? SeniorityGap,
    [property: JsonPropertyName("salaryMismatch")] string? SalaryMismatch,
    [property: JsonPropertyName("improvementSuggestions")] List<string> ImprovementSuggestions,
    [property: JsonPropertyName("overallGapSummary")] string OverallGapSummary
);

public sealed record ApplicationQualityGateResponse(
    [property: JsonPropertyName("qualityGateStatus")] string QualityGateStatus,
    [property: JsonPropertyName("canApply")] bool CanApply,
    [property: JsonPropertyName("requiresExplicitConfirmation")] bool RequiresExplicitConfirmation,
    [property: JsonPropertyName("confirmationRequiredReasons")] List<string> ConfirmationRequiredReasons,
    [property: JsonPropertyName("warnings")] List<string> Warnings
);

public sealed record HiringRecommendationResponse(
    [property: JsonPropertyName("verdict")] string Verdict,
    [property: JsonPropertyName("confidence")] decimal Confidence,
    [property: JsonPropertyName("oneParaSummary")] string OneParaSummary,
    [property: JsonPropertyName("keyReasons")] List<string> KeyReasons,
    [property: JsonPropertyName("hiringRisk")] string HiringRisk
);

internal sealed record AiTaskRequest(
    [property: JsonPropertyName("jobId")] string JobId,
    [property: JsonPropertyName("taskType")] string TaskType,
    [property: JsonPropertyName("inputs")] object Inputs
);

internal sealed record AiTaskResponse(
    [property: JsonPropertyName("status")] string Status,
    [property: JsonPropertyName("errorMessage")] string? ErrorMessage,
    [property: JsonPropertyName("resultData")] string? ResultData
);
