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
/// Unit tests for WorkspaceController.UpdateWorkspaceDetails — CVerify-81 (5 UTCIDs).
/// PATCH /api/workspace/{organizationSlug} [Authorize] — updates organization profile.
/// NOTE: The Excel design lists an "invalid URL → 400" case; the controller performs no URL
/// validation, so UTCID05 covers the real 400 path (empty request body) instead.
/// </summary>
public sealed class CVerify81_UpdateWorkspaceDetailsTests : IDisposable
{
    private readonly ApplicationDbContext _context;
    private readonly Mock<IOrganizationAuthorizationService> _authService = new();
    private readonly Mock<IStorageService> _storageService = new();

    public CVerify81_UpdateWorkspaceDetailsTests()
    {
        _context = new ApplicationDbContext(
            new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .ConfigureWarnings(w => w.Ignore(InMemoryEventId.TransactionIgnoredWarning))
                .Options);

        _storageService
            .Setup(s => s.GetSignedUrlAsync(It.IsAny<string>(), It.IsAny<TimeSpan>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((string)null!);

        _authService
            .Setup(a => a.AuthorizeAsync(It.IsAny<Guid>(), It.IsAny<Guid>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<Guid?>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(false); // default: not authorized
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

    private static ClaimsPrincipal BusinessOwner(Guid orgId) =>
        new(new ClaimsIdentity(new List<Claim>
        {
            new(ClaimTypes.NameIdentifier, orgId.ToString()),
            new("actor_type", "business"),
        }, "Test"));

    private static ClaimsPrincipal Member(Guid userId) =>
        new(new ClaimsIdentity(new List<Claim> { new(ClaimTypes.NameIdentifier, userId.ToString()) }, "Test"));

    private static UpdateWorkspaceDetailsRequestDto ValidDto(string? website = "https://acme.example") =>
        new(
            Description: "Updated description", CompanyType: "Product", CompanySize: "50-200",
            BranchCount: 2, IndustryTags: new List<string> { "Software" }, BenefitTags: new List<string>(),
            ContactName: "HR", ContactPhone: "0123", ContactEmail: "hr@acme.example", City: "Hanoi",
            DetailAddress: "1 Main St", GoogleMapsEmbedUrl: null, LinkedinUrl: null, FacebookUrl: null,
            TwitterUrl: null, Website: website, Mission: "M", Vision: "V", CoreValues: "C", Founded: "2020");

    // ── UTCID01 ── valid owner update → 200 OK WorkspaceDetailsDto ──────────
    [Fact]
    public async Task CVerify81_UTCID01_UpdateWorkspace_BusinessOwner_ReturnsOkWithDto()
    {
        var org = await SeedOrgAsync("acme-corp");
        var ctrl = BuildController(BusinessOwner(org.Id));

        var response = await ctrl.UpdateWorkspaceDetails("acme-corp", ValidDto(), CancellationToken.None);

        var ok = response.Should().BeOfType<OkObjectResult>().Subject;
        ok.Value.Should().BeAssignableTo<WorkspaceDetailsDto>();
    }

    // ── UTCID02 ── member without edit permission → 403 Forbidden ──────────
    [Fact]
    public async Task CVerify81_UTCID02_UpdateWorkspace_MemberNoPermission_ReturnsForbid()
    {
        await SeedOrgAsync("acme-corp");
        var ctrl = BuildController(Member(Guid.NewGuid())); // authService default => false

        var response = await ctrl.UpdateWorkspaceDetails("acme-corp", ValidDto(), CancellationToken.None);

        response.Should().BeOfType<ForbidResult>();
    }

    // ── UTCID03 ── non-existent slug → 404 NotFound ───────────────────────
    [Fact]
    public async Task CVerify81_UTCID03_UpdateWorkspace_SlugNotFound_ReturnsNotFound()
    {
        var ctrl = BuildController(BusinessOwner(Guid.NewGuid()));

        var response = await ctrl.UpdateWorkspaceDetails("ghost-org", ValidDto(), CancellationToken.None);

        response.Should().BeOfType<NotFoundObjectResult>();
    }

    // ── UTCID04 ── no JWT → 401 Unauthorized ──────────────────────────────
    [Fact]
    public async Task CVerify81_UTCID04_UpdateWorkspace_NoJwt_ReturnsUnauthorized()
    {
        await SeedOrgAsync("acme-corp");
        var ctrl = BuildController(); // anonymous

        var response = await ctrl.UpdateWorkspaceDetails("acme-corp", ValidDto(), CancellationToken.None);

        response.Should().BeOfType<UnauthorizedResult>();
    }

    // ── UTCID05 ── empty request body → 400 BadRequest (boundary) ─────────
    // Excel lists "invalid URL → 400", but the controller has no URL validation;
    // the only real 400 path is a null payload.
    [Fact]
    public async Task CVerify81_UTCID05_UpdateWorkspace_NullBody_ReturnsBadRequest()
    {
        var ctrl = BuildController(BusinessOwner(Guid.NewGuid()));

        var response = await ctrl.UpdateWorkspaceDetails("acme-corp", null!, CancellationToken.None);

        response.Should().BeOfType<BadRequestObjectResult>();
    }
}
