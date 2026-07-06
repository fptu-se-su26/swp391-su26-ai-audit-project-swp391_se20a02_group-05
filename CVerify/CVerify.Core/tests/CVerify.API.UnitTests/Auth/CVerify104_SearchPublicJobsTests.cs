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
/// Unit tests for PublicJobController.Search — CVerify-104 (6 UTCIDs).
/// GET /api/v1/public/jobs [AllowAnonymous] — public job search with filters/pagination.
/// </summary>
public sealed class CVerify104_SearchPublicJobsTests : IDisposable
{
    private readonly ApplicationDbContext _context;

    public CVerify104_SearchPublicJobsTests()
    {
        _context = new ApplicationDbContext(
            new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .ConfigureWarnings(w => w.Ignore(InMemoryEventId.TransactionIgnoredWarning))
                .Options);
    }

    public void Dispose() => _context.Dispose();

    private PublicJobController BuildController(ClaimsPrincipal? user = null)
    {
        var ctrl = new PublicJobController(_context, Mock.Of<IJobEligibilityService>(),
            Mock.Of<IExplainableMatchService>(), Mock.Of<IJobRankingStrategy>(), Mock.Of<IRecommendationProvider>());
        ctrl.ControllerContext = new ControllerContext
        {
            HttpContext = new DefaultHttpContext { User = user ?? new ClaimsPrincipal() },
        };
        return ctrl;
    }

    private async Task SeedPublishedJobsAsync()
    {
        var org = new Organization
        {
            Id = Guid.NewGuid(), Name = "Acme", Username = "acme", TaxCode = "TAX", Email = "a@org.com", Status = "active",
        };
        _context.Organizations.Add(org);
        for (var i = 0; i < 2; i++)
        {
            _context.JobVacancies.Add(new JobVacancy
            {
                Id = Guid.NewGuid(), OrganizationId = org.Id, Title = $"Backend Dev {i}",
                Department = "Engineering", WorkplaceType = "Hybrid", City = "Ho Chi Minh City",
                Type = "Full-time", Salary = "x", SalaryMinMax = "0-0", Experience = "Senior 5+ years",
                Degree = "Bachelor", Category = "Software", CoverUrl = "https://cover.example/x.png", Status = "Published", IsActive = true,
            });
        }
        await _context.SaveChangesAsync();
    }

    private static ClaimsPrincipal Authenticated() =>
        new(new ClaimsIdentity(new List<Claim> { new(ClaimTypes.NameIdentifier, Guid.NewGuid().ToString()) }, "Test"));

    // ── UTCID01 ── anonymous, no params → 200 OK ─────────────────────────
    [Fact]
    public async Task CVerify104_UTCID01_Search_AnonymousNoParams_ReturnsOk()
    {
        await SeedPublishedJobsAsync();
        var ctrl = BuildController();

        var response = await ctrl.Search(null, null, null, null, null, 1, 10, CancellationToken.None);

        response.Should().BeOfType<OkObjectResult>();
    }

    // ── UTCID02 ── authenticated (no search profile) → 200 OK ─────────────
    [Fact]
    public async Task CVerify104_UTCID02_Search_Authenticated_ReturnsOk()
    {
        await SeedPublishedJobsAsync();
        var ctrl = BuildController(Authenticated());

        var response = await ctrl.Search(null, null, null, null, null, 1, 10, CancellationToken.None);

        response.Should().BeOfType<OkObjectResult>();
    }

    // ── UTCID03 ── location filter → 200 OK ───────────────────────────────
    [Fact]
    public async Task CVerify104_UTCID03_Search_LocationFilter_ReturnsOk()
    {
        await SeedPublishedJobsAsync();
        var ctrl = BuildController();

        var response = await ctrl.Search(null, "Ho Chi Minh City", null, null, null, 1, 10, CancellationToken.None);

        response.Should().BeOfType<OkObjectResult>();
    }

    // ── UTCID04 ── employmentType filter → 200 OK ─────────────────────────
    [Fact]
    public async Task CVerify104_UTCID04_Search_EmploymentTypeFilter_ReturnsOk()
    {
        await SeedPublishedJobsAsync();
        var ctrl = BuildController();

        var response = await ctrl.Search(null, null, null, "Full-time", null, 1, 10, CancellationToken.None);

        response.Should().BeOfType<OkObjectResult>();
    }

    // ── UTCID05 ── empty query string → 200 OK (boundary) ─────────────────
    [Fact]
    public async Task CVerify104_UTCID05_Search_EmptyQuery_ReturnsOk()
    {
        await SeedPublishedJobsAsync();
        var ctrl = BuildController();

        var response = await ctrl.Search("", null, null, null, null, 1, 10, CancellationToken.None);

        response.Should().BeOfType<OkObjectResult>();
    }

    // ── UTCID06 ── page far beyond results → 200 OK, empty page (boundary) ─
    [Fact]
    public async Task CVerify104_UTCID06_Search_HighPage_ReturnsOk()
    {
        await SeedPublishedJobsAsync();
        var ctrl = BuildController();

        var response = await ctrl.Search(null, null, null, null, null, 9999, 10, CancellationToken.None);

        response.Should().BeOfType<OkObjectResult>();
    }
}
