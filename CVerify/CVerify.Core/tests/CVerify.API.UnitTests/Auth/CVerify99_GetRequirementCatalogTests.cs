using System;
using System.Collections.Generic;
using System.Security.Claims;
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
/// Unit tests for HiringRequirementController.GetCatalog — CVerify-99 (2 of 3 UTCIDs).
/// GET /api/v1/hiring-requirements/catalog [Authorize]. NOTE: "No JWT → 401" is framework-level.
/// </summary>
public sealed class CVerify99_GetRequirementCatalogTests
{
    private readonly Mock<ICapabilityCatalogService> _catalog = new();

    private HiringRequirementController BuildController()
    {
        var ctrl = new HiringRequirementController(
            Mock.Of<IHiringRequirementService>(), Mock.Of<ICandidateMatchService>(), Mock.Of<IConnectionMultiplexer>(),
            _catalog.Object, Mock.Of<IServiceScopeFactory>(),
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

    // ── UTCID01 ── no workspace filter → 200 OK ───────────────────────────
    [Fact]
    public void CVerify99_UTCID01_GetCatalog_NoFilter_ReturnsOk()
    {
        _catalog.Setup(c => c.GetCatalog(null)).Returns(new List<CapabilityCatalogDto>());
        var ctrl = BuildController();

        var response = ctrl.GetCatalog(null);

        response.Should().BeOfType<OkObjectResult>();
    }

    // ── UTCID02 ── workspaceId filter → 200 OK ────────────────────────────
    [Fact]
    public void CVerify99_UTCID02_GetCatalog_WithWorkspaceId_ReturnsOk()
    {
        var wsId = Guid.NewGuid();
        _catalog.Setup(c => c.GetCatalog(wsId)).Returns(new List<CapabilityCatalogDto>());
        var ctrl = BuildController();

        var response = ctrl.GetCatalog(wsId);

        response.Should().BeOfType<OkObjectResult>();
    }
}
