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

namespace CVerify.API.UnitTests.Auth;

/// <summary>
/// Unit tests for WorkspaceController.ToggleFollowWorkspace — CVerify-82 (4 UTCIDs).
/// POST /api/workspace/{organizationSlug}/follow [Authorize] — toggles follow status.
/// </summary>
public sealed class CVerify82_FollowWorkspaceTests : IDisposable
{
    private readonly ApplicationDbContext _context;
    private readonly Mock<IOrganizationAuthorizationService> _authService = new();
    private readonly Mock<IStorageService> _storageService = new();

    public CVerify82_FollowWorkspaceTests()
    {
        _context = new ApplicationDbContext(
            new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .ConfigureWarnings(w => w.Ignore(InMemoryEventId.TransactionIgnoredWarning))
                .Options);
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

    private static ClaimsPrincipal Member(Guid userId) =>
        new(new ClaimsIdentity(new List<Claim> { new(ClaimTypes.NameIdentifier, userId.ToString()) }, "Test"));

    // ── UTCID01 ── exists, not following → 200 OK { isFollowing:true } ─────
    [Fact]
    public async Task CVerify82_UTCID01_Follow_NotYetFollowing_ReturnsFollowingTrue()
    {
        var org = await SeedOrgAsync("acme-corp");
        var ctrl = BuildController(Member(Guid.NewGuid()));

        var response = await ctrl.ToggleFollowWorkspace("acme-corp", CancellationToken.None);

        var ok = response.Should().BeOfType<OkObjectResult>().Subject;
        var dto = ok.Value.Should().BeOfType<FollowToggleResponseDto>().Subject;
        dto.IsFollowing.Should().BeTrue();
    }

    // ── UTCID02 ── exists, currently following → 200 OK { isFollowing:false } ─
    [Fact]
    public async Task CVerify82_UTCID02_Follow_AlreadyFollowing_TogglesToFalse()
    {
        var org = await SeedOrgAsync("acme-corp");
        var userId = Guid.NewGuid();
        _context.OrganizationFollowers.Add(new OrganizationFollower
        {
            UserId = userId,
            OrganizationId = org.Id,
            FollowedAt = DateTimeOffset.UtcNow,
        });
        await _context.SaveChangesAsync();
        var ctrl = BuildController(Member(userId));

        var response = await ctrl.ToggleFollowWorkspace("acme-corp", CancellationToken.None);

        var ok = response.Should().BeOfType<OkObjectResult>().Subject;
        var dto = ok.Value.Should().BeOfType<FollowToggleResponseDto>().Subject;
        dto.IsFollowing.Should().BeFalse();
    }

    // ── UTCID03 ── non-existent slug → 404 NotFound ───────────────────────
    [Fact]
    public async Task CVerify82_UTCID03_Follow_SlugNotFound_ReturnsNotFound()
    {
        var ctrl = BuildController(Member(Guid.NewGuid()));

        var response = await ctrl.ToggleFollowWorkspace("ghost-org", CancellationToken.None);

        response.Should().BeOfType<NotFoundObjectResult>();
    }

    // ── UTCID04 ── no JWT → 401 Unauthorized ──────────────────────────────
    [Fact]
    public async Task CVerify82_UTCID04_Follow_NoJwt_ReturnsUnauthorized()
    {
        await SeedOrgAsync("acme-corp");
        var ctrl = BuildController(); // anonymous

        var response = await ctrl.ToggleFollowWorkspace("acme-corp", CancellationToken.None);

        response.Should().BeOfType<UnauthorizedResult>();
    }
}
