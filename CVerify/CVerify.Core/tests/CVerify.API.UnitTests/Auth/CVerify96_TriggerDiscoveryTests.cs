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
using CVerify.API.Modules.Shared.System.Controllers;
using CVerify.API.Modules.Shared.System.DTOs;
using CVerify.API.Modules.Shared.System.Services;

namespace CVerify.API.UnitTests.Auth;

/// <summary>
/// Unit tests for HiringRequirementController.TriggerDiscovery — CVerify-96 (2 of 4 UTCIDs).
/// POST /api/v1/hiring-requirements/{id}/candidate-matches/discover [Authorize].
/// NOTE: the action only maps KeyNotFound → 404; "already running → 409" is not surfaced here and
/// "No JWT → 401" is framework-level — both integration-only.
/// </summary>
public sealed class CVerify96_TriggerDiscoveryTests
{
    private readonly Mock<ICandidateMatchService> _matchService = new();

    private HiringRequirementController BuildController()
    {
        var ctrl = new HiringRequirementController(
            Mock.Of<IHiringRequirementService>(), _matchService.Object, Mock.Of<IConnectionMultiplexer>(),
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

    // ── UTCID01 ── published requirement, no discovery running → 200 OK ───
    [Fact]
    public async Task CVerify96_UTCID01_Discover_Valid_ReturnsOk()
    {
        _matchService.Setup(s => s.TriggerDiscoveryAsync(It.IsAny<Guid>(), It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new TriggerDiscoveryResponseDto(Guid.NewGuid(), default));
        var ctrl = BuildController();

        var response = await ctrl.TriggerDiscovery(Guid.NewGuid(), CancellationToken.None);

        response.Should().BeOfType<OkObjectResult>();
    }

    // ── UTCID03 ── non-existent id → 404 NotFound ─────────────────────────
    [Fact]
    public async Task CVerify96_UTCID03_Discover_NotFound_Returns404()
    {
        _matchService.Setup(s => s.TriggerDiscoveryAsync(It.IsAny<Guid>(), It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
            .ThrowsAsync(new KeyNotFoundException());
        var ctrl = BuildController();

        var response = await ctrl.TriggerDiscovery(Guid.NewGuid(), CancellationToken.None);

        response.Should().BeOfType<NotFoundObjectResult>();
    }
}
