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
/// Unit tests for JobVacancyController.GetByRequirementId — CVerify-100 (4 of 5 UTCIDs).
/// GET /api/v1/job-vacancies/requirement/{requirementId} [Authorize].
/// NOTE: "No JWT → 401" is framework-level ([Authorize]).
/// </summary>
public sealed class CVerify100_GetJobByRequirementTests : IDisposable
{
    private readonly ApplicationDbContext _context;
    private readonly Mock<IStorageService> _storage = new();

    public CVerify100_GetJobByRequirementTests()
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

    private async Task<JobVacancy> SeedVacancyAsync(Guid requirementId, string status)
    {
        var v = new JobVacancy
        {
            Id = Guid.NewGuid(), OrganizationId = Guid.NewGuid(), HiringRequirementId = requirementId,
            Title = "Backend Dev", Department = "Eng", WorkplaceType = "Hybrid", City = "Hanoi",
            Type = "Full-Time", Salary = "Negotiable", SalaryMinMax = "0-0", Experience = "5+ years",
            Degree = "Bachelor", Category = "Software", CoverUrl = "https://cover.example/x.png", Status = status, IsActive = status == "Published",
        };
        _context.JobVacancies.Add(v);
        await _context.SaveChangesAsync();
        return v;
    }

    // ── UTCID01 ── published requirement has a vacancy → 200 OK ───────────
    [Fact]
    public async Task CVerify100_UTCID01_GetByRequirement_PublishedHasVacancy_ReturnsOk()
    {
        var reqId = Guid.NewGuid();
        await SeedVacancyAsync(reqId, "Published");
        var ctrl = BuildController();

        var response = await ctrl.GetByRequirementId(reqId, CancellationToken.None);

        response.Should().BeOfType<OkObjectResult>();
    }

    // ── UTCID02 ── draft vacancy triggers metadata sync → 200 OK ──────────
    [Fact]
    public async Task CVerify100_UTCID02_GetByRequirement_DraftTriggersSync_ReturnsOk()
    {
        var reqId = Guid.NewGuid();
        await SeedVacancyAsync(reqId, "Draft");
        var ctrl = BuildController();

        var response = await ctrl.GetByRequirementId(reqId, CancellationToken.None);

        response.Should().BeOfType<OkObjectResult>();
    }

    // ── UTCID03 ── non-existent requirement → 404 NotFound ────────────────
    [Fact]
    public async Task CVerify100_UTCID03_GetByRequirement_NotFound_Returns404()
    {
        var ctrl = BuildController();

        var response = await ctrl.GetByRequirementId(Guid.NewGuid(), CancellationToken.None);

        response.Should().BeOfType<NotFoundObjectResult>();
    }

    // ── UTCID04 ── valid requirement, no vacancy created yet → 404 ────────
    [Fact]
    public async Task CVerify100_UTCID04_GetByRequirement_NoVacancyYet_Returns404()
    {
        // seed a vacancy for a different requirement so the table is non-empty
        await SeedVacancyAsync(Guid.NewGuid(), "Published");
        var ctrl = BuildController();

        var response = await ctrl.GetByRequirementId(Guid.NewGuid(), CancellationToken.None);

        response.Should().BeOfType<NotFoundObjectResult>();
    }
}
