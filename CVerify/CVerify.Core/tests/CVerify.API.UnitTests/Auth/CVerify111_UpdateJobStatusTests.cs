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
/// Unit tests for PublicJobController.UpdateStatus — CVerify-111 (5 UTCIDs).
/// PUT /api/v1/public/jobs/{id}/status [Authorize] — workspace member/admin only.
/// </summary>
public sealed class CVerify111_UpdateJobStatusTests : IDisposable
{
    private readonly ApplicationDbContext _context;

    public CVerify111_UpdateJobStatusTests()
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

    private async Task<JobVacancy> SeedJobAsync(string status)
    {
        var v = new JobVacancy
        {
            Id = Guid.NewGuid(), OrganizationId = Guid.NewGuid(), Title = "Backend Dev", Department = "Eng",
            WorkplaceType = "Hybrid", City = "Hanoi", Type = "Full-time", Salary = "x", SalaryMinMax = "0-0",
            Experience = "5+", Degree = "B", Category = "S", CoverUrl = "https://cover.example/x.png",
            Status = status, IsActive = status == "Published",
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

    private static UpdateJobStatusDto Dto(string status, bool active) => new() { Status = status, IsActive = active };

    // ── UTCID01 ── member closes an active job → 200 OK ───────────────────
    [Fact]
    public async Task CVerify111_UTCID01_UpdateStatus_MemberClose_ReturnsOk()
    {
        var job = await SeedJobAsync("Published");
        var userId = Guid.NewGuid();
        await AddMembershipAsync(job.OrganizationId, userId);
        var ctrl = BuildController(userId);

        var response = await ctrl.UpdateStatus(job.Id, Dto("Archived", false), CancellationToken.None);

        response.Should().BeOfType<OkObjectResult>();
    }

    // ── UTCID02 ── member re-activates a closed job → 200 OK ──────────────
    [Fact]
    public async Task CVerify111_UTCID02_UpdateStatus_MemberActivate_ReturnsOk()
    {
        var job = await SeedJobAsync("Archived");
        var userId = Guid.NewGuid();
        await AddMembershipAsync(job.OrganizationId, userId);
        var ctrl = BuildController(userId);

        var response = await ctrl.UpdateStatus(job.Id, Dto("Published", true), CancellationToken.None);

        response.Should().BeOfType<OkObjectResult>();
    }

    // ── UTCID03 ── regular candidate (not member) → 403 Forbidden ─────────
    [Fact]
    public async Task CVerify111_UTCID03_UpdateStatus_RegularCandidate_Returns403()
    {
        var job = await SeedJobAsync("Published");
        var ctrl = BuildController(Guid.NewGuid()); // no membership

        var response = await ctrl.UpdateStatus(job.Id, Dto("Archived", false), CancellationToken.None);

        response.Should().BeOfType<ForbidResult>();
    }

    // ── UTCID04 ── non-existent job → 404 NotFound ────────────────────────
    [Fact]
    public async Task CVerify111_UTCID04_UpdateStatus_NotFound_Returns404()
    {
        var ctrl = BuildController(Guid.NewGuid());

        var response = await ctrl.UpdateStatus(Guid.NewGuid(), Dto("Archived", false), CancellationToken.None);

        response.Should().BeOfType<NotFoundResult>();
    }

    // ── UTCID05 ── no JWT → 401 Unauthorized ──────────────────────────────
    [Fact]
    public async Task CVerify111_UTCID05_UpdateStatus_NoJwt_Returns401()
    {
        var ctrl = BuildController(null);

        var response = await ctrl.UpdateStatus(Guid.NewGuid(), Dto("Archived", false), CancellationToken.None);

        response.Should().BeOfType<UnauthorizedResult>();
    }
}
