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
/// Unit tests for JobVacancyController.PublishPosting — CVerify-103 (3 of 5 UTCIDs).
/// POST /api/v1/job-postings/{id}/publish [Authorize].
/// NOTE: the action has no publish-permission check (403) and "No JWT → 401" is framework-level —
/// those two cases are integration-only.
/// </summary>
public sealed class CVerify103_PublishJobVacancyTests : IDisposable
{
    private readonly ApplicationDbContext _context;
    private readonly Mock<IStorageService> _storage = new();
    private readonly Mock<IHiringRequirementService> _reqService = new();

    public CVerify103_PublishJobVacancyTests()
    {
        _context = new ApplicationDbContext(
            new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .ConfigureWarnings(w => w.Ignore(InMemoryEventId.TransactionIgnoredWarning))
                .Options);
        _storage.Setup(s => s.GetSignedUrlAsync(It.IsAny<string>(), It.IsAny<TimeSpan>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((string)null!);
        _reqService.Setup(s => s.PublishAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new RequirementSnapshot { Id = Guid.NewGuid(), Version = 1 });
    }

    public void Dispose() => _context.Dispose();

    private JobVacancyController BuildController()
    {
        var ctrl = new JobVacancyController(_context, _reqService.Object, _storage.Object,
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

    private async Task<JobVacancy> SeedVacancyAsync(string status, bool linkRequirement = true)
    {
        var v = new JobVacancy
        {
            Id = Guid.NewGuid(), OrganizationId = Guid.NewGuid(),
            HiringRequirementId = linkRequirement ? Guid.NewGuid() : (Guid?)null,
            Title = "Job", Department = "Eng", WorkplaceType = "Hybrid", City = "Hanoi", Type = "Full-Time",
            Salary = "x", SalaryMinMax = "0-0", Experience = "5+", Degree = "B", Category = "S", CoverUrl = "https://cover.example/x.png", Status = status,
        };
        _context.JobVacancies.Add(v);
        await _context.SaveChangesAsync();
        return v;
    }

    // ── UTCID01 ── draft vacancy → 200 OK published (IsActive=true) ───────
    [Fact]
    public async Task CVerify103_UTCID01_Publish_Draft_ReturnsOk()
    {
        var v = await SeedVacancyAsync("Draft");
        var ctrl = BuildController();

        var response = await ctrl.PublishPosting(v.Id, new PublishRequirementRequestDto(), CancellationToken.None);

        response.Should().BeOfType<OkObjectResult>();
    }

    // ── UTCID02 ── already-published vacancy → 400 BadRequest ─────────────
    [Fact]
    public async Task CVerify103_UTCID02_Publish_AlreadyPublished_Returns400()
    {
        var v = await SeedVacancyAsync("Published");
        var ctrl = BuildController();

        var response = await ctrl.PublishPosting(v.Id, new PublishRequirementRequestDto(), CancellationToken.None);

        response.Should().BeOfType<BadRequestObjectResult>();
    }

    // ── UTCID03 ── non-existent id → 404 NotFound ─────────────────────────
    [Fact]
    public async Task CVerify103_UTCID03_Publish_NotFound_Returns404()
    {
        var ctrl = BuildController();

        var response = await ctrl.PublishPosting(Guid.NewGuid(), new PublishRequirementRequestDto(), CancellationToken.None);

        response.Should().BeOfType<NotFoundObjectResult>();
    }
}
