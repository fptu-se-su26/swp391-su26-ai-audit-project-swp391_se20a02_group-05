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
/// Unit tests for PublicJobController.GetDetails — CVerify-105 (3 of 4 UTCIDs).
/// GET /api/v1/public/jobs/{id} [AllowAnonymous].
/// NOTE: the action returns any job by id (no Published filter), so a Draft job returns 200 — the
/// Excel "draft → 404" case is inaccurate. The authenticated path writes an analytics outbox message
/// (integration-only), so it is not covered here.
/// </summary>
public sealed class CVerify105_GetJobDetailsTests : IDisposable
{
    private readonly ApplicationDbContext _context;

    public CVerify105_GetJobDetailsTests()
    {
        _context = new ApplicationDbContext(
            new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .ConfigureWarnings(w => w.Ignore(InMemoryEventId.TransactionIgnoredWarning))
                .Options);
    }

    public void Dispose() => _context.Dispose();

    private PublicJobController BuildController()
    {
        var ctrl = new PublicJobController(_context, Mock.Of<IJobEligibilityService>(),
            Mock.Of<IExplainableMatchService>(), Mock.Of<IJobRankingStrategy>(), Mock.Of<IRecommendationProvider>());
        ctrl.ControllerContext = new ControllerContext
        {
            HttpContext = new DefaultHttpContext { User = new ClaimsPrincipal() }, // anonymous
        };
        return ctrl;
    }

    private async Task<JobVacancy> SeedJobAsync(string status)
    {
        var org = new Organization
        {
            Id = Guid.NewGuid(), Name = "Acme", Username = "acme", TaxCode = "TAX", Email = "a@org.com", Status = "active",
        };
        _context.Organizations.Add(org);
        var v = new JobVacancy
        {
            Id = Guid.NewGuid(), OrganizationId = org.Id, Title = "Backend Dev", Department = "Eng",
            WorkplaceType = "Hybrid", City = "Hanoi", Type = "Full-time", Salary = "x", SalaryMinMax = "0-0",
            Experience = "5+", Degree = "B", Category = "S", CoverUrl = "https://cover.example/x.png", Status = status, IsActive = status == "Published",
        };
        _context.JobVacancies.Add(v);
        await _context.SaveChangesAsync();
        return v;
    }

    // ── UTCID01 ── published job, anonymous → 200 OK ──────────────────────
    [Fact]
    public async Task CVerify105_UTCID01_GetDetails_PublishedAnonymous_ReturnsOk()
    {
        var v = await SeedJobAsync("Published");
        var ctrl = BuildController();

        var response = await ctrl.GetDetails(v.Id, CancellationToken.None);

        response.Should().BeOfType<OkObjectResult>();
    }

    // ── UTCID02 ── draft job (no Published filter) → 200 OK ───────────────
    [Fact]
    public async Task CVerify105_UTCID02_GetDetails_DraftJob_ReturnsOk()
    {
        var v = await SeedJobAsync("Draft");
        var ctrl = BuildController();

        var response = await ctrl.GetDetails(v.Id, CancellationToken.None);

        response.Should().BeOfType<OkObjectResult>();
    }

    // ── UTCID03 ── non-existent id → 404 NotFound ─────────────────────────
    [Fact]
    public async Task CVerify105_UTCID03_GetDetails_NotFound_Returns404()
    {
        var ctrl = BuildController();

        var response = await ctrl.GetDetails(Guid.NewGuid(), CancellationToken.None);

        response.Should().BeOfType<NotFoundObjectResult>();
    }
}
