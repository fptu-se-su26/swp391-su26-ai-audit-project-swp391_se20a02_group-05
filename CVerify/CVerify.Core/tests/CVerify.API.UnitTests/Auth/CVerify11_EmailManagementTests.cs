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
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;
using CVerify.API.Modules.Auth.Controllers;
using CVerify.API.Modules.Auth.DTOs;
using CVerify.API.Modules.Auth.Services;
using CVerify.API.Modules.Shared.Domain.Entities;
using CVerify.API.Modules.Shared.Domain.Enums;
using CVerify.API.Modules.Shared.Persistence;

namespace CVerify.API.UnitTests.Auth;

/// <summary>
/// Unit tests for AuthController email management endpoints — CVerify-11 (8 UTCIDs).
/// GET /api/auth/emails, POST /api/auth/emails/send-otp, DELETE /api/auth/emails/{id}.
/// Logic lives in the controller using the service-locator pattern for ApplicationDbContext.
/// </summary>
public sealed class CVerify11_EmailManagementTests : IDisposable
{
    private readonly ServiceProvider _sp;
    private readonly ApplicationDbContext _context;

    private readonly Mock<IAuthService>             _authService             = new();
    private readonly Mock<IIdentityStateResolver>   _identityStateResolver   = new();
    private readonly Mock<IWorkspaceProvisioningService> _workspaceProv       = new();

    private readonly AuthController _sut;

    public CVerify11_EmailManagementTests()
    {
        var services = new ServiceCollection();
        services.AddDbContext<ApplicationDbContext>(opts =>
            opts.UseInMemoryDatabase(Guid.NewGuid().ToString())
                .ConfigureWarnings(w => w.Ignore(InMemoryEventId.TransactionIgnoredWarning)));
        _sp = services.BuildServiceProvider();
        _context = _sp.GetRequiredService<ApplicationDbContext>();

        _authService
            .Setup(a => a.SendOtpAsync(It.IsAny<SendOtpRequest>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new SendOtpResponse(Guid.NewGuid(), "new@example.com", 60));
        _authService
            .Setup(a => a.VerifyOtpAsync(It.IsAny<VerifyOtpRequest>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new VerifyOtpResponse(Guid.NewGuid(), "new@example.com", "VALID_TOKEN"));
        _authService
            .Setup(a => a.ClaimPendingRelationshipsAsync(It.IsAny<Guid>()))
            .Returns(Task.CompletedTask);
        _identityStateResolver
            .Setup(r => r.InvalidateCacheAsync(It.IsAny<string>()))
            .Returns(Task.CompletedTask);

        _sut = new AuthController(
            _authService.Object,
            _identityStateResolver.Object,
            new Mock<ILogger<AuthController>>().Object,
            _workspaceProv.Object);
    }

    public void Dispose() => _sp.Dispose();

    private void SetupUser(Guid userId)
    {
        var ctx = new DefaultHttpContext { RequestServices = _sp };
        ctx.User = new ClaimsPrincipal(new ClaimsIdentity(
            new[] { new Claim(ClaimTypes.NameIdentifier, userId.ToString()) }, "Test"));
        _sut.ControllerContext = new ControllerContext { HttpContext = ctx };
    }

    private async Task<User> SeedUserAsync(string email = "user@example.com")
    {
        var user = new User
        {
            Id = Guid.NewGuid(),
            Email = email,
            FullName = "Test User",
            Username = "testuser",
            Status = UserStatus.ACTIVE,
            EmailVerifiedAt = DateTime.UtcNow,
        };
        _context.Users.Add(user);
        await _context.SaveChangesAsync();
        return user;
    }

    // ── UTCID01 ───────────────────────────────────────────────────────────
    [Fact]
    public async Task CVerify11_UTCID01_GetEmails_UserWithOnePrimaryEmail_ReturnsListWithOneItem()
    {
        var user = await SeedUserAsync();
        SetupUser(user.Id);

        var result = await _sut.GetEmails(CancellationToken.None);

        var ok = result.Should().BeOfType<OkObjectResult>().Subject;
        var list = ok.Value.Should().BeAssignableTo<System.Collections.IEnumerable>().Subject;
        var count = 0;
        foreach (var _ in list) count++;
        count.Should().Be(1, "only the primary email is present");
    }

    // ── UTCID02 ───────────────────────────────────────────────────────────
    [Fact]
    public async Task CVerify11_UTCID02_GetEmails_UserWithThreeEmails_ReturnsListWithThreeItems()
    {
        var user = await SeedUserAsync();
        user.LinkedEmails.Add(new LinkedEmail { Id = Guid.NewGuid(), Email = "second@example.com", IsVerified = true });
        user.LinkedEmails.Add(new LinkedEmail { Id = Guid.NewGuid(), Email = "third@example.com",  IsVerified = true });
        await _context.SaveChangesAsync();
        SetupUser(user.Id);

        var result = await _sut.GetEmails(CancellationToken.None);

        var ok = result.Should().BeOfType<OkObjectResult>().Subject;
        var list = ok.Value.Should().BeAssignableTo<System.Collections.IEnumerable>().Subject;
        var count = 0;
        foreach (var _ in list) count++;
        count.Should().Be(3, "primary + 2 secondary emails");
    }

    // ── UTCID03 ───────────────────────────────────────────────────────────
    [Fact]
    public async Task CVerify11_UTCID03_SendOtp_NewEmailNotInSystem_Returns200WithSendOtpResponse()
    {
        var user = await SeedUserAsync();
        SetupUser(user.Id);

        var req = new AuthController.SendEmailLinkOtpRequest { Email = "new@example.com" };
        var result = await _sut.SendLinkEmailOtp(req, CancellationToken.None);

        result.Should().BeOfType<OkObjectResult>();
    }

    // ── UTCID04 ───────────────────────────────────────────────────────────
    [Fact]
    public async Task CVerify11_UTCID04_SendOtp_EmailAlreadyRegistered_Returns400BadRequest()
    {
        var user = await SeedUserAsync("taken@example.com");
        var otherUser = await SeedUserAsync("sender@example.com");
        SetupUser(otherUser.Id);

        var req = new AuthController.SendEmailLinkOtpRequest { Email = "taken@example.com" };
        var result = await _sut.SendLinkEmailOtp(req, CancellationToken.None);

        result.Should().BeOfType<BadRequestObjectResult>();
    }

    // ── UTCID05 ───────────────────────────────────────────────────────────
    [Fact]
    public async Task CVerify11_UTCID05_SendOtp_UserAtMaxTwoSecondary_Returns400BadRequest()
    {
        var user = await SeedUserAsync();
        user.LinkedEmails.Add(new LinkedEmail { Id = Guid.NewGuid(), Email = "s1@example.com", IsVerified = true });
        user.LinkedEmails.Add(new LinkedEmail { Id = Guid.NewGuid(), Email = "s2@example.com", IsVerified = true });
        await _context.SaveChangesAsync();
        SetupUser(user.Id);

        var req = new AuthController.SendEmailLinkOtpRequest { Email = "third@example.com" };
        var result = await _sut.SendLinkEmailOtp(req, CancellationToken.None);

        result.Should().BeOfType<BadRequestObjectResult>();
    }

    // ── UTCID06 ───────────────────────────────────────────────────────────
    [Fact]
    public async Task CVerify11_UTCID06_DeleteLinkedEmail_ValidSecondaryEmailId_Returns200Success()
    {
        var user = await SeedUserAsync();
        var linkedEmailId = Guid.NewGuid();
        user.LinkedEmails.Add(new LinkedEmail { Id = linkedEmailId, Email = "secondary@example.com", IsVerified = true });
        await _context.SaveChangesAsync();
        SetupUser(user.Id);

        var result = await _sut.DeleteLinkedEmail(linkedEmailId, CancellationToken.None);

        result.Should().BeOfType<OkObjectResult>();
    }

    // ── UTCID07 ───────────────────────────────────────────────────────────
    [Fact]
    public async Task CVerify11_UTCID07_DeleteLinkedEmail_NonExistentEmailId_Returns404NotFound()
    {
        var user = await SeedUserAsync();
        SetupUser(user.Id);

        var result = await _sut.DeleteLinkedEmail(Guid.NewGuid(), CancellationToken.None);

        result.Should().BeOfType<NotFoundObjectResult>();
    }

    // ── UTCID08 ───────────────────────────────────────────────────────────
    // No JWT → controller finds no NameIdentifier claim → Unauthorized.
    [Fact]
    public async Task CVerify11_UTCID08_GetEmails_NoJwt_Returns401Unauthorized()
    {
        var ctx = new DefaultHttpContext { RequestServices = _sp };
        // No user claims — anonymous
        _sut.ControllerContext = new ControllerContext { HttpContext = ctx };

        var result = await _sut.GetEmails(CancellationToken.None);

        result.Should().BeOfType<UnauthorizedResult>();
    }
}
