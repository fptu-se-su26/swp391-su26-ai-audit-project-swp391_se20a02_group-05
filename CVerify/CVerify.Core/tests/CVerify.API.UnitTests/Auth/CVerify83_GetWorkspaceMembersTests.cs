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
using CVerify.API.Modules.Auth.Services;
using CVerify.API.Modules.Shared.Domain.Entities;
using CVerify.API.Modules.Shared.Persistence;
using CVerify.API.Modules.Shared.Storage.Interfaces;

namespace CVerify.API.UnitTests.Auth;

/// <summary>
/// Unit tests for WorkspaceController.GetWorkspaceMembers — CVerify-83 (3 of 5 UTCIDs).
/// GET /api/workspace/{organizationSlug}/members — paginated member list.
/// NOTE: The happy paths (public listing / member listing / search / pagination) execute a raw
/// SQL query (Database.SqlQueryRaw against user_profiles) that the EF in-memory provider cannot
/// run — those cases are integration-only. The authorization/not-found guards run BEFORE the raw
/// SQL and are covered here at unit level.
/// </summary>
public sealed class CVerify83_GetWorkspaceMembersTests : IDisposable
{
    private readonly ApplicationDbContext _context;
    private readonly Mock<IOrganizationAuthorizationService> _authService = new();
    private readonly Mock<IStorageService> _storageService = new();

    public CVerify83_GetWorkspaceMembersTests()
    {
        _context = new ApplicationDbContext(
            new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .ConfigureWarnings(w => w.Ignore(InMemoryEventId.TransactionIgnoredWarning))
                .Options);

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

    private static ClaimsPrincipal Member(Guid userId) =>
        new(new ClaimsIdentity(new List<Claim> { new(ClaimTypes.NameIdentifier, userId.ToString()) }, "Test"));

    private static ClaimsPrincipal BusinessActor(Guid actorId) =>
        new(new ClaimsIdentity(new List<Claim>
        {
            new(ClaimTypes.NameIdentifier, actorId.ToString()),
            new("actor_type", "business"),
        }, "Test"));

    // ── UTCID01 ── non-existent slug → 404 NotFound ───────────────────────
    [Fact]
    public async Task CVerify83_UTCID01_GetMembers_SlugNotFound_ReturnsNotFound()
    {
        var ctrl = BuildController(Member(Guid.NewGuid()));

        var response = await ctrl.GetWorkspaceMembers("ghost-org", page: 1, pageSize: 10, search: null, publicOnly: false);

        response.Should().BeOfType<NotFoundObjectResult>();
    }

    // ── UTCID02 ── authenticated member without view permission → 403 ──────
    [Fact]
    public async Task CVerify83_UTCID02_GetMembers_MemberNoPermission_ReturnsForbid()
    {
        await SeedOrgAsync("acme-corp");
        var ctrl = BuildController(Member(Guid.NewGuid())); // authService default => false

        var response = await ctrl.GetWorkspaceMembers("acme-corp", page: 1, pageSize: 10, search: null, publicOnly: false);

        response.Should().BeOfType<ForbidResult>();
    }

    // ── UTCID03 ── business actor for a different org → 403 (boundary) ─────
    [Fact]
    public async Task CVerify83_UTCID03_GetMembers_BusinessNotOwner_ReturnsForbid()
    {
        await SeedOrgAsync("acme-corp");
        var ctrl = BuildController(BusinessActor(Guid.NewGuid())); // not this org's id

        var response = await ctrl.GetWorkspaceMembers("acme-corp", page: 1, pageSize: 10, search: null, publicOnly: false);

        response.Should().BeOfType<ForbidResult>();
    }
}
