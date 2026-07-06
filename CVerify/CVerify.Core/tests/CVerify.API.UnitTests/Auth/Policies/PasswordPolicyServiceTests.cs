using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.Extensions.Configuration;
using Xunit;
using CVerify.API.Modules.Auth.Services.PasswordPolicies;
using CVerify.API.Modules.Shared.Exceptions;

namespace CVerify.API.UnitTests.Auth.Policies;

/// <summary>
/// Unit tests for PasswordPolicyService.Validate / ValidateAndThrowAsync — CVerify-121 (12 UTCIDs).
/// </summary>
public sealed class PasswordPolicyServiceTests
{
    private static PasswordPolicyService Build()
        => new(new ConfigurationBuilder().Build());

    // ── UTCID01 ───────────────────────────────────────────────────────────
    [Fact]
    public void CVerify121_UTCID01_Validate_ValidPasswordDefault_ReturnsIsValidTrue()
    {
        var sut = Build();
        var result = sut.Validate("Secure1!Pass", "Default");
        result.IsValid.Should().BeTrue();
        result.FailedRuleMessages.Should().BeEmpty();
    }

    // ── UTCID02 ───────────────────────────────────────────────────────────
    [Fact]
    public async Task CVerify121_UTCID02_Validate_TooShortDefault_ReturnsIsValidFalseAndThrows()
    {
        var sut = Build();
        var result = sut.Validate("Short1!", "Default");   // 7 chars
        result.IsValid.Should().BeFalse();
        result.FailedRuleMessages.Should().ContainMatch("*at least 8*");

        await FluentActions.Invoking(() => sut.ValidateAndThrowAsync("Short1!", "Default"))
            .Should().ThrowAsync<PasswordPolicyViolationException>();
    }

    // ── UTCID03 ───────────────────────────────────────────────────────────
    [Fact]
    public async Task CVerify121_UTCID03_Validate_NoUppercaseDefault_ReturnsInvalidAndThrows()
    {
        var sut = Build();
        var result = sut.Validate("alllower1!", "Default");
        result.IsValid.Should().BeFalse();
        result.FailedRuleMessages.Should().ContainMatch("*uppercase*");

        await FluentActions.Invoking(() => sut.ValidateAndThrowAsync("alllower1!", "Default"))
            .Should().ThrowAsync<PasswordPolicyViolationException>();
    }

    // ── UTCID04 ───────────────────────────────────────────────────────────
    [Fact]
    public async Task CVerify121_UTCID04_Validate_NoLowercaseDefault_ReturnsInvalidAndThrows()
    {
        var sut = Build();
        var result = sut.Validate("ALLCAPS1!", "Default");
        result.IsValid.Should().BeFalse();
        result.FailedRuleMessages.Should().ContainMatch("*lowercase*");

        await FluentActions.Invoking(() => sut.ValidateAndThrowAsync("ALLCAPS1!", "Default"))
            .Should().ThrowAsync<PasswordPolicyViolationException>();
    }

    // ── UTCID05 ───────────────────────────────────────────────────────────
    [Fact]
    public async Task CVerify121_UTCID05_Validate_NoDigitDefault_ReturnsInvalidAndThrows()
    {
        var sut = Build();
        var result = sut.Validate("NoDigitHere!", "Default");
        result.IsValid.Should().BeFalse();
        result.FailedRuleMessages.Should().ContainMatch("*digit*");

        await FluentActions.Invoking(() => sut.ValidateAndThrowAsync("NoDigitHere!", "Default"))
            .Should().ThrowAsync<PasswordPolicyViolationException>();
    }

    // ── UTCID06 ───────────────────────────────────────────────────────────
    [Fact]
    public async Task CVerify121_UTCID06_Validate_NoSpecialCharDefault_ReturnsInvalidAndThrows()
    {
        var sut = Build();
        var result = sut.Validate("NoSpecial123", "Default");
        result.IsValid.Should().BeFalse();
        result.FailedRuleMessages.Should().ContainMatch("*special*");

        await FluentActions.Invoking(() => sut.ValidateAndThrowAsync("NoSpecial123", "Default"))
            .Should().ThrowAsync<PasswordPolicyViolationException>();
    }

    // ── UTCID07 ───────────────────────────────────────────────────────────
    [Fact]
    public void CVerify121_UTCID07_Validate_ValidPasswordEnterprise_ReturnsIsValidTrue()
    {
        var sut = Build();
        var result = sut.Validate("Enterprise1!Pass", "Enterprise");   // 16 chars
        result.IsValid.Should().BeTrue();
        result.FailedRuleMessages.Should().BeEmpty();
    }

    // ── UTCID08 ───────────────────────────────────────────────────────────
    [Fact]
    public async Task CVerify121_UTCID08_Validate_TooShortForEnterprise_ReturnsInvalidAndThrows()
    {
        var sut = Build();
        // 11 chars — Enterprise requires 12
        var result = sut.Validate("Secure1!Pas", "Enterprise");
        result.IsValid.Should().BeFalse();
        result.FailedRuleMessages.Should().ContainMatch("*at least 12*");

        await FluentActions.Invoking(() => sut.ValidateAndThrowAsync("Secure1!Pas", "Enterprise"))
            .Should().ThrowAsync<PasswordPolicyViolationException>();
    }

    // ── UTCID09 ───────────────────────────────────────────────────────────
    [Fact]
    public void CVerify121_UTCID09_Validate_UnknownPolicyId_FallsBackToDefaultAndPasses()
    {
        var sut = Build();
        var result = sut.Validate("Secure1!Pass", "NonExistent");
        result.IsValid.Should().BeTrue();
    }

    // ── UTCID10 ───────────────────────────────────────────────────────────
    [Fact]
    public void CVerify121_UTCID10_Validate_ExactlyMinLengthDefault_ReturnsIsValidTrue()
    {
        var sut = Build();
        var result = sut.Validate("Secure1!", "Default");   // exactly 8 chars
        result.IsValid.Should().BeTrue();
        result.FailedRuleMessages.Should().BeEmpty();
    }

    // ── UTCID11 ───────────────────────────────────────────────────────────
    [Fact]
    public void CVerify121_UTCID11_Validate_NullPassword_ReturnsInvalidWithRequiredMessage()
    {
        var sut = Build();
        var result = sut.Validate(null!, "Default");
        result.IsValid.Should().BeFalse();
        result.FailedRuleMessages.Should().ContainMatch("*required*");
    }

    // ── UTCID12 ───────────────────────────────────────────────────────────
    [Fact]
    public void CVerify121_UTCID12_Validate_EmptyPassword_ReturnsInvalidWithRequiredMessage()
    {
        var sut = Build();
        var result = sut.Validate("", "Default");
        result.IsValid.Should().BeFalse();
        result.FailedRuleMessages.Should().ContainMatch("*required*");
    }
}
