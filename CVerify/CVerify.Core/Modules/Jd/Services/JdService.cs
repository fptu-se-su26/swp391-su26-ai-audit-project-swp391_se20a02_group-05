using System;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using CVerify.API.Modules.Jd.DTOs;
using CVerify.API.Modules.Jd.Entities;
using CVerify.API.Modules.Shared.Configuration;
using CVerify.API.Modules.Shared.Persistence;
using CVerify.API.Modules.Shared.System.Services;

namespace CVerify.API.Modules.Jd.Services;

public sealed class JdService : IJdService
{
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly IHmacSignatureService _hmacService;
    private readonly EnvConfiguration _envConfig;
    private readonly ApplicationDbContext _context;
    private readonly ILogger<JdService> _logger;

    private static readonly JsonSerializerOptions _jsonOpts = new() { PropertyNameCaseInsensitive = true };

    public JdService(
        IHttpClientFactory httpClientFactory,
        IHmacSignatureService hmacService,
        EnvConfiguration envConfig,
        ApplicationDbContext context,
        ILogger<JdService> logger)
    {
        _httpClientFactory = httpClientFactory;
        _hmacService = hmacService;
        _envConfig = envConfig;
        _context = context;
        _logger = logger;
    }

    public async Task<JdCreateResponse> CreateJdAsync(Guid userId, JdFormRequest request, CancellationToken cancellationToken = default)
    {
        var jobId = $"jd-{Guid.CreateVersion7()}";

        // Step 1 — L3-002: Validate & Normalize
        var validationResult = await ExecuteTaskAsync(jobId, "JdFieldValidator", new { jdRaw = request }, cancellationToken);
        if (validationResult.GetProperty("isValid").GetBoolean() == false)
        {
            var errors = JsonSerializer.Deserialize<System.Collections.Generic.List<string>>(
                validationResult.GetProperty("validationErrors").GetRawText()) ?? [];

            return new JdCreateResponse(
                JdId: jobId,
                IsValid: false,
                ValidationErrors: errors,
                NormalizedJd: null,
                GeneratedJdText: null,
                WordCount: 0,
                StoredAt: null);
        }

        var normalizedJd = JsonSerializer.Deserialize<object>(
            validationResult.GetProperty("normalizedJd").GetRawText());

        // Step 2 — L3-003: Generate JD Text
        var generationResult = await ExecuteTaskAsync(jobId, "AiJdGenerator", new { normalizedJd }, cancellationToken);
        var generatedText = generationResult.GetProperty("generatedJdText").GetString() ?? string.Empty;
        var wordCount = generationResult.TryGetProperty("wordCount", out var wcProp) ? wcProp.GetInt32() : 0;

        // Step 3 — L3-004: Store JD
        var storageResult = await ExecuteTaskAsync(jobId, "JdStorageService", new
        {
            normalizedJd,
            generatedJdText = generatedText,
            jdId = jobId
        }, cancellationToken);

        var storedAt = storageResult.TryGetProperty("storedAt", out var satProp)
            ? satProp.GetString()
            : DateTimeOffset.UtcNow.ToString("O");

        var structuredJson = JsonSerializer.Serialize(normalizedJd);
        var entity = new StandardizedJd
        {
            Id = jobId,
            OwnerUserId = userId,
            JobTitle = request.JobTitle.Trim(),
            Seniority = request.Seniority.Trim(),
            Department = request.Department?.Trim() ?? string.Empty,
            EmploymentType = request.EmploymentType?.Trim() ?? string.Empty,
            Location = request.Location.Trim(),
            WorkMode = ResolveWorkMode(request),
            Industry = request.Industry?.Trim() ?? string.Empty,
            HiringPriority = request.HiringPriority?.Trim() ?? string.Empty,
            Currency = request.Currency.Trim().ToUpperInvariant(),
            SalaryMin = request.SalaryMin,
            SalaryMax = request.SalaryMax,
            StructuredJson = structuredJson,
            HumanReadableText = generatedText,
            CreatedAt = DateTimeOffset.UtcNow,
            UpdatedAt = DateTimeOffset.UtcNow
        };

        _context.StandardizedJds.Add(entity);
        await _context.SaveChangesAsync(cancellationToken);

        return new JdCreateResponse(
            JdId: jobId,
            IsValid: true,
            ValidationErrors: [],
            NormalizedJd: normalizedJd,
            GeneratedJdText: generatedText,
            WordCount: wordCount,
            StoredAt: storedAt);
    }

    public async Task<IReadOnlyList<JdSummaryResponse>> ListJdsAsync(Guid userId, CancellationToken cancellationToken = default)
    {
        return await _context.StandardizedJds
            .AsNoTracking()
            .Where(jd => jd.OwnerUserId == userId)
            .OrderByDescending(jd => jd.CreatedAt)
            .Select(jd => new JdSummaryResponse(
                jd.Id,
                jd.JobTitle,
                jd.Seniority,
                jd.SalaryMin,
                jd.SalaryMax,
                jd.Currency,
                jd.CreatedAt.ToString("O"),
                jd.UpdatedAt.ToString("O"),
                jd.Department,
                jd.EmploymentType,
                jd.Location,
                jd.WorkMode,
                jd.Industry,
                jd.HiringPriority))
            .ToListAsync(cancellationToken);
    }

    public async Task<JdDetailResponse?> GetJdAsync(Guid userId, string jdId, CancellationToken cancellationToken = default)
    {
        var entity = await _context.StandardizedJds
            .AsNoTracking()
            .FirstOrDefaultAsync(jd => jd.OwnerUserId == userId && jd.Id == jdId, cancellationToken);

        return entity == null ? null : MapDetail(entity);
    }

    public async Task<JdDetailResponse?> UpdateJdAsync(Guid userId, string jdId, JdUpdateRequest request, CancellationToken cancellationToken = default)
    {
        var entity = await _context.StandardizedJds
            .FirstOrDefaultAsync(jd => jd.OwnerUserId == userId && jd.Id == jdId, cancellationToken);

        if (entity == null) return null;

        if (request.NormalizedJd != null)
        {
            entity.JobTitle = request.NormalizedJd.JobTitle.Trim();
            entity.Seniority = request.NormalizedJd.Seniority.Trim();
            entity.Department = request.NormalizedJd.Department?.Trim() ?? string.Empty;
            entity.EmploymentType = request.NormalizedJd.EmploymentType?.Trim() ?? string.Empty;
            entity.Location = request.NormalizedJd.Location.Trim();
            entity.WorkMode = ResolveWorkMode(request.NormalizedJd);
            entity.Industry = request.NormalizedJd.Industry?.Trim() ?? string.Empty;
            entity.HiringPriority = request.NormalizedJd.HiringPriority?.Trim() ?? string.Empty;
            entity.Currency = request.NormalizedJd.Currency.Trim().ToUpperInvariant();
            entity.SalaryMin = request.NormalizedJd.SalaryMin;
            entity.SalaryMax = request.NormalizedJd.SalaryMax;
            entity.StructuredJson = JsonSerializer.Serialize(request.NormalizedJd);
        }

        if (!string.IsNullOrWhiteSpace(request.GeneratedJdText))
        {
            entity.HumanReadableText = request.GeneratedJdText.Trim();
        }

        entity.UpdatedAt = DateTimeOffset.UtcNow;
        await _context.SaveChangesAsync(cancellationToken);
        return MapDetail(entity);
    }

    public async Task<bool> DeleteJdAsync(Guid userId, string jdId, CancellationToken cancellationToken = default)
    {
        var entity = await _context.StandardizedJds
            .FirstOrDefaultAsync(jd => jd.OwnerUserId == userId && jd.Id == jdId, cancellationToken);

        if (entity == null) return false;

        _context.StandardizedJds.Remove(entity);
        await _context.SaveChangesAsync(cancellationToken);
        return true;
    }

    private async Task<JsonElement> ExecuteTaskAsync(string jobId, string taskType, object inputs, CancellationToken cancellationToken)
    {
        var taskPath = "/api/v1/analysis/task/execute";
        var payload = JsonSerializer.Serialize(new AiTaskRequest(jobId, taskType, inputs));

        var (signature, timestamp, nonce) = _hmacService.CreateSignatureHeaders("POST", taskPath, payload);

        var request = new HttpRequestMessage(HttpMethod.Post, taskPath)
        {
            Content = new StringContent(payload, Encoding.UTF8, "application/json")
        };
        request.Headers.Add("X-Client-Id", _envConfig.Ai.ClientId);
        request.Headers.Add("X-Timestamp", timestamp);
        request.Headers.Add("X-Nonce", nonce);
        request.Headers.Add("X-Correlation-Id", jobId);
        request.Headers.Add("X-Signature", signature);

        var httpClient = _httpClientFactory.CreateClient("AiServiceClient");
        using var response = await httpClient.SendAsync(request, cancellationToken);

        var responseJson = await response.Content.ReadAsStringAsync(cancellationToken);

        if (!response.IsSuccessStatusCode)
        {
            throw new HttpRequestException($"AI task {taskType} failed ({response.StatusCode}): {responseJson}");
        }

        var taskResponse = JsonSerializer.Deserialize<AiTaskResponse>(responseJson, _jsonOpts)
            ?? throw new InvalidOperationException($"Failed to deserialize response from {taskType}");

        if (taskResponse.Status == "Failed")
        {
            throw new InvalidOperationException($"AI task {taskType} returned Failed: {taskResponse.ErrorMessage}");
        }

        if (string.IsNullOrEmpty(taskResponse.ResultData))
        {
            throw new InvalidOperationException($"AI task {taskType} returned empty ResultData");
        }

        using var doc = JsonDocument.Parse(taskResponse.ResultData);
        return doc.RootElement.Clone();
    }

    private static JdDetailResponse MapDetail(StandardizedJd entity)
    {
        using var doc = JsonDocument.Parse(entity.StructuredJson);
        return new JdDetailResponse(
            entity.Id,
            doc.RootElement.Clone(),
            entity.HumanReadableText,
            entity.CreatedAt.ToString("O"),
            entity.UpdatedAt.ToString("O"));
    }

    private static string ResolveWorkMode(JdFormRequest request)
    {
        return !string.IsNullOrWhiteSpace(request.WorkMode)
            ? request.WorkMode.Trim()
            : request.WorkingModel.Trim();
    }
}
