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
/// Unit tests for PublicJobController.Apply — CVerify-107 (4 of 6 UTCIDs).
/// POST /api/v1/public/jobs/{id}/apply [Authorize].
/// NOTE: a duplicate application returns 400 (Excel's "409" is inaccurate). The action performs no
/// job-existence / closed-job checks, so "closed → 400" and "non-existent → 404" are integration-only.
/// </summary>
public sealed class CVerify107_ApplyForJobTests : IDisposable
{
    private readonly ApplicationDbContext _context;
    private readonly Mock<IJobEligibilityService> _eligibility = new();

    public CVerify107_ApplyForJobTests()
    {
        _context = new ApplicationDbContext(
            new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .ConfigureWarnings(w => w.Ignore(InMemoryEventId.TransactionIgnoredWarning))
                .Options);
        _eligibility.Setup(e => e.CheckEligibilityAsync(It.IsAny<Guid>(), It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new EligibilityReportDto(true, false, new List<EligibilityRequirementCheck>(), "ok"));
    }

    public void Dispose() => _context.Dispose();

    private PublicJobController BuildController(Guid? userId)
    {
        var user = userId.HasValue
            ? new ClaimsPrincipal(new ClaimsIdentity(new List<Claim> { new(ClaimTypes.NameIdentifier, userId.Value.ToString()) }, "Test"))
            : new ClaimsPrincipal();
        var ctrl = new PublicJobController(_context, _eligibility.Object, Mock.Of<IExplainableMatchService>(),
            Mock.Of<IJobRankingStrategy>(), Mock.Of<IRecommendationProvider>());
        ctrl.ControllerContext = new ControllerContext { HttpContext = new DefaultHttpContext { User = user } };
        return ctrl;
    }

    private async Task<JobVacancy> SeedJobAsync()
    {
        var v = new JobVacancy
        {
            Id = Guid.NewGuid(), OrganizationId = Guid.NewGuid(), Title = "Backend Dev", Department = "Eng",
            WorkplaceType = "Hybrid", City = "Hanoi", Type = "Full-time", Salary = "x", SalaryMinMax = "0-0",
            Experience = "5+", Degree = "B", Category = "S", CoverUrl = "https://cover.example/x.png", Status = "Published", IsActive = true,
        };
        _context.JobVacancies.Add(v);
        await _context.SaveChangesAsync();
        return v;
    }

    // ── UTCID01 ── first application → 201 Created ────────────────────────
    [Fact]
    public async Task CVerify107_UTCID01_Apply_FirstApplication_Returns201()
    {
        var job = await SeedJobAsync();
        var ctrl = BuildController(Guid.NewGuid());

        var response = await ctrl.Apply(job.Id, CancellationToken.None);

        response.Should().BeOfType<CreatedAtActionResult>();
    }

    // ── UTCID02 ── already applied → 400 BadRequest ───────────────────────
    [Fact]
    public async Task CVerify107_UTCID02_Apply_AlreadyApplied_Returns400()
    {
        var job = await SeedJobAsync();
        var userId = Guid.NewGuid();
        _context.JobApplications.Add(new JobApplication { Id = Guid.NewGuid(), JobVacancyId = job.Id, CandidateId = userId, Status = "Applied" });
        await _context.SaveChangesAsync();
        var ctrl = BuildController(userId);

        var response = await ctrl.Apply(job.Id, CancellationToken.None);

        response.Should().BeOfType<BadRequestObjectResult>();
    }

    // ── UTCID06 ── minimal candidate profile, first application → 201 ─────
    [Fact]
    public async Task CVerify107_UTCID06_Apply_MinimalProfile_Returns201()
    {
        var job = await SeedJobAsync();
        var ctrl = BuildController(Guid.NewGuid());

        var response = await ctrl.Apply(job.Id, CancellationToken.None);

        response.Should().BeOfType<CreatedAtActionResult>();
    }

    // ── UTCID05 ── no JWT → 401 Unauthorized ──────────────────────────────
    [Fact]
    public async Task CVerify107_UTCID05_Apply_NoJwt_Returns401()
    {
        var ctrl = BuildController(null);

        var response = await ctrl.Apply(Guid.NewGuid(), CancellationToken.None);

        response.Should().BeOfType<UnauthorizedResult>();
    }
}
