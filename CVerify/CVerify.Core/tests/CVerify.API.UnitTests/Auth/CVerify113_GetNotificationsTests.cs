using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading;
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
/// Unit tests for NotificationController.GetNotifications — CVerify-113 (5 UTCIDs).
/// GET /api/notifications [Authorize].
/// </summary>
public sealed class CVerify113_GetNotificationsTests : IDisposable
{
    private readonly ApplicationDbContext _context;

    public CVerify113_GetNotificationsTests()
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

    private async Task SeedNotificationsAsync(Guid userId, int count, bool read = false)
    {
        for (var i = 0; i < count; i++)
        {
            _context.InAppNotifications.Add(new InAppNotification
            {
                Id = Guid.NewGuid(), UserId = userId, NotificationType = "JobApplied", ResourceType = "job",
                IsRead = read, CreatedAt = DateTimeOffset.UtcNow.AddMinutes(-i),
            });
        }
        await _context.SaveChangesAsync();
    }

    // ── UTCID01 ── default paging → 200 OK ────────────────────────────────
    [Fact]
    public async Task CVerify113_UTCID01_GetNotifications_Defaults_ReturnsOk()
    {
        var userId = Guid.NewGuid();
        await SeedNotificationsAsync(userId, 3);
        var ctrl = BuildController(userId);

        var response = await ctrl.GetNotifications(1, 20, false, CancellationToken.None);

        response.Should().BeOfType<OkObjectResult>();
    }

    // ── UTCID02 ── unreadOnly filter → 200 OK ─────────────────────────────
    [Fact]
    public async Task CVerify113_UTCID02_GetNotifications_UnreadOnly_ReturnsOk()
    {
        var userId = Guid.NewGuid();
        await SeedNotificationsAsync(userId, 2, read: false);
        await SeedNotificationsAsync(userId, 1, read: true);
        var ctrl = BuildController(userId);

        var response = await ctrl.GetNotifications(1, 20, true, CancellationToken.None);

        response.Should().BeOfType<OkObjectResult>();
    }

    // ── UTCID03 ── no notifications → 200 OK (empty) ──────────────────────
    [Fact]
    public async Task CVerify113_UTCID03_GetNotifications_NoNotifications_ReturnsOk()
    {
        var ctrl = BuildController(Guid.NewGuid());

        var response = await ctrl.GetNotifications(1, 20, false, CancellationToken.None);

        response.Should().BeOfType<OkObjectResult>();
    }

    // ── UTCID04 ── no JWT → 401 Unauthorized ──────────────────────────────
    [Fact]
    public async Task CVerify113_UTCID04_GetNotifications_NoJwt_Returns401()
    {
        var ctrl = BuildController(null);

        var response = await ctrl.GetNotifications(1, 20, false, CancellationToken.None);

        response.Should().BeOfType<UnauthorizedResult>();
    }

    // ── UTCID05 ── page 2 (boundary) → 200 OK ─────────────────────────────
    [Fact]
    public async Task CVerify113_UTCID05_GetNotifications_Page2_ReturnsOk()
    {
        var userId = Guid.NewGuid();
        await SeedNotificationsAsync(userId, 3);
        var ctrl = BuildController(userId);

        var response = await ctrl.GetNotifications(2, 10, false, CancellationToken.None);

        response.Should().BeOfType<OkObjectResult>();
    }
}
