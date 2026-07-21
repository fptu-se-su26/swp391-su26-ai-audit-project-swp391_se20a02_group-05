using System;
using System.Text;
using Xunit;
using FluentAssertions;
using CVerify.API.Modules.Shared.Security;

namespace CVerify.API.UnitTests.Security
{
    public class AdvancedCryptoHelperTests
    {
        [Fact]
        public void GenerateSecureSalt_DefaultSize_ShouldReturnNonEmptyBase64String()
        {
            // Act
            string salt = AdvancedCryptoHelper.GenerateSecureSalt();

            // Assert
            salt.Should().NotBeNullOrWhiteSpace();
            Action act = () => Convert.FromBase64String(salt);
            act.Should().NotThrow("salt should be a valid Base64 string");
        }

        [Theory]
        [InlineData(16)]
        [InlineData(32)]
        [InlineData(64)]
        public void GenerateSecureSalt_CustomSize_ShouldCreateExpectedLength(int size)
        {
            // Act
            string salt = AdvancedCryptoHelper.GenerateSecureSalt(size);
            byte[] saltBytes = Convert.FromBase64String(salt);

            // Assert
            saltBytes.Should().HaveCount(size);
        }

        [Fact]
        public void GenerateSecureSalt_NegativeSize_ShouldThrowArgumentException()
        {
            // Act
            Action act = () => AdvancedCryptoHelper.GenerateSecureSalt(-1);

            // Assert
            act.Should().Throw<ArgumentException>();
        }

        [Fact]
        public void GenerateSecureSalt_MultipleCalls_ShouldReturnUniqueValues()
        {
            // Arrange & Act
            string salt1 = AdvancedCryptoHelper.GenerateSecureSalt();
            string salt2 = AdvancedCryptoHelper.GenerateSecureSalt();

            // Assert
            salt1.Should().NotBe(salt2, "cryptographically secure salts must be unique");
        }

        [Fact]
        public void ComputeSha256Hash_ValidInputs_ShouldReturnHexHash()
        {
            // Arrange
            string text = "Hello World";
            string salt = AdvancedCryptoHelper.GenerateSecureSalt();

            // Act
            string hash = AdvancedCryptoHelper.ComputeSha256Hash(text, salt);

            // Assert
            hash.Should().HaveLength(64); // 256 bits = 32 bytes = 64 hex characters
            hash.Should().MatchRegex("^[0-9a-f]{64}$");
        }

        [Fact]
        public void ComputeSha512Hash_ValidInputs_ShouldReturnHexHash()
        {
            // Arrange
            string text = "Hello World";
            string salt = AdvancedCryptoHelper.GenerateSecureSalt();

            // Act
            string hash = AdvancedCryptoHelper.ComputeSha512Hash(text, salt);

            // Assert
            hash.Should().HaveLength(128); // 512 bits = 64 bytes = 128 hex characters
            hash.Should().MatchRegex("^[0-9a-f]{128}$");
        }

        [Fact]
        public void ComputeHash_NullInputs_ShouldThrowArgumentNullException()
        {
            // Act
            Action act1 = () => AdvancedCryptoHelper.ComputeSha256Hash(null!, "salt");
            Action act2 = () => AdvancedCryptoHelper.ComputeSha256Hash("text", null!);

            // Assert
            act1.Should().Throw<ArgumentNullException>();
            act2.Should().Throw<ArgumentNullException>();
        }

        [Fact]
        public void HMAC_ComputeAndVerify_ShouldBeValid()
        {
            // Arrange
            byte[] data = Encoding.UTF8.GetBytes("Critical Payload Data");
            byte[] key = Encoding.UTF8.GetBytes("secret_key_12345");
            byte[] wrongKey = Encoding.UTF8.GetBytes("secret_key_wrong");

            // Act
            byte[] signature = AdvancedCryptoHelper.ComputeHmacSha256(data, key);
            bool verifySuccess = AdvancedCryptoHelper.VerifyHmacSha256(data, key, signature);
            bool verifyWrongKey = AdvancedCryptoHelper.VerifyHmacSha256(data, wrongKey, signature);
            bool verifyWrongData = AdvancedCryptoHelper.VerifyHmacSha256(Encoding.UTF8.GetBytes("Modified data"), key, signature);

            // Assert
            verifySuccess.Should().BeTrue();
            verifyWrongKey.Should().BeFalse();
            verifyWrongData.Should().BeFalse();
        }

        [Fact]
        public void Base64Url_RoundTrip_ShouldRestoreOriginalBytes()
        {
            // Arrange
            byte[] original = Encoding.UTF8.GetBytes("Url-safe verification string? + / = characters.");

            // Act
            string encoded = AdvancedCryptoHelper.Base64UrlEncode(original);
            byte[] decoded = AdvancedCryptoHelper.Base64UrlDecode(encoded);

            // Assert
            encoded.Should().NotContain("+");
            encoded.Should().NotContain("/");
            encoded.Should().NotContain("=");
            decoded.Should().Equal(original);
        }
    }
}
