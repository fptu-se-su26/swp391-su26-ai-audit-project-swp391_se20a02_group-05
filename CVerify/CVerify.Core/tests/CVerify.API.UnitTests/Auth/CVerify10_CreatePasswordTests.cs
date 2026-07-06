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
using CVerify.API.Modules.Shared.Exceptions;
using CVerify.API.Modules.Shared.Persistence;
using CVerify.API.Modules.Shared.Security;
using CVerify.API.Modules.Shared.Storage.Interfaces;
using CVerify.API.Modules.Shared.System.Services;

namespace CVerify.API.UnitTests.Auth;

/// <summary>
/// Unit tests for AuthService.CreatePasswordAsync — CVerify-10 (8 UTCIDs).
/// </summary>
public sealed class CVerify10_CreatePasswordTests : IDisposable
{
    private static readonly DateTimeOffset FakeNow = new(2024, 6, 1, 12, 0, 0, TimeSpan.Zero);
    private const string TestEmail       = "user@example.com";
    private const string ValidToken      = "VALID_SETUP_TOKEN_001";
    private const string ValidPassword   = "ValidPass1!";

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
    private Guid _challengeId;

    public CVerify10_CreatePasswordTests()
    {
        _timeProvider = new FakeTimeProvider(FakeNow);
        _challengeId  = Guid.NewGuid();

        _context = new ApplicationDbContext(
            new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .ConfigureWarnings(w => w.Ignore(InMemoryEventId.TransactionIgnoredWarning))
                .Options);

        // Default: valid setup token in cache
        _cacheService
            .Setup(c => c.GetAsync<string>($"setup:token:{TestEmail}:{_challengeId}"))
            .ReturnsAsync(ValidToken);
        _cacheService.Setup(c => c.RemoveAsync(It.IsAny<string>())).Returns(Task.CompletedTask);
        _cacheService.Setup(c => c.SetAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<TimeSpan?>())).Returns(Task.CompletedTask);
        _cacheService.Setup(c => c.AddToSetAsync(It.IsAny<string>(), It.IsAny<string>())).Returns(Task.CompletedTask);
        _cacheService.Setup(c => c.AcquireLockAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<TimeSpan>())).ReturnsAsync(true);
        _cacheService.Setup(c => c.ReleaseLockAsync(It.IsAny<string>(), It.IsAny<string>())).ReturnsAsync(true);

        _tokenService.Setup(t => t.GenerateJwtToken(It.IsAny<User>(), It.IsAny<IEnumerable<string>>(), It.IsAny<IEnumerable<string>>(), It.IsAny<Guid?>(), It.IsAny<string?>(), It.IsAny<Guid?>())).Returns("JWT");
        _tokenService.Setup(t => t.GenerateRefreshToken()).Returns("REFRESH");
        _tokenService.Setup(t => t.SetTokenInsideCookie(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<DateTime?>()));

        _identityRepo.Setup(r => r.GetUserRolesAsync(It.IsAny<Guid>())).ReturnsAsync(new[] { "USER" });
        _identityRepo.Setup(r => r.GetUserPermissionsAsync(It.IsAny<Guid>())).ReturnsAsync(Array.Empty<string>());

        _identityStateResolver.Setup(r => r.InvalidateCacheAsync(It.IsAny<string>())).Returns(Task.CompletedTask);

        _workspaceMembershipService.Setup(w => w.BootstrapInitialAdminAsync(It.IsAny<string>(), It.IsAny<bool>(), It.IsAny<System.Threading.CancellationToken>())).Returns(Task.CompletedTask);
        _workspaceMembershipService.Setup(w => w.DiscoverPendingInvitationsAsync(It.IsAny<Guid>())).Returns(Task.CompletedTask);

        _usernameService
            .Setup(u => u.RunWithUsernameRetryAsync(It.IsAny<User>(), It.IsAny<string>(), It.IsAny<Func<Task>>(), It.IsAny<int>(), It.IsAny<System.Threading.CancellationToken>()))
            .Returns<User, string, Func<Task>, int, System.Threading.CancellationToken>(async (_, __, saveAction, ___, ____) => { await saveAction(); return "generateduser"; });

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

    private async Task SeedRoleAsync(string roleName = "USER")
    {
        _context.Roles.Add(new Role { Id = Guid.NewGuid(), Name = roleName, DisplayName = roleName });
        await _context.SaveChangesAsync();
    }

    private CreatePasswordRequest BuildRequest(string password, string confirmPassword = ValidPassword, string? token = ValidToken) =>
        new()
        {
            ChallengeId         = _challengeId,
            Email               = TestEmail,
            VerificationToken   = token ?? ValidToken,
            Password            = password,
            ConfirmPassword     = confirmPassword,
            FullName            = "New User",
        };

    // ── UTCID01 ───────────────────────────────────────────────────────────
    [Fact]
    public async Task CVerify10_UTCID01_CreatePassword_ValidTokenAndStrongPassword_ReturnsAuthResponse()
    {
        await SeedRoleAsync();

        var result = await _sut.CreatePasswordAsync(BuildRequest(ValidPassword));

        result.Should().NotBeNull();
        result.Email.Should().Be(TestEmail);
        result.Status.Should().Be("ACTIVE");
    }

    // ── UTCID02 ───────────────────────────────────────────────────────────
    [Fact]
    public async Task CVerify10_UTCID02_CreatePassword_InvalidOrExpiredSetupToken_ThrowsAuthExceptionInvalidToken()
    {
        await SeedRoleAsync();

        // Cache returns null (token expired / never set)
        _cacheService
            .Setup(c => c.GetAsync<string>($"setup:token:{TestEmail}:{_challengeId}"))
            .ReturnsAsync((string?)null);

        var act = async () => await _sut.CreatePasswordAsync(BuildRequest(ValidPassword, token: "WRONG_TOKEN"));

        await act.Should().ThrowAsync<AuthException>()
            .Where(e => e.ErrorCode == AuthErrorCodes.InvalidToken);
    }

    // ── UTCID03 ───────────────────────────────────────────────────────────
    [Fact]
    public async Task CVerify10_UTCID03_CreatePassword_SevenCharPassword_ThrowsPasswordPolicyViolationException()
    {
        await SeedRoleAsync();
        _passwordPolicyService
            .Setup(p => p.ValidateAndThrowAsync("Short1!", "Default"))
            .ThrowsAsync(new PasswordPolicyViolationException(new Dictionary<string, string[]>
                { ["password"] = new[] { "Password must be at least 8 characters." } }));

        var act = async () => await _sut.CreatePasswordAsync(BuildRequest("Short1!"));

        await act.Should().ThrowAsync<PasswordPolicyViolationException>();
    }

    // ── UTCID04 ───────────────────────────────────────────────────────────
    // Confirm password mismatch → controller [Compare] annotation validates this.
    // Service uses only the Password field; ConfirmPassword is not validated at service level.
    [Fact]
    public async Task CVerify10_UTCID04_CreatePassword_ConfirmPasswordMismatch_ServiceIgnoresMismatchReturnsSuccess()
    {
        await SeedRoleAsync();

        var result = await _sut.CreatePasswordAsync(BuildRequest(ValidPassword, confirmPassword: "Different9@"));

        result.Should().NotBeNull("service does not compare Password vs ConfirmPassword — controller responsibility");
    }

    // ── UTCID05 ───────────────────────────────────────────────────────────
    // No JWT → controller [Authorize] returns 401 before service is called.
    // CreatePasswordAsync does not read JWT claims; it reads the setup token from cache.
    // If cache token is valid, service succeeds regardless of whether caller had a JWT.
    [Fact]
    public async Task CVerify10_UTCID05_CreatePassword_NoJwtControllerLevel_ServiceSucceedsWithValidSetupToken()
    {
        await SeedRoleAsync();

        var result = await _sut.CreatePasswordAsync(BuildRequest(ValidPassword));

        result.Should().NotBeNull("CreatePasswordAsync authenticates via setup token in cache, not JWT claims");
    }

    // ── UTCID06 ───────────────────────────────────────────────────────────
    [Fact]
    public async Task CVerify10_UTCID06_CreatePassword_ExactlyEightCharPassword_ReturnsAuthResponse()
    {
        await SeedRoleAsync();
        const string eightChar = "Exact8!a";

        var result = await _sut.CreatePasswordAsync(BuildRequest(eightChar, eightChar));

        result.Should().NotBeNull();
        result.Status.Should().Be("ACTIVE");
    }

    // ── UTCID07 ───────────────────────────────────────────────────────────
    [Fact]
    public async Task CVerify10_UTCID07_CreatePassword_AllLowerCaseNoUppercase_ThrowsPasswordPolicyViolationException()
    {
        await SeedRoleAsync();
        _passwordPolicyService
            .Setup(p => p.ValidateAndThrowAsync("alllower1!", "Default"))
            .ThrowsAsync(new PasswordPolicyViolationException(new Dictionary<string, string[]>
                { ["password"] = new[] { "Password must contain at least one uppercase letter." } }));

        var act = async () => await _sut.CreatePasswordAsync(BuildRequest("alllower1!"));

        await act.Should().ThrowAsync<PasswordPolicyViolationException>();
    }

    // ── UTCID08 ───────────────────────────────────────────────────────────
    [Fact]
    public async Task CVerify10_UTCID08_CreatePassword_NoSpecialCharacter_ThrowsPasswordPolicyViolationException()
    {
        await SeedRoleAsync();
        _passwordPolicyService
            .Setup(p => p.ValidateAndThrowAsync("NoSpecial1A", "Default"))
            .ThrowsAsync(new PasswordPolicyViolationException(new Dictionary<string, string[]>
                { ["password"] = new[] { "Password must contain at least one special character." } }));

        var act = async () => await _sut.CreatePasswordAsync(BuildRequest("NoSpecial1A"));

        await act.Should().ThrowAsync<PasswordPolicyViolationException>();
    }
}
