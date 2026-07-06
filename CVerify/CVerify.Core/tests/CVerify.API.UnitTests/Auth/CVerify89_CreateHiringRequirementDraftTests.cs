using System;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.Extensions.Logging;
using Moq;
using StackExchange.Redis;
using Xunit;
using CVerify.API.Modules.Shared.Domain.Entities;
using CVerify.API.Modules.Shared.Persistence;
using CVerify.API.Modules.Shared.System.DTOs;
using CVerify.API.Modules.Shared.System.Services;

namespace CVerify.API.UnitTests.Auth;

/// <summary>
/// Unit tests for HiringRequirementService.CreateDraftAsync — CVerify-89 (5 UTCIDs).
/// POST /api/v1/hiring-requirements [Authorize] — creates a draft hiring requirement.
/// Tested at the service level (the controller only maps exceptions to HTTP codes).
/// NOTE: the real request DTO keys off OrganizationSlug (not workspaceId as the Excel design
/// states); a missing org/workspace surfaces as KeyNotFoundException → HTTP 404/400.
/// </summary>
public sealed class CVerify89_CreateHiringRequirementDraftTests : IDisposable
{
    private readonly ApplicationDbContext _context;
    private readonly Mock<ICapabilityCatalogService> _catalogService = new();

    public CVerify89_CreateHiringRequirementDraftTests()
    {
        _context = new ApplicationDbContext(
            new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .ConfigureWarnings(w => w.Ignore(InMemoryEventId.TransactionIgnoredWarning))
                .Options);
    }

    public void Dispose() => _context.Dispose();

    private HiringRequirementService BuildService() => new(
        _context,
        _catalogService.Object,
        Mock.Of<IHttpClientFactory>(),
        Mock.Of<IHmacSignatureService>(),
        Mock.Of<IConnectionMultiplexer>(),
        Mock.Of<ILogger<HiringRequirementService>>(),
        Mock.Of<IAiStreamingSessionService>(),
        Mock.Of<IAiCancellationManager>());

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

    private async Task SeedWorkspaceAsync(Guid orgId)
    {
        _context.Workspaces.Add(new Workspace
        {
            Id = Guid.NewGuid(),
            OrganizationId = orgId,
            DisplayName = "Default Workspace",
            Slug = "default",
        });
        await _context.SaveChangesAsync();
    }

    private static CreateHiringRequirementRequestDto Request(string slug, string title = "Backend Dev") =>
        new(OrganizationSlug: slug, Title: title, Department: "Engineering", Seniority: "Senior",
            WorkplaceType: "Hybrid", City: "Hanoi", EmploymentType: "Full-Time", Headcount: 2);

    // ── UTCID01 ── org + workspace exist → returns Draft v1 ───────────────
    [Fact]
    public async Task CVerify89_UTCID01_CreateDraft_ValidOrgAndWorkspace_ReturnsDraft()
    {
        var org = await SeedOrgAsync("acme-corp");
        await SeedWorkspaceAsync(org.Id);
        var sut = BuildService();

        var result = await sut.CreateDraftAsync(Request("acme-corp"), Guid.NewGuid(), CancellationToken.None);

        result.Should().NotBeNull();
        result.Status.Should().Be("Draft");
        result.Version.Should().Be(1);
        result.OrganizationId.Should().Be(org.Id);
    }

    // ── UTCID02 ── draft is persisted to the database ─────────────────────
    [Fact]
    public async Task CVerify89_UTCID02_CreateDraft_PersistsToDatabase()
    {
        var org = await SeedOrgAsync("acme-corp");
        await SeedWorkspaceAsync(org.Id);
        var sut = BuildService();

        var result = await sut.CreateDraftAsync(Request("acme-corp", "Platform Engineer"), Guid.NewGuid(), CancellationToken.None);

        var persisted = await _context.HiringRequirements.FindAsync(result.Id);
        persisted.Should().NotBeNull("the draft must be saved");
        persisted!.Title.Should().Be("Platform Engineer");
    }

    // ── UTCID03 ── non-existent org slug → KeyNotFoundException ───────────
    [Fact]
    public async Task CVerify89_UTCID03_CreateDraft_OrgNotFound_Throws()
    {
        var sut = BuildService();

        var act = async () => await sut.CreateDraftAsync(Request("ghost-org"), Guid.NewGuid(), CancellationToken.None);

        await act.Should().ThrowAsync<KeyNotFoundException>().WithMessage("*Organization not found*");
    }

    // ── UTCID04 ── org exists but no workspace → KeyNotFoundException ─────
    [Fact]
    public async Task CVerify89_UTCID04_CreateDraft_WorkspaceNotFound_Throws()
    {
        await SeedOrgAsync("acme-corp"); // no workspace seeded
        var sut = BuildService();

        var act = async () => await sut.CreateDraftAsync(Request("acme-corp"), Guid.NewGuid(), CancellationToken.None);

        await act.Should().ThrowAsync<KeyNotFoundException>().WithMessage("*Workspace not found*");
    }

    // ── UTCID05 ── 255-char title boundary → still creates Draft v1 ───────
    [Fact]
    public async Task CVerify89_UTCID05_CreateDraft_MaxLengthTitle_Succeeds()
    {
        var org = await SeedOrgAsync("acme-corp");
        await SeedWorkspaceAsync(org.Id);
        var sut = BuildService();
        var longTitle = new string('A', 255);

        var result = await sut.CreateDraftAsync(Request("acme-corp", longTitle), Guid.NewGuid(), CancellationToken.None);

        result.Title.Should().HaveLength(255);
        result.Status.Should().Be("Draft");
    }
}
