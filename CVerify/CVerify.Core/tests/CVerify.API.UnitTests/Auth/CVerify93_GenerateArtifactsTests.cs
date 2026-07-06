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
/// Unit tests for HiringRequirementController.GenerateArtifacts — CVerify-93 (2 of 5 UTCIDs).
/// POST /api/v1/hiring-requirements/{id}/generate-artifacts [Authorize] → 202 Accepted.
/// NOTE: the controller only guards existence (404); "not ready" (400), "already generating" (409)
/// and "No JWT" (401) are decided inside the async service / [Authorize] filter — integration-only.
/// </summary>
public sealed class CVerify93_GenerateArtifactsTests
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

    private static HiringRequirement Req() => new()
    {
        Id = Guid.NewGuid(), Title = "T", Department = "Eng", Seniority = "Senior",
        WorkplaceType = "Hybrid", EmploymentType = "Full-Time", Status = "Ready",
    };

    // ── UTCID01 ── requirement exists → 202 Accepted (queued) ─────────────
    [Fact]
    public async Task CVerify93_UTCID01_Generate_RequirementExists_Returns202()
    {
        _service.Setup(s => s.GetByIdAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>())).ReturnsAsync(Req());
        var ctrl = BuildController();

        var response = await ctrl.GenerateArtifacts(Guid.NewGuid());

        response.Should().BeOfType<AcceptedResult>();
    }

    // ── UTCID03 ── non-existent id → 404 NotFound ─────────────────────────
    [Fact]
    public async Task CVerify93_UTCID03_Generate_NotFound_Returns404()
    {
        _service.Setup(s => s.GetByIdAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>())).ThrowsAsync(new KeyNotFoundException());
        var ctrl = BuildController();

        var response = await ctrl.GenerateArtifacts(Guid.NewGuid());

        response.Should().BeOfType<NotFoundObjectResult>();
    }
}
