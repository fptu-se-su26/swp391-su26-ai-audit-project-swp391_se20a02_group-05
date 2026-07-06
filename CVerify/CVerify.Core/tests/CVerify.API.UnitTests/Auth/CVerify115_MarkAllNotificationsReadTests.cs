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
/// Unit tests for NotificationController.MarkAllAsRead — CVerify-115 (3 UTCIDs).
/// PUT /api/notifications/read-all [Authorize].
/// </summary>
public sealed class CVerify115_MarkAllNotificationsReadTests : IDisposable
{
    private readonly ApplicationDbContext _context;

    public CVerify115_MarkAllNotificationsReadTests()
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

    private async Task SeedNotificationsAsync(Guid userId, int count, bool read)
    {
        for (var i = 0; i < count; i++)
        {
            _context.InAppNotifications.Add(new InAppNotification
            {
                Id = Guid.NewGuid(), UserId = userId, NotificationType = "JobApplied", ResourceType = "job", IsRead = read,
            });
        }
        await _context.SaveChangesAsync();
    }

    // ── UTCID01 ── has unread notifications → 200 OK ──────────────────────
    [Fact]
    public async Task CVerify115_UTCID01_MarkAll_HasUnread_ReturnsOk()
    {
        var userId = Guid.NewGuid();
        await SeedNotificationsAsync(userId, 3, read: false);
        var ctrl = BuildController(userId);

        var response = await ctrl.MarkAllAsRead();

        response.Should().BeOfType<OkResult>();
    }

    // ── UTCID02 ── no unread notifications → 200 OK ───────────────────────
    [Fact]
    public async Task CVerify115_UTCID02_MarkAll_NoUnread_ReturnsOk()
    {
        var userId = Guid.NewGuid();
        await SeedNotificationsAsync(userId, 2, read: true);
        var ctrl = BuildController(userId);

        var response = await ctrl.MarkAllAsRead();

        response.Should().BeOfType<OkResult>();
    }

    // ── UTCID03 ── no JWT → 401 Unauthorized ──────────────────────────────
    [Fact]
    public async Task CVerify115_UTCID03_MarkAll_NoJwt_Returns401()
    {
        var ctrl = BuildController(null);

        var response = await ctrl.MarkAllAsRead();

        response.Should().BeOfType<UnauthorizedResult>();
    }
}
