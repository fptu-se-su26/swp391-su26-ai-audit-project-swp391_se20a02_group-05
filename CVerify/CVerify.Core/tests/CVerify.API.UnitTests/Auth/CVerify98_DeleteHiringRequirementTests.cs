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
using CVerify.API.Modules.Shared.System.Services;

namespace CVerify.API.UnitTests.Auth;

/// <summary>
/// Unit tests for HiringRequirementController.Delete — CVerify-98 (2 of 5 UTCIDs).
/// DELETE /api/v1/hiring-requirements/{id} [Authorize] → 204 (soft delete).
/// NOTE: the action only maps KeyNotFound → 404; "cannot delete published (400)", "no permission
/// (403)" and "No JWT (401)" are not surfaced by this action — integration-only.
/// </summary>
public sealed class CVerify98_DeleteHiringRequirementTests
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

    // ── UTCID01 ── own draft requirement → 204 No Content ─────────────────
    [Fact]
    public async Task CVerify98_UTCID01_Delete_OwnDraft_Returns204()
    {
        _service.Setup(s => s.DeleteAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>())).Returns(Task.CompletedTask);
        var ctrl = BuildController();

        var response = await ctrl.Delete(Guid.NewGuid(), CancellationToken.None);

        response.Should().BeOfType<NoContentResult>();
    }

    // ── UTCID03 ── non-existent id → 404 NotFound ─────────────────────────
    [Fact]
    public async Task CVerify98_UTCID03_Delete_NotFound_Returns404()
    {
        _service.Setup(s => s.DeleteAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>())).ThrowsAsync(new KeyNotFoundException());
        var ctrl = BuildController();

        var response = await ctrl.Delete(Guid.NewGuid(), CancellationToken.None);

        response.Should().BeOfType<NotFoundObjectResult>();
    }
}
