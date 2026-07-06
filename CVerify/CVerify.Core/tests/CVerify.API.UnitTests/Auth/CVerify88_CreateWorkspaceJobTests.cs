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
using CVerify.API.Modules.Auth.Controllers;
using CVerify.API.Modules.Auth.DTOs;
using CVerify.API.Modules.Auth.Services;
using CVerify.API.Modules.Shared.Domain.Entities;
using CVerify.API.Modules.Shared.Persistence;
using CVerify.API.Modules.Shared.Storage.Interfaces;
using CVerify.API.Modules.Shared.System.DTOs;

namespace CVerify.API.UnitTests.Auth;

/// <summary>
/// Unit tests for WorkspaceController.CreateJob — CVerify-88 (4 UTCIDs).
/// POST /api/workspace/{organizationSlug}/jobs [Authorize] — creates a job vacancy (members only).
/// </summary>
public sealed class CVerify88_CreateWorkspaceJobTests : IDisposable
{
    private readonly ApplicationDbContext _context;
    private readonly Mock<IOrganizationAuthorizationService> _authService = new();
    private readonly Mock<IStorageService> _storageService = new();

    public CVerify88_CreateWorkspaceJobTests()
    {
        _context = new ApplicationDbContext(
            new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .ConfigureWarnings(w => w.Ignore(InMemoryEventId.TransactionIgnoredWarning))
                .Options);

        _storageService
            .Setup(s => s.GetSignedUrlAsync(It.IsAny<string>(), It.IsAny<TimeSpan>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((string)null!);
    }

    public void Dispose() => _context.Dispose();

    private WorkspaceController BuildController(ClaimsPrincipal? user = null)
    {
        var ctrl = new WorkspaceController(_context, _authService.Object, _storageService.Object);
        ctrl.ControllerContext = new ControllerContext
        {
            HttpContext = new DefaultHttpContext { User = user ?? new ClaimsPrincipal() },
        };
        return ctrl;
    }

    private async Task<Organization> SeedOrgAsync(string slug)
    {
        var org = new Organization
        {
            Id = Guid.NewGuid(),
            Name = $"Org {slug}",
            Username = slug,
            TaxCode = $"TAX-{slug}",
            Email = $"{slug}@org.com",
            Status = "active",
        };
        _context.Organizations.Add(org);
        await _context.SaveChangesAsync();
        return org;
    }

    private async Task SeedMembershipAsync(Guid orgId, Guid userId, string role = "HR")
    {
        _context.OrganizationMemberships.Add(new OrganizationMembership
        {
            OrganizationId = orgId,
            UserId = userId,
            Role = role,
            Status = "active",
        });
        await _context.SaveChangesAsync();
    }

    private static ClaimsPrincipal Member(Guid userId) =>
        new(new ClaimsIdentity(new List<Claim> { new(ClaimTypes.NameIdentifier, userId.ToString()) }, "Test"));

    private static CreateJobRequestDto Job(string? title) =>
        new(
            Title: title!, Department: "Engineering", WorkplaceType: "Hybrid", City: "Hanoi",
            Type: "Full-time", Salary: "Competitive", SalaryMinMax: "1000-2000", Headcount: 1,
            Gender: "Any", Experience: "2y", Degree: "Bachelor", Category: "Backend",
            Description: new List<string>(), Requirements: new List<string>(), Benefits: new List<string>(),
            Tags: new List<string>(), Skills: new List<string>(), CoverUrl: "https://cover.example/x.png");

    // ── UTCID01 ── active member valid job → 200 OK JobVacancyDto ─────────
    [Fact]
    public async Task CVerify88_UTCID01_CreateJob_ActiveMember_ReturnsOkWithDto()
    {
        var org = await SeedOrgAsync("acme-corp");
        var userId = Guid.NewGuid();
        await SeedMembershipAsync(org.Id, userId);
        var ctrl = BuildController(Member(userId));

        var response = await ctrl.CreateJob("acme-corp", Job("Backend Developer"), CancellationToken.None);

        var ok = response.Should().BeOfType<OkObjectResult>().Subject;
        ok.Value.Should().BeOfType<JobVacancyDto>();
    }

    // ── UTCID02 ── authenticated non-member → 403 Forbidden ───────────────
    [Fact]
    public async Task CVerify88_UTCID02_CreateJob_NonMember_ReturnsForbid()
    {
        await SeedOrgAsync("acme-corp");
        var ctrl = BuildController(Member(Guid.NewGuid())); // no membership seeded

        var response = await ctrl.CreateJob("acme-corp", Job("Backend Developer"), CancellationToken.None);

        response.Should().BeOfType<ForbidResult>();
    }

    // ── UTCID03 ── null title → 400 BadRequest ────────────────────────────
    [Fact]
    public async Task CVerify88_UTCID03_CreateJob_NullTitle_ReturnsBadRequest()
    {
        await SeedOrgAsync("acme-corp");
        var ctrl = BuildController(Member(Guid.NewGuid()));

        var response = await ctrl.CreateJob("acme-corp", Job(null), CancellationToken.None);

        response.Should().BeOfType<BadRequestObjectResult>();
    }

    // ── UTCID04 ── no JWT → 401 Unauthorized ──────────────────────────────
    [Fact]
    public async Task CVerify88_UTCID04_CreateJob_NoJwt_ReturnsUnauthorized()
    {
        await SeedOrgAsync("acme-corp");
        var ctrl = BuildController(); // anonymous

        var response = await ctrl.CreateJob("acme-corp", Job("Backend Developer"), CancellationToken.None);

        response.Should().BeOfType<UnauthorizedResult>();
    }
}
