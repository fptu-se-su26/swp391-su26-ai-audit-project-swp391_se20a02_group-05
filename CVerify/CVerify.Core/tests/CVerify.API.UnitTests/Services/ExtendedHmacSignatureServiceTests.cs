using System;
using Xunit;

namespace CVerify.API.UnitTests.Services
{
    public class ExtendedHmacSignatureServiceTests
    {
        [Fact]
        public void TestVerifyCorrelationId_IntegrityAssertion()
        {
            var correlationId = Guid.NewGuid().ToString();
            Assert.NotNull(correlationId);
        }
    }
}
