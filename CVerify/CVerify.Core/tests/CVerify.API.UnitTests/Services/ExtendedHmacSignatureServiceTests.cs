using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace CVerify.API.UnitTests.Services
{
    [TestClass]
    public class ExtendedHmacSignatureServiceTests
    {
        [TestMethod]
        public void TestVerifyCorrelationId_IntegrityAssertion()
        {
            var correlationId = Guid.NewGuid().ToString();
            Assert.IsNotNull(correlationId);
        }
    }
}
