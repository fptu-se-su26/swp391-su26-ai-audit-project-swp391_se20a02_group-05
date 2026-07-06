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
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;
using CVerify.API.Modules.Shared.Domain.Entities;
using CVerify.API.Modules.Shared.Persistence;
using CVerify.API.Modules.Shared.Storage.Interfaces;
using CVerify.API.Modules.Shared.System.Controllers;
using CVerify.API.Modules.Shared.System.Services;

namespace CVerify.API.UnitTests.Auth;

/// <summary>
/// Unit tests for JobVacancyController.CreateDraft — CVerify-101 (4 of 5 UTCIDs).
/// POST /api/v1/job-vacancies/requirement/{requirementId}/create-draft [Authorize].
/// NOTE: an existing vacancy returns 400 (Excel's "409" is inaccurate); "No JWT → 401" is framework-level.
/// </summary>
public sealed class CVerify101_CreateJobVacancyDraftTests : IDisposable
{
    private readonly ApplicationDbContext _context;
    private readonly Mock<IStorageService> _storage = new();

    public CVerify101_CreateJobVacancyDraftTests()
    {
        _context = new ApplicationDbContext(
            new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .ConfigureWarnings(w => w.Ignore(InMemoryEventId.TransactionIgnoredWarning))
                .Options);
        _storage.Setup(s => s.GetSignedUrlAsync(It.IsAny<string>(), It.IsAny<TimeSpan>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((string)null!);
    }

    public void Dispose() => _context.Dispose();

    private JobVacancyController BuildController()
    {
        var ctrl = new JobVacancyController(_context, Mock.Of<IHiringRequirementService>(), _storage.Object,
            Mock.Of<ILogger<JobVacancyController>>());
        ctrl.ControllerContext = new ControllerContext
        {
            HttpContext = new DefaultHttpContext
            {
                User = new ClaimsPrincipal(new ClaimsIdentity(new List<Claim> { new(ClaimTypes.NameIdentifier, Guid.NewGuid().ToString()) }, "Test")),
            },
        };
        return ctrl;
    }

    private async Task<HiringRequirement> SeedRequirementAsync(string status)
    {
        var req = new HiringRequirement
        {
            Id = Guid.NewGuid(), OrganizationId = Guid.NewGuid(), WorkspaceId = Guid.NewGuid(),
            Title = "Backend Dev", Department = "Eng", Seniority = "Senior", WorkplaceType = "Hybrid",
            EmploymentType = "Full-Time", Status = status, Headcount = 1,
        };
        _context.HiringRequirements.Add(req);
        await _context.SaveChangesAsync();
        return req;
    }

    // ── UTCID01 ── Ready requirement → 201 Created ────────────────────────
    [Fact]
    public async Task CVerify101_UTCID01_CreateDraft_Ready_Returns201()
    {
        var req = await SeedRequirementAsync("Ready");
        var ctrl = BuildController();

        var response = await ctrl.CreateDraft(req.Id, CancellationToken.None);

        response.Should().BeOfType<CreatedAtActionResult>();
    }

    // ── UTCID02 ── Draft requirement (not ready) → 400 BadRequest ─────────
    [Fact]
    public async Task CVerify101_UTCID02_CreateDraft_NotReady_Returns400()
    {
        var req = await SeedRequirementAsync("Draft");
        var ctrl = BuildController();

        var response = await ctrl.CreateDraft(req.Id, CancellationToken.None);

        response.Should().BeOfType<BadRequestObjectResult>();
    }

    // ── UTCID03 ── non-existent requirement → 404 NotFound ────────────────
    [Fact]
    public async Task CVerify101_UTCID03_CreateDraft_NotFound_Returns404()
    {
        var ctrl = BuildController();

        var response = await ctrl.CreateDraft(Guid.NewGuid(), CancellationToken.None);

        response.Should().BeOfType<NotFoundObjectResult>();
    }

    // ── UTCID04 ── requirement already has a vacancy → 400 BadRequest ─────
    [Fact]
    public async Task CVerify101_UTCID04_CreateDraft_VacancyExists_Returns400()
    {
        var req = await SeedRequirementAsync("Ready");
        _context.JobVacancies.Add(new JobVacancy
        {
            Id = Guid.NewGuid(), OrganizationId = req.OrganizationId, HiringRequirementId = req.Id,
            Title = "Existing", Department = "Eng", WorkplaceType = "Hybrid", City = "Hanoi", Type = "Full-Time",
            Salary = "x", SalaryMinMax = "0-0", Experience = "5+", Degree = "B", Category = "S", CoverUrl = "https://cover.example/x.png", Status = "Draft",
        });
        await _context.SaveChangesAsync();
        var ctrl = BuildController();

        var response = await ctrl.CreateDraft(req.Id, CancellationToken.None);

        response.Should().BeOfType<BadRequestObjectResult>();
    }
}
