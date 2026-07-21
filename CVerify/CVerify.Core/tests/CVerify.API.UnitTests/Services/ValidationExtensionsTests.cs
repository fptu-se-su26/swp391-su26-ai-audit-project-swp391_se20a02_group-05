using System;
using Xunit;
using FluentAssertions;
using CVerify.API.Modules.Shared.System.Extensions;

namespace CVerify.API.UnitTests.Services
{
    public class ValidationExtensionsTests
    {
        [Theory]
        [InlineData(null, 0.0)]
        [InlineData("", 0.0)]
        [InlineData("12345", 16.6)] // 5 * log2(10) ~ 5 * 3.32 = 16.6
        [InlineData("abcdefgh", 37.6)] // 8 * log2(26) ~ 8 * 4.7 = 37.6
        [InlineData("Ab1!", 26.2)] // 4 * log2(26+26+10+33) = 4 * log2(95) ~ 4 * 6.57 = 26.28
        public void CalculatePasswordEntropy_ShouldCalculateExpectedEntropy(string? password, double minimumExpected)
        {
            // Act
            double entropy = password.CalculatePasswordEntropy();

            // Assert
            entropy.Should().BeGreaterThanOrEqualTo(minimumExpected);
        }

        [Theory]
        [InlineData(null, false)]
        [InlineData("", false)]
        [InlineData("1234567", false)] // Too short
        [InlineData("12345678", false)] // Length 8 but low entropy (only digits)
        [InlineData("Abcd1234!", true)] // Strong enough
        [InlineData("superSecureP@ssword123", true)] // Strong enough
        public void IsSecurePassword_ShouldValidateCorrectly(string? password, bool expectedResult)
        {
            // Act
            bool result = password.IsSecurePassword();

            // Assert
            result.Should().Be(expectedResult);
        }

        [Theory]
        [InlineData("test@domain.com", true)]
        [InlineData("user.name+tag@sub.domain.co.uk", true)]
        [InlineData("user@[123.123.123.123]", true)]
        [InlineData("user@123.123.123.123", false)]
        [InlineData("plainaddress", false)]
        [InlineData("@missinglocal.com", false)]
        [InlineData("missingdomain@.com", false)]
        [InlineData("two@@domains.com", false)]
        [InlineData("user@domain..com", false)]
        public void IsValidEmailSyntax_ShouldValidateCorrectly(string? email, bool expectedResult)
        {
            // Act
            bool result = email.IsValidEmailSyntax();

            // Assert
            result.Should().Be(expectedResult);
        }

        [Theory]
        [InlineData("https://github.com/octocat/Hello-World", true, "octocat", "Hello-World")]
        [InlineData("http://github.com/octocat/Hello-World.git", true, "octocat", "Hello-World")]
        [InlineData("git@github.com:octocat/Hello-World.git", true, "octocat", "Hello-World")]
        [InlineData("github.com:octocat/Hello-World", true, "octocat", "Hello-World")]
        [InlineData("https://github.com/octocat/Hello-World/tree/master", true, "octocat", "Hello-World")]
        [InlineData("https://gitlab.com/octocat/Hello-World", false, "", "")]
        [InlineData("invalid-url", false, "", "")]
        [InlineData(null, false, "", "")]
        public void TryParseGitHubUrl_ShouldParseAndValidate(string? url, bool expectedSuccess, string expectedOwner, string expectedRepo)
        {
            // Act
            bool success = url.TryParseGitHubUrl(out string owner, out string repo);

            // Assert
            success.Should().Be(expectedSuccess);
            if (expectedSuccess)
            {
                owner.Should().Be(expectedOwner);
                repo.Should().Be(expectedRepo);
            }
        }
    }
}
