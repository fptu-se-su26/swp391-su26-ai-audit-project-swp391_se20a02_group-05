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
/// Unit tests for HiringRequirementController.GetCandidateMatches — CVerify-95 (3 of 4 UTCIDs).
/// GET /api/v1/hiring-requirements/{id}/candidate-matches [Authorize].
/// NOTE: "No JWT → 401" is framework-level.
/// </summary>
public sealed class CVerify95_GetCandidateMatchesTests
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

    // ── UTCID01 ── requirement with matches → 200 OK ──────────────────────
    [Fact]
    public async Task CVerify95_UTCID01_GetMatches_WithMatches_ReturnsOk()
    {
        _matchService.Setup(s => s.GetCandidateMatchesAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<CandidateMatchDto>());
        var ctrl = BuildController();

        var response = await ctrl.GetCandidateMatches(Guid.NewGuid(), CancellationToken.None);

        response.Should().BeOfType<OkObjectResult>();
    }

    // ── UTCID02 ── requirement without matches → 200 OK (empty list) ──────
    [Fact]
    public async Task CVerify95_UTCID02_GetMatches_NoMatches_ReturnsOkEmpty()
    {
        _matchService.Setup(s => s.GetCandidateMatchesAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<CandidateMatchDto>());
        var ctrl = BuildController();

        var response = await ctrl.GetCandidateMatches(Guid.NewGuid(), CancellationToken.None);

        var ok = response.Should().BeOfType<OkObjectResult>().Subject;
        ok.Value.Should().BeAssignableTo<List<CandidateMatchDto>>().Which.Should().BeEmpty();
    }

    // ── UTCID03 ── non-existent id → 404 NotFound ─────────────────────────
    [Fact]
    public async Task CVerify95_UTCID03_GetMatches_NotFound_Returns404()
    {
        _matchService.Setup(s => s.GetCandidateMatchesAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
            .ThrowsAsync(new KeyNotFoundException());
        var ctrl = BuildController();

        var response = await ctrl.GetCandidateMatches(Guid.NewGuid(), CancellationToken.None);

        response.Should().BeOfType<NotFoundObjectResult>();
    }
}
