using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Moq;
using Xunit;
using CVerify.API.Modules.Intelligence.Services;
using CVerify.API.Modules.Shared.Domain.Entities;
using CVerify.API.Modules.Shared.Persistence;
using CVerify.API.Modules.Shared.System.Controllers;
using CVerify.API.Modules.Shared.System.Services;

namespace CVerify.API.UnitTests.Auth;

/// <summary>
/// Unit tests for PublicJobController.GetEligibility — CVerify-106 (5 UTCIDs).
/// GET /api/v1/public/jobs/{id}/eligibility [Authorize].
/// </summary>
public sealed class CVerify106_GetJobEligibilityTests : IDisposable
{
    private readonly ApplicationDbContext _context;
    private readonly Mock<IJobEligibilityService> _eligibility = new();
    private readonly Mock<IExplainableMatchService> _match = new();

    public CVerify106_GetJobEligibilityTests()
    {
        _context = new ApplicationDbContext(
            new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .ConfigureWarnings(w => w.Ignore(InMemoryEventId.TransactionIgnoredWarning))
                .Options);
        _match.Setup(m => m.EvaluateMatchAsync(It.IsAny<Guid>(), It.IsAny<Guid>()))
            .ReturnsAsync(new MatchingEvaluation { Id = Guid.NewGuid(), AggregateScore = 70, ConfidenceLevel = "Medium" });
    }

    public void Dispose() => _context.Dispose();

    private PublicJobController BuildController(bool authenticated = true)
    {
        var user = authenticated
            ? new ClaimsPrincipal(new ClaimsIdentity(new List<Claim> { new(ClaimTypes.NameIdentifier, Guid.NewGuid().ToString()) }, "Test"))
            : new ClaimsPrincipal();
        var ctrl = new PublicJobController(_context, _eligibility.Object, _match.Object,
            Mock.Of<IJobRankingStrategy>(), Mock.Of<IRecommendationProvider>());
        ctrl.ControllerContext = new ControllerContext { HttpContext = new DefaultHttpContext { User = user } };
        return ctrl;
    }

    private void SetupReport(bool isEligible)
    {
        _eligibility.Setup(e => e.CheckEligibilityAsync(It.IsAny<Guid>(), It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new EligibilityReportDto(isEligible, !isEligible, new List<EligibilityRequirementCheck>(), "reason"));
    }

    // ── UTCID01 ── matching assessment → 200 OK (eligible) ────────────────
    [Fact]
    public async Task CVerify106_UTCID01_Eligibility_Match_ReturnsOkEligible()
    {
        SetupReport(true);
        var ctrl = BuildController();

        var response = await ctrl.GetEligibility(Guid.NewGuid(), CancellationToken.None);

        response.Should().BeOfType<OkObjectResult>();
    }

    // ── UTCID02 ── lacks required skills → 200 OK (isEligible:false) ──────
    [Fact]
    public async Task CVerify106_UTCID02_Eligibility_MissingSkills_ReturnsOkIneligible()
    {
        SetupReport(false);
        var ctrl = BuildController();

        var response = await ctrl.GetEligibility(Guid.NewGuid(), CancellationToken.None);

        response.Should().BeOfType<OkObjectResult>();
    }

    // ── UTCID03 ── non-existent job → 404 NotFound ────────────────────────
    [Fact]
    public async Task CVerify106_UTCID03_Eligibility_NotFound_Returns404()
    {
        _eligibility.Setup(e => e.CheckEligibilityAsync(It.IsAny<Guid>(), It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
            .ThrowsAsync(new ArgumentException("Job not found."));
        var ctrl = BuildController();

        var response = await ctrl.GetEligibility(Guid.NewGuid(), CancellationToken.None);

        response.Should().BeOfType<NotFoundObjectResult>();
    }

    // ── UTCID04 ── user has no assessment → 200 OK (isEligible:false) ─────
    [Fact]
    public async Task CVerify106_UTCID04_Eligibility_NoAssessment_ReturnsOk()
    {
        SetupReport(false);
        var ctrl = BuildController();

        var response = await ctrl.GetEligibility(Guid.NewGuid(), CancellationToken.None);

        response.Should().BeOfType<OkObjectResult>();
    }

    // ── UTCID05 ── no JWT → 401 Unauthorized ──────────────────────────────
    [Fact]
    public async Task CVerify106_UTCID05_Eligibility_NoJwt_Returns401()
    {
        var ctrl = BuildController(authenticated: false);

        var response = await ctrl.GetEligibility(Guid.NewGuid(), CancellationToken.None);

        response.Should().BeOfType<UnauthorizedResult>();
    }
}
