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
/// Unit tests for WorkspaceController.GetPosts — CVerify-87 (3 UTCIDs).
/// GET /api/workspace/{organizationSlug}/posts [AllowAnonymous] — returns public workspace posts.
/// </summary>
public sealed class CVerify87_GetWorkspacePostsTests : IDisposable
{
    private readonly ApplicationDbContext _context;
    private readonly Mock<IOrganizationAuthorizationService> _authService = new();
    private readonly Mock<IStorageService> _storageService = new();

    public CVerify87_GetWorkspacePostsTests()
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

    private async Task SeedPostAsync(Guid orgId, string content)
    {
        _context.WorkspacePosts.Add(new WorkspacePost
        {
            Id = Guid.CreateVersion7(),
            OrganizationId = orgId,
            CreatedByUserId = Guid.NewGuid(),
            Category = "Announcement",
            Content = content,
            CreatedAt = DateTimeOffset.UtcNow,
            UpdatedAt = DateTimeOffset.UtcNow,
        });
        await _context.SaveChangesAsync();
    }

    // ── UTCID01 ── org has posts → 200 OK with populated list ─────────────
    [Fact]
    public async Task CVerify87_UTCID01_GetPosts_HasPosts_ReturnsList()
    {
        var org = await SeedOrgAsync("acme-corp");
        await SeedPostAsync(org.Id, "Post 1");
        await SeedPostAsync(org.Id, "Post 2");
        var ctrl = BuildController(); // anonymous

        var response = await ctrl.GetPosts("acme-corp", CancellationToken.None);

        var ok = response.Should().BeOfType<OkObjectResult>().Subject;
        ok.Value.Should().BeAssignableTo<List<WorkspacePostDto>>()
            .Which.Should().HaveCount(2);
    }

    // ── UTCID02 ── org has no posts → 200 OK with empty list ──────────────
    [Fact]
    public async Task CVerify87_UTCID02_GetPosts_NoPosts_ReturnsEmptyList()
    {
        await SeedOrgAsync("acme-corp");
        var ctrl = BuildController();

        var response = await ctrl.GetPosts("acme-corp", CancellationToken.None);

        var ok = response.Should().BeOfType<OkObjectResult>().Subject;
        ok.Value.Should().BeAssignableTo<List<WorkspacePostDto>>()
            .Which.Should().BeEmpty();
    }

    // ── UTCID03 ── non-existent slug → 404 NotFound ───────────────────────
    [Fact]
    public async Task CVerify87_UTCID03_GetPosts_SlugNotFound_ReturnsNotFound()
    {
        var ctrl = BuildController();

        var response = await ctrl.GetPosts("ghost-org", CancellationToken.None);

        response.Should().BeOfType<NotFoundObjectResult>();
    }
}
