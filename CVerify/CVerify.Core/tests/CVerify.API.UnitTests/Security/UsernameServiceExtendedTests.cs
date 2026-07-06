using System;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.Extensions.Time.Testing;
using Moq;
using Xunit;
using CVerify.API.Modules.Shared.Domain.Entities;
using CVerify.API.Modules.Shared.Domain.Enums;
using CVerify.API.Modules.Shared.Exceptions;
using CVerify.API.Modules.Shared.Persistence;
using CVerify.API.Modules.Shared.Security;
using CVerify.API.Modules.Shared.System.Services;

namespace CVerify.API.UnitTests.Security;

/// <summary>
/// Extended unit tests for UsernameService — CVerify-124 (16 UTCIDs).
/// Covers ValidateUsername, IsReserved, Normalize, GenerateBaseUsername,
/// GenerateUniqueUsernameAsync, and CheckChangeCooldownAsync.
/// </summary>
public sealed class UsernameServiceExtendedTests : IDisposable
{
    private static readonly DateTimeOffset FakeNow = new(2024, 6, 1, 12, 0, 0, TimeSpan.Zero);

    private readonly ApplicationDbContext _context;
    private readonly FakeTimeProvider     _timeProvider;
    private readonly UsernameService      _sut;

    public UsernameServiceExtendedTests()
    {
        _timeProvider = new FakeTimeProvider(FakeNow);

        _context = new ApplicationDbContext(
            new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .ConfigureWarnings(w => w.Ignore(InMemoryEventId.TransactionIgnoredWarning))
                .Options);

        var rateLimitSvc = new Mock<IRateLimitPolicyService>();
        rateLimitSvc.Setup(r => r.ShouldEnforceCooldowns()).Returns(true);

        _sut = new UsernameService(_context, _timeProvider, null!, rateLimitSvc.Object);
    }

    public void Dispose() => _context.Dispose();

    // ── UTCID01 ───────────────────────────────────────────────────────────
    [Fact]
    public void CVerify124_UTCID01_ValidateUsername_ValidUsername_DoesNotThrow()
    {
        var act = () => _sut.ValidateUsername("john_doe");
        act.Should().NotThrow();
    }

    // ── UTCID02 ───────────────────────────────────────────────────────────
    [Fact]
    public void CVerify124_UTCID02_ValidateUsername_ReservedWord_ThrowsValidationException()
    {
        var act = () => _sut.ValidateUsername("admin");
        act.Should().Throw<ValidationException>().WithMessage("*reserved*");
    }

    // ── UTCID03 ───────────────────────────────────────────────────────────
    [Fact]
    public void CVerify124_UTCID03_ValidateUsername_TwoCharUsername_ThrowsTooShort()
    {
        var act = () => _sut.ValidateUsername("ab");
        act.Should().Throw<ValidationException>().WithMessage("*3 characters*");
    }

    // ── UTCID04 ───────────────────────────────────────────────────────────
    [Fact]
    public void CVerify124_UTCID04_ValidateUsername_ThirtyOneCharUsername_ThrowsTooLong()
    {
        var act = () => _sut.ValidateUsername(new string('a', 31));
        act.Should().Throw<ValidationException>().WithMessage("*30 characters*");
    }

    // ── UTCID05 ───────────────────────────────────────────────────────────
    [Fact]
    public void CVerify124_UTCID05_ValidateUsername_InvalidCharacters_ThrowsValidationException()
    {
        var act = () => _sut.ValidateUsername("user$name");
        act.Should().Throw<ValidationException>().WithMessage("*alphanumeric*");
    }

    // ── UTCID06 ───────────────────────────────────────────────────────────
    [Fact]
    public void CVerify124_UTCID06_IsReserved_UppercaseAdmin_ReturnsTrueCaseInsensitive()
    {
        _sut.IsReserved("ADMIN").Should().BeTrue();
    }

    // ── UTCID07 ───────────────────────────────────────────────────────────
    [Fact]
    public void CVerify124_UTCID07_IsReserved_RegularUsername_ReturnsFalse()
    {
        _sut.IsReserved("regularuser").Should().BeFalse();
    }

    // ── UTCID08 ───────────────────────────────────────────────────────────
    [Fact]
    public void CVerify124_UTCID08_Normalize_MixedCaseWithSpaces_ReturnsTrimmedLowercase()
    {
        _sut.Normalize("  John_DOE  ").Should().Be("john_doe");
    }

    // ── UTCID09 ───────────────────────────────────────────────────────────
    [Fact]
    public void CVerify124_UTCID09_GenerateBaseUsername_EmailWithDot_ReturnsLocalPart()
    {
        _sut.GenerateBaseUsername("john.doe@email.com").Should().Be("john.doe");
    }

    // ── UTCID10 ───────────────────────────────────────────────────────────
    [Fact]
    public async Task CVerify124_UTCID10_GenerateUniqueUsernameAsync_NoCollision_ReturnsBaseUsername()
    {
        var username = await _sut.GenerateUniqueUsernameAsync("john.doe@email.com");
        username.Should().Be("john.doe");
    }

    // ── UTCID11 ───────────────────────────────────────────────────────────
    [Fact]
    public async Task CVerify124_UTCID11_GenerateUniqueUsernameAsync_BaseUsernameTaken_ReturnsUsernameWithSuffix()
    {
        _context.Users.Add(new User
        {
            Id       = Guid.NewGuid(),
            Email    = "other@example.com",
            FullName = "Other User",
            Username = "john.doe",
            Status   = UserStatus.ACTIVE,
        });
        await _context.SaveChangesAsync();

        var username = await _sut.GenerateUniqueUsernameAsync("john.doe@email.com");
        username.Should().Be("john.doe1");
    }

    // ── UTCID12 ───────────────────────────────────────────────────────────
    [Fact]
    public async Task CVerify124_UTCID12_CheckChangeCooldownAsync_ChangedFiveDaysAgo_ThrowsValidationException()
    {
        var userId = Guid.NewGuid();
        _context.Users.Add(new User
        {
            Id                   = userId,
            Email                = "user@example.com",
            FullName             = "Test User",
            Username             = "testuser",
            Status               = UserStatus.ACTIVE,
            LastUsernameChangeAt = FakeNow.AddDays(-5),
        });
        await _context.SaveChangesAsync();

        var act = async () => await _sut.CheckChangeCooldownAsync(userId);
        await act.Should().ThrowAsync<ValidationException>().WithMessage("*30 days*");
    }

    // ── UTCID13 ───────────────────────────────────────────────────────────
    [Fact]
    public async Task CVerify124_UTCID13_CheckChangeCooldownAsync_ChangedThirtyOneDaysAgo_DoesNotThrow()
    {
        var userId = Guid.NewGuid();
        _context.Users.Add(new User
        {
            Id                   = userId,
            Email                = "user2@example.com",
            FullName             = "Test User2",
            Username             = "testuser2",
            Status               = UserStatus.ACTIVE,
            LastUsernameChangeAt = FakeNow.AddDays(-31),
        });
        await _context.SaveChangesAsync();

        var act = async () => await _sut.CheckChangeCooldownAsync(userId);
        await act.Should().NotThrowAsync();
    }

    // ── UTCID14 ───────────────────────────────────────────────────────────
    [Fact]
    public void CVerify124_UTCID14_ValidateUsername_ExactlyTwoChars_BoundaryThrowsTooShort()
    {
        var act = () => _sut.ValidateUsername("ab");  // exactly 2 chars
        act.Should().Throw<ValidationException>().WithMessage("*3 characters*");
    }

    // ── UTCID15 ───────────────────────────────────────────────────────────
    [Fact]
    public void CVerify124_UTCID15_ValidateUsername_ExactlyThirtyChars_BoundaryDoesNotThrow()
    {
        var act = () => _sut.ValidateUsername(new string('a', 30));  // exactly 30 chars
        act.Should().NotThrow();
    }

    // ── UTCID16 ───────────────────────────────────────────────────────────
    [Theory]
    [InlineData("login")]
    [InlineData("register")]
    public void CVerify124_UTCID16_ValidateUsername_OtherReservedWords_ThrowsReservedException(string reserved)
    {
        var act = () => _sut.ValidateUsername(reserved);
        act.Should().Throw<ValidationException>().WithMessage("*reserved*");
    }
}
