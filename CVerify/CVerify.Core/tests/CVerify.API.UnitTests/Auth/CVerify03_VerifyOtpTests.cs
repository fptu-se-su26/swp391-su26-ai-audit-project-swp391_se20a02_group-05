using System;
using System.Collections.Generic;
using System.Net;
using System.Security.Cryptography;
using System.Text;
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
using CVerify.API.Modules.Auth.Entities;
using CVerify.API.Modules.Auth.Enums;
using CVerify.API.Modules.Auth.Services;
using CVerify.API.Modules.Auth.Services.OtpPolicies;
using CVerify.API.Modules.Auth.Services.PasswordPolicies;
using CVerify.API.Modules.Shared.Configuration;
using CVerify.API.Modules.Shared.Diagnostics;
using CVerify.API.Modules.Shared.Domain.Entities;
using CVerify.API.Modules.Shared.Exceptions;
using CVerify.API.Modules.Shared.Persistence;
using CVerify.API.Modules.Shared.Security;
using CVerify.API.Modules.Shared.Storage.Interfaces;
using CVerify.API.Modules.Shared.System.Services;

namespace CVerify.API.UnitTests.Auth;

/// <summary>
/// Unit tests for AuthService.VerifyOtpAsync — CVerify-03 (10 UTCIDs).
/// </summary>
public sealed class CVerify03_VerifyOtpTests : IDisposable
{
    private static readonly DateTimeOffset FakeNow = new(2024, 6, 1, 12, 0, 0, TimeSpan.Zero);
    private const string TestJwtKey = "test-jwt-key-for-unit-tests-only!!";
    private const string TestPurpose = "LOGIN";
    private const string TestEmail = "user@example.com";

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

    public CVerify03_VerifyOtpTests()
    {
        _timeProvider = new FakeTimeProvider(FakeNow);

        _context = new ApplicationDbContext(
            new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .ConfigureWarnings(w => w.Ignore(InMemoryEventId.TransactionIgnoredWarning))
                .Options);

        _cacheService.Setup(c => c.AcquireLockAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<TimeSpan>())).ReturnsAsync(true);
        _cacheService.Setup(c => c.ReleaseLockAsync(It.IsAny<string>(), It.IsAny<string>())).ReturnsAsync(true);
        _cacheService.Setup(c => c.SetAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<TimeSpan?>())).Returns(Task.CompletedTask);

        // OTP policy mock — returns a policy with MaxRetries=3
        var policy = new OtpPolicyDefinition { Length = 6, AllowedCharacters = "Numeric", MaxRetries = 3 };
        _otpPolicyService.Setup(o => o.GetPolicy(It.IsAny<string>())).Returns(policy);
        // Default: ValidateAndThrow does nothing (valid code)

        _rateLimitPolicyService.Setup(r => r.ShouldEnforceCooldowns()).Returns(true);

        var connection = new Mock<ConnectionInfo>();
        connection.Setup(c => c.RemoteIpAddress).Returns(IPAddress.Loopback);
        _request.Setup(r => r.Cookies).Returns(_cookies.Object);
        _request.Setup(r => r.Headers).Returns(new HeaderDictionary { ["User-Agent"] = "TestBrowser/1.0" });
        var httpCtx = new Mock<HttpContext>();
        httpCtx.Setup(c => c.Request).Returns(_request.Object);
        httpCtx.Setup(c => c.Connection).Returns(connection.Object);
        _httpCtxAccessor.Setup(a => a.HttpContext).Returns(httpCtx.Object);

        // EnvConfiguration with a known JwtKey for HMAC hash computation
        var envConfig = new EnvConfiguration
        {
            Jwt = new JwtSettings { Key = TestJwtKey, Issuer = "test", Audience = "test" },
            SuperAdmin = new SuperAdminSettings { Email = "admin@system.com" }
        };

        _sut = new AuthService(_context, _tokenService.Object, _cacheService.Object, _accountService.Object,
            _identityRepo.Object, _httpCtxAccessor.Object, envConfig, _logger.Object,
            new AuthMetrics(), _timeProvider, _httpClientFactory.Object, _identityStateResolver.Object,
            _passwordPolicyService.Object, _otpPolicyService.Object, _storageService.Object,
            _rateLimitPolicyService.Object, _googleTokenValidator.Object, _usernameService.Object,
            _workspaceMembershipService.Object);
    }

    public void Dispose() => _context.Dispose();

    private static string ComputeOtpHash(string otp)
    {
        using var hmac = new HMACSHA256(Encoding.UTF8.GetBytes(TestJwtKey));
        return Convert.ToHexString(hmac.ComputeHash(Encoding.UTF8.GetBytes(otp))).ToLowerInvariant();
    }

    private async Task<(Guid challengeId, OtpVerification verification)> SeedActiveOtpAsync(
        string code,
        string email = TestEmail,
        string purpose = TestPurpose,
        OtpSessionStatus status = OtpSessionStatus.ACTIVE,
        DateTimeOffset? expiresAt = null,
        DateTimeOffset? consumedAt = null)
    {
        var challengeId = Guid.NewGuid();
        var verification = new OtpVerification
        {
            Id          = Guid.NewGuid(),
            ChallengeId = challengeId,
            Email       = email,
            OtpHash     = ComputeOtpHash(code),
            Purpose     = purpose,
            Status      = status,
            ExpiresAt   = expiresAt ?? FakeNow.AddMinutes(5),
            ConsumedAt  = consumedAt,
            Attempts    = 0,
        };
        _context.OtpVerifications.Add(verification);
        await _context.SaveChangesAsync();
        return (challengeId, verification);
    }

    // ── UTCID01 ───────────────────────────────────────────────────────────
    [Fact]
    public async Task CVerify03_UTCID01_VerifyOtp_ValidCodeAndChallenge_ReturnsVerifyOtpResponse()
    {
        const string code = "123456";
        var (challengeId, _) = await SeedActiveOtpAsync(code);

        var result = await _sut.VerifyOtpAsync(new VerifyOtpRequest(challengeId, TestEmail, code, TestPurpose));

        result.Should().NotBeNull();
        result.ChallengeId.Should().Be(challengeId);
        result.Email.Should().Be(TestEmail);
        result.VerificationToken.Should().NotBeNullOrEmpty();
    }

    // ── UTCID02 ───────────────────────────────────────────────────────────
    [Fact]
    public async Task CVerify03_UTCID02_VerifyOtp_WrongCode_ThrowsAuthException()
    {
        var (challengeId, _) = await SeedActiveOtpAsync("123456");

        var act = async () => await _sut.VerifyOtpAsync(new VerifyOtpRequest(challengeId, TestEmail, "654321", TestPurpose));

        await act.Should().ThrowAsync<AuthException>().Where(e => e.ErrorCode == AuthErrorCodes.InvalidCredentials);
    }

    // ── UTCID03 ───────────────────────────────────────────────────────────
    [Fact]
    public async Task CVerify03_UTCID03_VerifyOtp_ExpiredChallenge_ThrowsAuthExceptionExpiredToken()
    {
        const string code = "123456";
        var (challengeId, _) = await SeedActiveOtpAsync(code, expiresAt: FakeNow.AddMinutes(-1));

        var act = async () => await _sut.VerifyOtpAsync(new VerifyOtpRequest(challengeId, TestEmail, code, TestPurpose));

        await act.Should().ThrowAsync<AuthException>().Where(e => e.ErrorCode == AuthErrorCodes.ExpiredToken);
    }

    // ── UTCID04 ───────────────────────────────────────────────────────────
    [Fact]
    public async Task CVerify03_UTCID04_VerifyOtp_AlreadyVerifiedChallenge_ThrowsAuthExceptionTokenAlreadyConsumed()
    {
        const string code = "123456";
        var (challengeId, _) = await SeedActiveOtpAsync(code, status: OtpSessionStatus.VERIFIED, consumedAt: FakeNow.AddMinutes(-1));

        var act = async () => await _sut.VerifyOtpAsync(new VerifyOtpRequest(challengeId, TestEmail, code, TestPurpose));

        await act.Should().ThrowAsync<AuthException>().Where(e => e.ErrorCode == AuthErrorCodes.TokenAlreadyConsumed);
    }

    // ── UTCID05 ───────────────────────────────────────────────────────────
    [Fact]
    public async Task CVerify03_UTCID05_VerifyOtp_NonExistentChallenge_ThrowsAuthExceptionInvalidToken()
    {
        var act = async () => await _sut.VerifyOtpAsync(new VerifyOtpRequest(Guid.NewGuid(), TestEmail, "123456", TestPurpose));

        await act.Should().ThrowAsync<AuthException>().Where(e => e.ErrorCode == AuthErrorCodes.InvalidToken);
    }

    // ── UTCID06 ───────────────────────────────────────────────────────────
    [Fact]
    public async Task CVerify03_UTCID06_VerifyOtp_SevenDigitCode_ThrowsOtpPolicyViolationException()
    {
        _otpPolicyService
            .Setup(o => o.ValidateAndThrow("1234567", TestPurpose))
            .Throws(new OtpPolicyViolationException(new Dictionary<string, string[]> { ["code"] = new[] { "OTP must be 6 digits." } }));

        var act = async () => await _sut.VerifyOtpAsync(new VerifyOtpRequest(Guid.NewGuid(), TestEmail, "1234567", TestPurpose));

        await act.Should().ThrowAsync<OtpPolicyViolationException>();
    }

    // ── UTCID07 ───────────────────────────────────────────────────────────
    [Fact]
    public async Task CVerify03_UTCID07_VerifyOtp_CodeAllZeros_Returns200WithVerifyOtpResponse()
    {
        const string code = "000000";
        var (challengeId, _) = await SeedActiveOtpAsync(code);

        var result = await _sut.VerifyOtpAsync(new VerifyOtpRequest(challengeId, TestEmail, code, TestPurpose));

        result.Should().NotBeNull();
        result.ChallengeId.Should().Be(challengeId);
        result.VerificationToken.Should().NotBeNullOrEmpty();
    }

    // ── UTCID08 ───────────────────────────────────────────────────────────
    [Fact]
    public async Task CVerify03_UTCID08_VerifyOtp_CodeAllNines_Returns200WithVerifyOtpResponse()
    {
        const string code = "999999";
        var (challengeId, _) = await SeedActiveOtpAsync(code);

        var result = await _sut.VerifyOtpAsync(new VerifyOtpRequest(challengeId, TestEmail, code, TestPurpose));

        result.Should().NotBeNull();
        result.ChallengeId.Should().Be(challengeId);
        result.VerificationToken.Should().NotBeNullOrEmpty();
    }

    // ── UTCID09 ───────────────────────────────────────────────────────────
    [Fact]
    public async Task CVerify03_UTCID09_VerifyOtp_AlphaNumericCode_ThrowsOtpPolicyViolationException()
    {
        _otpPolicyService
            .Setup(o => o.ValidateAndThrow("abc123", TestPurpose))
            .Throws(new OtpPolicyViolationException(new Dictionary<string, string[]> { ["code"] = new[] { "OTP must be numeric." } }));

        var act = async () => await _sut.VerifyOtpAsync(new VerifyOtpRequest(Guid.NewGuid(), TestEmail, "abc123", TestPurpose));

        await act.Should().ThrowAsync<OtpPolicyViolationException>();
    }

    // ── UTCID10 ───────────────────────────────────────────────────────────
    [Fact]
    public async Task CVerify03_UTCID10_VerifyOtp_FiveDigitCode_ThrowsOtpPolicyViolationException()
    {
        _otpPolicyService
            .Setup(o => o.ValidateAndThrow("12345", TestPurpose))
            .Throws(new OtpPolicyViolationException(new Dictionary<string, string[]> { ["code"] = new[] { "OTP must be 6 digits." } }));

        var act = async () => await _sut.VerifyOtpAsync(new VerifyOtpRequest(Guid.NewGuid(), TestEmail, "12345", TestPurpose));

        await act.Should().ThrowAsync<OtpPolicyViolationException>();
    }
}
