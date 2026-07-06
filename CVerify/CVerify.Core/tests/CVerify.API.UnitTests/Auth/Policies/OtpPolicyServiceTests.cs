using FluentAssertions;
using Microsoft.Extensions.Configuration;
using Xunit;
using CVerify.API.Modules.Auth.Services.OtpPolicies;
using CVerify.API.Modules.Shared.Configuration;
using CVerify.API.Modules.Shared.Exceptions;

namespace CVerify.API.UnitTests.Auth.Policies;

/// <summary>
/// Unit tests for OtpPolicyService.Validate / ValidateAndThrow / GetPolicy — CVerify-122 (12 UTCIDs).
/// </summary>
public sealed class OtpPolicyServiceTests
{
    private static OtpPolicyService Build(bool disableRateLimits = false)
    {
        var config  = new ConfigurationBuilder().Build();
        var envConf = new EnvConfiguration { Security = new SecuritySettings { DisableRateLimits = disableRateLimits } };
        return new OtpPolicyService(config, envConf);
    }

    // ── UTCID01 ───────────────────────────────────────────────────────────
    [Fact]
    public void CVerify122_UTCID01_Validate_SixDigitCode_ReturnsTrue()
    {
        Build().Validate("123456", "Default").Should().BeTrue();
    }

    // ── UTCID02 ───────────────────────────────────────────────────────────
    [Fact]
    public void CVerify122_UTCID02_Validate_NullCode_ReturnsFalse()
    {
        Build().Validate(null!, "Default").Should().BeFalse();
    }

    // ── UTCID03 ───────────────────────────────────────────────────────────
    [Fact]
    public void CVerify122_UTCID03_Validate_EmptyString_ReturnsFalse()
    {
        Build().Validate("", "Default").Should().BeFalse();
    }

    // ── UTCID04 ───────────────────────────────────────────────────────────
    [Fact]
    public void CVerify122_UTCID04_Validate_FiveDigitCode_ReturnsFalse()
    {
        Build().Validate("12345", "Default").Should().BeFalse();
    }

    // ── UTCID05 ───────────────────────────────────────────────────────────
    [Fact]
    public void CVerify122_UTCID05_Validate_SevenDigitCode_ReturnsFalse()
    {
        Build().Validate("1234567", "Default").Should().BeFalse();
    }

    // ── UTCID06 ───────────────────────────────────────────────────────────
    [Fact]
    public void CVerify122_UTCID06_Validate_AlphaNumericCode_ReturnsFalse()
    {
        Build().Validate("abc123", "Default").Should().BeFalse();
    }

    // ── UTCID07 ───────────────────────────────────────────────────────────
    [Fact]
    public void CVerify122_UTCID07_Validate_CodeWithSpace_ReturnsFalse()
    {
        Build().Validate("123 56", "Default").Should().BeFalse();
    }

    // ── UTCID08 ───────────────────────────────────────────────────────────
    [Fact]
    public void CVerify122_UTCID08_Validate_AllZeros_ReturnsTrue()
    {
        Build().Validate("000000", "Default").Should().BeTrue();
    }

    // ── UTCID09 ───────────────────────────────────────────────────────────
    [Fact]
    public void CVerify122_UTCID09_Validate_AllNines_ReturnsTrue()
    {
        Build().Validate("999999", "Default").Should().BeTrue();
    }

    // ── UTCID10 ───────────────────────────────────────────────────────────
    [Fact]
    public void CVerify122_UTCID10_GetPolicy_Default_ReturnsCorrectDefinition()
    {
        var policy = Build().GetPolicy("Default");
        policy.Length.Should().Be(6);
        policy.AllowedCharacters.Should().BeEquivalentTo("Numeric");
    }

    // ── UTCID11 ───────────────────────────────────────────────────────────
    [Fact]
    public void CVerify122_UTCID11_ValidateAndThrow_ValidCode_DoesNotThrow()
    {
        var act = () => Build().ValidateAndThrow("123456", "Default");
        act.Should().NotThrow();
    }

    // ── UTCID12 ───────────────────────────────────────────────────────────
    [Fact]
    public void CVerify122_UTCID12_ValidateAndThrow_InvalidCode_ThrowsOtpPolicyViolationException()
    {
        var act = () => Build().ValidateAndThrow("abc", "Default");
        act.Should().Throw<OtpPolicyViolationException>();
    }
}
