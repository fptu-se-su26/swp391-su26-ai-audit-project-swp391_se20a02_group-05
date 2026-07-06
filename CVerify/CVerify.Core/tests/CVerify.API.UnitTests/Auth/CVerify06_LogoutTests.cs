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
using CVerify.API.Modules.Auth.Entities;
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
/// Unit tests for AuthService.LogoutAsync — CVerify-06 (4 UTCIDs).
/// </summary>
public sealed class CVerify06_LogoutTests : IDisposable
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

    private readonly Mock<IRequestCookieCollection> _cookies = new();
    private readonly Mock<HttpRequest>              _request = new();
    private readonly Mock<HttpContext>              _httpCtx = new();

    private readonly AuthService _sut;

    public CVerify06_LogoutTests()
    {
        _timeProvider = new FakeTimeProvider(FakeNow);

        _context = new ApplicationDbContext(
            new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .ConfigureWarnings(w => w.Ignore(InMemoryEventId.TransactionIgnoredWarning))
                .Options);

        var connection = new Mock<ConnectionInfo>();
        connection.Setup(c => c.RemoteIpAddress).Returns(IPAddress.Loopback);
        _request.Setup(r => r.Cookies).Returns(_cookies.Object);
        _request.Setup(r => r.Headers).Returns(new HeaderDictionary { ["User-Agent"] = "TestBrowser/1.0" });
        _httpCtx.Setup(c => c.Request).Returns(_request.Object);
        _httpCtx.Setup(c => c.Connection).Returns(connection.Object);
        _httpCtxAccessor.Setup(a => a.HttpContext).Returns(_httpCtx.Object);

        _tokenService.Setup(t => t.RemoveTokenFromCookie(It.IsAny<string>()));

        _sut = new AuthService(_context, _tokenService.Object, _cacheService.Object, _accountService.Object,
            _identityRepo.Object, _httpCtxAccessor.Object, new EnvConfiguration(), _logger.Object,
            new AuthMetrics(), _timeProvider, _httpClientFactory.Object, _identityStateResolver.Object,
            _passwordPolicyService.Object, _otpPolicyService.Object, _storageService.Object,
            _rateLimitPolicyService.Object, _googleTokenValidator.Object, _usernameService.Object,
            _workspaceMembershipService.Object);
    }

    public void Dispose() => _context.Dispose();

    private void SetupCookie(string? value) =>
        _cookies.Setup(c => c["refresh_token"]).Returns(value);

    private async Task<(User user, RefreshToken token)> SeedActiveSessionAsync(string tokenStr = "REFRESH_TOKEN_001")
    {
        var user = new User
        {
            Id      = Guid.NewGuid(),
            Email   = "user@example.com",
            FullName = "Test User",
            Status  = UserStatus.ACTIVE,
            EmailVerifiedAt = DateTime.UtcNow,
        };
        var token = new RefreshToken
        {
            Id        = Guid.NewGuid(),
            UserId    = user.Id,
            Token     = tokenStr,
            SessionId = Guid.NewGuid(),
            ExpiresAt = FakeNow.AddHours(24),
        };
        _context.Users.Add(user);
        _context.RefreshTokens.Add(token);
        await _context.SaveChangesAsync();
        return (user, token);
    }

    // ── UTCID01 ───────────────────────────────────────────────────────────
    [Fact]
    public async Task CVerify06_UTCID01_Logout_WithValidRefreshTokenCookie_RevokesTokenAndClearsCookies()
    {
        const string tokenStr = "REFRESH_TOKEN_001";
        var (_, token) = await SeedActiveSessionAsync(tokenStr);
        SetupCookie(tokenStr);

        await _sut.LogoutAsync();

        // Token should be revoked in DB
        var storedToken = await _context.RefreshTokens.FindAsync(token.Id);
        storedToken!.RevokedAt.Should().NotBeNull("refresh token must be revoked on logout");

        // Cookies must be cleared
        _tokenService.Verify(t => t.RemoveTokenFromCookie("access_token"), Times.Once);
        _tokenService.Verify(t => t.RemoveTokenFromCookie("refresh_token"), Times.Once);
    }

    // ── UTCID02 ───────────────────────────────────────────────────────────
    [Fact]
    public async Task CVerify06_UTCID02_Logout_NoCookies_CompletesWithoutError()
    {
        SetupCookie(null);

        var act = async () => await _sut.LogoutAsync();

        await act.Should().NotThrowAsync();
        _tokenService.Verify(t => t.RemoveTokenFromCookie("access_token"), Times.Once);
        _tokenService.Verify(t => t.RemoveTokenFromCookie("refresh_token"), Times.Once);
    }

    // ── UTCID03 ───────────────────────────────────────────────────────────
    [Fact]
    public async Task CVerify06_UTCID03_Logout_AlreadyClearedCookies_CompletesWithoutError()
    {
        SetupCookie("");

        var act = async () => await _sut.LogoutAsync();

        await act.Should().NotThrowAsync();
        _tokenService.Verify(t => t.RemoveTokenFromCookie("access_token"), Times.Once);
        _tokenService.Verify(t => t.RemoveTokenFromCookie("refresh_token"), Times.Once);
    }

    // ── UTCID04 ───────────────────────────────────────────────────────────
    [Fact]
    public async Task CVerify06_UTCID04_Logout_WithExpiredJwtCookie_ClearsCookiesAnyway()
    {
        // Expired token value in cookie — no DB record exists for it, so no DB update.
        SetupCookie("EXPIRED_TOKEN_NOT_IN_DB");

        var act = async () => await _sut.LogoutAsync();

        await act.Should().NotThrowAsync();
        _tokenService.Verify(t => t.RemoveTokenFromCookie("access_token"), Times.Once);
        _tokenService.Verify(t => t.RemoveTokenFromCookie("refresh_token"), Times.Once);
    }
}
