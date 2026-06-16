using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace CVerify.API.Modules.Jd.DTOs;

public sealed record JdFormRequest(
    [property: JsonPropertyName("jobTitle")] string JobTitle,
    [property: JsonPropertyName("seniority")] string Seniority,
    [property: JsonPropertyName("requiredSkills")] List<string> RequiredSkills,
    [property: JsonPropertyName("preferredSkills")] List<string> PreferredSkills,
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
