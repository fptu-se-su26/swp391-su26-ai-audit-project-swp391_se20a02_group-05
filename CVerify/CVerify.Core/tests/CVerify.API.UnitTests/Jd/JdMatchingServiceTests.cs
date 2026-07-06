using CVerify.API.Modules.Jd.DTOs;
using CVerify.API.Modules.Jd.Services;
using FluentAssertions;
using Xunit;

namespace CVerify.API.UnitTests.Jd;

public sealed class JdMatchingServiceTests
{
    private readonly JdMatchingService _service = new();

    [Fact]
    public void CalculateMatch_ShouldReturnStrongMatch_ForAlignedCandidate()
    {
        var result = _service.CalculateMatch(CreateRequest());

        result.MatchScorePercent.Should().BeGreaterThan(80m);
        result.CappedMatchScorePercent.Should().Be(result.MatchScorePercent);
        result.MissingRequiredSkills.Should().BeEmpty();
        result.QualityGate.RequiresExplicitConfirmation.Should().BeFalse();
        result.HiringRecommendation.Verdict.Should().Be("Yes");
    }

    [Fact]
    public void CalculateMatch_ShouldCapScore_WhenSalaryHardMismatch()
    {
        var request = CreateRequest(desiredSalary: 5000m, minimumAcceptableSalary: 4500m);

        var result = _service.CalculateMatch(request);

        result.SalaryMatchScore.Should().Be(0m);
        result.SalaryMatchType.Should().Be("hard_mismatch");
        result.CappedMatchScorePercent.Should().BeLessThanOrEqualTo(60m);
        result.ActiveFlags.Should().Contain("SALARY_HARD_MISMATCH");
        result.QualityGate.ConfirmationRequiredReasons.Should().Contain("salary_mismatch");
    }

    [Fact]
    public void CalculateMatch_ShouldFlagInsufficientSkills_WhenRequiredCoverageIsLow()
    {
        var request = CreateRequest(candidateSkills:
        [
            new CandidateSkillEvidence("HTML", 3m, "weak")
        ]);

        var result = _service.CalculateMatch(request);

        result.SkillMatchScore.Should().BeLessThan(0.4m);
        result.MissingRequiredSkills.Should().Contain(["React", "TypeScript"]);
        result.ActiveFlags.Should().Contain("INSUFFICIENT_SKILLS");
    }

    [Fact]
    public void CalculateMatch_ShouldTreatEmptyResponsibilitiesAsFullResponsibilityMatch()
    {
        var jd = CreateJd() with { Responsibilities = [] };
        var request = CreateRequest(normalizedJd: jd, candidateResponsibilities: []);

        var result = _service.CalculateMatch(request);

        result.ResponsibilityMatchScore.Should().Be(1m);
        result.UncoveredResponsibilities.Should().BeEmpty();
    }

    [Fact]
    public void CalculateMatch_ShouldFlagLargeSeniorityGap()
    {
        var request = CreateRequest(candidateLevel: "L1", normalizedJd: CreateJd() with { Seniority = "Staff" });

        var result = _service.CalculateMatch(request);

        result.SeniorityMatchScore.Should().Be(0.3m);
        result.LevelGap.Should().Be(-3);
        result.ActiveFlags.Should().Contain("SENIORITY_GAP");
        result.QualityGate.ConfirmationRequiredReasons.Should().Contain("seniority_gap");
    }

    [Fact]
    public void CalculateMatch_ShouldNotThrow_WhenOptionalListsAreNull()
    {
        var jd = CreateJd() with { PreferredSkills = null, Responsibilities = null! };
        var request = new JdMatchRequest(
            jd,
            null!,
            null!,
            null!,
            2500m,
            2000m,
            "USD",
            null,
            null);

        var act = () => _service.CalculateMatch(request);

        act.Should().NotThrow();
    }

    [Fact]
    public void CalculateMatch_ShouldNotCompareRawSalary_WhenCurrenciesDiffer()
    {
        var request = CreateRequest(
            desiredSalary: 70_000_000m,
            minimumAcceptableSalary: 60_000_000m,
            salaryCurrency: "VND",
            normalizedJd: CreateJd() with { Currency = "USD", SalaryMax = 3000m });

        var result = _service.CalculateMatch(request);

        result.SalaryMatchScore.Should().Be(1m);
        result.SalaryMatchType.Should().Be("currency_mismatch");
        result.ActiveFlags.Should().Contain("CURRENCY_MISMATCH");
        result.ActiveFlags.Should().NotContain("SALARY_HARD_MISMATCH");
    }

    private static JdMatchRequest CreateRequest(
        JdFormRequest? normalizedJd = null,
        List<CandidateSkillEvidence>? candidateSkills = null,
        List<string>? candidateResponsibilities = null,
        string candidateLevel = "L3",
        decimal desiredSalary = 2500m,
        decimal minimumAcceptableSalary = 2000m,
        string salaryCurrency = "USD")
    {
        return new JdMatchRequest(
            normalizedJd ?? CreateJd(),
            candidateSkills ??
            [
                new CandidateSkillEvidence("ReactJS", 5m, "strong"),
                new CandidateSkillEvidence("TypeScript", 4m, "strong"),
                new CandidateSkillEvidence("PostgreSQL", 3m, "moderate")
            ],
            candidateResponsibilities ?? ["Design and implement REST APIs", "Build frontend features with React"],
            candidateLevel,
            desiredSalary,
            minimumAcceptableSalary,
            salaryCurrency,
            "Engineer",
            ["hybrid"]);
    }

    private static JdFormRequest CreateJd()
    {
        return new JdFormRequest(
            "Senior Software Engineer",
            "Senior",
            ["React", "TypeScript"],
            ["PostgreSQL"],
            ["Design and implement REST APIs", "Build frontend features"],
            3,
            6,
            "Bachelor's Degree",
            "Advanced",
            2000m,
            3000m,
            "USD",
            "Ho Chi Minh City",
            "hybrid");
    }
}
