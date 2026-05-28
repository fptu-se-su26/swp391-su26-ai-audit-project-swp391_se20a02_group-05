using CVerify.API.Application.Security.PasswordPolicies;
using FluentAssertions;
using Microsoft.Extensions.Configuration;
using Xunit;

namespace CVerify.API.UnitTests.Security;

/// <summary>White-box unit tests for password policy validation branches.</summary>
public class PasswordPolicyServiceTests
{
    private static PasswordPolicyService CreateService() =>
        new(new ConfigurationBuilder()
            .AddInMemoryCollection(new Dictionary<string, string?>())
            .Build());

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    public void Validate_EmptyPassword_IsInvalid(string? password)
    {
        var result = CreateService().Validate(password!, "Default");
        result.IsValid.Should().BeFalse();
        result.FailedRuleMessages.Should().Contain(m => m.Contains("required", StringComparison.OrdinalIgnoreCase));
    }

    [Fact]
    public void Validate_WhitespaceOnly_FailsValidation()
    {
        var result = CreateService().Validate("   ", "Default");
        result.IsValid.Should().BeFalse();
        result.FailedRuleMessages.Should().NotBeEmpty();
    }

    [Fact]
    public void Validate_DefaultPolicy_ValidPassword_Passes()
    {
        var result = CreateService().Validate("Secure1!aa", "Default");
        result.IsValid.Should().BeTrue();
        result.FailedRuleMessages.Should().BeEmpty();
    }

    [Theory]
    [InlineData("Ab1!xyz")] // 7 chars
    [InlineData("noupper1!")]
    [InlineData("NOLOWER1!")]
    [InlineData("NoDigits!!")]
    [InlineData("NoSpecial1")]
    public void Validate_DefaultPolicy_InvalidVariants_Fail(string password)
    {
        var result = CreateService().Validate(password, "Default");
        result.IsValid.Should().BeFalse();
        result.FailedRuleMessages.Should().NotBeEmpty();
    }

    [Fact]
    public void Validate_EnterprisePolicy_RequiresTwelveCharacters()
    {
        var svc = CreateService();
        svc.Validate("Secure1!aaa", "Enterprise").IsValid.Should().BeFalse();
        svc.Validate("Secure1!aaaaa", "Enterprise").IsValid.Should().BeTrue();
    }

    [Fact]
    public void Validate_UnknownPolicyId_FallsBackToDefault()
    {
        var result = CreateService().Validate("Secure1!aa", "UnknownPolicy");
        result.IsValid.Should().BeTrue();
    }
}
