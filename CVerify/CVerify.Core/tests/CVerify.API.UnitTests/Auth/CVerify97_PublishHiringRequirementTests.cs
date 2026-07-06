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
/// Unit tests for HiringRequirementController.Publish — CVerify-97 (4 of 5 UTCIDs).
/// POST /api/v1/hiring-requirements/{id}/publish [Authorize].
/// NOTE: "No JWT → 401" is framework-level. Draft / already-published both surface as
/// InvalidOperationException → 400.
/// </summary>
public sealed class CVerify97_PublishHiringRequirementTests
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

    // ── UTCID01 ── Ready requirement → 200 OK { snapshotId, version } ─────
    [Fact]
    public async Task CVerify97_UTCID01_Publish_Ready_ReturnsOk()
    {
        _service.Setup(s => s.PublishAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new RequirementSnapshot { Id = Guid.NewGuid(), Version = 1 });
        var ctrl = BuildController();

        var response = await ctrl.Publish(Guid.NewGuid(), CancellationToken.None);

        response.Should().BeOfType<OkObjectResult>();
    }

    // ── UTCID02 ── Draft requirement → 400 BadRequest (not publishable) ───
    [Fact]
    public async Task CVerify97_UTCID02_Publish_Draft_Returns400()
    {
        _service.Setup(s => s.PublishAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
            .ThrowsAsync(new InvalidOperationException("Requirement is not in a publishable state."));
        var ctrl = BuildController();

        var response = await ctrl.Publish(Guid.NewGuid(), CancellationToken.None);

        response.Should().BeOfType<BadRequestObjectResult>();
    }

    // ── UTCID03 ── already-published requirement → 400 BadRequest ─────────
    [Fact]
    public async Task CVerify97_UTCID03_Publish_AlreadyPublished_Returns400()
    {
        _service.Setup(s => s.PublishAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
            .ThrowsAsync(new InvalidOperationException("Hiring requirement is already published."));
        var ctrl = BuildController();

        var response = await ctrl.Publish(Guid.NewGuid(), CancellationToken.None);

        response.Should().BeOfType<BadRequestObjectResult>();
    }

    // ── UTCID04 ── non-existent id → 404 NotFound ─────────────────────────
    [Fact]
    public async Task CVerify97_UTCID04_Publish_NotFound_Returns404()
    {
        _service.Setup(s => s.PublishAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
            .ThrowsAsync(new KeyNotFoundException());
        var ctrl = BuildController();

        var response = await ctrl.Publish(Guid.NewGuid(), CancellationToken.None);

        response.Should().BeOfType<NotFoundObjectResult>();
    }
}
