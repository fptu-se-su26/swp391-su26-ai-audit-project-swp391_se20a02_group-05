using System;
using System.Collections.Generic;
using System.Net;
using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;
using FluentAssertions;
using Google.Apis.Auth;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Time.Testing;
using Moq;
using Xunit;
using CVerify.API.Modules.Auth.DTOs;
using CVerify.API.Modules.Auth.Entities;
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
/// Unit tests for AuthService.LinkGoogleAccountAsync — CVerify-12 (6 UTCIDs).
/// </summary>
public sealed class CVerify12_LinkGoogleAccountTests : IDisposable
{
    private static readonly DateTimeOffset FakeNow = new(2024, 6, 1, 12, 0, 0, TimeSpan.Zero);
    private const string ValidGoogleToken  = "VALID_GOOGLE_TOKEN";
    private const string InvalidToken     = "INVALID_TOKEN";
    private const string GoogleSubject    = "google_uid_abc123";
    private const string GoogleEmail      = "googleuser@gmail.com";

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

    private Guid _currentUserId;
    private readonly AuthService _sut;

    public CVerify12_LinkGoogleAccountTests()
    {
        _timeProvider = new FakeTimeProvider(FakeNow);
        _currentUserId = Guid.NewGuid();

        _context = new ApplicationDbContext(
            new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .ConfigureWarnings(w => w.Ignore(InMemoryEventId.TransactionIgnoredWarning))
                .Options);

        SetupUserContext(_currentUserId);

        _cacheService.Setup(c => c.RemoveAsync(It.IsAny<string>())).Returns(Task.CompletedTask);
        _cacheService.Setup(c => c.SetAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<TimeSpan?>())).Returns(Task.CompletedTask);
        _cacheService.Setup(c => c.AddToSetAsync(It.IsAny<string>(), It.IsAny<string>())).Returns(Task.CompletedTask);

        // Default: valid Google token → valid payload
        _googleTokenValidator
            .Setup(v => v.ValidateAsync(ValidGoogleToken, It.IsAny<GoogleJsonWebSignature.ValidationSettings>()))
            .ReturnsAsync(new GoogleJsonWebSignature.Payload
            {
                Subject       = GoogleSubject,
                Email         = GoogleEmail,
                EmailVerified = true,
            });

        // Invalid token → throws
        _googleTokenValidator
            .Setup(v => v.ValidateAsync(InvalidToken, It.IsAny<GoogleJsonWebSignature.ValidationSettings>()))
            .ThrowsAsync(new InvalidJwtException("Token validation failed."));

        _workspaceMembershipService
            .Setup(w => w.BootstrapInitialAdminAsync(It.IsAny<string>(), It.IsAny<bool>(), It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);
        _workspaceMembershipService
            .Setup(w => w.DiscoverPendingInvitationsAsync(It.IsAny<Guid>()))
            .Returns(Task.CompletedTask);

        _identityStateResolver
            .Setup(r => r.InvalidateCacheAsync(It.IsAny<string>()))
            .Returns(Task.CompletedTask);

        _sut = new AuthService(
            _context, _tokenService.Object, _cacheService.Object, _accountService.Object,
            _identityRepo.Object, _httpCtxAccessor.Object, new EnvConfiguration(), _logger.Object,
            new AuthMetrics(), _timeProvider, _httpClientFactory.Object, _identityStateResolver.Object,
            _passwordPolicyService.Object, _otpPolicyService.Object, _storageService.Object,
            _rateLimitPolicyService.Object, _googleTokenValidator.Object, _usernameService.Object,
            _workspaceMembershipService.Object);
    }

    public void Dispose() => _context.Dispose();

    private void SetupUserContext(Guid userId)
    {
        var claims = new[] { new Claim(ClaimTypes.NameIdentifier, userId.ToString()) };
        var identity = new ClaimsIdentity(claims, "Test");
        var principal = new ClaimsPrincipal(identity);

        var connection = new Mock<ConnectionInfo>();
        connection.Setup(c => c.RemoteIpAddress).Returns(IPAddress.Loopback);
        _request.Setup(r => r.Cookies).Returns(_cookies.Object);
        _request.Setup(r => r.Headers).Returns(new HeaderDictionary { ["User-Agent"] = "TestBrowser/1.0" });

        var httpCtx = new Mock<HttpContext>();
        httpCtx.Setup(c => c.Request).Returns(_request.Object);
        httpCtx.Setup(c => c.Connection).Returns(connection.Object);
        httpCtx.Setup(c => c.User).Returns(principal);
        _httpCtxAccessor.Setup(a => a.HttpContext).Returns(httpCtx.Object);
    }

    private async Task<User> SeedActiveUserAsync()
    {
        var user = new User
        {
            Id              = _currentUserId,
            Email           = "user@example.com",
            FullName        = "Test User",
            Username        = "testuser",
            Status          = UserStatus.ACTIVE,
            EmailVerifiedAt = DateTime.UtcNow,
        };
        _context.Users.Add(user);
        await _context.SaveChangesAsync();
        return user;
    }

    // ── UTCID01 ───────────────────────────────────────────────────────────
    [Fact]
    public async Task CVerify12_UTCID01_LinkGoogle_ValidTokenNewAccount_ReturnsTrue()
    {
        await SeedActiveUserAsync();

        var result = await _sut.LinkGoogleAccountAsync(new LinkGoogleRequest(ValidGoogleToken));

        result.Should().BeTrue();
        var provider = await _context.AuthProviders.FirstOrDefaultAsync(p => p.ProviderKey == GoogleSubject);
        provider.Should().NotBeNull();
        provider!.ProviderName.Should().Be("google");
    }

    // ── UTCID02 ───────────────────────────────────────────────────────────
    [Fact]
    public async Task CVerify12_UTCID02_LinkGoogle_SubjectAlreadyLinkedToDifferentUser_ThrowsAuthExceptionAccountConflict()
    {
        await SeedActiveUserAsync();

        // Another user already owns this Google sub
        var otherUserId = Guid.NewGuid();
        _context.Users.Add(new User { Id = otherUserId, Email = "other@example.com", FullName = "Other", Username = "other", Status = UserStatus.ACTIVE, EmailVerifiedAt = DateTime.UtcNow });
        _context.AuthProviders.Add(new AuthProvider
        {
            Id           = Guid.NewGuid(),
            UserId       = otherUserId,
            ProviderName = "google",
            ProviderKey  = GoogleSubject,
            ScopeValidationStatus = ProviderScopeStatus.Valid,
        });
        await _context.SaveChangesAsync();

        var act = async () => await _sut.LinkGoogleAccountAsync(new LinkGoogleRequest(ValidGoogleToken));

        await act.Should().ThrowAsync<AuthException>()
            .Where(e => e.ErrorCode == AuthErrorCodes.AccountConflict);
    }

    // ── UTCID03 ───────────────────────────────────────────────────────────
    // Google account already linked to current user → service reactivates/upserts and returns true.
    [Fact]
    public async Task CVerify12_UTCID03_LinkGoogle_AlreadyLinkedToCurrentUser_ReturnsTrue()
    {
        await SeedActiveUserAsync();
        _context.AuthProviders.Add(new AuthProvider
        {
            Id           = Guid.NewGuid(),
            UserId       = _currentUserId,
            ProviderName = "google",
            ProviderKey  = GoogleSubject,
            ScopeValidationStatus = ProviderScopeStatus.Valid,
        });
        await _context.SaveChangesAsync();

        var result = await _sut.LinkGoogleAccountAsync(new LinkGoogleRequest(ValidGoogleToken));

        result.Should().BeTrue("service upserts — no duplicate error at service layer");
    }

    // ── UTCID04 ───────────────────────────────────────────────────────────
    [Fact]
    public async Task CVerify12_UTCID04_LinkGoogle_InvalidToken_ThrowsAuthExceptionInvalidCredentials()
    {
        await SeedActiveUserAsync();

        var act = async () => await _sut.LinkGoogleAccountAsync(new LinkGoogleRequest(InvalidToken));

        await act.Should().ThrowAsync<AuthException>()
            .Where(e => e.ErrorCode == AuthErrorCodes.InvalidCredentials);
    }

    // ── UTCID05 ───────────────────────────────────────────────────────────
    // No JWT → controller [Authorize] returns 401.
    // At service level: missing HttpContext user claim → UnauthorizedAccessException.
    [Fact]
    public async Task CVerify12_UTCID05_LinkGoogle_NoJwt_ServiceThrowsUnauthorizedAccessException()
    {
        // Override context accessor to simulate missing auth
        var emptyCtx = new Mock<HttpContext>();
        emptyCtx.Setup(c => c.User).Returns(new ClaimsPrincipal(new ClaimsIdentity()));
        emptyCtx.Setup(c => c.Request).Returns(_request.Object);
        emptyCtx.Setup(c => c.Connection).Returns(new Mock<ConnectionInfo>().Object);
        _httpCtxAccessor.Setup(a => a.HttpContext).Returns(emptyCtx.Object);

        var act = async () => await _sut.LinkGoogleAccountAsync(new LinkGoogleRequest(ValidGoogleToken));

        await act.Should().ThrowAsync<UnauthorizedAccessException>();
    }

    // ── UTCID06 ───────────────────────────────────────────────────────────
    // User has max providers; service does not enforce max — it upserts.
    // Max enforcement would be at the controller level.
    [Fact]
    public async Task CVerify12_UTCID06_LinkGoogle_UserAlreadyHasMaxLinkedProviders_ServiceStillReturnsTrue()
    {
        await SeedActiveUserAsync();
        // Seed 3 different Google accounts for other subs (not the one being linked)
        for (int i = 1; i <= 3; i++)
        {
            _context.AuthProviders.Add(new AuthProvider
            {
                Id           = Guid.NewGuid(),
                UserId       = _currentUserId,
                ProviderName = "google",
                ProviderKey  = $"other_sub_{i}",
                ScopeValidationStatus = ProviderScopeStatus.Valid,
            });
        }
        await _context.SaveChangesAsync();

        // Linking a new sub
        var result = await _sut.LinkGoogleAccountAsync(new LinkGoogleRequest(ValidGoogleToken));

        result.Should().BeTrue("service does not enforce max-provider limit — controller responsibility");
    }
}
