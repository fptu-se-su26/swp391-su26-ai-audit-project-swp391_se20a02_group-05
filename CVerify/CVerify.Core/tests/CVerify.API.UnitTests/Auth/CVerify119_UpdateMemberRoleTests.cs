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
/// Unit tests for MemberController.UpdateMember — CVerify-119 (6 UTCIDs).
/// PUT /api/organizations/{orgSlug}/members/{memberId} [Authorize]. Cannot suspend the last Owner.
/// </summary>
public sealed class CVerify119_UpdateMemberRoleTests : IDisposable
{
    private readonly ApplicationDbContext _context;
    private readonly Mock<IOrganizationAuthorizationService> _authService = new();

    public CVerify119_UpdateMemberRoleTests()
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

    private async Task SeedMemberAsync(Guid orgId, Guid memberId, string status = "active", bool owner = false)
    {
        _context.OrganizationMemberships.Add(new OrganizationMembership
        {
            OrganizationId = orgId, UserId = memberId, Role = owner ? "OWNER" : "MEMBER", Status = status,
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

    // ── UTCID01 ── active member → suspended → 204 No Content ─────────────
    [Fact]
    public async Task CVerify119_UTCID01_Update_Suspend_Returns204()
    {
        var org = await SeedOrgAsync();
        var memberId = Guid.NewGuid();
        await SeedMemberAsync(org.Id, memberId, "active");
        AllowPermission(true);
        var ctrl = BuildController(Member(Guid.NewGuid()));

        var response = await ctrl.UpdateMember("acme-corp", memberId, new UpdateMemberDto("suspended"), CancellationToken.None);

        response.Should().BeOfType<NoContentResult>();
    }

    // ── UTCID02 ── suspended member → active → 204 No Content ─────────────
    [Fact]
    public async Task CVerify119_UTCID02_Update_Reactivate_Returns204()
    {
        var org = await SeedOrgAsync();
        var memberId = Guid.NewGuid();
        await SeedMemberAsync(org.Id, memberId, "suspended");
        AllowPermission(true);
        var ctrl = BuildController(Member(Guid.NewGuid()));

        var response = await ctrl.UpdateMember("acme-corp", memberId, new UpdateMemberDto("active"), CancellationToken.None);

        response.Should().BeOfType<NoContentResult>();
    }

    // ── UTCID03 ── last active Owner → suspended → 400 BadRequest ─────────
    [Fact]
    public async Task CVerify119_UTCID03_Update_LastOwner_Returns400()
    {
        var org = await SeedOrgAsync();
        var memberId = Guid.NewGuid();
        await SeedMemberAsync(org.Id, memberId, "active", owner: true);
        AllowPermission(true);
        var ctrl = BuildController(Member(Guid.NewGuid()));

        var response = await ctrl.UpdateMember("acme-corp", memberId, new UpdateMemberDto("suspended"), CancellationToken.None);

        response.Should().BeOfType<BadRequestObjectResult>();
    }

    // ── UTCID04 ── non-existent member → 404 NotFound ─────────────────────
    [Fact]
    public async Task CVerify119_UTCID04_Update_NotFound_Returns404()
    {
        await SeedOrgAsync();
        AllowPermission(true);
        var ctrl = BuildController(Member(Guid.NewGuid()));

        var response = await ctrl.UpdateMember("acme-corp", Guid.NewGuid(), new UpdateMemberDto("suspended"), CancellationToken.None);

        response.Should().BeOfType<NotFoundObjectResult>();
    }

    // ── UTCID05 ── view-only permission → 403 Forbidden ───────────────────
    [Fact]
    public async Task CVerify119_UTCID05_Update_ViewOnly_Returns403()
    {
        await SeedOrgAsync();
        AllowPermission(false);
        var ctrl = BuildController(Member(Guid.NewGuid()));

        var response = await ctrl.UpdateMember("acme-corp", Guid.NewGuid(), new UpdateMemberDto("suspended"), CancellationToken.None);

        response.Should().BeOfType<ForbidResult>();
    }

    // ── UTCID06 ── no JWT → 401 Unauthorized ──────────────────────────────
    [Fact]
    public async Task CVerify119_UTCID06_Update_NoJwt_Returns401()
    {
        await SeedOrgAsync();
        var ctrl = BuildController(null);

        var response = await ctrl.UpdateMember("acme-corp", Guid.NewGuid(), new UpdateMemberDto("suspended"), CancellationToken.None);

        response.Should().BeOfType<UnauthorizedResult>();
    }
}
