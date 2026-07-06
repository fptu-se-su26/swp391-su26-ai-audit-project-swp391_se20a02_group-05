using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.EntityFrameworkCore.Infrastructure;
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
using CVerify.API.Modules.Shared.Exceptions;
using CVerify.API.Modules.Shared.Persistence;
using CVerify.API.Modules.Shared.Security;
using CVerify.API.Modules.Shared.Storage.Interfaces;
using CVerify.API.Modules.Shared.System.Services;

namespace CVerify.API.UnitTests.Auth;

/// <summary>
/// Production-ready unit tests for POST /api/auth/refresh-token (CVerify-125).
/// System Under Test: <see cref="AuthService.RefreshTokenAsync"/> — the business-logic
/// layer that drives the thin <see cref="CVerify.API.Modules.Auth.Controllers.AuthController.RefreshToken"/> endpoint.
///
/// All 15 UTCIDs are mapped 1-to-1 to [Fact] methods using the global naming scheme
/// CVerify125_UTCID##_*.  External dependencies (EF InMemory, Redis/cache, token service,
/// identity repository, HTTP-context) are mocked; the SUT itself is never mocked.
/// </summary>
public sealed class AuthControllerRefreshTokenTests : IDisposable
{
    // Fixed reference point used by FakeTimeProvider for grace-period & replay-attack tests.
    private static readonly DateTimeOffset FakeNow =
        new DateTimeOffset(2024, 6, 1, 12, 0, 0, TimeSpan.Zero);

    // ── mocks & fakes ──────────────────────────────────────────────────────────
    private readonly FakeTimeProvider _timeProvider;
    private readonly ApplicationDbContext _context;

    private readonly Mock<ITokenService> _tokenService          = new();
    private readonly Mock<ICacheService> _cacheService          = new();
    private readonly Mock<IAccountService> _accountService      = new();
    private readonly Mock<IIdentityRepository> _identityRepo    = new();
    private readonly Mock<IHttpContextAccessor> _httpCtxAccessor = new();
    private readonly Mock<ILogger<AuthService>> _logger          = new();
    private readonly Mock<IHttpClientFactory> _httpClientFactory = new();
    private readonly Mock<IIdentityStateResolver> _identityStateResolver = new();
    private readonly Mock<IPasswordPolicyService> _passwordPolicyService = new();
    private readonly Mock<IOtpPolicyService> _otpPolicyService   = new();
    private readonly Mock<IStorageService> _storageService       = new();
    private readonly Mock<IRateLimitPolicyService> _rateLimitPolicyService = new();
    private readonly Mock<IGoogleTokenValidator> _googleTokenValidator   = new();
    private readonly Mock<IUsernameService> _usernameService     = new();
    private readonly Mock<IWorkspaceMembershipService> _workspaceMembershipService = new();

    // HTTP-context helpers (re-configured per test via SetupCookie / SetupUserAgent)
    private readonly Mock<IRequestCookieCollection> _cookies = new();
    private readonly Mock<HttpRequest> _request              = new();

    // The AuthService instance wired with all mocked dependencies.
    private readonly AuthService _sut;

    // ── constructor: shared test bootstrap ────────────────────────────────────
    public AuthControllerRefreshTokenTests()
    {
        _timeProvider = new FakeTimeProvider(FakeNow);

        _context = new ApplicationDbContext(
            new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .ConfigureWarnings(w => w.Ignore(InMemoryEventId.TransactionIgnoredWarning))
                .Options);

        // Both Redis lock levels succeed by default; individual tests override as needed.
        _cacheService
            .Setup(c => c.AcquireLockAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<TimeSpan>()))
            .ReturnsAsync(true);
        _cacheService
            .Setup(c => c.ReleaseLockAsync(It.IsAny<string>(), It.IsAny<string>()))
            .ReturnsAsync(true);

        // Token generation stubs.
        _tokenService.Setup(t => t.GenerateRefreshToken()).Returns("NEW_REFRESH_TOKEN");
        _tokenService.Setup(t => t.GenerateJwtToken(
                It.IsAny<User>(), It.IsAny<IEnumerable<string>>(), It.IsAny<IEnumerable<string>>(),
                It.IsAny<Guid?>(), It.IsAny<string?>(), It.IsAny<Guid?>()))
            .Returns("ACCESS_JWT_TOKEN");
        _tokenService.Setup(t => t.GenerateCompanyJwtToken(
                It.IsAny<OrganizationCredential>(), It.IsAny<IEnumerable<string>>(),
                It.IsAny<IEnumerable<string>>(), It.IsAny<Guid?>()))
            .Returns("COMPANY_JWT_TOKEN");
        _tokenService.Setup(t => t.SetTokenInsideCookie(
            It.IsAny<string>(), It.IsAny<string>(), It.IsAny<DateTime?>()));

        // Identity repo: every user gets the "USER" role, no permissions.
        _identityRepo.Setup(r => r.GetUserRolesAsync(It.IsAny<Guid>()))
            .ReturnsAsync(new[] { "USER" });
        _identityRepo.Setup(r => r.GetUserPermissionsAsync(It.IsAny<Guid>()))
            .ReturnsAsync(Array.Empty<string>());

        // Mock HTTP context plumbing.
        var connection = new Mock<ConnectionInfo>();
        connection.Setup(c => c.RemoteIpAddress).Returns(IPAddress.Loopback);

        _request.Setup(r => r.Cookies).Returns(_cookies.Object);
        _request.Setup(r => r.Headers)
            .Returns(new HeaderDictionary { ["User-Agent"] = "TestBrowser/1.0" });

        var httpCtx = new Mock<HttpContext>();
        httpCtx.Setup(c => c.Request).Returns(_request.Object);
        httpCtx.Setup(c => c.Connection).Returns(connection.Object);
        _httpCtxAccessor.Setup(a => a.HttpContext).Returns(httpCtx.Object);

        _sut = BuildAuthService(_context);
    }

    // ── helpers ───────────────────────────────────────────────────────────────

    private void SetupCookie(string? value) =>
        _cookies.Setup(c => c["refresh_token"]).Returns(value);

    private void SetupUserAgent(string ua) =>
        _request.Setup(r => r.Headers)
            .Returns(new HeaderDictionary { ["User-Agent"] = ua });

    private static User CreateUser(UserStatus status = UserStatus.ACTIVE) => new()
    {
        Id   = Guid.NewGuid(),
        Email    = "testuser@example.com",
        FullName = "Test User",
        Status   = status
    };

    private static RefreshToken CreateUserRefreshToken(
        Guid userId,
        string tokenStr,
        bool rememberMe            = false,
        DateTimeOffset? expiresAt  = null,
        DateTimeOffset? revokedAt  = null,
        Guid? replacedByTokenId    = null,
        Guid? sessionId            = null,
        string? userAgent          = null) => new()
    {
        Id               = Guid.NewGuid(),
        UserId           = userId,
        OrganizationId   = null,
        Token            = tokenStr,
        SessionId        = sessionId ?? Guid.NewGuid(),
        RememberMe       = rememberMe,
        ExpiresAt        = expiresAt ?? DateTimeOffset.UtcNow.AddHours(24),
        RevokedAt        = revokedAt,
        ReplacedByTokenId = replacedByTokenId,
        UserAgent        = userAgent
    };

    private static RefreshToken CreateOrgRefreshToken(
        Guid organizationId,
        string tokenStr,
        bool rememberMe           = false,
        DateTimeOffset? expiresAt = null,
        Guid? sessionId           = null) => new()
    {
        Id             = Guid.NewGuid(),
        UserId         = null,
        OrganizationId = organizationId,
        Token          = tokenStr,
        SessionId      = sessionId ?? Guid.NewGuid(),
        RememberMe     = rememberMe,
        ExpiresAt      = expiresAt ?? DateTimeOffset.UtcNow.AddHours(24)
    };

    private AuthService BuildAuthService(ApplicationDbContext ctx) => new(
        ctx,
        _tokenService.Object,
        _cacheService.Object,
        _accountService.Object,
        _identityRepo.Object,
        _httpCtxAccessor.Object,
        new EnvConfiguration(),
        _logger.Object,
        new AuthMetrics(),
        _timeProvider,
        _httpClientFactory.Object,
        _identityStateResolver.Object,
        _passwordPolicyService.Object,
        _otpPolicyService.Object,
        _storageService.Object,
        _rateLimitPolicyService.Object,
        _googleTokenValidator.Object,
        _usernameService.Object,
        _workspaceMembershipService.Object);

    // ── UTCID01 ───────────────────────────────────────────────────────────────
    /// <summary>
    /// Valid user refresh token with RememberMe=false.
    /// Expected: HTTP 200 · new access + 24-hour refresh cookie · old token revoked in DB.
    /// Type: Normal (N)
    /// </summary>
    [Fact]
    public async Task CVerify125_UTCID01_RefreshToken_ValidUserTokenRememberMeFalse_Returns200With24HourCookie()
    {
        // Arrange
        var user      = CreateUser();
        const string tokenStr = "UTCID01_USER_TOKEN";
        var token = CreateUserRefreshToken(user.Id, tokenStr, rememberMe: false);

        await _context.Users.AddAsync(user);
        await _context.RefreshTokens.AddAsync(token);
        await _context.SaveChangesAsync();
        SetupCookie(tokenStr);

        // Act
        var result = await _sut.RefreshTokenAsync();

        // Assert – response
        result.Should().NotBeNull();
        result!.Id.Should().Be(user.Id);
        result.Email.Should().Be(user.Email);
        result.Status.Should().Be(UserStatus.ACTIVE.ToString());

        // Assert – token rotation: a new refresh token was generated
        _tokenService.Verify(t => t.GenerateRefreshToken(), Times.Once);

        // Assert – 24-hour cookie (RememberMe=false)
        _tokenService.Verify(t =>
            t.SetTokenInsideCookie(
                "refresh_token",
                "NEW_REFRESH_TOKEN",
                It.Is<DateTime?>(d => d.HasValue && d.Value > DateTime.UtcNow.AddHours(23) && d.Value < DateTime.UtcNow.AddHours(25))),
            Times.Once);

        // Assert – 15-minute access cookie
        _tokenService.Verify(t =>
            t.SetTokenInsideCookie("access_token", "ACCESS_JWT_TOKEN", It.IsAny<DateTime?>()),
            Times.Once);

        // Assert – old token revoked in DB
        var dbToken = await _context.RefreshTokens
            .AsNoTracking()
            .FirstAsync(t => t.Token == tokenStr);
        dbToken.RevokedAt.Should().NotBeNull("the old token must be revoked after rotation");
        dbToken.ReplacedByToken.Should().Be("NEW_REFRESH_TOKEN");

        // Assert – audit log written
        var auditEntry = await _context.AuditLogs.FirstOrDefaultAsync(a => a.EventType == "TOKEN_ROTATED");
        auditEntry.Should().NotBeNull("TOKEN_ROTATED audit event must be persisted");
        auditEntry!.Description.Should().Contain(token.SessionId.ToString());
    }

    // ── UTCID02 ───────────────────────────────────────────────────────────────
    /// <summary>
    /// Valid user refresh token with RememberMe=true.
    /// Expected: HTTP 200 · new 7-day refresh cookie.
    /// Type: Boundary (B)
    /// </summary>
    [Fact]
    public async Task CVerify125_UTCID02_RefreshToken_ValidUserTokenRememberMeTrue_Returns200With7DayCookie()
    {
        // Arrange
        var user      = CreateUser();
        const string tokenStr = "UTCID02_REMEMBER_ME_TOKEN";
        var token = CreateUserRefreshToken(user.Id, tokenStr, rememberMe: true);

        await _context.Users.AddAsync(user);
        await _context.RefreshTokens.AddAsync(token);
        await _context.SaveChangesAsync();
        SetupCookie(tokenStr);

        // Act
        var result = await _sut.RefreshTokenAsync();

        // Assert
        result.Should().NotBeNull();

        // Assert – 7-day cookie for RememberMe=true
        _tokenService.Verify(t =>
            t.SetTokenInsideCookie(
                "refresh_token",
                "NEW_REFRESH_TOKEN",
                It.Is<DateTime?>(d => d.HasValue && d.Value > DateTime.UtcNow.AddDays(6) && d.Value < DateTime.UtcNow.AddDays(8))),
            Times.Once);

        var dbToken = await _context.RefreshTokens
            .AsNoTracking()
            .FirstAsync(t => t.Token == tokenStr);
        dbToken.RevokedAt.Should().NotBeNull();
    }

    // ── UTCID03 ───────────────────────────────────────────────────────────────
    /// <summary>
    /// Valid company (OrganizationId-based) refresh token, organization is ACTIVE.
    /// Expected: HTTP 200 · AuthResponse with Roles=["BUSINESS"] · company JWT issued.
    /// Type: Normal (N)
    /// </summary>
    [Fact]
    public async Task CVerify125_UTCID03_RefreshToken_ValidCompanyToken_Returns200WithBusinessRole()
    {
        // Arrange
        var org = new Organization
        {
            Id       = Guid.NewGuid(),
            Name     = "Acme Corp",
            TaxCode  = "TAX123",
            Email    = "acme@corp.test",
            Username = "acme",
            Status   = "active",
            DeletedAt = null
        };
        var credential = new OrganizationCredential
        {
            OrganizationId = org.Id,
            Username       = "acme",
            PasswordHash   = "hashed",
            Organization   = org
        };
        const string tokenStr = "UTCID03_ORG_TOKEN";
        var token = CreateOrgRefreshToken(org.Id, tokenStr);

        await _context.Organizations.AddAsync(org);
        await _context.OrganizationCredentials.AddAsync(credential);
        await _context.RefreshTokens.AddAsync(token);
        await _context.SaveChangesAsync();
        SetupCookie(tokenStr);

        // Act
        var result = await _sut.RefreshTokenAsync();

        // Assert
        result.Should().NotBeNull();
        result!.Id.Should().Be(org.Id);
        result.Email.Should().Be(org.Email);
        result.Roles.Should().Contain("BUSINESS");
        result.Status.Should().Be("ACTIVE");

        // Company JWT generator must be used (not user JWT generator)
        _tokenService.Verify(t =>
            t.GenerateCompanyJwtToken(
                It.IsAny<OrganizationCredential>(),
                It.IsAny<IEnumerable<string>>(),
                It.IsAny<IEnumerable<string>>(),
                It.IsAny<Guid?>()),
            Times.Once);
        _tokenService.Verify(t =>
            t.GenerateJwtToken(
                It.IsAny<User>(), It.IsAny<IEnumerable<string>>(), It.IsAny<IEnumerable<string>>(),
                It.IsAny<Guid?>(), It.IsAny<string?>(), It.IsAny<Guid?>()),
            Times.Never);

        // COMPANY_TOKEN_ROTATED audit log
        var auditEntry = await _context.AuditLogs.FirstOrDefaultAsync(a => a.EventType == "COMPANY_TOKEN_ROTATED");
        auditEntry.Should().NotBeNull();
    }

    // ── UTCID04 ───────────────────────────────────────────────────────────────
    /// <summary>
    /// No refresh_token cookie present in the request.
    /// Expected: service returns null → controller throws AuthenticationException → HTTP 401.
    /// Type: Abnormal (A)
    /// </summary>
    [Fact]
    public async Task CVerify125_UTCID04_RefreshToken_NoCookiePresent_ReturnsNull()
    {
        // Arrange – cookie collection returns null for the key
        SetupCookie(null);

        // Act
        var result = await _sut.RefreshTokenAsync();

        // Assert
        result.Should().BeNull("absent cookie must yield null so the controller returns 401");

        // Lock must never be acquired when there is no cookie.
        _cacheService.Verify(c =>
            c.AcquireLockAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<TimeSpan>()),
            Times.Never);
    }

    // ── UTCID05 ───────────────────────────────────────────────────────────────
    /// <summary>
    /// Cookie is present but its value is an empty string.
    /// Expected: service returns null → HTTP 401.
    /// Type: Abnormal (A)
    /// </summary>
    [Fact]
    public async Task CVerify125_UTCID05_RefreshToken_EmptyCookieValue_ReturnsNull()
    {
        // Arrange
        SetupCookie(string.Empty);

        // Act
        var result = await _sut.RefreshTokenAsync();

        // Assert
        result.Should().BeNull();
        _cacheService.Verify(c =>
            c.AcquireLockAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<TimeSpan>()),
            Times.Never);
    }

    // ── UTCID06 ───────────────────────────────────────────────────────────────
    /// <summary>
    /// Cookie contains a random/fabricated base-64 string with no matching DB record.
    /// Expected: service returns null → HTTP 401.
    /// Type: Abnormal (A)
    /// </summary>
    [Fact]
    public async Task CVerify125_UTCID06_RefreshToken_FabricatedToken_NoDbRecord_ReturnsNull()
    {
        // Arrange – no seeded tokens; the DB is empty
        SetupCookie("aFakedBase64StringThatHasNoMatchInDB==");

        // Act
        var result = await _sut.RefreshTokenAsync();

        // Assert
        result.Should().BeNull("a fabricated token must never authenticate");
    }

    // ── UTCID07 ───────────────────────────────────────────────────────────────
    /// <summary>
    /// Token exists in DB but IsExpired=true (ExpiresAt is in the past).
    /// Expected: service returns null → HTTP 401.
    /// Type: Abnormal (A)
    /// </summary>
    [Fact]
    public async Task CVerify125_UTCID07_RefreshToken_ExpiredToken_ReturnsNull()
    {
        // Arrange
        var user      = CreateUser();
        const string tokenStr = "UTCID07_EXPIRED_TOKEN";
        // IsExpired uses DateTimeOffset.UtcNow directly, so set ExpiresAt to a real past date.
        var token = CreateUserRefreshToken(user.Id, tokenStr,
            expiresAt: DateTimeOffset.UtcNow.AddHours(-2));

        await _context.Users.AddAsync(user);
        await _context.RefreshTokens.AddAsync(token);
        await _context.SaveChangesAsync();
        SetupCookie(tokenStr);

        // Act
        var result = await _sut.RefreshTokenAsync();

        // Assert
        result.Should().BeNull("an expired token must be rejected with null so the controller returns 401");

        // Token must NOT be further modified (already expired; no rotation attempted)
        _tokenService.Verify(t => t.GenerateRefreshToken(), Times.Never);
    }

    // ── UTCID08 ───────────────────────────────────────────────────────────────
    /// <summary>
    /// Replay-attack detection: IsRevoked=true AND RevokedAt &gt; 10 seconds ago.
    /// Expected:
    ///   • RevokeSessionChainAsync revokes all active tokens in the compromised session.
    ///   • SECURITY ALERT [Warning] logged with EVENT=TOKEN_REUSE_DETECTED.
    ///   • TOKEN_THEFT_DETECTED audit log persisted.
    ///   • AuthException("Token reuse detected.") thrown → HTTP 400/401.
    /// Type: Abnormal (A)
    /// </summary>
    [Fact]
    public async Task CVerify125_UTCID08_RefreshToken_RevokedTokenOlderThan10Seconds_RevokeSessionChainAndThrowSecurityAlert()
    {
        // Arrange
        var sessionId = Guid.NewGuid();
        var user      = CreateUser();

        // Revoked 15 seconds before FakeNow → outside the 10-second grace window → Reused
        var revokedAt = FakeNow.AddSeconds(-15);

        const string attackTokenStr = "UTCID08_REUSED_TOKEN";
        var reuseToken = CreateUserRefreshToken(user.Id, attackTokenStr,
            sessionId: sessionId,
            revokedAt: revokedAt);

        // Another active token belonging to the same session (should be chain-revoked)
        var siblingToken = CreateUserRefreshToken(user.Id, "UTCID08_SIBLING_TOKEN",
            sessionId: sessionId);

        await _context.Users.AddAsync(user);
        await _context.RefreshTokens.AddRangeAsync(reuseToken, siblingToken);
        await _context.SaveChangesAsync();
        SetupCookie(attackTokenStr);

        // Act & Assert – exception thrown
        var act = () => _sut.RefreshTokenAsync();
        await act.Should()
            .ThrowAsync<AuthException>()
            .WithMessage("*Token reuse detected*");

        // Assert – entire session chain revoked
        var stillActive = await _context.RefreshTokens
            .AsNoTracking()
            .Where(t => t.SessionId == sessionId && t.RevokedAt == null)
            .ToListAsync();
        stillActive.Should().BeEmpty("RevokeSessionChainAsync must revoke every token in the compromised session");

        // Assert – structured SECURITY ALERT warning logged
        _logger.Verify(l => l.Log(
                LogLevel.Warning,
                It.IsAny<EventId>(),
                It.Is<It.IsAnyType>((v, _) => v.ToString()!.Contains("TOKEN_REUSE_DETECTED")),
                It.IsAny<Exception?>(),
                It.IsAny<Func<It.IsAnyType, Exception?, string>>()),
            Times.AtLeastOnce);

        // Assert – TOKEN_THEFT_DETECTED audit event written
        var theftLog = await _context.AuditLogs
            .AsNoTracking()
            .FirstOrDefaultAsync(a => a.EventType == "TOKEN_THEFT_DETECTED");
        theftLog.Should().NotBeNull("TOKEN_THEFT_DETECTED audit must be persisted on breach detection");
    }

    // ── UTCID09 ───────────────────────────────────────────────────────────────
    /// <summary>
    /// Grace-period safe race: IsRevoked=true AND RevokedAt &lt;= 10 seconds ago.
    /// Expected:
    ///   • No new refresh token generated; the previously-issued replacement is re-sent.
    ///   • Information log: "Safe concurrent refresh race handled."
    ///   • HTTP 200 AuthResponse.
    /// Type: Boundary (B)
    /// </summary>
    [Fact]
    public async Task CVerify125_UTCID09_RefreshToken_RevokedWithinGracePeriod_ReturnsExistingReplacementTokenWithoutNewRotation()
    {
        // Arrange
        var sessionId = Guid.NewGuid();
        var user      = CreateUser();

        // Revoked only 5 seconds before FakeNow → inside the 10-second grace window → WithinGracePeriod
        var revokedAt = FakeNow.AddSeconds(-5);

        const string replacementTokenStr = "UTCID09_REPLACEMENT_TOKEN";
        var replacementToken = CreateUserRefreshToken(user.Id, replacementTokenStr,
            sessionId: sessionId,
            rememberMe: false);

        const string originalTokenStr = "UTCID09_ORIGINAL_TOKEN";
        var originalToken = CreateUserRefreshToken(user.Id, originalTokenStr,
            sessionId: sessionId,
            revokedAt: revokedAt,
            replacedByTokenId: replacementToken.Id);

        await _context.Users.AddAsync(user);
        // Add replacement first so the FK is satisfied when originalToken references its Id
        await _context.RefreshTokens.AddAsync(replacementToken);
        await _context.RefreshTokens.AddAsync(originalToken);
        await _context.SaveChangesAsync();
        SetupCookie(originalTokenStr);

        // Act
        var result = await _sut.RefreshTokenAsync();

        // Assert – valid response returned (grace-period path)
        result.Should().NotBeNull();
        result!.Id.Should().Be(user.Id);

        // Assert – NO new refresh token generated; existing replacement re-issued
        _tokenService.Verify(t => t.GenerateRefreshToken(), Times.Never,
            "grace-period path must reuse the existing replacement, never create a second rotation");

        // Assert – replacement token's value pushed to cookie
        _tokenService.Verify(t =>
            t.SetTokenInsideCookie("refresh_token", replacementTokenStr, It.IsAny<DateTime?>()),
            Times.Once);

        // Assert – informational log emitted
        _logger.Verify(l => l.Log(
                LogLevel.Information,
                It.IsAny<EventId>(),
                It.Is<It.IsAnyType>((v, _) => v.ToString()!.Contains("Safe concurrent refresh race handled")),
                It.IsAny<Exception?>(),
                It.IsAny<Func<It.IsAnyType, Exception?, string>>()),
            Times.Once);
    }

    // ── UTCID10 ───────────────────────────────────────────────────────────────
    /// <summary>
    /// Valid token but the requesting User-Agent differs from the one stored in the DB.
    /// Expected: HTTP 200 – rotation succeeds; SECURITY WARNING logged but NOT blocked.
    /// Type: Boundary (B)
    /// </summary>
    [Fact]
    public async Task CVerify125_UTCID10_RefreshToken_UserAgentMismatch_RotationSucceedsWithSecurityWarning()
    {
        // Arrange
        var user      = CreateUser();
        const string tokenStr     = "UTCID10_UA_MISMATCH_TOKEN";
        const string originalAgent = "OriginalBrowser/1.0";

        var token = CreateUserRefreshToken(user.Id, tokenStr, userAgent: originalAgent);

        await _context.Users.AddAsync(user);
        await _context.RefreshTokens.AddAsync(token);
        await _context.SaveChangesAsync();
        SetupCookie(tokenStr);

        // Simulate the incoming request using a different browser
        SetupUserAgent("DifferentBrowser/2.0");

        // Act
        var result = await _sut.RefreshTokenAsync();

        // Assert – rotation is NOT blocked by UA mismatch
        result.Should().NotBeNull("User-Agent change is a warning, not a hard block");
        _tokenService.Verify(t => t.GenerateRefreshToken(), Times.Once);

        // Assert – security warning logged
        _logger.Verify(l => l.Log(
                LogLevel.Warning,
                It.IsAny<EventId>(),
                It.Is<It.IsAnyType>((v, _) =>
                    v.ToString()!.Contains("SECURITY WARNING") &&
                    v.ToString()!.Contains("User-Agent changed")),
                It.IsAny<Exception?>(),
                It.IsAny<Func<It.IsAnyType, Exception?, string>>()),
            Times.Once);
    }

    // ── UTCID11 ───────────────────────────────────────────────────────────────
    /// <summary>
    /// Concurrent-request simulation: per-token lock acquired but the per-user (session)
    /// lock fails after 5 retries — models the second concurrent client hitting the
    /// user-level lock held by the first.
    /// Expected: AuthException("Concurrent session operations detected.") → HTTP 400.
    /// Type: Boundary (B)
    /// </summary>
    [Fact]
    public async Task CVerify125_UTCID11_RefreshToken_UserLevelLockNotAcquiredAfterRetries_ThrowsConcurrentSessionOperationsException()
    {
        // Arrange
        var user      = CreateUser();
        const string tokenStr = "UTCID11_CONCURRENT_TOKEN";
        var token = CreateUserRefreshToken(user.Id, tokenStr);

        await _context.Users.AddAsync(user);
        await _context.RefreshTokens.AddAsync(token);
        await _context.SaveChangesAsync();
        SetupCookie(tokenStr);

        // Per-token lock succeeds; per-user (session) lock always fails.
        _cacheService
            .Setup(c => c.AcquireLockAsync(
                It.Is<string>(k => k.StartsWith("lock:token:rotate:")),
                It.IsAny<string>(), It.IsAny<TimeSpan>()))
            .ReturnsAsync(true);
        _cacheService
            .Setup(c => c.AcquireLockAsync(
                It.Is<string>(k => k.StartsWith("lock:user:sessions:")),
                It.IsAny<string>(), It.IsAny<TimeSpan>()))
            .ReturnsAsync(false);

        // Act & Assert
        var act = () => _sut.RefreshTokenAsync();
        await act.Should()
            .ThrowAsync<AuthException>()
            .WithMessage("*Concurrent session operations detected*");
    }

    // ── UTCID12 ───────────────────────────────────────────────────────────────
    /// <summary>
    /// The per-token Redis lock is already held externally; all 5 acquisition retries fail.
    /// Expected:
    ///   • Warning logged: "Token rotation request rejected due to lock contention."
    ///   • AuthException("Concurrent token rotation detected.") thrown → HTTP 400.
    /// Type: Abnormal (A)
    /// </summary>
    [Fact]
    public async Task CVerify125_UTCID12_RefreshToken_PerTokenLockHeldExternally_ThrowsLockContentionException()
    {
        // Arrange
        const string tokenStr = "UTCID12_LOCKED_TOKEN";
        SetupCookie(tokenStr);

        // Per-token lock always fails (externally held).
        _cacheService
            .Setup(c => c.AcquireLockAsync(
                It.Is<string>(k => k.StartsWith("lock:token:rotate:")),
                It.IsAny<string>(), It.IsAny<TimeSpan>()))
            .ReturnsAsync(false);

        // Act & Assert
        var act = () => _sut.RefreshTokenAsync();
        await act.Should()
            .ThrowAsync<AuthException>()
            .WithMessage("*Concurrent token rotation detected*");

        // Assert – contention warning logged
        _logger.Verify(l => l.Log(
                LogLevel.Warning,
                It.IsAny<EventId>(),
                It.Is<It.IsAnyType>((v, _) =>
                    v.ToString()!.Contains("Token rotation request rejected due to lock contention")),
                It.IsAny<Exception?>(),
                It.IsAny<Func<It.IsAnyType, Exception?, string>>()),
            Times.Once);

        // Assert – 5 acquisition attempts made (1 attempt × 5 retries)
        _cacheService.Verify(c =>
            c.AcquireLockAsync(
                It.Is<string>(k => k.StartsWith("lock:token:rotate:")),
                It.IsAny<string>(), It.IsAny<TimeSpan>()),
            Times.Exactly(5));
    }

    // ── UTCID13 ───────────────────────────────────────────────────────────────
    /// <summary>
    /// Valid company refresh token but the underlying Organization has been soft-deleted
    /// (Organization.DeletedAt != null).
    /// Expected: service returns null → HTTP 401.
    /// Type: Abnormal (A)
    /// </summary>
    [Fact]
    public async Task CVerify125_UTCID13_RefreshToken_SoftDeletedOrganization_ReturnsNull()
    {
        // Arrange
        var org = new Organization
        {
            Id        = Guid.NewGuid(),
            Name      = "Ghost Corp",
            TaxCode   = "TAX999",
            Email     = "ghost@corp.test",
            Username  = "ghostcorp",
            Status    = "archived",
            DeletedAt = DateTimeOffset.UtcNow.AddDays(-7) // soft-deleted
        };
        var credential = new OrganizationCredential
        {
            OrganizationId = org.Id,
            Username       = "ghostcorp",
            PasswordHash   = "hash",
            Organization   = org
            // DeletedAt is null → credential itself is NOT deleted; only the org is
        };
        const string tokenStr = "UTCID13_DELETED_ORG_TOKEN";
        var token = CreateOrgRefreshToken(org.Id, tokenStr);

        await _context.Organizations.AddAsync(org);
        await _context.OrganizationCredentials.AddAsync(credential);
        await _context.RefreshTokens.AddAsync(token);
        await _context.SaveChangesAsync();
        SetupCookie(tokenStr);

        // Act
        var result = await _sut.RefreshTokenAsync();

        // Assert
        result.Should().BeNull("tokens for soft-deleted organisations must not be rotated");
        _tokenService.Verify(t => t.GenerateRefreshToken(), Times.Never);
    }

    // ── UTCID14 ───────────────────────────────────────────────────────────────
    /// <summary>
    /// Database SaveChangesAsync throws DbUpdateException during RotateRefreshTokenAsync.
    /// Expected:
    ///   • rotationTx.RollbackAsync() invoked (no-op for InMemory, verified by absence of new token).
    ///   • DbUpdateException propagates to caller → HTTP 500.
    ///   • Old token NOT permanently revoked; DB state unchanged.
    /// Type: Abnormal (A)
    /// </summary>
    [Fact]
    public async Task CVerify125_UTCID14_RefreshToken_DbUpdateExceptionDuringRotation_PropagatesExceptionAndPreservesAtomicity()
    {
        // Arrange – seed data via a healthy context sharing the same InMemory store
        var dbName = Guid.NewGuid().ToString();
        var dbOptions = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseInMemoryDatabase(dbName)
            .ConfigureWarnings(w => w.Ignore(InMemoryEventId.TransactionIgnoredWarning))
            .Options;

        await using var seedCtx = new ApplicationDbContext(dbOptions);
        var user      = CreateUser();
        const string tokenStr = "UTCID14_DB_FAULT_TOKEN";
        var token = CreateUserRefreshToken(user.Id, tokenStr, rememberMe: false);

        await seedCtx.Users.AddAsync(user);
        await seedCtx.RefreshTokens.AddAsync(token);
        await seedCtx.SaveChangesAsync();

        // Activate the fault AFTER seeding so the setup succeeds.
        await using var faultingCtx = new FaultingDbContext(dbOptions) { ShouldFault = true };
        var faultingSut = BuildAuthService(faultingCtx);
        SetupCookie(tokenStr);

        // Act & Assert – DbUpdateException propagates
        var act = () => faultingSut.RefreshTokenAsync();
        await act.Should().ThrowAsync<DbUpdateException>();

        // Assert – DB atomicity: old token NOT revoked in the InMemory store
        var dbToken = await seedCtx.RefreshTokens
            .AsNoTracking()
            .FirstAsync(t => t.Token == tokenStr);
        dbToken.RevokedAt.Should().BeNull(
            "the transaction rollback must leave the old token unmodified (atomicity preserved)");

        // Assert – no new refresh token was persisted
        var newTokenCount = await seedCtx.RefreshTokens.CountAsync();
        newTokenCount.Should().Be(1, "no new token should be persisted when the save fails");
    }

    // ── UTCID15 ───────────────────────────────────────────────────────────────
    /// <summary>
    /// Token belongs to a user whose status has been changed to BANNED post-issuance.
    /// Expected: HTTP 200 – rotation completes; AuthResponse.Status = "BANNED".
    /// This is a documented design gap: BANNED status is not enforced at the refresh boundary.
    /// Type: Boundary (B)
    /// </summary>
    [Fact]
    public async Task CVerify125_UTCID15_RefreshToken_BannedUser_RotationSucceedsWithBannedStatusInResponse()
    {
        // Arrange
        var bannedUser  = CreateUser(UserStatus.BANNED);
        const string tokenStr = "UTCID15_BANNED_TOKEN";
        var token = CreateUserRefreshToken(bannedUser.Id, tokenStr, rememberMe: false);

        await _context.Users.AddAsync(bannedUser);
        await _context.RefreshTokens.AddAsync(token);
        await _context.SaveChangesAsync();
        SetupCookie(tokenStr);

        // Act
        var result = await _sut.RefreshTokenAsync();

        // Assert – DESIGN GAP: refresh is not blocked for BANNED users
        result.Should().NotBeNull(
            "current implementation does not enforce BANNED status at the token-refresh boundary (known design gap)");
        result!.Status.Should().Be(UserStatus.BANNED.ToString(),
            "AuthResponse must reflect the actual user status even when BANNED");

        // Assert – token rotation still occurred
        _tokenService.Verify(t => t.GenerateRefreshToken(), Times.Once);
        var dbToken = await _context.RefreshTokens
            .AsNoTracking()
            .FirstAsync(t => t.Token == tokenStr);
        dbToken.RevokedAt.Should().NotBeNull("the old token is revoked even for a BANNED user");
    }

    // ── IDisposable ───────────────────────────────────────────────────────────
    public void Dispose() => _context.Dispose();

    // ── FaultingDbContext ─────────────────────────────────────────────────────
    /// <summary>
    /// Subclass of <see cref="ApplicationDbContext"/> that throws
    /// <see cref="DbUpdateException"/> on every <c>SaveChangesAsync</c> call when
    /// <see cref="ShouldFault"/> is set to <c>true</c>.  Used exclusively for UTCID14.
    /// </summary>
    private sealed class FaultingDbContext : ApplicationDbContext
    {
        public bool ShouldFault { get; set; }

        public FaultingDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options) { }

        public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            if (ShouldFault)
                throw new DbUpdateException(
                    "Simulated database failure",
                    new InvalidOperationException("Connection timeout"));
            return base.SaveChangesAsync(cancellationToken);
        }

        public override Task<int> SaveChangesAsync(
            bool acceptAllChangesOnSuccess,
            CancellationToken cancellationToken = default)
        {
            if (ShouldFault)
                throw new DbUpdateException(
                    "Simulated database failure",
                    new InvalidOperationException("Connection timeout"));
            return base.SaveChangesAsync(acceptAllChangesOnSuccess, cancellationToken);
        }
    }
}
