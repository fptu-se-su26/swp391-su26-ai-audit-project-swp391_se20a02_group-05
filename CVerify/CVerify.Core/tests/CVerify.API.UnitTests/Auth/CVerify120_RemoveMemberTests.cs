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
using CVerify.API.Modules.Shared.Domain.Services;
using CVerify.API.Modules.Shared.Domain.Entities;
using CVerify.API.Modules.Shared.Persistence;
using CVerify.API.Modules.Shared.System.Services;

namespace CVerify.API.UnitTests.Auth;

/// <summary>
/// Unit tests for MemberController.RemoveMember — CVerify-120 (5 UTCIDs).
/// DELETE /api/organizations/{orgSlug}/members/{memberId} [Authorize]. Cannot remove the last Owner.
/// </summary>
public sealed class CVerify120_RemoveMemberTests : IDisposable
{
    private readonly ApplicationDbContext _context;
    private readonly Mock<IOrganizationAuthorizationService> _authService = new();

    public CVerify120_RemoveMemberTests()
    {
        _context = new ApplicationDbContext(
            new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .ConfigureWarnings(w => w.Ignore(InMemoryEventId.TransactionIgnoredWarning))
                .Options);
    }

    public void Dispose() => _context.Dispose();

    private MemberController BuildController(ClaimsPrincipal? user)
    {
        var ctrl = new MemberController(_context, _authService.Object, Mock.Of<IBusinessRoleService>(),
            Mock.Of<ICacheService>(), Mock.Of<IActivityEventPublisher>());
        ctrl.ControllerContext = new ControllerContext
        {
            HttpContext = new DefaultHttpContext { User = user ?? new ClaimsPrincipal() },
        };
        return ctrl;
    }

    private static ClaimsPrincipal Member(Guid userId) =>
        new(new ClaimsIdentity(new List<Claim> { new(ClaimTypes.NameIdentifier, userId.ToString()) }, "Test"));

    private void AllowPermission(bool allowed) =>
        _authService.Setup(a => a.AuthorizeAsync(It.IsAny<Guid>(), It.IsAny<Guid>(), It.IsAny<string>(),
            It.IsAny<string>(), It.IsAny<Guid?>(), It.IsAny<CancellationToken>())).ReturnsAsync(allowed);

    private async Task<Organization> SeedOrgAsync()
    {
        var org = new Organization
        {
            Id = Guid.NewGuid(), Name = "Acme", Username = "acme-corp", TaxCode = "TAX", Email = "a@org.com", Status = "active",
        };
        _context.Organizations.Add(org);
        await _context.SaveChangesAsync();
        return org;
    }

    private async Task SeedMemberAsync(Guid orgId, Guid memberId, bool owner = false)
    {
        _context.OrganizationMemberships.Add(new OrganizationMembership
        {
            OrganizationId = orgId, UserId = memberId, Role = owner ? "OWNER" : "MEMBER", Status = "active",
        });
        if (owner)
        {
            var role = new Role { Id = Guid.NewGuid(), Name = "owner", DisplayName = "Owner" };
            _context.Set<Role>().Add(role);
            _context.RoleAssignments.Add(new RoleAssignment
            {
                Id = Guid.NewGuid(), UserId = memberId, RoleId = role.Id, Role = role,
                ScopeType = "ORGANIZATION", ScopeId = orgId,
            });
        }
        await _context.SaveChangesAsync();
    }

    // ── UTCID01 ── regular member → 204 No Content ────────────────────────
    [Fact]
    public async Task CVerify120_UTCID01_Remove_RegularMember_Returns204()
    {
        var org = await SeedOrgAsync();
        var memberId = Guid.NewGuid();
        await SeedMemberAsync(org.Id, memberId);
        AllowPermission(true);
        var ctrl = BuildController(Member(Guid.NewGuid()));

        var response = await ctrl.RemoveMember("acme-corp", memberId, CancellationToken.None);

        response.Should().BeOfType<NoContentResult>();
    }

    // ── UTCID02 ── last Owner → 400 BadRequest ────────────────────────────
    [Fact]
    public async Task CVerify120_UTCID02_Remove_LastOwner_Returns400()
    {
        var org = await SeedOrgAsync();
        var memberId = Guid.NewGuid();
        await SeedMemberAsync(org.Id, memberId, owner: true);
        AllowPermission(true);
        var ctrl = BuildController(Member(Guid.NewGuid()));

        var response = await ctrl.RemoveMember("acme-corp", memberId, CancellationToken.None);

        response.Should().BeOfType<BadRequestObjectResult>();
    }

    // ── UTCID03 ── non-existent member → 404 NotFound ─────────────────────
    [Fact]
    public async Task CVerify120_UTCID03_Remove_NotFound_Returns404()
    {
        await SeedOrgAsync();
        AllowPermission(true);
        var ctrl = BuildController(Member(Guid.NewGuid()));

        var response = await ctrl.RemoveMember("acme-corp", Guid.NewGuid(), CancellationToken.None);

        response.Should().BeOfType<NotFoundObjectResult>();
    }

    // ── UTCID04 ── view-only permission → 403 Forbidden ───────────────────
    [Fact]
    public async Task CVerify120_UTCID04_Remove_ViewOnly_Returns403()
    {
        await SeedOrgAsync();
        AllowPermission(false);
        var ctrl = BuildController(Member(Guid.NewGuid()));

        var response = await ctrl.RemoveMember("acme-corp", Guid.NewGuid(), CancellationToken.None);

        response.Should().BeOfType<ForbidResult>();
    }

    // ── UTCID05 ── no JWT → 401 Unauthorized ──────────────────────────────
    [Fact]
    public async Task CVerify120_UTCID05_Remove_NoJwt_Returns401()
    {
        await SeedOrgAsync();
        var ctrl = BuildController(null);

        var response = await ctrl.RemoveMember("acme-corp", Guid.NewGuid(), CancellationToken.None);

        response.Should().BeOfType<UnauthorizedResult>();
    }
}
