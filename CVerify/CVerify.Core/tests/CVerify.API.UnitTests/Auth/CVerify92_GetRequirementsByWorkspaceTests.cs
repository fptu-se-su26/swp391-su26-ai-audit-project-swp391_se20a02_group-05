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
using CVerify.API.Modules.Shared.System.DTOs;
using CVerify.API.Modules.Shared.System.Services;

namespace CVerify.API.UnitTests.Auth;

/// <summary>
/// Unit tests for HiringRequirementController.GetByWorkspaceId — CVerify-92 (2 of 5 UTCIDs).
/// GET /api/v1/hiring-requirements/workspace/{workspaceId} [Authorize].
/// NOTE: the action has no 404/403 guards (the service returns an empty page for unknown/inaccessible
/// workspaces) and "No JWT" is framework-level — those three cases are integration-only.
/// </summary>
public sealed class CVerify92_GetRequirementsByWorkspaceTests
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

    private void SetupReturns(int count)
    {
        var items = new List<HiringRequirement>();
        for (var i = 0; i < count; i++)
            items.Add(new HiringRequirement { Id = Guid.NewGuid(), Title = $"R{i}", Department = "Eng", Seniority = "Senior", WorkplaceType = "Hybrid", EmploymentType = "Full-Time", Status = "Draft" });
        _service
            .Setup(s => s.GetByWorkspaceIdAsync(It.IsAny<Guid>(), It.IsAny<string?>(), It.IsAny<string?>(), It.IsAny<string?>(),
                It.IsAny<string?>(), It.IsAny<string?>(), It.IsAny<int>(), It.IsAny<int>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new PaginatedListDto<HiringRequirement>(items, count, 1, 10));
    }

    // ── UTCID01 ── valid workspace, default paging → 200 OK ───────────────
    [Fact]
    public async Task CVerify92_UTCID01_GetByWorkspace_Valid_ReturnsOk()
    {
        SetupReturns(3);
        var ctrl = BuildController();

        var response = await ctrl.GetByWorkspaceId(Guid.NewGuid(), null, null, null, null, null, 1, 10, CancellationToken.None);

        var ok = response.Should().BeOfType<OkObjectResult>().Subject;
        ok.Value.Should().BeOfType<PaginatedListDto<HiringRequirement>>()
            .Which.TotalCount.Should().Be(3);
    }

    // ── UTCID02 ── status filter → 200 OK ─────────────────────────────────
    [Fact]
    public async Task CVerify92_UTCID02_GetByWorkspace_StatusFilter_ReturnsOk()
    {
        SetupReturns(1);
        var ctrl = BuildController();

        var response = await ctrl.GetByWorkspaceId(Guid.NewGuid(), null, null, "Draft", null, null, 1, 10, CancellationToken.None);

        response.Should().BeOfType<OkObjectResult>();
    }
}
