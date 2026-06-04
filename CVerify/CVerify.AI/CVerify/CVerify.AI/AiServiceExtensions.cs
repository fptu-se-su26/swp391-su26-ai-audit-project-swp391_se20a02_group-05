namespace CVerify.AI;

using Microsoft.Extensions.DependencyInjection;
using CVerify.AI.Agents;
using CVerify.AI.Agents.CvAgent;
using CVerify.AI.Agents.GitHubAgent;
using CVerify.AI.Agents.SkillExtractionAgent;
using CVerify.AI.Agents.VerificationAgent;
using CVerify.AI.Agents.ScoringAgent;
using CVerify.AI.Agents.MatchingAgent;
using CVerify.AI.Agents.RecommendationAgent;
using CVerify.AI.Orchestrators;
using CVerify.AI.Extractors;
using CVerify.AI.Prompts;
using CVerify.AI.Security;
using CVerify.AI.Parsing;
using CVerify.AI.Embedding;
using CVerify.AI.Skills;
using CVerify.AI.GitHub;
using CVerify.AI.Scoring;
using CVerify.AI.Monitoring;

public static class AiServiceExtensions
{
    public static IServiceCollection AddAiServices(this IServiceCollection services)
    {
        // Register Agents
        services.AddScoped<CvAgent>();
        services.AddScoped<GitHubAgent>();
        services.AddScoped<SkillExtractionAgent>();
        services.AddScoped<VerificationAgent>();
        services.AddScoped<ScoringAgent>();
        services.AddScoped<MatchingAgent>();
        services.AddScoped<RecommendationAgent>();

        // Register Orchestrators
        services.AddScoped<ICvAnalysisOrchestrator, CvAnalysisOrchestrator>();
        services.AddScoped<IGitHubAnalysisOrchestrator, GitHubAnalysisOrchestrator>();
        services.AddScoped<IJobMatchingOrchestrator, JobMatchingOrchestrator>();

        // Register Extractors
        services.AddScoped<ITextExtractor, PdfTextExtractor>();
        services.AddScoped<ITextExtractor, DocxTextExtractor>();
        services.AddScoped<ITextExtractor, OcrTextExtractor>();

        // Register Prompts
        services.AddScoped<IPromptFactory, CvPromptFactory>();
        services.AddScoped<IPromptFactory, GitHubPromptFactory>();
        services.AddScoped<IPromptFactory, MatchingPromptFactory>();

        // Register Security
        services.AddScoped<IPromptSanitizer, PromptSanitizer>();
        services.AddScoped<InputBoundary>();
        services.AddScoped<MaliciousRepoDetector>();

        // Register Parsing
        services.AddScoped<IStructuredOutputParser, JsonSchemaValidator>();
        services.AddScoped<LlmResponseParser>();

        // Register Embedding
        services.AddScoped<IEmbeddingService, OpenAiEmbeddingService>();

        // Register Skills
        services.AddScoped<ISkillOntology, SkillOntologyService>();
        services.AddScoped<SkillNormalizer>();

        // Register GitHub
        services.AddScoped<ICodeSampler, CodeSampler>();
        services.AddScoped<IRepoSelector, RepoSelector>();
        services.AddScoped<ITechnologyDetector, TechnologyDetector>();
        services.AddScoped<IArchitecturePatternDetector, ArchitecturePatternDetector>();

        // Register Scoring
        services.AddScoped<IWeightedScoringEngine, WeightedScoringEngine>();
        services.AddScoped<IPercentileService, PercentileService>();

        // Register Monitoring
        services.AddScoped<IAiCostTracker, AiCostTracker>();
        services.AddScoped<IPipelineMetrics, PipelineMetrics>();

        return services;
    }
}
