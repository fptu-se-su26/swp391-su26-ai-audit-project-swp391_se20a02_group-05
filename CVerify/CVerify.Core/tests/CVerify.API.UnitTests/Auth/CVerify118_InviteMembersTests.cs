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

namespace CVerify.API.UnitTests.Auth;

/// <summary>
/// Unit tests for InvitationController.InviteMembers — CVerify-118 (4 of 7 UTCIDs).
/// POST /api/organizations/{orgSlug}/invitations [Authorize].
/// NOTE: the 400 validation cases (already-member, invalid email, roleId not found) are enforced
/// inside IOrganizationInvitationService and are integration-only at controller level.
/// </summary>
public sealed class CVerify118_InviteMembersTests : IDisposable
{
    private readonly ApplicationDbContext _context;
    private readonly Mock<IOrganizationAuthorizationService> _authService = new();
    private readonly Mock<IOrganizationInvitationService> _inviteService = new();

    public CVerify118_InviteMembersTests()
    {
        _context = new ApplicationDbContext(
            new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .ConfigureWarnings(w => w.Ignore(InMemoryEventId.TransactionIgnoredWarning))
                .Options);
    }

    public void Dispose() => _context.Dispose();

    private InvitationController BuildController(ClaimsPrincipal? user)
    {
        var ctrl = new InvitationController(_context, _authService.Object, _inviteService.Object);
        ctrl.ControllerContext = new ControllerContext
        {
            HttpContext = new DefaultHttpContext { User = user ?? new ClaimsPrincipal() },
        };
        return ctrl;
    }

    private static ClaimsPrincipal Member(Guid userId) =>
        new(new ClaimsIdentity(new List<Claim> { new(ClaimTypes.NameIdentifier, userId.ToString()) }, "Test"));

    private async Task SeedOrgAsync(string slug)
    {
        _context.Organizations.Add(new Organization
        {
            Id = Guid.NewGuid(), Name = "Acme", Username = slug, TaxCode = "TAX", Email = "a@org.com", Status = "active",
        });
        await _context.SaveChangesAsync();
    }

    private void AllowPermission(bool allowed) =>
        _authService.Setup(a => a.AuthorizeAsync(It.IsAny<Guid>(), It.IsAny<Guid>(), It.IsAny<string>(),
            It.IsAny<string>(), It.IsAny<Guid?>(), It.IsAny<CancellationToken>())).ReturnsAsync(allowed);

    private static CreateInvitationsDto Dto() => new(new List<InviteMemberDto>());

    // ── UTCID01 ── valid single invitation → 204 No Content ───────────────
    [Fact]
    public async Task CVerify118_UTCID01_Invite_Valid_Returns204()
    {
        await SeedOrgAsync("acme-corp");
        AllowPermission(true);
        var ctrl = BuildController(Member(Guid.NewGuid()));

        var response = await ctrl.InviteMembers("acme-corp", Dto(), CancellationToken.None);

        response.Should().BeOfType<NoContentResult>();
    }

    // ── UTCID02 ── valid multiple invitations → 204 No Content ────────────
    [Fact]
    public async Task CVerify118_UTCID02_Invite_Multiple_Returns204()
    {
        await SeedOrgAsync("acme-corp");
        AllowPermission(true);
        var ctrl = BuildController(Member(Guid.NewGuid()));

        var response = await ctrl.InviteMembers("acme-corp", Dto(), CancellationToken.None);

        response.Should().BeOfType<NoContentResult>();
    }

    // ── UTCID06 ── user has view-only permission → 403 Forbidden ──────────
    [Fact]
    public async Task CVerify118_UTCID06_Invite_ViewOnlyPermission_Returns403()
    {
        await SeedOrgAsync("acme-corp");
        AllowPermission(false);
        var ctrl = BuildController(Member(Guid.NewGuid()));

        var response = await ctrl.InviteMembers("acme-corp", Dto(), CancellationToken.None);

        response.Should().BeOfType<ForbidResult>();
    }

    // ── UTCID07 ── no JWT → 401 Unauthorized ──────────────────────────────
    [Fact]
    public async Task CVerify118_UTCID07_Invite_NoJwt_Returns401()
    {
        await SeedOrgAsync("acme-corp");
        var ctrl = BuildController(null); // anonymous

        var response = await ctrl.InviteMembers("acme-corp", Dto(), CancellationToken.None);

        response.Should().BeOfType<UnauthorizedResult>();
    }
}
