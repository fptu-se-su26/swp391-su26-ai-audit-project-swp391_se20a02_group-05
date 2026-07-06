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
/// Unit tests for WorkspaceController.CreatePost — CVerify-86 (4 UTCIDs).
/// POST /api/workspace/{organizationSlug}/posts [Authorize] — creates a workspace feed post (members only).
/// </summary>
public sealed class CVerify86_CreateWorkspacePostTests : IDisposable
{
    private readonly ApplicationDbContext _context;
    private readonly Mock<IOrganizationAuthorizationService> _authService = new();
    private readonly Mock<IStorageService> _storageService = new();

    public CVerify86_CreateWorkspacePostTests()
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

    private async Task SeedMembershipAsync(Guid orgId, Guid userId, string role = "MEMBER")
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

    private static CreateWorkspacePostRequestDto ValidPost(string content = "Exciting news about our product launch!") =>
        new(Category: "Announcement", Content: content);

    // ── UTCID01 ── active member posts valid content → 200 OK WorkspacePostDto ─
    [Fact]
    public async Task CVerify86_UTCID01_CreatePost_ActiveMember_ReturnsOkWithDto()
    {
        var org = await SeedOrgAsync("acme-corp");
        var userId = Guid.NewGuid();
        await SeedMembershipAsync(org.Id, userId);
        var ctrl = BuildController(Member(userId));

        var response = await ctrl.CreatePost("acme-corp", ValidPost(), CancellationToken.None);

        var ok = response.Should().BeOfType<OkObjectResult>().Subject;
        ok.Value.Should().BeOfType<WorkspacePostDto>();
    }

    // ── UTCID02 ── authenticated non-member → 403 Forbidden ───────────────
    [Fact]
    public async Task CVerify86_UTCID02_CreatePost_NonMember_ReturnsForbid()
    {
        await SeedOrgAsync("acme-corp");
        var ctrl = BuildController(Member(Guid.NewGuid())); // no membership seeded

        var response = await ctrl.CreatePost("acme-corp", ValidPost(), CancellationToken.None);

        response.Should().BeOfType<ForbidResult>();
    }

    // ── UTCID03 ── empty content → 400 BadRequest ─────────────────────────
    [Fact]
    public async Task CVerify86_UTCID03_CreatePost_EmptyContent_ReturnsBadRequest()
    {
        await SeedOrgAsync("acme-corp");
        var ctrl = BuildController(Member(Guid.NewGuid()));

        var response = await ctrl.CreatePost("acme-corp", ValidPost(content: "   "), CancellationToken.None);

        response.Should().BeOfType<BadRequestObjectResult>();
    }

    // ── UTCID04 ── no JWT → 401 Unauthorized ──────────────────────────────
    [Fact]
    public async Task CVerify86_UTCID04_CreatePost_NoJwt_ReturnsUnauthorized()
    {
        await SeedOrgAsync("acme-corp");
        var ctrl = BuildController(); // anonymous

        var response = await ctrl.CreatePost("acme-corp", ValidPost(), CancellationToken.None);

        response.Should().BeOfType<UnauthorizedResult>();
    }
}
