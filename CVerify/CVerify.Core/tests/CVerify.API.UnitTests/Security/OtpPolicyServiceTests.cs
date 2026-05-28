using CVerify.API.Application.Exceptions;
using CVerify.API.Application.Security.OtpPolicies;
using FluentAssertions;
using Microsoft.Extensions.Configuration;
using Xunit;

namespace CVerify.API.UnitTests.Security;

/// <summary>White-box unit tests for OTP policy validation.</summary>
public class OtpPolicyServiceTests
{
    private static OtpPolicyService CreateService() =>
        new(new ConfigurationBuilder()
            .AddInMemoryCollection(new Dictionary<string, string?>())
            .Build());

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("12345")]
    [InlineData("1234567")]
    [InlineData("12ab56")]
    [InlineData("abcdef")]
    public void Validate_InvalidCodes_ReturnsFalse(string? code)
    {
        CreateService().Validate(code!).Should().BeFalse();
    }

    [Theory]
    [InlineData("123456")]
    [InlineData("000000")]
    [InlineData("999999")]
    public void Validate_ValidNumericCodes_ReturnsTrue(string code)
    {
        CreateService().Validate(code).Should().BeTrue();
    }

    [Fact]
    public void ValidateAndThrow_InvalidCode_ThrowsOtpPolicyViolation()
    {
        var act = () => CreateService().ValidateAndThrow("12ab", "Default");
        act.Should().Throw<OtpPolicyViolationException>();
    }

    [Fact]
    public void ValidateAndThrow_ValidCode_DoesNotThrow()
    {
        var act = () => CreateService().ValidateAndThrow("654321", "Default");
        act.Should().NotThrow();
    }
}
