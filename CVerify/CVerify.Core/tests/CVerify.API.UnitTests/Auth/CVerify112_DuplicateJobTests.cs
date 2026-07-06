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
/// Unit tests for PublicJobController.Duplicate — CVerify-112 (4 UTCIDs).
/// POST /api/v1/public/jobs/{id}/duplicate [Authorize] — workspace member/admin only.
/// NOTE: a successful duplicate returns 201 Created (Excel's "200" is inaccurate).
/// </summary>
public sealed class CVerify112_DuplicateJobTests : IDisposable
{
    private readonly ApplicationDbContext _context;

    public CVerify112_DuplicateJobTests()
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
            Experience = "5+", Degree = "B", Category = "S", CoverUrl = "https://cover.example/x.png",
            Status = "Published", IsActive = true,
        };
        _context.JobVacancies.Add(v);
        await _context.SaveChangesAsync();
        return v;
    }

    private async Task AddMembershipAsync(Guid orgId, Guid userId)
    {
        _context.OrganizationMemberships.Add(new OrganizationMembership
        {
            OrganizationId = orgId, UserId = userId, Role = "HR", Status = "active",
        });
        await _context.SaveChangesAsync();
    }

    // ── UTCID01 ── member duplicates a published job → 201 Created ────────
    [Fact]
    public async Task CVerify112_UTCID01_Duplicate_Member_Returns201()
    {
        var job = await SeedJobAsync();
        var userId = Guid.NewGuid();
        await AddMembershipAsync(job.OrganizationId, userId);
        var ctrl = BuildController(userId);

        var response = await ctrl.Duplicate(job.Id, CancellationToken.None);

        response.Should().BeOfType<CreatedAtActionResult>();
    }

    // ── UTCID02 ── regular candidate (not member) → 403 Forbidden ─────────
    [Fact]
    public async Task CVerify112_UTCID02_Duplicate_RegularCandidate_Returns403()
    {
        var job = await SeedJobAsync();
        var ctrl = BuildController(Guid.NewGuid()); // no membership

        var response = await ctrl.Duplicate(job.Id, CancellationToken.None);

        response.Should().BeOfType<ForbidResult>();
    }

    // ── UTCID03 ── non-existent job → 404 NotFound ────────────────────────
    [Fact]
    public async Task CVerify112_UTCID03_Duplicate_NotFound_Returns404()
    {
        var ctrl = BuildController(Guid.NewGuid());

        var response = await ctrl.Duplicate(Guid.NewGuid(), CancellationToken.None);

        response.Should().BeOfType<NotFoundResult>();
    }

    // ── UTCID04 ── no JWT → 401 Unauthorized ──────────────────────────────
    [Fact]
    public async Task CVerify112_UTCID04_Duplicate_NoJwt_Returns401()
    {
        var ctrl = BuildController(null);

        var response = await ctrl.Duplicate(Guid.NewGuid(), CancellationToken.None);

        response.Should().BeOfType<UnauthorizedResult>();
    }
}
