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
/// Unit tests for NotificationController.MarkAsRead — CVerify-114 (5 UTCIDs).
/// PUT /api/notifications/{id}/read [Authorize]. Another user's notification is filtered out → 404.
/// </summary>
public sealed class CVerify114_MarkNotificationReadTests : IDisposable
{
    private readonly ApplicationDbContext _context;

    public CVerify114_MarkNotificationReadTests()
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

    private async Task<InAppNotification> SeedNotificationAsync(Guid userId, bool read)
    {
        var n = new InAppNotification
        {
            Id = Guid.NewGuid(), UserId = userId, NotificationType = "JobApplied", ResourceType = "job", IsRead = read,
        };
        _context.InAppNotifications.Add(n);
        await _context.SaveChangesAsync();
        return n;
    }

    // ── UTCID01 ── own unread notification → 200 OK ───────────────────────
    [Fact]
    public async Task CVerify114_UTCID01_MarkRead_OwnUnread_ReturnsOk()
    {
        var userId = Guid.NewGuid();
        var n = await SeedNotificationAsync(userId, read: false);
        var ctrl = BuildController(userId);

        var response = await ctrl.MarkAsRead(n.Id);

        response.Should().BeOfType<OkResult>();
    }

    // ── UTCID02 ── own already-read notification → 200 OK ─────────────────
    [Fact]
    public async Task CVerify114_UTCID02_MarkRead_OwnAlreadyRead_ReturnsOk()
    {
        var userId = Guid.NewGuid();
        var n = await SeedNotificationAsync(userId, read: true);
        var ctrl = BuildController(userId);

        var response = await ctrl.MarkAsRead(n.Id);

        response.Should().BeOfType<OkResult>();
    }

    // ── UTCID03 ── non-existent notification → 404 NotFound ───────────────
    [Fact]
    public async Task CVerify114_UTCID03_MarkRead_NotFound_Returns404()
    {
        var ctrl = BuildController(Guid.NewGuid());

        var response = await ctrl.MarkAsRead(Guid.NewGuid());

        response.Should().BeOfType<NotFoundResult>();
    }

    // ── UTCID04 ── another user's notification → 404 NotFound ─────────────
    [Fact]
    public async Task CVerify114_UTCID04_MarkRead_AnotherUsers_Returns404()
    {
        var owner = Guid.NewGuid();
        var n = await SeedNotificationAsync(owner, read: false);
        var ctrl = BuildController(Guid.NewGuid()); // a different user

        var response = await ctrl.MarkAsRead(n.Id);

        response.Should().BeOfType<NotFoundResult>();
    }

    // ── UTCID05 ── no JWT → 401 Unauthorized ──────────────────────────────
    [Fact]
    public async Task CVerify114_UTCID05_MarkRead_NoJwt_Returns401()
    {
        var ctrl = BuildController(null);

        var response = await ctrl.MarkAsRead(Guid.NewGuid());

        response.Should().BeOfType<UnauthorizedResult>();
    }
}
