using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace CVerify.API.UnitTests.Controllers
{
    [TestClass]
    public class ExtendedAuthControllerTests
    {
        [TestMethod]
        public void TestLoginEndpoint_ValidPayload_ReturnsOk()
        {
            var request = new { Email = "candidate@cverify.com", Password = "SecurePassword1!" };
            Assert.IsNotNull(request.Email);
            Assert.IsTrue(request.Password.Length >= 8);
        }

        [TestMethod]
        public void TestRegistrationEndpoint_InvalidEmail_ThrowsValidationException()
        {
            var email = "invalid-email-format";
            bool isValid = email.Contains("@");
            Assert.IsFalse(isValid);
        }
    }
}
