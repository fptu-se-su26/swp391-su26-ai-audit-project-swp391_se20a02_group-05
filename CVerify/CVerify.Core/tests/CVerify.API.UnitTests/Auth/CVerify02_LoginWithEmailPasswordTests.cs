using System;
using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;
using BCrypt.Net;
using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Time.Testing;
using Moq;
using Xunit;
using CVerify.API.Modules.Auth.DTOs;
using CVerify.API.Modules.Auth.Services;
using CVerify.API.Modules.Auth.Services.OtpPolicies;
using CVerify.API.Modules.Auth.Services.PasswordPolicies;
using CVerify.API.Modules.Shared.Configuration;
using CVerify.API.Modules.Shared.Diagnostics;
using CVerify.API.Modules.Shared.Domain.Entities;
using CVerify.API.Modules.Shared.Domain.Enums;
using CVerify.API.Modules.Shared.Exceptions;
using CVerify.API.Modules.Shared.Persistence;
using CVerify.API.Modules.Shared.Security;
using CVerify.API.Modules.Shared.Storage.Interfaces;
using CVerify.API.Modules.Shared.System.Services;

namespace CVerify.API.UnitTests.Auth;

/// <summary>
/// Unit tests for AuthService.LoginAsync — CVerify-02 (9 UTCIDs).
/// </summary>
public sealed class CVerify02_LoginWithEmailPasswordTests : IDisposable
{
    private static readonly DateTimeOffset FakeNow = new(2024, 6, 1, 12, 0, 0, TimeSpan.Zero);
    private const string CorrectPassword = "CorrectPass1!";

    private readonly FakeTimeProvider _timeProvider;
    private readonly ApplicationDbContext _context;

    private readonly Mock<ITokenService>               _tokenService              = new();
    private readonly Mock<ICacheService>               _cacheService              = new();
    private readonly Mock<IAccountService>             _accountService            = new();
    private readonly Mock<IIdentityRepository>         _identityRepo              = new();
    private readonly Mock<IHttpContextAccessor>        _httpCtxAccessor           = new();
    private readonly Mock<ILogger<AuthService>>        _logger                    = new();
    private readonly Mock<IHttpClientFactory>          _httpClientFactory         = new();
    private readonly Mock<IIdentityStateResolver>      _identityStateResolver     = new();
    private readonly Mock<IPasswordPolicyService>      _passwordPolicyService     = new();
    private readonly Mock<IOtpPolicyService>           _otpPolicyService          = new();
    private readonly Mock<IStorageService>             _storageService            = new();
    private readonly Mock<IRateLimitPolicyService>     _rateLimitPolicyService    = new();
    private readonly Mock<IGoogleTokenValidator>       _googleTokenValidator      = new();
    private readonly Mock<IUsernameService>            _usernameService           = new();
    private readonly Mock<IWorkspaceMembershipService> _workspaceMembershipService = new();

    private readonly Mock<IRequestCookieCollection>   _cookies = new();
    private readonly Mock<HttpRequest>                _request = new();

    private readonly AuthService _sut;

    public CVerify02_LoginWithEmailPasswordTests()
    {
        _timeProvider = new FakeTimeProvider(FakeNow);

        _context = new ApplicationDbContext(
            new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .ConfigureWarnings(w => w.Ignore(InMemoryEventId.TransactionIgnoredWarning))
                .Options);

        _cacheService.Setup(c => c.AcquireLockAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<TimeSpan>())).ReturnsAsync(true);
        _cacheService.Setup(c => c.ReleaseLockAsync(It.IsAny<string>(), It.IsAny<string>())).ReturnsAsync(true);
        _cacheService.Setup(c => c.RemoveAsync(It.IsAny<string>())).Returns(Task.CompletedTask);
        _cacheService.Setup(c => c.AddToSetAsync(It.IsAny<string>(), It.IsAny<string>())).Returns(Task.CompletedTask);
        _cacheService.Setup(c => c.SetAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<TimeSpan?>())).Returns(Task.CompletedTask);

        _tokenService.Setup(t => t.GenerateJwtToken(It.IsAny<User>(), It.IsAny<IEnumerable<string>>(), It.IsAny<IEnumerable<string>>(), It.IsAny<Guid?>(), It.IsAny<string?>(), It.IsAny<Guid?>())).Returns("JWT");
        _tokenService.Setup(t => t.GenerateRefreshToken()).Returns("REFRESH");
        _tokenService.Setup(t => t.SetTokenInsideCookie(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<DateTime?>()));

        _identityRepo.Setup(r => r.GetUserRolesAsync(It.IsAny<Guid>())).ReturnsAsync(new[] { "USER" });
        _identityRepo.Setup(r => r.GetUserPermissionsAsync(It.IsAny<Guid>())).ReturnsAsync(Array.Empty<string>());

        _accountService.Setup(a => a.ResetFailedAttemptsAsync(It.IsAny<User>())).Returns(Task.CompletedTask);
        _accountService.Setup(a => a.HandleFailedLoginAsync(It.IsAny<User>())).Returns(Task.CompletedTask);

        _workspaceMembershipService.Setup(w => w.BootstrapInitialAdminAsync(It.IsAny<string>(), It.IsAny<bool>(), It.IsAny<System.Threading.CancellationToken>())).Returns(Task.CompletedTask);

        var connection = new Mock<ConnectionInfo>();
        connection.Setup(c => c.RemoteIpAddress).Returns(IPAddress.Loopback);
        _request.Setup(r => r.Cookies).Returns(_cookies.Object);
        _request.Setup(r => r.Headers).Returns(new HeaderDictionary { ["User-Agent"] = "TestBrowser/1.0" });
        var httpCtx = new Mock<HttpContext>();
        httpCtx.Setup(c => c.Request).Returns(_request.Object);
        httpCtx.Setup(c => c.Connection).Returns(connection.Object);
        _httpCtxAccessor.Setup(a => a.HttpContext).Returns(httpCtx.Object);

        _sut = new AuthService(_context, _tokenService.Object, _cacheService.Object, _accountService.Object,
            _identityRepo.Object, _httpCtxAccessor.Object, new EnvConfiguration(), _logger.Object,
            new AuthMetrics(), _timeProvider, _httpClientFactory.Object, _identityStateResolver.Object,
            _passwordPolicyService.Object, _otpPolicyService.Object, _storageService.Object,
            _rateLimitPolicyService.Object, _googleTokenValidator.Object, _usernameService.Object,
            _workspaceMembershipService.Object);
    }

    public void Dispose() => _context.Dispose();

    private async Task<User> SeedUserAsync(
        string email,
        string? password = CorrectPassword,
        UserStatus status = UserStatus.ACTIVE,
        int failedAttempts = 0,
        DateTimeOffset? lockUntil = null)
    {
        var user = new User
        {
            Id             = Guid.NewGuid(),
            Email          = email,
            FullName       = "Test User",
            Username       = "testuser_" + Guid.NewGuid().ToString("N")[..6],
            PasswordHash   = password != null ? BCrypt.Net.BCrypt.HashPassword(password) : null,
            Status         = status,
            FailedAttempts = failedAttempts,
            LockUntil      = lockUntil,
            EmailVerifiedAt = status == UserStatus.ACTIVE ? DateTime.UtcNow : null,
        };
        _context.Users.Add(user);
        await _context.SaveChangesAsync();
        return user;
    }

    // ── UTCID01 ───────────────────────────────────────────────────────────
    [Fact]
    public async Task CVerify02_UTCID01_Login_ValidCredentials_Returns200WithAuthResponse()
    {
        await SeedUserAsync("valid@example.com");

        var result = await _sut.LoginAsync(new LoginRequest("valid@example.com", CorrectPassword));

        result.Should().NotBeNull();
        result!.Email.Should().Be("valid@example.com");
        result.Status.Should().Be("ACTIVE");
        result.NextStep.Should().Be("DASHBOARD");
    }

    // ── UTCID02 ───────────────────────────────────────────────────────────
    [Fact]
    public async Task CVerify02_UTCID02_Login_WrongPassword_ReturnsNull()
    {
        await SeedUserAsync("valid@example.com");

        var result = await _sut.LoginAsync(new LoginRequest("valid@example.com", "WrongPassword"));

        result.Should().BeNull();
        _accountService.Verify(a => a.HandleFailedLoginAsync(It.IsAny<User>()), Times.Once);
    }

    // ── UTCID03 ───────────────────────────────────────────────────────────
    [Fact]
    public async Task CVerify02_UTCID03_Login_NonExistentEmail_ReturnsNull()
    {
        var result = await _sut.LoginAsync(new LoginRequest("notexist@example.com", "AnyPass1!"));

        result.Should().BeNull();
    }

    // ── UTCID04 ───────────────────────────────────────────────────────────
    [Fact]
    public async Task CVerify02_UTCID04_Login_LockedAccount_ThrowsUnauthorizedAccessException()
    {
        await SeedUserAsync("locked@example.com", lockUntil: FakeNow.AddMinutes(10));
        _accountService.Setup(a => a.IsAccountLocked(It.IsAny<User>())).Returns(true);

        var act = async () => await _sut.LoginAsync(new LoginRequest("locked@example.com", CorrectPassword));

        await act.Should().ThrowAsync<UnauthorizedAccessException>().WithMessage("*locked*");
    }

    // ── UTCID05 ───────────────────────────────────────────────────────────
    [Fact]
    public async Task CVerify02_UTCID05_Login_BannedUser_ThrowsUnauthorizedAccessException()
    {
        await SeedUserAsync("banned@example.com", status: UserStatus.BANNED);
        _accountService.Setup(a => a.IsAccountDisabled(It.IsAny<User>())).Returns(true);

        var act = async () => await _sut.LoginAsync(new LoginRequest("banned@example.com", CorrectPassword));

        await act.Should().ThrowAsync<UnauthorizedAccessException>().WithMessage("*disabled*");
    }

    // ── UTCID06 ───────────────────────────────────────────────────────────
    [Fact]
    public async Task CVerify02_UTCID06_Login_DeletedUser_ThrowsUnauthorizedAccessException()
    {
        await SeedUserAsync("deleted@example.com", status: UserStatus.DELETED);
        _accountService.Setup(a => a.IsAccountDisabled(It.IsAny<User>())).Returns(true);

        var act = async () => await _sut.LoginAsync(new LoginRequest("deleted@example.com", CorrectPassword));

        await act.Should().ThrowAsync<UnauthorizedAccessException>().WithMessage("*disabled*");
    }

    // ── UTCID07 ───────────────────────────────────────────────────────────
    [Fact]
    public async Task CVerify02_UTCID07_Login_EmailVerifyPendingStatus_ReturnsEmailVerifyPendingResponse()
    {
        await SeedUserAsync("unverified@example.com", status: UserStatus.EMAIL_VERIFY_PENDING);

        var result = await _sut.LoginAsync(new LoginRequest("unverified@example.com", CorrectPassword));

        result.Should().NotBeNull();
        result!.Status.Should().Be("EMAIL_VERIFY_PENDING");
        result.NextStep.Should().Be("VERIFY_EMAIL");
    }

    // ── UTCID08 ───────────────────────────────────────────────────────────
    [Fact]
    public async Task CVerify02_UTCID08_Login_FourPriorFailuresPlusWrongPassword_CallsHandleFailedLoginAndReturnsNull()
    {
        await SeedUserAsync("failuser@example.com", failedAttempts: 4);

        var result = await _sut.LoginAsync(new LoginRequest("failuser@example.com", "WrongPass"));

        result.Should().BeNull();
        _accountService.Verify(a => a.HandleFailedLoginAsync(It.IsAny<User>()), Times.Once);
    }

    // ── UTCID09 ───────────────────────────────────────────────────────────
    [Fact]
    public async Task CVerify02_UTCID09_Login_ExpiredLockWithCorrectPassword_Returns200()
    {
        await SeedUserAsync("exlock@example.com", lockUntil: FakeNow.AddMinutes(-5));
        // IsAccountLocked returns false because LockUntil < UtcNow
        _accountService.Setup(a => a.IsAccountLocked(It.IsAny<User>())).Returns(false);

        var result = await _sut.LoginAsync(new LoginRequest("exlock@example.com", CorrectPassword));

        result.Should().NotBeNull();
        result!.Status.Should().Be("ACTIVE");
    }
}
