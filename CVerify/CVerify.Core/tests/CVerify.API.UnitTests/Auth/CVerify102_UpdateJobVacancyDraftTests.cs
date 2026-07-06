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
using CVerify.API.Modules.Shared.System.DTOs;
using CVerify.API.Modules.Shared.System.Services;

namespace CVerify.API.UnitTests.Auth;

/// <summary>
/// Unit tests for JobVacancyController.UpdatePostingDraft — CVerify-102 (3 of 4 UTCIDs).
/// PUT /api/v1/job-postings/{id} [Authorize]. NOTE: "No JWT → 401" is framework-level.
/// </summary>
public sealed class CVerify102_UpdateJobVacancyDraftTests : IDisposable
{
    private readonly ApplicationDbContext _context;
    private readonly Mock<IStorageService> _storage = new();

    public CVerify102_UpdateJobVacancyDraftTests()
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

    private async Task<JobVacancy> SeedVacancyAsync(string status)
    {
        var v = new JobVacancy
        {
            Id = Guid.NewGuid(), OrganizationId = Guid.NewGuid(), Title = "Old", Department = "Eng",
            WorkplaceType = "Hybrid", City = "Hanoi", Type = "Full-Time", Salary = "x", SalaryMinMax = "0-0",
            Experience = "5+", Degree = "B", Category = "S", CoverUrl = "https://cover.example/x.png", Status = status,
        };
        _context.JobVacancies.Add(v);
        await _context.SaveChangesAsync();
        return v;
    }

    private static UpdateJobVacancyDto Dto(string title = "Updated Title") =>
        new(Title: title, Department: "Eng", WorkplaceType: "Hybrid", City: "Hanoi", Type: "Full-Time",
            Salary: "Competitive", SalaryMinMax: "1000-2000", Headcount: 1, Gender: "Any", Experience: "5+",
            Degree: "Bachelor", Category: "Backend", Description: new List<string>(), Requirements: new List<string>(),
            Benefits: new List<string>(), Tags: new List<string>(), Skills: new List<string>(),
            CoverUrl: "https://cover.example/x.png", AcquisitionStrategy: "Hybrid", DiscoveryProfileJson: "{}");

    // ── UTCID01 ── draft vacancy → 200 OK updated ─────────────────────────
    [Fact]
    public async Task CVerify102_UTCID01_Update_Draft_ReturnsOk()
    {
        var v = await SeedVacancyAsync("Draft");
        var ctrl = BuildController();

        var response = await ctrl.UpdatePostingDraft(v.Id, Dto(), CancellationToken.None);

        response.Should().BeOfType<OkObjectResult>();
    }

    // ── UTCID02 ── published vacancy → 400 BadRequest (cannot edit) ───────
    [Fact]
    public async Task CVerify102_UTCID02_Update_Published_Returns400()
    {
        var v = await SeedVacancyAsync("Published");
        var ctrl = BuildController();

        var response = await ctrl.UpdatePostingDraft(v.Id, Dto(), CancellationToken.None);

        response.Should().BeOfType<BadRequestObjectResult>();
    }

    // ── UTCID03 ── non-existent id → 404 NotFound ─────────────────────────
    [Fact]
    public async Task CVerify102_UTCID03_Update_NotFound_Returns404()
    {
        var ctrl = BuildController();

        var response = await ctrl.UpdatePostingDraft(Guid.NewGuid(), Dto(), CancellationToken.None);

        response.Should().BeOfType<NotFoundObjectResult>();
    }
}
