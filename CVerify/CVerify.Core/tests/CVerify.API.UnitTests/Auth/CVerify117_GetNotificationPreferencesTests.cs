using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Xunit;
using CVerify.API.Modules.Shared.Domain.Entities;
using CVerify.API.Modules.Shared.Persistence;
using CVerify.API.Modules.Shared.System.Controllers;

namespace CVerify.API.UnitTests.Auth;

/// <summary>
/// Unit tests for NotificationController.GetPreferences — CVerify-117 (3 UTCIDs).
/// GET /api/notifications/preferences [Authorize].
/// </summary>
public sealed class CVerify117_GetNotificationPreferencesTests : IDisposable
{
    private readonly ApplicationDbContext _context;

    public CVerify117_GetNotificationPreferencesTests()
    {
        _context = new ApplicationDbContext(
            new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .ConfigureWarnings(w => w.Ignore(InMemoryEventId.TransactionIgnoredWarning))
                .Options);
    }

    public void Dispose() => _context.Dispose();

    private NotificationController BuildController(Guid? userId)
    {
        var user = userId.HasValue
            ? new ClaimsPrincipal(new ClaimsIdentity(new List<Claim> { new(ClaimTypes.NameIdentifier, userId.Value.ToString()) }, "Test"))
            : new ClaimsPrincipal();
        var ctrl = new NotificationController(_context);
        ctrl.ControllerContext = new ControllerContext { HttpContext = new DefaultHttpContext { User = user } };
        return ctrl;
    }

    // ── UTCID01 ── user has custom preferences → 200 OK ───────────────────
    [Fact]
    public async Task CVerify117_UTCID01_GetPreferences_Custom_ReturnsOk()
    {
        var userId = Guid.NewGuid();
        _context.NotificationPreferences.Add(new NotificationPreference
        {
            Id = Guid.NewGuid(), UserId = userId, NotificationType = "JobApplied", Channel = "email", IsEnabled = false,
        });
        await _context.SaveChangesAsync();
        var ctrl = BuildController(userId);

        var response = await ctrl.GetPreferences();

        response.Should().BeOfType<OkObjectResult>();
    }

    // ── UTCID02 ── no custom preferences (defaults) → 200 OK (empty) ──────
    [Fact]
    public async Task CVerify117_UTCID02_GetPreferences_Default_ReturnsOk()
    {
        var ctrl = BuildController(Guid.NewGuid());

        var response = await ctrl.GetPreferences();

        response.Should().BeOfType<OkObjectResult>();
    }

    // ── UTCID03 ── no JWT → 401 Unauthorized ──────────────────────────────
    [Fact]
    public async Task CVerify117_UTCID03_GetPreferences_NoJwt_Returns401()
    {
        var ctrl = BuildController(null);

        var response = await ctrl.GetPreferences();

        response.Should().BeOfType<UnauthorizedResult>();
    }
}
