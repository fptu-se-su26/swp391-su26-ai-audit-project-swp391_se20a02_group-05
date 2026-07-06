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
/// Unit tests for HiringRequirementController.GetById — CVerify-91 (3 of 4 UTCIDs).
/// GET /api/v1/hiring-requirements/{id} [Authorize]. NOTE: "No JWT → 401" is enforced by the
/// [Authorize] filter (framework level), not inside the action, so it is integration-only.
/// </summary>
public sealed class CVerify91_GetHiringRequirementByIdTests
{
    private readonly Mock<IHiringRequirementService> _service = new();
    private readonly Mock<ICandidateMatchService> _matchService = new();

    private HiringRequirementController BuildController(ClaimsPrincipal? user = null)
    {
        var ctrl = new HiringRequirementController(
            _service.Object, _matchService.Object, Mock.Of<IConnectionMultiplexer>(),
            Mock.Of<ICapabilityCatalogService>(), Mock.Of<IServiceScopeFactory>(),
            Mock.Of<ILogger<HiringRequirementController>>(), Mock.Of<IAiStreamingSessionService>());
        ctrl.ControllerContext = new ControllerContext
        {
            HttpContext = new DefaultHttpContext { User = user ?? Authenticated() },
        };
        return ctrl;
    }

    private static ClaimsPrincipal Authenticated() =>
        new(new ClaimsIdentity(new List<Claim> { new(ClaimTypes.NameIdentifier, Guid.NewGuid().ToString()) }, "Test"));

    private static HiringRequirement Req(string status) => new()
    {
        Id = Guid.NewGuid(), Title = "Backend Dev", Department = "Engineering", Seniority = "Senior",
        WorkplaceType = "Hybrid", EmploymentType = "Full-Time", Status = status, Version = 1,
    };

    // ── UTCID01 ── draft requirement → 200 OK ─────────────────────────────
    [Fact]
    public async Task CVerify91_UTCID01_GetById_DraftRequirement_ReturnsOk()
    {
        _service.Setup(s => s.GetByIdAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>())).ReturnsAsync(Req("Draft"));
        var ctrl = BuildController();

        var response = await ctrl.GetById(Guid.NewGuid(), CancellationToken.None);

        response.Should().BeOfType<OkObjectResult>();
    }

    // ── UTCID02 ── published requirement → 200 OK ─────────────────────────
    [Fact]
    public async Task CVerify91_UTCID02_GetById_PublishedRequirement_ReturnsOk()
    {
        _service.Setup(s => s.GetByIdAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>())).ReturnsAsync(Req("Published"));
        var ctrl = BuildController();

        var response = await ctrl.GetById(Guid.NewGuid(), CancellationToken.None);

        response.Should().BeOfType<OkObjectResult>();
    }

    // ── UTCID03 ── non-existent id → 404 NotFound ─────────────────────────
    [Fact]
    public async Task CVerify91_UTCID03_GetById_NotFound_Returns404()
    {
        _service.Setup(s => s.GetByIdAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>())).ThrowsAsync(new KeyNotFoundException());
        var ctrl = BuildController();

        var response = await ctrl.GetById(Guid.NewGuid(), CancellationToken.None);

        response.Should().BeOfType<NotFoundObjectResult>();
    }
}
