using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Moq;
using StackExchange.Redis;
using Xunit;
using CVerify.API.Modules.Shared.Domain.Entities;
using CVerify.API.Modules.Shared.System.Controllers;
using CVerify.API.Modules.Shared.System.Services;

namespace CVerify.API.UnitTests.Auth;

/// <summary>
/// Unit tests for HiringRequirementController.GetArtifacts — CVerify-94 (3 of 4 UTCIDs).
/// GET /api/v1/hiring-requirements/{id}/artifacts [Authorize].
/// NOTE: when NO artifacts exist the controller returns 404 (Excel's "200 empty" is inaccurate);
/// "No JWT → 401" is framework-level.
/// </summary>
public sealed class CVerify94_GetArtifactsTests
{
    private readonly Mock<IHiringRequirementService> _service = new();

    private HiringRequirementController BuildController()
    {
        var ctrl = new HiringRequirementController(
            _service.Object, Mock.Of<ICandidateMatchService>(), Mock.Of<IConnectionMultiplexer>(),
            Mock.Of<ICapabilityCatalogService>(), Mock.Of<IServiceScopeFactory>(),
            Mock.Of<ILogger<HiringRequirementController>>(), Mock.Of<IAiStreamingSessionService>());
        ctrl.ControllerContext = new ControllerContext
        {
            HttpContext = new DefaultHttpContext
            {
                User = new ClaimsPrincipal(new ClaimsIdentity(new List<Claim> { new(ClaimTypes.NameIdentifier, Guid.NewGuid().ToString()) }, "Test")),
            },
        };
        return ctrl;
    }

    private static HiringRequirement Req(bool withArtifacts)
    {
        var req = new HiringRequirement
        {
            Id = Guid.NewGuid(), Title = "T", Department = "Eng", Seniority = "Senior",
            WorkplaceType = "Hybrid", EmploymentType = "Full-Time", Status = "Ready",
        };
        if (withArtifacts)
        {
            req.RequirementArtifacts.Add(new RequirementArtifact
            {
                Id = Guid.NewGuid(), HiringRequirementId = req.Id, ArtifactType = "JobDescription",
                MarkdownContent = "# Job Description", Status = "Generated",
            });
        }
        return req;
    }

    // ── UTCID01 ── requirement with completed artifacts → 200 OK ──────────
    [Fact]
    public async Task CVerify94_UTCID01_GetArtifacts_WithArtifacts_ReturnsOk()
    {
        _service.Setup(s => s.GetByIdAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>())).ReturnsAsync(Req(true));
        var ctrl = BuildController();

        var response = await ctrl.GetArtifacts(Guid.NewGuid(), CancellationToken.None);

        response.Should().BeOfType<OkObjectResult>();
    }

    // ── UTCID02 ── requirement with no artifacts yet → 404 NotFound ───────
    [Fact]
    public async Task CVerify94_UTCID02_GetArtifacts_NoArtifacts_Returns404()
    {
        _service.Setup(s => s.GetByIdAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>())).ReturnsAsync(Req(false));
        var ctrl = BuildController();

        var response = await ctrl.GetArtifacts(Guid.NewGuid(), CancellationToken.None);

        response.Should().BeOfType<NotFoundObjectResult>();
    }

    // ── UTCID03 ── non-existent id → 404 NotFound ─────────────────────────
    [Fact]
    public async Task CVerify94_UTCID03_GetArtifacts_NotFound_Returns404()
    {
        _service.Setup(s => s.GetByIdAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>())).ThrowsAsync(new KeyNotFoundException());
        var ctrl = BuildController();

        var response = await ctrl.GetArtifacts(Guid.NewGuid(), CancellationToken.None);

        response.Should().BeOfType<NotFoundObjectResult>();
    }
}
