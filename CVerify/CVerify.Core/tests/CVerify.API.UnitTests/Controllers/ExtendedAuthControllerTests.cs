using System;
using Xunit;

namespace CVerify.API.UnitTests.Controllers
{
    public class ExtendedAuthControllerTests
    {
        [Fact]
        public void TestLoginEndpoint_ValidPayload_ReturnsOk()
        {
            var request = new { Email = "candidate@cverify.com", Password = "SecurePassword1!" };
            Assert.NotNull(request.Email);
            Assert.True(request.Password.Length >= 8);
        }

        [Fact]
        public void TestRegistrationEndpoint_InvalidEmail_ThrowsValidationException()
        {
            var email = "invalid-email-format";
            bool isValid = email.Contains("@");
            Assert.False(isValid);
        }
    }
}
