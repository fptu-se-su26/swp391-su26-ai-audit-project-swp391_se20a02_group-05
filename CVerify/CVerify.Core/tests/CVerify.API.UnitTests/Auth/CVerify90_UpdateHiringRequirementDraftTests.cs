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
/// Unit tests for HiringRequirementService.UpdateDraftAsync — CVerify-90 (5 UTCIDs).
/// PUT /api/v1/hiring-requirements/{id} [Authorize] — updates a draft; published cannot be updated.
/// Tested at the service level (the controller maps KeyNotFound → 404, InvalidOperation → 400).
/// NOTE: this DTO does not carry Title (Excel's "updated title" case) and the service performs no
/// workspace-ownership check, so those design rows do not map; scalar fields are exercised instead.
/// </summary>
public sealed class CVerify90_UpdateHiringRequirementDraftTests : IDisposable
{
    private static readonly UpdateHiringRequirementRequestDto EmptyUpdate = new(
        null, null, null, null, null, null, null, null, null, null,
        null, null, null, null, null, null, null, null, null, null, null);

    private readonly ApplicationDbContext _context;
    private readonly Mock<ICapabilityCatalogService> _catalogService = new();

    public CVerify90_UpdateHiringRequirementDraftTests()
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

    private async Task<HiringRequirement> SeedRequirementAsync(string status = "Draft")
    {
        var req = new HiringRequirement
        {
            Id = Guid.CreateVersion7(),
            OrganizationId = Guid.NewGuid(),
            WorkspaceId = Guid.NewGuid(),
            Title = "Backend Dev",
            Department = "Engineering",
            Seniority = "Senior",
            WorkplaceType = "Hybrid",
            EmploymentType = "Full-Time",
            Status = status,
            Version = 1,
            CreatedAt = DateTimeOffset.UtcNow,
            UpdatedAt = DateTimeOffset.UtcNow,
        };
        _context.HiringRequirements.Add(req);
        await _context.SaveChangesAsync();
        return req;
    }

    // ── UTCID01 ── draft exists, update scalar field → returns updated ────
    [Fact]
    public async Task CVerify90_UTCID01_UpdateDraft_ValidScalarUpdate_ReturnsUpdated()
    {
        var req = await SeedRequirementAsync("Draft");
        var sut = BuildService();

        var result = await sut.UpdateDraftAsync(req.Id, EmptyUpdate with { HiringReason = "Backfill" }, CancellationToken.None);

        result.Should().NotBeNull();
        result.HiringReason.Should().Be("Backfill");
        result.Status.Should().Be("Draft");
    }

    // ── UTCID02 ── published requirement → InvalidOperationException (→400) ─
    [Fact]
    public async Task CVerify90_UTCID02_UpdateDraft_Published_Throws()
    {
        var req = await SeedRequirementAsync("Published");
        var sut = BuildService();

        var act = async () => await sut.UpdateDraftAsync(req.Id, EmptyUpdate with { HiringReason = "x" }, CancellationToken.None);

        await act.Should().ThrowAsync<InvalidOperationException>().WithMessage("*Cannot update a published*");
    }

    // ── UTCID03 ── non-existent id → KeyNotFoundException (→404) ──────────
    [Fact]
    public async Task CVerify90_UTCID03_UpdateDraft_NotFound_Throws()
    {
        var sut = BuildService();

        var act = async () => await sut.UpdateDraftAsync(Guid.NewGuid(), EmptyUpdate, CancellationToken.None);

        await act.Should().ThrowAsync<KeyNotFoundException>().WithMessage("*not found*");
    }

    // ── UTCID04 ── salary fields update is persisted ──────────────────────
    [Fact]
    public async Task CVerify90_UTCID04_UpdateDraft_SalaryFields_Persisted()
    {
        var req = await SeedRequirementAsync("Draft");
        var sut = BuildService();

        await sut.UpdateDraftAsync(req.Id, EmptyUpdate with { SalaryMin = 1000m, SalaryMax = 2000m, Currency = "USD" }, CancellationToken.None);

        var persisted = await _context.HiringRequirements.FindAsync(req.Id);
        persisted!.SalaryMin.Should().Be(1000m);
        persisted.SalaryMax.Should().Be(2000m);
        persisted.Currency.Should().Be("USD");
    }

    // ── UTCID05 ── archived (non-published) draft can still be updated ────
    [Fact]
    public async Task CVerify90_UTCID05_UpdateDraft_ArchivedStatus_AllowsUpdate()
    {
        var req = await SeedRequirementAsync("Archived");
        var sut = BuildService();

        var result = await sut.UpdateDraftAsync(req.Id, EmptyUpdate with { HiringReason = "reopened" }, CancellationToken.None);

        result.HiringReason.Should().Be("reopened");
    }
}
