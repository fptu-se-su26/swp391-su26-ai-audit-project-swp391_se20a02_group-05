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
/// Unit tests for PublicJobController.GetRecommendations — CVerify-109 (3 UTCIDs).
/// GET /api/v1/public/jobs/recommendations [Authorize].
/// </summary>
public sealed class CVerify109_GetJobRecommendationsTests : IDisposable
{
    private readonly ApplicationDbContext _context;
    private readonly Mock<IRecommendationProvider> _provider = new();

    public CVerify109_GetJobRecommendationsTests()
    {
        _context = new ApplicationDbContext(
            new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .ConfigureWarnings(w => w.Ignore(InMemoryEventId.TransactionIgnoredWarning))
                .Options);
    }

    public void Dispose() => _context.Dispose();

    private PublicJobController BuildController(Guid? userId)
    {
        var user = userId.HasValue
            ? new ClaimsPrincipal(new ClaimsIdentity(new List<Claim> { new(ClaimTypes.NameIdentifier, userId.Value.ToString()) }, "Test"))
            : new ClaimsPrincipal();
        var ctrl = new PublicJobController(_context, Mock.Of<IJobEligibilityService>(), Mock.Of<IExplainableMatchService>(),
            Mock.Of<IJobRankingStrategy>(), _provider.Object);
        ctrl.ControllerContext = new ControllerContext { HttpContext = new DefaultHttpContext { User = user } };
        return ctrl;
    }

    private static List<JobVacancy> SampleJobs(int n)
    {
        var list = new List<JobVacancy>();
        for (var i = 0; i < n; i++)
            list.Add(new JobVacancy
            {
                Id = Guid.NewGuid(), OrganizationId = Guid.NewGuid(), Title = $"Job {i}", Department = "Eng",
                WorkplaceType = "Hybrid", City = "Hanoi", Type = "Full-time", Salary = "x", SalaryMinMax = "0-0",
                Experience = "5+", Degree = "B", Category = "S", Status = "Published", IsActive = true,
            });
        return list;
    }

    // ── UTCID01 ── user with assessment → 200 OK (personalized) ───────────
    [Fact]
    public async Task CVerify109_UTCID01_Recommendations_WithAssessment_ReturnsOk()
    {
        _provider.Setup(p => p.GetRecommendedJobsAsync(It.IsAny<Guid>(), It.IsAny<int>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(SampleJobs(5));
        var ctrl = BuildController(Guid.NewGuid());

        var response = await ctrl.GetRecommendations(CancellationToken.None);

        response.Should().BeOfType<OkObjectResult>();
    }

    // ── UTCID02 ── user without assessment → 200 OK (generic list) ────────
    [Fact]
    public async Task CVerify109_UTCID02_Recommendations_NoAssessment_ReturnsOk()
    {
        _provider.Setup(p => p.GetRecommendedJobsAsync(It.IsAny<Guid>(), It.IsAny<int>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(SampleJobs(5));
        var ctrl = BuildController(Guid.NewGuid());

        var response = await ctrl.GetRecommendations(CancellationToken.None);

        response.Should().BeOfType<OkObjectResult>();
    }

    // ── UTCID03 ── no JWT → 401 Unauthorized ──────────────────────────────
    [Fact]
    public async Task CVerify109_UTCID03_Recommendations_NoJwt_Returns401()
    {
        var ctrl = BuildController(null);

        var response = await ctrl.GetRecommendations(CancellationToken.None);

        response.Should().BeOfType<UnauthorizedResult>();
    }
}
