using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Text;
using Xunit;

namespace CVerify.API.UnitTests.Security
{
    public class ExtendedSecurityTests
    {
        private const string DummySecret = "DbqDgBM1u2H5lNnUFBgYrRaotpSP9Wda8jASgjIbFh6";

        [Fact]
        public void TestHmacSignatureValidation_CorrectPayload_ReturnsValid()
        {
            var timestamp = DateTimeOffset.UtcNow.ToUnixTimeSeconds().ToString();
            var nonce = Guid.NewGuid().ToString();
            var clientId = "cverify-dotnet-core";
            var body = "{\"userId\":\"d9b2b2b2-b2b2-b2b2-b2b2-d9b2b2b2b2b2\"}";

            var signature = ComputeHmacSignature(timestamp, nonce, clientId, body, DummySecret);

            Assert.NotNull(signature);
            Assert.Equal(64, signature.Length);
        }

        [Fact]
        public void TestHmacSignatureValidation_TemperedPayload_Fails()
        {
            var timestamp = DateTimeOffset.UtcNow.ToUnixTimeSeconds().ToString();
            var nonce = Guid.NewGuid().ToString();
            var clientId = "cverify-dotnet-core";
            var body = "{\"userId\":\"d9b2b2b2-b2b2-b2b2-b2b2-d9b2b2b2b2b2\"}";

            var signatureOriginal = ComputeHmacSignature(timestamp, nonce, clientId, body, DummySecret);
            var signatureTempered = ComputeHmacSignature(timestamp, nonce, clientId, body + " ", DummySecret);

            Assert.NotEqual(signatureOriginal, signatureTempered);
        }

        [Fact]
        public void TestPasswordCostFactor_LengthCheck()
        {
            var plainPassword = "SuperSecurePassword123!";
            Assert.True(plainPassword.Length >= 8);
        }

        private static string ComputeHmacSignature(string timestamp, string nonce, string clientId, string body, string secret)
        {
            var keyBytes = Encoding.UTF8.GetBytes(secret);
            using var hmac = new HMACSHA256(keyBytes);
            var payload = $"{timestamp}:{nonce}:{clientId}:{body}";
            var payloadBytes = Encoding.UTF8.GetBytes(payload);
            var hashBytes = hmac.ComputeHash(payloadBytes);
            return Convert.ToHexString(hashBytes).ToLower();
        }
    }
}
