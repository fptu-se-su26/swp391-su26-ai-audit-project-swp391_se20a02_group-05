using System;
using System.Collections.Generic;
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
/// Unit tests for WorkspaceController.GetMyOrganizations — CVerify-79 (4 UTCIDs).
/// GET /api/workspace/my-organizations [Authorize] — returns the caller's organization memberships.
/// Controller-level test: reads directly from DbContext.
/// </summary>
public sealed class CVerify79_GetMyOrganizationsTests : IDisposable
{
    private readonly ApplicationDbContext _context;
    private readonly Mock<IOrganizationAuthorizationService> _authService = new();
    private readonly Mock<IStorageService> _storageService = new();

    public CVerify79_GetMyOrganizationsTests()
    {
        _context = new ApplicationDbContext(
            new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .ConfigureWarnings(w => w.Ignore(InMemoryEventId.TransactionIgnoredWarning))
                .Options);
    }

    public void Dispose() => _context.Dispose();

    private WorkspaceController BuildController(Guid userId, string actorType = "candidate")
    {
        var claims = new List<Claim>
        {
            new(ClaimTypes.NameIdentifier, userId.ToString()),
            new("actor_type", actorType),
        };
        var identity = new ClaimsIdentity(claims, "Test");
        var principal = new ClaimsPrincipal(identity);

        var ctrl = new WorkspaceController(_context, _authService.Object, _storageService.Object);
        ctrl.ControllerContext = new ControllerContext
        {
            HttpContext = new DefaultHttpContext { User = principal },
        };
        return ctrl;
    }

    // ── UTCID01 ───────────────────────────────────────────────────────────
    // Candidate user with active membership in 2 orgs → 200 OK with 2 items.
    [Fact]
    public async Task CVerify79_UTCID01_GetMyOrganizations_CandidateWithMemberships_ReturnsList()
    {
        var userId = Guid.NewGuid();
        var org1 = new Organization { Id = Guid.NewGuid(), Name = "OrgA", Username = "orga", TaxCode = "TAX1", Email = "a@org.com", Status = "active" };
        var org2 = new Organization { Id = Guid.NewGuid(), Name = "OrgB", Username = "orgb", TaxCode = "TAX2", Email = "b@org.com", Status = "active" };
        _context.Organizations.AddRange(org1, org2);
        _context.OrganizationMemberships.Add(new OrganizationMembership { Id = Guid.NewGuid(), OrganizationId = org1.Id, UserId = userId, Role = "MEMBER", Status = "active" });
        _context.OrganizationMemberships.Add(new OrganizationMembership { Id = Guid.NewGuid(), OrganizationId = org2.Id, UserId = userId, Role = "MEMBER", Status = "active" });
        await _context.SaveChangesAsync();

        var ctrl = BuildController(userId, "candidate");
        var response = await ctrl.GetMyOrganizations();

        var ok = response.Should().BeOfType<OkObjectResult>().Subject;
        var list = ok.Value.Should().BeAssignableTo<List<LinkedOrganizationDto>>().Subject;
        list.Should().HaveCount(2);
    }

    // ── UTCID02 ───────────────────────────────────────────────────────────
    // Candidate with no memberships → 200 OK with empty list.
    [Fact]
    public async Task CVerify79_UTCID02_GetMyOrganizations_NoMemberships_ReturnsEmptyList()
    {
        var userId = Guid.NewGuid();
        var ctrl = BuildController(userId, "candidate");

        var response = await ctrl.GetMyOrganizations();

        var ok = response.Should().BeOfType<OkObjectResult>().Subject;
        var list = ok.Value.Should().BeAssignableTo<List<LinkedOrganizationDto>>().Subject;
        list.Should().BeEmpty();
    }

    // ── UTCID03 ───────────────────────────────────────────────────────────
    // Business account (actor_type='business') → own org returned as single-item list.
    [Fact]
    public async Task CVerify79_UTCID03_GetMyOrganizations_BusinessAccount_ReturnsOwnOrg()
    {
        var orgId = Guid.NewGuid();
        _context.Organizations.Add(new Organization
        {
            Id = orgId, Name = "MyBiz", Username = "mybiz",
            TaxCode = "BIZTAX", Email = "biz@test.com", Status = "active",
        });
        await _context.SaveChangesAsync();

        var ctrl = BuildController(orgId, "business"); // userId == orgId for business accounts

        var response = await ctrl.GetMyOrganizations();

        var ok = response.Should().BeOfType<OkObjectResult>().Subject;
        var list = ok.Value.Should().BeAssignableTo<List<LinkedOrganizationDto>>().Subject;
        list.Should().HaveCount(1);
        list[0].Name.Should().Be("MyBiz");
    }

    // ── UTCID04 ───────────────────────────────────────────────────────────
    // No JWT → controller [Authorize] returns 401.
    // Simulated at controller level: no NameIdentifier claim → Unauthorized.
    [Fact]
    public async Task CVerify79_UTCID04_GetMyOrganizations_NoJwt_ReturnsUnauthorized()
    {
        var ctrl = new WorkspaceController(_context, _authService.Object, _storageService.Object);
        ctrl.ControllerContext = new ControllerContext
        {
            HttpContext = new DefaultHttpContext { User = new ClaimsPrincipal() }, // no claims
        };

        var response = await ctrl.GetMyOrganizations();

        response.Should().BeOfType<UnauthorizedResult>("missing NameIdentifier claim triggers Unauthorized");
    }
}
