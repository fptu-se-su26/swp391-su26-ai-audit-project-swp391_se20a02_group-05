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
/// Unit tests for PublicJobController.GetApplicants — CVerify-110 (4 UTCIDs).
/// GET /api/v1/public/jobs/{id}/applicants [Authorize] — workspace member or admin only.
/// </summary>
public sealed class CVerify110_GetJobApplicantsTests : IDisposable
{
    private readonly ApplicationDbContext _context;

    public CVerify110_GetJobApplicantsTests()
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

    // ── UTCID01 ── authenticated workspace member → 200 OK ────────────────
    [Fact]
    public async Task CVerify110_UTCID01_GetApplicants_Member_ReturnsOk()
    {
        var job = await SeedJobAsync();
        var userId = Guid.NewGuid();
        _context.OrganizationMemberships.Add(new OrganizationMembership
        {
            OrganizationId = job.OrganizationId, UserId = userId, Role = "HR", Status = "active",
        });
        await _context.SaveChangesAsync();
        var ctrl = BuildController(userId);

        var response = await ctrl.GetApplicants(job.Id, CancellationToken.None);

        response.Should().BeOfType<OkObjectResult>();
    }

    // ── UTCID02 ── authenticated regular candidate (not member) → 403 ─────
    [Fact]
    public async Task CVerify110_UTCID02_GetApplicants_RegularCandidate_Returns403()
    {
        var job = await SeedJobAsync();
        var ctrl = BuildController(Guid.NewGuid()); // no membership, not admin

        var response = await ctrl.GetApplicants(job.Id, CancellationToken.None);

        response.Should().BeOfType<ForbidResult>();
    }

    // ── UTCID03 ── non-existent job → 404 NotFound ────────────────────────
    [Fact]
    public async Task CVerify110_UTCID03_GetApplicants_NotFound_Returns404()
    {
        var ctrl = BuildController(Guid.NewGuid());

        var response = await ctrl.GetApplicants(Guid.NewGuid(), CancellationToken.None);

        response.Should().BeOfType<NotFoundResult>();
    }

    // ── UTCID04 ── no JWT → 401 Unauthorized ──────────────────────────────
    [Fact]
    public async Task CVerify110_UTCID04_GetApplicants_NoJwt_Returns401()
    {
        var ctrl = BuildController(null);

        var response = await ctrl.GetApplicants(Guid.NewGuid(), CancellationToken.None);

        response.Should().BeOfType<UnauthorizedResult>();
    }
}
