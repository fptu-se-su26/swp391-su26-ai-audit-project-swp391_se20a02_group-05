using System;
using System.Collections.Generic;
using System.Net;
using System.Security.Claims;
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
/// Unit tests for AuthService.ChangePasswordAsync — CVerify-05 (10 UTCIDs).
/// </summary>
public sealed class CVerify05_ChangePasswordTests : IDisposable
{
    private static readonly DateTimeOffset FakeNow = new(2024, 6, 1, 12, 0, 0, TimeSpan.Zero);
    private const string OldPassword = "OldPass1!";
    private const string NewPassword = "NewPass2@";

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
    private readonly Mock<HttpContext>                _httpCtx = new();

    private readonly AuthService _sut;
    private Guid _currentUserId;

    public CVerify05_ChangePasswordTests()
    {
        _timeProvider    = new FakeTimeProvider(FakeNow);
        _currentUserId   = Guid.NewGuid();

        _context = new ApplicationDbContext(
            new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .ConfigureWarnings(w => w.Ignore(InMemoryEventId.TransactionIgnoredWarning))
                .Options);

        _cacheService.Setup(c => c.AcquireLockAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<TimeSpan>())).ReturnsAsync(true);
        _cacheService.Setup(c => c.ReleaseLockAsync(It.IsAny<string>(), It.IsAny<string>())).ReturnsAsync(true);

        var connection = new Mock<ConnectionInfo>();
        connection.Setup(c => c.RemoteIpAddress).Returns(IPAddress.Loopback);
        _request.Setup(r => r.Cookies).Returns(_cookies.Object);
        _request.Setup(r => r.Headers).Returns(new HeaderDictionary { ["User-Agent"] = "TestBrowser/1.0" });
        _request.Setup(r => r.Cookies["refresh_token"]).Returns((string?)null);
        _httpCtx.Setup(c => c.Request).Returns(_request.Object);
        _httpCtx.Setup(c => c.Connection).Returns(connection.Object);
        _httpCtxAccessor.Setup(a => a.HttpContext).Returns(_httpCtx.Object);
        SetupAuthUser(_currentUserId);

        _sut = new AuthService(_context, _tokenService.Object, _cacheService.Object, _accountService.Object,
            _identityRepo.Object, _httpCtxAccessor.Object, new EnvConfiguration(), _logger.Object,
            new AuthMetrics(), _timeProvider, _httpClientFactory.Object, _identityStateResolver.Object,
            _passwordPolicyService.Object, _otpPolicyService.Object, _storageService.Object,
            _rateLimitPolicyService.Object, _googleTokenValidator.Object, _usernameService.Object,
            _workspaceMembershipService.Object);
    }

    public void Dispose() => _context.Dispose();

    private void SetupAuthUser(Guid userId)
    {
        var claims = new ClaimsPrincipal(new ClaimsIdentity(
            new[] { new Claim(ClaimTypes.NameIdentifier, userId.ToString()) }, "test"));
        _httpCtx.Setup(c => c.User).Returns(claims);
    }

    private void SetupNoAuthUser()
    {
        _httpCtx.Setup(c => c.User).Returns(new ClaimsPrincipal(new ClaimsIdentity()));
    }

    private async Task<User> SeedUserAsync(Guid? userId = null, string? passwordHash = null)
    {
        var id = userId ?? _currentUserId;
        var user = new User
        {
            Id           = id,
            Email        = $"user-{id:N}@example.com",
            FullName     = "Test User",
            Username     = "testuser",
            PasswordHash = passwordHash ?? BCrypt.Net.BCrypt.HashPassword(OldPassword),
            Status       = UserStatus.ACTIVE,
            EmailVerifiedAt = DateTime.UtcNow,
        };
        _context.Users.Add(user);
        await _context.SaveChangesAsync();
        return user;
    }

    // ── UTCID01 ───────────────────────────────────────────────────────────
    [Fact]
    public async Task CVerify05_UTCID01_ChangePassword_ValidCurrentAndNewPassword_ReturnsTrue()
    {
        await SeedUserAsync();

        var result = await _sut.ChangePasswordAsync(new ChangePasswordRequest(OldPassword, NewPassword, NewPassword));

        result.Should().BeTrue();
    }

    // ── UTCID02 ───────────────────────────────────────────────────────────
    [Fact]
    public async Task CVerify05_UTCID02_ChangePassword_WrongCurrentPassword_ThrowsAuthException()
    {
        await SeedUserAsync();

        var act = async () => await _sut.ChangePasswordAsync(new ChangePasswordRequest("WrongPass!", NewPassword, NewPassword));

        await act.Should().ThrowAsync<AuthException>()
            .Where(e => e.ErrorCode == AuthErrorCodes.InvalidCredentials && e.Message.Contains("current password"));
    }

    // ── UTCID03 ───────────────────────────────────────────────────────────
    [Fact]
    public async Task CVerify05_UTCID03_ChangePassword_NewPasswordTooShort_ThrowsPasswordPolicyViolationException()
    {
        await SeedUserAsync();
        _passwordPolicyService
            .Setup(p => p.ValidateAndThrowAsync("Short1!", "Default"))
            .ThrowsAsync(new PasswordPolicyViolationException(new Dictionary<string, string[]> { ["password"] = new[] { "Password must be at least 8 characters." } }));

        var act = async () => await _sut.ChangePasswordAsync(new ChangePasswordRequest(OldPassword, "Short1!", "Short1!"));

        await act.Should().ThrowAsync<PasswordPolicyViolationException>();
    }

    // ── UTCID04 ───────────────────────────────────────────────────────────
    [Fact]
    public async Task CVerify05_UTCID04_ChangePassword_NoUppercaseInNewPassword_ThrowsPasswordPolicyViolationException()
    {
        await SeedUserAsync();
        _passwordPolicyService
            .Setup(p => p.ValidateAndThrowAsync("alllower1!", "Default"))
            .ThrowsAsync(new PasswordPolicyViolationException(new Dictionary<string, string[]> { ["password"] = new[] { "Password must contain at least one uppercase letter." } }));

        var act = async () => await _sut.ChangePasswordAsync(new ChangePasswordRequest(OldPassword, "alllower1!", "alllower1!"));

        await act.Should().ThrowAsync<PasswordPolicyViolationException>();
    }

    // ── UTCID05 ───────────────────────────────────────────────────────────
    [Fact]
    public async Task CVerify05_UTCID05_ChangePassword_NoDigitInNewPassword_ThrowsPasswordPolicyViolationException()
    {
        await SeedUserAsync();
        _passwordPolicyService
            .Setup(p => p.ValidateAndThrowAsync("NoDigit!Pass", "Default"))
            .ThrowsAsync(new PasswordPolicyViolationException(new Dictionary<string, string[]> { ["password"] = new[] { "Password must contain at least one digit." } }));

        var act = async () => await _sut.ChangePasswordAsync(new ChangePasswordRequest(OldPassword, "NoDigit!Pass", "NoDigit!Pass"));

        await act.Should().ThrowAsync<PasswordPolicyViolationException>();
    }

    // ── UTCID06 ───────────────────────────────────────────────────────────
    [Fact]
    public async Task CVerify05_UTCID06_ChangePassword_NoSpecialCharInNewPassword_ThrowsPasswordPolicyViolationException()
    {
        await SeedUserAsync();
        _passwordPolicyService
            .Setup(p => p.ValidateAndThrowAsync("NoSpecial1Char", "Default"))
            .ThrowsAsync(new PasswordPolicyViolationException(new Dictionary<string, string[]> { ["password"] = new[] { "Password must contain at least one special character." } }));

        var act = async () => await _sut.ChangePasswordAsync(new ChangePasswordRequest(OldPassword, "NoSpecial1Char", "NoSpecial1Char"));

        await act.Should().ThrowAsync<PasswordPolicyViolationException>();
    }

    // ── UTCID07 ───────────────────────────────────────────────────────────
    [Fact]
    public async Task CVerify05_UTCID07_ChangePassword_ConfirmPasswordMismatch_ServiceIgnoresMismatchReturnsTrue()
    {
        // Model validation (controller) catches confirm-mismatch before service is called.
        // Service only uses NewPassword; ConfirmNewPassword is not checked here.
        await SeedUserAsync();

        var result = await _sut.ChangePasswordAsync(new ChangePasswordRequest(OldPassword, NewPassword, "Different3#"));

        result.Should().BeTrue("service does not enforce confirmPassword equality — that is controller responsibility");
    }

    // ── UTCID08 ───────────────────────────────────────────────────────────
    [Fact]
    public async Task CVerify05_UTCID08_ChangePassword_NoAuthentication_ThrowsUnauthorizedAccessException()
    {
        SetupNoAuthUser();

        var act = async () => await _sut.ChangePasswordAsync(new ChangePasswordRequest(OldPassword, NewPassword, NewPassword));

        await act.Should().ThrowAsync<UnauthorizedAccessException>().WithMessage("*not authenticated*");
    }

    // ── UTCID09 ───────────────────────────────────────────────────────────
    [Fact]
    public async Task CVerify05_UTCID09_ChangePassword_ExactlyEightCharNewPassword_ReturnsTrue()
    {
        await SeedUserAsync();
        const string eightCharPass = "Exact8!a";

        var result = await _sut.ChangePasswordAsync(new ChangePasswordRequest(OldPassword, eightCharPass, eightCharPass));

        result.Should().BeTrue();
    }

    // ── UTCID10 ───────────────────────────────────────────────────────────
    [Fact]
    public async Task CVerify05_UTCID10_ChangePassword_NewPasswordSameAsOld_ThrowsAuthExceptionPasswordPolicyViolation()
    {
        await SeedUserAsync();

        var act = async () => await _sut.ChangePasswordAsync(new ChangePasswordRequest(OldPassword, OldPassword, OldPassword));

        await act.Should().ThrowAsync<AuthException>()
            .Where(e => e.ErrorCode == AuthErrorCodes.PasswordPolicyViolation && e.Message.Contains("same"));
    }
}
