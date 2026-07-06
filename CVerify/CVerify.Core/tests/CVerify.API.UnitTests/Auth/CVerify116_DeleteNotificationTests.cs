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
/// Unit tests for NotificationController.DeleteNotification — CVerify-116 (4 UTCIDs).
/// DELETE /api/notifications/{id} [Authorize] (soft delete). Another user's notification → 404.
/// </summary>
public sealed class CVerify116_DeleteNotificationTests : IDisposable
{
    private readonly ApplicationDbContext _context;

    public CVerify116_DeleteNotificationTests()
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

    private async Task<InAppNotification> SeedNotificationAsync(Guid userId)
    {
        var n = new InAppNotification
        {
            Id = Guid.NewGuid(), UserId = userId, NotificationType = "JobApplied", ResourceType = "job",
        };
        _context.InAppNotifications.Add(n);
        await _context.SaveChangesAsync();
        return n;
    }

    // ── UTCID01 ── own notification → 200 OK ──────────────────────────────
    [Fact]
    public async Task CVerify116_UTCID01_Delete_Own_ReturnsOk()
    {
        var userId = Guid.NewGuid();
        var n = await SeedNotificationAsync(userId);
        var ctrl = BuildController(userId);

        var response = await ctrl.DeleteNotification(n.Id);

        response.Should().BeOfType<OkResult>();
    }

    // ── UTCID02 ── non-existent notification → 404 NotFound ───────────────
    [Fact]
    public async Task CVerify116_UTCID02_Delete_NotFound_Returns404()
    {
        var ctrl = BuildController(Guid.NewGuid());

        var response = await ctrl.DeleteNotification(Guid.NewGuid());

        response.Should().BeOfType<NotFoundResult>();
    }

    // ── UTCID03 ── another user's notification → 404 NotFound ─────────────
    [Fact]
    public async Task CVerify116_UTCID03_Delete_AnotherUsers_Returns404()
    {
        var owner = Guid.NewGuid();
        var n = await SeedNotificationAsync(owner);
        var ctrl = BuildController(Guid.NewGuid()); // different user

        var response = await ctrl.DeleteNotification(n.Id);

        response.Should().BeOfType<NotFoundResult>();
    }

    // ── UTCID04 ── no JWT → 401 Unauthorized ──────────────────────────────
    [Fact]
    public async Task CVerify116_UTCID04_Delete_NoJwt_Returns401()
    {
        var ctrl = BuildController(null);

        var response = await ctrl.DeleteNotification(Guid.NewGuid());

        response.Should().BeOfType<UnauthorizedResult>();
    }
}
