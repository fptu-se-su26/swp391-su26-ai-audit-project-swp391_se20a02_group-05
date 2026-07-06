using System;
using System.Security.Claims;
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
/// Unit tests for WorkspaceController.GetWorkspaceDetails — CVerify-80 (4 UTCIDs).
/// GET /api/workspace/{organizationSlug} [AllowAnonymous] — public workspace profile by slug.
/// Controller-level test: reads from DbContext directly.
/// </summary>
public sealed class CVerify80_GetWorkspaceDetailsTests : IDisposable
{
    private readonly ApplicationDbContext _context;
    private readonly Mock<IOrganizationAuthorizationService> _authService = new();
    private readonly Mock<IStorageService> _storageService = new();

    public CVerify80_GetWorkspaceDetailsTests()
    {
        _context = new ApplicationDbContext(
            new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .ConfigureWarnings(w => w.Ignore(InMemoryEventId.TransactionIgnoredWarning))
                .Options);

        _storageService
            .Setup(s => s.GetSignedUrlAsync(It.IsAny<string>(), It.IsAny<TimeSpan>(), It.IsAny<System.Threading.CancellationToken>()))
            .ReturnsAsync((string)null!);

        _authService
            .Setup(a => a.AuthorizeAsync(It.IsAny<Guid>(), It.IsAny<Guid>(), It.IsAny<string>()))
            .ReturnsAsync(false); // default: not authorized as member
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

    private async Task<Organization> SeedOrgAsync(string slug, bool deleted = false)
    {
        var org = new Organization
        {
            Id = Guid.NewGuid(),
            Name = $"Org {slug}",
            Username = slug,
            TaxCode = $"TAX-{slug}",
            Email = $"{slug}@org.com",
            Status = "active",
            DeletedAt = deleted ? DateTimeOffset.UtcNow : null,
        };
        _context.Organizations.Add(org);
        await _context.SaveChangesAsync();
        return org;
    }

    // ── UTCID01 ───────────────────────────────────────────────────────────
    // Valid slug, anonymous visitor → 200 OK with public WorkspaceDetailsDto.
    [Fact]
    public async Task CVerify80_UTCID01_GetWorkspaceDetails_ValidSlug_ReturnsOkWithDto()
    {
        await SeedOrgAsync("techcorp");
        var ctrl = BuildController(); // anonymous

        var response = await ctrl.GetWorkspaceDetails("techcorp");

        response.Should().BeOfType<OkObjectResult>("valid slug returns 200 OK");
        var ok = (OkObjectResult)response;
        ok.Value.Should().NotBeNull();
    }

    // ── UTCID02 ───────────────────────────────────────────────────────────
    // Non-existent slug → 404 NotFound.
    [Fact]
    public async Task CVerify80_UTCID02_GetWorkspaceDetails_SlugNotFound_ReturnsNotFound()
    {
        var ctrl = BuildController();

        var response = await ctrl.GetWorkspaceDetails("ghost-org-slug");

        response.Should().BeOfType<NotFoundObjectResult>("non-existent slug returns 404");
    }

    // ── UTCID03 ───────────────────────────────────────────────────────────
    // AllowAnonymous — unauthenticated request → 200 OK (public endpoint, no auth required).
    [Fact]
    public async Task CVerify80_UTCID03_GetWorkspaceDetails_Anonymous_ReturnsPublicView()
    {
        await SeedOrgAsync("publicco");
        var ctrl = BuildController(new ClaimsPrincipal()); // no claims

        var response = await ctrl.GetWorkspaceDetails("publicco");

        response.Should().BeOfType<OkObjectResult>("AllowAnonymous — public orgs visible without JWT");
    }

    // ── UTCID04 ───────────────────────────────────────────────────────────
    // Business account JWT with matching orgId → 200 OK with OWNER role and full permissions.
    [Fact]
    public async Task CVerify80_UTCID04_GetWorkspaceDetails_BusinessOwner_ReturnsOwnerView()
    {
        var org = await SeedOrgAsync("mybizorg");

        var claims = new System.Collections.Generic.List<Claim>
        {
            new(ClaimTypes.NameIdentifier, org.Id.ToString()),
            new("actor_type", "business"),
        };
        var principal = new ClaimsPrincipal(new ClaimsIdentity(claims, "Test"));
        var ctrl = BuildController(principal);

        var response = await ctrl.GetWorkspaceDetails("mybizorg");

        var ok = response.Should().BeOfType<OkObjectResult>().Subject;
        var dto = ok.Value.Should().BeAssignableTo<WorkspaceDetailsDto>().Subject;
        dto.UserRole.Should().Be("OWNER", "business account is the owner of its own org");
    }
}
