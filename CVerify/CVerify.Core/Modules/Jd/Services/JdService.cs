using System;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using CVerify.API.Modules.Jd.DTOs;
using CVerify.API.Modules.Shared.Configuration;
using CVerify.API.Modules.Shared.System.Services;

namespace CVerify.API.Modules.Jd.Services;

public sealed class JdService : IJdService
{
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly IHmacSignatureService _hmacService;
    private readonly EnvConfiguration _envConfig;
    private readonly ILogger<JdService> _logger;

    private static readonly JsonSerializerOptions _jsonOpts = new() { PropertyNameCaseInsensitive = true };

    public JdService(
        IHttpClientFactory httpClientFactory,
        IHmacSignatureService hmacService,
        EnvConfiguration envConfig,
        ILogger<JdService> logger)
    {
        _httpClientFactory = httpClientFactory;
        _hmacService = hmacService;
        _envConfig = envConfig;
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

        return new JdCreateResponse(
            JdId: jobId,
            IsValid: true,
            ValidationErrors: [],
            NormalizedJd: normalizedJd,
            GeneratedJdText: generatedText,
            WordCount: wordCount,
            StoredAt: storedAt);
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

        var httpClient = _httpClientFactory.CreateClient("AiService");
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
}
