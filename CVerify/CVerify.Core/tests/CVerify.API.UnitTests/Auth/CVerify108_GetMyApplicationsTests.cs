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
/// Unit tests for PublicJobController.GetApplications — CVerify-108 (3 UTCIDs).
/// GET /api/v1/public/jobs/applications [Authorize].
/// </summary>
public sealed class CVerify108_GetMyApplicationsTests : IDisposable
{
    private readonly ApplicationDbContext _context;

    public CVerify108_GetMyApplicationsTests()
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
            Mock.Of<IJobRankingStrategy>(), Mock.Of<IRecommendationProvider>());
        ctrl.ControllerContext = new ControllerContext { HttpContext = new DefaultHttpContext { User = user } };
        return ctrl;
    }

    private async Task SeedApplicationAsync(Guid userId)
    {
        var job = new JobVacancy
        {
            Id = Guid.NewGuid(), OrganizationId = Guid.NewGuid(), Title = "Backend Dev", Department = "Eng",
            WorkplaceType = "Hybrid", City = "Hanoi", Type = "Full-time", Salary = "x", SalaryMinMax = "0-0",
            Experience = "5+", Degree = "B", Category = "S", CoverUrl = "https://cover.example/x.png", Status = "Published", IsActive = true,
        };
        _context.JobVacancies.Add(job);
        _context.JobApplications.Add(new JobApplication { Id = Guid.NewGuid(), JobVacancyId = job.Id, CandidateId = userId, Status = "Applied" });
        await _context.SaveChangesAsync();
    }

    // ── UTCID01 ── has applications → 200 OK ──────────────────────────────
    [Fact]
    public async Task CVerify108_UTCID01_GetApplications_HasApps_ReturnsOk()
    {
        var userId = Guid.NewGuid();
        await SeedApplicationAsync(userId);
        var ctrl = BuildController(userId);

        var response = await ctrl.GetApplications(CancellationToken.None);

        response.Should().BeOfType<OkObjectResult>();
    }

    // ── UTCID02 ── no applications → 200 OK (empty) ───────────────────────
    [Fact]
    public async Task CVerify108_UTCID02_GetApplications_NoApps_ReturnsOk()
    {
        var ctrl = BuildController(Guid.NewGuid());

        var response = await ctrl.GetApplications(CancellationToken.None);

        response.Should().BeOfType<OkObjectResult>();
    }

    // ── UTCID03 ── no JWT → 401 Unauthorized ──────────────────────────────
    [Fact]
    public async Task CVerify108_UTCID03_GetApplications_NoJwt_Returns401()
    {
        var ctrl = BuildController(null);

        var response = await ctrl.GetApplications(CancellationToken.None);

        response.Should().BeOfType<UnauthorizedResult>();
    }
}
