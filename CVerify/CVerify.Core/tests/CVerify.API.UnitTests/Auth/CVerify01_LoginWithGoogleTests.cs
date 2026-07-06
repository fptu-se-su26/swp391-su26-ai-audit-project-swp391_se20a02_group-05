using System;
using System.Collections.Generic;
using System.Net;
using System.Text;
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
using CVerify.API.Modules.Shared.Persistence;
using CVerify.API.Modules.Shared.Security;
using CVerify.API.Modules.Shared.Storage.Interfaces;
using CVerify.API.Modules.Shared.System.Services;

namespace CVerify.API.UnitTests.Auth;

/// <summary>
/// Unit tests for AuthService.LoginWithGoogleAsync — CVerify-01 (7 UTCIDs).
/// </summary>
public sealed class CVerify01_LoginWithGoogleTests : IDisposable
{
    private static readonly DateTimeOffset FakeNow = new(2024, 6, 1, 12, 0, 0, TimeSpan.Zero);

    private readonly FakeTimeProvider _timeProvider;
    private readonly ApplicationDbContext _context;

    private readonly Mock<ITokenService>                  _tokenService              = new();
    private readonly Mock<ICacheService>                  _cacheService              = new();
    private readonly Mock<IAccountService>                _accountService            = new();
    private readonly Mock<IIdentityRepository>            _identityRepo              = new();
    private readonly Mock<IHttpContextAccessor>           _httpCtxAccessor           = new();
    private readonly Mock<ILogger<AuthService>>           _logger                    = new();
    private readonly Mock<IHttpClientFactory>             _httpClientFactory         = new();
    private readonly Mock<IIdentityStateResolver>         _identityStateResolver     = new();
    private readonly Mock<IPasswordPolicyService>         _passwordPolicyService     = new();
    private readonly Mock<IOtpPolicyService>              _otpPolicyService          = new();
    private readonly Mock<IStorageService>                _storageService            = new();
    private readonly Mock<IRateLimitPolicyService>        _rateLimitPolicyService    = new();
    private readonly Mock<IGoogleTokenValidator>          _googleTokenValidator      = new();
    private readonly Mock<IUsernameService>               _usernameService           = new();
    private readonly Mock<IWorkspaceMembershipService>    _workspaceMembershipService = new();

    private readonly Mock<IRequestCookieCollection>       _cookies  = new();
    private readonly Mock<HttpRequest>                    _request  = new();
    private readonly Mock<HttpContext>                    _httpCtx  = new();

    private readonly AuthService _sut;

    public CVerify01_LoginWithGoogleTests()
    {
        _timeProvider = new FakeTimeProvider(FakeNow);

        _context = new ApplicationDbContext(
            new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .ConfigureWarnings(w => w.Ignore(InMemoryEventId.TransactionIgnoredWarning))
                .Options);

        // Cache defaults
        _cacheService.Setup(c => c.AcquireLockAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<TimeSpan>())).ReturnsAsync(true);
        _cacheService.Setup(c => c.ReleaseLockAsync(It.IsAny<string>(), It.IsAny<string>())).ReturnsAsync(true);
        _cacheService.Setup(c => c.RemoveAsync(It.IsAny<string>())).Returns(Task.CompletedTask);
        _cacheService.Setup(c => c.AddToSetAsync(It.IsAny<string>(), It.IsAny<string>())).Returns(Task.CompletedTask);

        // Token stubs
        _tokenService.Setup(t => t.GenerateJwtToken(It.IsAny<User>(), It.IsAny<IEnumerable<string>>(), It.IsAny<IEnumerable<string>>(), It.IsAny<Guid?>(), It.IsAny<string?>(), It.IsAny<Guid?>())).Returns("JWT");
        _tokenService.Setup(t => t.GenerateRefreshToken()).Returns("REFRESH");
        _tokenService.Setup(t => t.SetTokenInsideCookie(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<DateTime?>()));

        // Identity repo defaults
        _identityRepo.Setup(r => r.GetUserRolesAsync(It.IsAny<Guid>())).ReturnsAsync(new[] { "USER" });
        _identityRepo.Setup(r => r.GetUserPermissionsAsync(It.IsAny<Guid>())).ReturnsAsync(Array.Empty<string>());

        // Identity state resolver
        _identityStateResolver.Setup(s => s.InvalidateCacheAsync(It.IsAny<string>())).Returns(Task.CompletedTask);

        // Workspace bootstrap
        _workspaceMembershipService.Setup(w => w.BootstrapInitialAdminAsync(It.IsAny<string>(), It.IsAny<bool>(), It.IsAny<System.Threading.CancellationToken>())).Returns(Task.CompletedTask);
        _workspaceMembershipService.Setup(w => w.DiscoverPendingInvitationsAsync(It.IsAny<Guid>())).Returns(Task.CompletedTask);

        // Username service for new-user auto-registration
        _usernameService
            .Setup(u => u.RunWithUsernameRetryAsync(It.IsAny<User>(), It.IsAny<string>(), It.IsAny<Func<Task>>(), It.IsAny<int>(), It.IsAny<System.Threading.CancellationToken>()))
            .Returns(async (User _, string _, Func<Task> save, int _, System.Threading.CancellationToken _) => { await save(); return "generated_username"; });

        // HTTP context
        var connection = new Mock<ConnectionInfo>();
        connection.Setup(c => c.RemoteIpAddress).Returns(IPAddress.Loopback);
        _request.Setup(r => r.Cookies).Returns(_cookies.Object);
        _request.Setup(r => r.Headers).Returns(new HeaderDictionary { ["User-Agent"] = "TestBrowser/1.0" });
        _httpCtx.Setup(c => c.Request).Returns(_request.Object);
        _httpCtx.Setup(c => c.Connection).Returns(connection.Object);
        _httpCtxAccessor.Setup(a => a.HttpContext).Returns(_httpCtx.Object);

        _sut = new AuthService(_context, _tokenService.Object, _cacheService.Object, _accountService.Object,
            _identityRepo.Object, _httpCtxAccessor.Object, new EnvConfiguration(), _logger.Object,
            new AuthMetrics(), _timeProvider, _httpClientFactory.Object, _identityStateResolver.Object,
            _passwordPolicyService.Object, _otpPolicyService.Object, _storageService.Object,
            _rateLimitPolicyService.Object, _googleTokenValidator.Object, _usernameService.Object,
            _workspaceMembershipService.Object);
    }

    public void Dispose() => _context.Dispose();

    private static GoogleJsonWebSignature.Payload MakePayload(
        string subject = "google-sub-001",
        string email = "user@gmail.com",
        string? name = "Test User",
        string? picture = "https://example.com/avatar.jpg",
        bool emailVerified = true) => new()
    {
        Subject = subject,
        Email = email,
        Name = name,
        Picture = picture,
        EmailVerified = emailVerified
    };

    private async Task<(User user, Role role)> SeedExistingGoogleUserAsync(
        string subject = "google-sub-001",
        string email = "user@gmail.com",
        string? avatarUrl = "https://example.com/avatar.jpg",
        UserStatus status = UserStatus.ACTIVE)
    {
        var role = new Role { Id = Guid.NewGuid(), Name = "USER", DisplayName = "User" };
        var user = new User
        {
            Id         = Guid.NewGuid(),
            Email      = email,
            FullName   = "Test User",
            Username   = "testuser",
            AvatarUrl  = avatarUrl,
            AvatarSource = AvatarSource.Google,
            Status     = status,
            EmailVerifiedAt = DateTime.UtcNow,
        };
        var provider = new AuthProvider
        {
            Id           = Guid.NewGuid(),
            UserId       = user.Id,
            ProviderName = "Google",
            ProviderKey  = subject,
            ProviderAccountId = email,
            CreatedAt    = FakeNow
        };

        _context.Roles.Add(role);
        _context.Users.Add(user);
        _context.AuthProviders.Add(provider);
        await _context.SaveChangesAsync();
        return (user, role);
    }

    // ── UTCID01 ───────────────────────────────────────────────────────────
    [Fact]
    public async Task CVerify01_UTCID01_LoginWithGoogle_ExistingUserByAuthProvider_Returns200WithAuthResponse()
    {
        const string subject = "google-sub-001";
        var (user, _) = await SeedExistingGoogleUserAsync(subject: subject);
        var payload = MakePayload(subject: subject, email: user.Email);
        _googleTokenValidator.Setup(v => v.ValidateAsync(It.IsAny<string>(), It.IsAny<GoogleJsonWebSignature.ValidationSettings>())).ReturnsAsync(payload);

        var result = await _sut.LoginWithGoogleAsync(new GoogleLoginRequest("valid-id-token"));

        result.Should().NotBeNull();
        result!.Id.Should().Be(user.Id);
        result.Email.Should().Be(user.Email.ToLowerInvariant());
        result.Status.Should().Be("ACTIVE");
    }

    // ── UTCID02 ───────────────────────────────────────────────────────────
    [Fact]
    public async Task CVerify01_UTCID02_LoginWithGoogle_NewUser_AutoRegistersAndReturns200()
    {
        var role = new Role { Id = Guid.NewGuid(), Name = "USER", DisplayName = "User" };
        _context.Roles.Add(role);
        await _context.SaveChangesAsync();

        var payload = MakePayload(subject: "brand-new-sub", email: "newuser@gmail.com");
        _googleTokenValidator.Setup(v => v.ValidateAsync(It.IsAny<string>(), It.IsAny<GoogleJsonWebSignature.ValidationSettings>())).ReturnsAsync(payload);

        var result = await _sut.LoginWithGoogleAsync(new GoogleLoginRequest("valid-id-token"));

        result.Should().NotBeNull();
        result!.Email.Should().Be("newuser@gmail.com");
        result.Status.Should().Be("ACTIVE");
    }

    // ── UTCID03 ───────────────────────────────────────────────────────────
    [Fact]
    public async Task CVerify01_UTCID03_LoginWithGoogle_ExpiredToken_ThrowsUnauthorizedAccessException()
    {
        _googleTokenValidator
            .Setup(v => v.ValidateAsync(It.IsAny<string>(), It.IsAny<GoogleJsonWebSignature.ValidationSettings>()))
            .ThrowsAsync(new InvalidJwtException("Token is expired."));

        var act = async () => await _sut.LoginWithGoogleAsync(new GoogleLoginRequest("expired-token"));

        await act.Should().ThrowAsync<UnauthorizedAccessException>().WithMessage("*Google ID Token*");
    }

    // ── UTCID04 ───────────────────────────────────────────────────────────
    [Fact]
    public async Task CVerify01_UTCID04_LoginWithGoogle_NullPayload_ThrowsUnauthorizedAccessException()
    {
        _googleTokenValidator
            .Setup(v => v.ValidateAsync(It.IsAny<string>(), It.IsAny<GoogleJsonWebSignature.ValidationSettings>()))
            .ReturnsAsync((GoogleJsonWebSignature.Payload?)null);

        var act = async () => await _sut.LoginWithGoogleAsync(new GoogleLoginRequest(""));

        await act.Should().ThrowAsync<UnauthorizedAccessException>().WithMessage("*Google authentication*");
    }

    // ── UTCID05 ───────────────────────────────────────────────────────────
    [Fact]
    public async Task CVerify01_UTCID05_LoginWithGoogle_BannedUser_ThrowsUnauthorizedAccessException()
    {
        const string subject = "banned-sub";
        await SeedExistingGoogleUserAsync(subject: subject, email: "banned@gmail.com", status: UserStatus.BANNED);
        var payload = MakePayload(subject: subject, email: "banned@gmail.com");
        _googleTokenValidator.Setup(v => v.ValidateAsync(It.IsAny<string>(), It.IsAny<GoogleJsonWebSignature.ValidationSettings>())).ReturnsAsync(payload);
        _accountService.Setup(a => a.IsAccountDisabled(It.IsAny<User>())).Returns(true);

        var act = async () => await _sut.LoginWithGoogleAsync(new GoogleLoginRequest("valid-id-token"));

        await act.Should().ThrowAsync<UnauthorizedAccessException>().WithMessage("*disabled*");
    }

    // ── UTCID06 ───────────────────────────────────────────────────────────
    [Fact]
    public async Task CVerify01_UTCID06_LoginWithGoogle_DeletedUser_ThrowsUnauthorizedAccessException()
    {
        const string subject = "deleted-sub";
        await SeedExistingGoogleUserAsync(subject: subject, email: "deleted@gmail.com", status: UserStatus.DELETED);
        var payload = MakePayload(subject: subject, email: "deleted@gmail.com");
        _googleTokenValidator.Setup(v => v.ValidateAsync(It.IsAny<string>(), It.IsAny<GoogleJsonWebSignature.ValidationSettings>())).ReturnsAsync(payload);
        _accountService.Setup(a => a.IsAccountDisabled(It.IsAny<User>())).Returns(true);

        var act = async () => await _sut.LoginWithGoogleAsync(new GoogleLoginRequest("valid-id-token"));

        await act.Should().ThrowAsync<UnauthorizedAccessException>().WithMessage("*disabled*");
    }

    // ── UTCID07 ───────────────────────────────────────────────────────────
    [Fact]
    public async Task CVerify01_UTCID07_LoginWithGoogle_UserHasNoAvatar_Returns200WithNullAvatarUrl()
    {
        const string subject = "no-avatar-sub";
        await SeedExistingGoogleUserAsync(subject: subject, email: "noavatar@gmail.com", avatarUrl: null);
        var payload = MakePayload(subject: subject, email: "noavatar@gmail.com", picture: null);
        _googleTokenValidator.Setup(v => v.ValidateAsync(It.IsAny<string>(), It.IsAny<GoogleJsonWebSignature.ValidationSettings>())).ReturnsAsync(payload);

        var result = await _sut.LoginWithGoogleAsync(new GoogleLoginRequest("valid-id-token"));

        result.Should().NotBeNull();
        result!.AvatarUrl.Should().BeNull();
    }
}
