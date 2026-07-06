using System;
using System.Collections.Generic;
using System.Net;
using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Time.Testing;
using Moq;
using Xunit;
using CVerify.API.Modules.Auth.Services;
using CVerify.API.Modules.Auth.Services.OtpPolicies;
using CVerify.API.Modules.Auth.Services.PasswordPolicies;
using CVerify.API.Modules.Shared.Configuration;
using CVerify.API.Modules.Shared.Diagnostics;
using CVerify.API.Modules.Shared.Domain.Entities;
using CVerify.API.Modules.Shared.Domain.Enums;
using CVerify.API.Modules.Shared.Exceptions;
using CVerify.API.Modules.Shared.Domain.Entities;
using CVerify.API.Modules.Shared.Persistence;
using CVerify.API.Modules.Shared.Security;
using CVerify.API.Modules.Shared.Storage.Interfaces;
using CVerify.API.Modules.Shared.System.Services;

namespace CVerify.API.UnitTests.Auth;

/// <summary>
/// Unit tests for AuthService.DeleteMeAsync — CVerify-14 (5 UTCIDs).
/// DELETE /api/auth/me — soft-deletes the authenticated user's account.
/// </summary>
public sealed class CVerify14_DeleteAccountTests : IDisposable
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

    public CVerify14_DeleteAccountTests()
    {
        _timeProvider = new FakeTimeProvider(FakeNow);

        _context = new ApplicationDbContext(
            new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .ConfigureWarnings(w => w.Ignore(InMemoryEventId.TransactionIgnoredWarning))
                .Options);

        _cacheService.Setup(c => c.RemoveAsync(It.IsAny<string>())).Returns(Task.CompletedTask);
        _tokenService.Setup(t => t.RemoveTokenFromCookie(It.IsAny<string>()));
        _workspaceMembershipService.Setup(w => w.DiscoverPendingInvitationsAsync(It.IsAny<Guid>())).Returns(Task.CompletedTask);

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
        var principal = new ClaimsPrincipal(new ClaimsIdentity(claims, "Test"));

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

    private async Task<User> SeedActiveUserAsync(string email = "user@example.com")
    {
        var user = new User
        {
            Id              = Guid.NewGuid(),
            Email           = email,
            FullName        = "Delete Me",
            Username        = "deleteme",
            Status          = UserStatus.ACTIVE,
            EmailVerifiedAt = DateTime.UtcNow,
        };
        _context.Users.Add(user);
        await _context.SaveChangesAsync();
        return user;
    }

    // ── UTCID01 ───────────────────────────────────────────────────────────
    [Fact]
    public async Task CVerify14_UTCID01_DeleteMe_ActiveUserWithValidJwt_ReturnsTrue()
    {
        var user = await SeedActiveUserAsync();
        SetupUserContext(user.Id);

        var result = await _sut.DeleteMeAsync();

        result.Should().BeTrue();
        var dbUser = await _context.Users.FindAsync(user.Id);
        dbUser!.Status.Should().Be(UserStatus.DELETED);
        dbUser.DeletedAt.Should().NotBeNull();
    }

    // ── UTCID02 ───────────────────────────────────────────────────────────
    // No JWT → service reads from HttpContext; if no claim → returns false.
    [Fact]
    public async Task CVerify14_UTCID02_DeleteMe_NoJwt_ReturnsFalse()
    {
        var httpCtx = new Mock<HttpContext>();
        httpCtx.Setup(c => c.User).Returns(new ClaimsPrincipal(new ClaimsIdentity()));
        httpCtx.Setup(c => c.Request).Returns(_request.Object);
        _httpCtxAccessor.Setup(a => a.HttpContext).Returns(httpCtx.Object);

        var result = await _sut.DeleteMeAsync();

        result.Should().BeFalse("service returns false when no NameIdentifier claim is present");
    }

    // ── UTCID03 ───────────────────────────────────────────────────────────
    [Fact]
    public async Task CVerify14_UTCID03_DeleteMe_AlreadyDeletedUser_ReturnsFalse()
    {
        var user = await SeedActiveUserAsync();
        user.DeletedAt = FakeNow.DateTime;
        await _context.SaveChangesAsync();
        SetupUserContext(user.Id);

        var result = await _sut.DeleteMeAsync();

        result.Should().BeFalse("user already has DeletedAt set — service short-circuits");
    }

    // ── UTCID04 ───────────────────────────────────────────────────────────
    [Fact]
    public async Task CVerify14_UTCID04_DeleteMe_OrganizationOwner_ThrowsBusinessRuleException()
    {
        var user = await SeedActiveUserAsync();
        SetupUserContext(user.Id);

        // Seed organization authority for this user
        var org = new Organization
        {
            Id       = Guid.NewGuid(),
            Name     = "Test Org",
            TaxCode  = "TAX001",
            Email    = "org@example.com",
            Username = "testorg",
        };
        _context.Organizations.Add(org);
        _context.OrganizationAuthorities.Add(new OrganizationAuthority
        {
            Id             = Guid.NewGuid(),
            UserId         = user.Id,
            OrganizationId = org.Id,
            Role           = "organization_owner",
        });
        await _context.SaveChangesAsync();

        var act = async () => await _sut.DeleteMeAsync();

        await act.Should().ThrowAsync<BusinessRuleException>()
            .WithMessage("*owner*");
    }

    // ── UTCID05 ───────────────────────────────────────────────────────────
    // No prior delete-request (initiation) — DeleteMeAsync does not require it; soft-deletes directly.
    [Fact]
    public async Task CVerify14_UTCID05_DeleteMe_NoPriorDeleteRequest_ServiceStillSoftDeletes()
    {
        var user = await SeedActiveUserAsync();
        SetupUserContext(user.Id);

        var result = await _sut.DeleteMeAsync();

        result.Should().BeTrue("service does not check for prior initiation — it deletes directly");
    }
}
