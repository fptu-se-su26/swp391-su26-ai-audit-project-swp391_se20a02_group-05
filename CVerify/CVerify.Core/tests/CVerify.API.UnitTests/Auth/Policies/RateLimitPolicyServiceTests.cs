using FluentAssertions;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Time.Testing;
using Moq;
using Xunit;
using CVerify.API.Modules.Shared.Configuration;
using CVerify.API.Modules.Shared.System.Services;

namespace CVerify.API.UnitTests.Auth.Policies;

/// <summary>
/// Unit tests for RateLimitPolicyService — CVerify-123 (7 UTCIDs).
/// </summary>
public sealed class RateLimitPolicyServiceTests
{
    private static RateLimitPolicyService Build(bool disableRateLimits)
    {
        var envConf = new EnvConfiguration { Security = new SecuritySettings { DisableRateLimits = disableRateLimits } };
        var env     = new Mock<IHostEnvironment>();
        env.Setup(e => e.EnvironmentName).Returns("Testing");
        var logger  = new Mock<ILogger<RateLimitPolicyService>>();
        return new RateLimitPolicyService(envConf, env.Object, logger.Object, new FakeTimeProvider());
    }

    // ── UTCID01 ───────────────────────────────────────────────────────────
    [Fact]
    public void CVerify123_UTCID01_ShouldEnforceCooldowns_DisableRateLimitsFalse_ReturnsTrue()
    {
        var sut = Build(disableRateLimits: false);
        sut.ShouldEnforceCooldowns().Should().BeTrue();
    }

    // ── UTCID02 ───────────────────────────────────────────────────────────
    [Fact]
    public void CVerify123_UTCID02_ShouldEnforceCooldowns_DisableRateLimitsTrue_ReturnsFalse()
    {
        var sut = Build(disableRateLimits: true);
        sut.ShouldEnforceCooldowns().Should().BeFalse();
    }

    // ── UTCID03 ───────────────────────────────────────────────────────────
    [Fact]
    public void CVerify123_UTCID03_LogBypass_DisableRateLimitsTrue_LogsInformationMessage()
    {
        var envConf = new EnvConfiguration { Security = new SecuritySettings { DisableRateLimits = true } };
        var env     = new Mock<IHostEnvironment>();
        env.Setup(e => e.EnvironmentName).Returns("Testing");
        var logger  = new Mock<ILogger<RateLimitPolicyService>>();
        var sut     = new RateLimitPolicyService(envConf, env.Object, logger.Object, new FakeTimeProvider());

        sut.LogBypass("SendOtp", "/api/auth/send-otp", "user@example.com");

        logger.Verify(
            l => l.Log(
                LogLevel.Information,
                It.IsAny<EventId>(),
                It.Is<It.IsAnyType>((v, _) => v.ToString()!.Contains("SendOtp")),
                null,
                It.IsAny<Func<It.IsAnyType, Exception?, string>>()),
            Times.Once);
    }

    // ── UTCID04 ───────────────────────────────────────────────────────────
    [Fact]
    public void CVerify123_UTCID04_ShouldEnforceCooldowns_DisableRateLimitsFalse_CooldownsAreEnforced()
    {
        var sut = Build(disableRateLimits: false);
        // When ShouldEnforceCooldowns() is true, callers must enforce rate limits
        sut.ShouldEnforceCooldowns().Should().BeTrue("cooldowns must be enforced in production mode");
    }

    // ── UTCID05 ───────────────────────────────────────────────────────────
    [Fact]
    public void CVerify123_UTCID05_LogBypass_WithActionDetails_LogsCorrectPayload()
    {
        var envConf = new EnvConfiguration { Security = new SecuritySettings { DisableRateLimits = true } };
        var env     = new Mock<IHostEnvironment>();
        env.Setup(e => e.EnvironmentName).Returns("Testing");
        var logger  = new Mock<ILogger<RateLimitPolicyService>>();
        var sut     = new RateLimitPolicyService(envConf, env.Object, logger.Object, new FakeTimeProvider());

        sut.LogBypass("SendOtp", "/api/auth/send-otp", "user@ex.com");

        logger.Verify(
            l => l.Log(
                LogLevel.Information,
                It.IsAny<EventId>(),
                It.Is<It.IsAnyType>((v, _) =>
                    v.ToString()!.Contains("SendOtp") &&
                    v.ToString()!.Contains("/api/auth/send-otp") &&
                    v.ToString()!.Contains("user@ex.com")),
                null,
                It.IsAny<Func<It.IsAnyType, Exception?, string>>()),
            Times.Once);
    }

    // ── UTCID06 ───────────────────────────────────────────────────────────
    [Fact]
    public void CVerify123_UTCID06_DisableRateLimits_Property_ReflectsConfigValue()
    {
        var sut = Build(disableRateLimits: true);
        sut.DisableRateLimits.Should().BeTrue();
    }

    // ── UTCID07 ───────────────────────────────────────────────────────────
    [Fact]
    public void CVerify123_UTCID07_DisableRateLimitsTrue_ShouldEnforceCooldownsFalse_LockoutBypassed()
    {
        var sut = Build(disableRateLimits: true);
        // When ShouldEnforceCooldowns() returns false, any account-lockout check
        // dependent on it is bypassed — callers skip lockout enforcement.
        sut.ShouldEnforceCooldowns().Should().BeFalse("lockout checks must be bypassed when DisableRateLimits=true");
        sut.DisableRateLimits.Should().BeTrue();
    }
}
