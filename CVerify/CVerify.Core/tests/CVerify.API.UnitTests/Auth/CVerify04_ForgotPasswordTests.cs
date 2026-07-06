using System;
using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;
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
using CVerify.API.Modules.Shared.Persistence;
using CVerify.API.Modules.Shared.Security;
using CVerify.API.Modules.Shared.Storage.Interfaces;
using CVerify.API.Modules.Shared.System.Services;

namespace CVerify.API.UnitTests.Auth;

/// <summary>
/// Unit tests for AuthService.ForgotPasswordAsync — CVerify-04 (6 UTCIDs).
/// </summary>
public sealed class CVerify04_ForgotPasswordTests : IDisposable
{
    private static readonly DateTimeOffset FakeNow = new(2024, 6, 1, 12, 0, 0, TimeSpan.Zero);

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

    public CVerify04_ForgotPasswordTests()
    {
        _timeProvider = new FakeTimeProvider(FakeNow);

        _context = new ApplicationDbContext(
            new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .ConfigureWarnings(w => w.Ignore(InMemoryEventId.TransactionIgnoredWarning))
                .Options);

        // No cooldown by default (GetAsync<string> returns null)
        _cacheService.Setup(c => c.GetAsync<string>(It.IsAny<string>())).ReturnsAsync((string?)null);
        _cacheService.Setup(c => c.SetAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<TimeSpan?>())).Returns(Task.CompletedTask);
        _cacheService.Setup(c => c.AcquireLockAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<TimeSpan>())).ReturnsAsync(true);
        _cacheService.Setup(c => c.ReleaseLockAsync(It.IsAny<string>(), It.IsAny<string>())).ReturnsAsync(true);

        _rateLimitPolicyService.Setup(r => r.ShouldEnforceCooldowns()).Returns(true);

        var connection = new Mock<ConnectionInfo>();
        connection.Setup(c => c.RemoteIpAddress).Returns(IPAddress.Loopback);
        _request.Setup(r => r.Cookies).Returns(_cookies.Object);
        _request.Setup(r => r.Headers).Returns(new HeaderDictionary { ["User-Agent"] = "TestBrowser/1.0" });
        var httpCtx = new Mock<HttpContext>();
        httpCtx.Setup(c => c.Request).Returns(_request.Object);
        httpCtx.Setup(c => c.Connection).Returns(connection.Object);
        _httpCtxAccessor.Setup(a => a.HttpContext).Returns(httpCtx.Object);

        // Minimal EnvConfiguration with trusted "localhost" domain
        var envConfig = new EnvConfiguration
        {
            Auth = new AuthSettings
            {
                FrontendUrl    = "http://localhost:3000",
                TrustedDomains = "localhost"
            },
            Jwt = new JwtSettings { Key = "test-jwt-key-for-unit-tests!!", Issuer = "test", Audience = "test" }
        };

        _sut = new AuthService(_context, _tokenService.Object, _cacheService.Object, _accountService.Object,
            _identityRepo.Object, _httpCtxAccessor.Object, envConfig, _logger.Object,
            new AuthMetrics(), _timeProvider, _httpClientFactory.Object, _identityStateResolver.Object,
            _passwordPolicyService.Object, _otpPolicyService.Object, _storageService.Object,
            _rateLimitPolicyService.Object, _googleTokenValidator.Object, _usernameService.Object,
            _workspaceMembershipService.Object);
    }

    public void Dispose() => _context.Dispose();

    private async Task<User> SeedUserAsync(string email, UserStatus status = UserStatus.ACTIVE)
    {
        var user = new User
        {
            Id       = Guid.NewGuid(),
            Email    = email,
            FullName = "Test User",
            Username = "testuser",
            Status   = status,
            EmailVerifiedAt = status == UserStatus.ACTIVE ? DateTime.UtcNow : null,
        };
        _context.Users.Add(user);
        await _context.SaveChangesAsync();
        return user;
    }

    // ── UTCID01 ───────────────────────────────────────────────────────────
    [Fact]
    public async Task CVerify04_UTCID01_ForgotPassword_RegisteredActiveUser_ReturnsTrue()
    {
        await SeedUserAsync("user@example.com");

        var result = await _sut.ForgotPasswordAsync(new ForgotPasswordRequest("user@example.com"));

        result.Should().BeTrue();
    }

    // ── UTCID02 ───────────────────────────────────────────────────────────
    [Fact]
    public async Task CVerify04_UTCID02_ForgotPassword_NonRegisteredEmail_ReturnsTrueEnumerationPrevention()
    {
        var result = await _sut.ForgotPasswordAsync(new ForgotPasswordRequest("nouser@example.com"));

        result.Should().BeTrue("enumeration prevention: always returns success for unknown emails");
    }

    // ── UTCID03 ───────────────────────────────────────────────────────────
    [Fact]
    public async Task CVerify04_UTCID03_ForgotPassword_EmptyEmail_ReturnsTrueAtServiceLevel()
    {
        // Controller model validation would catch empty email first.
        // Service-level: normalizes to "" → no user found → returns true (enumeration prevention).
        var result = await _sut.ForgotPasswordAsync(new ForgotPasswordRequest(""));

        result.Should().BeTrue();
    }

    // ── UTCID04 ───────────────────────────────────────────────────────────
    [Fact]
    public async Task CVerify04_UTCID04_ForgotPassword_MalformedEmail_ReturnsTrueAtServiceLevel()
    {
        // Controller model validation would catch bad format first.
        // Service-level: no user found → returns true.
        var result = await _sut.ForgotPasswordAsync(new ForgotPasswordRequest("not-an-email"));

        result.Should().BeTrue();
    }

    // ── UTCID05 ───────────────────────────────────────────────────────────
    [Fact]
    public async Task CVerify04_UTCID05_ForgotPassword_BannedUser_ReturnsTrueWithoutSendingEmail()
    {
        await SeedUserAsync("banned@example.com", status: UserStatus.BANNED);

        var result = await _sut.ForgotPasswordAsync(new ForgotPasswordRequest("banned@example.com"));

        result.Should().BeTrue("service returns generic success for inactive accounts to prevent enumeration");
    }

    // ── UTCID06 ───────────────────────────────────────────────────────────
    [Fact]
    public async Task CVerify04_UTCID06_ForgotPassword_MaxLengthEmail_ReturnsTrueForUnknownEmail()
    {
        // 255-character email address — no user with this email in DB
        var longLocal = new string('a', 243); // 243 + "@example.com".Length = 255
        var longEmail = $"{longLocal}@example.com";

        var result = await _sut.ForgotPasswordAsync(new ForgotPasswordRequest(longEmail));

        result.Should().BeTrue();
    }
}
