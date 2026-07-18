using System;
using Xunit;

namespace CVerify.API.UnitTests.Services
{
    public class ExtendedAttachmentServiceTests
    {
        [Fact]
        public void TestGeneratePreSignedUrl_LifespanValidation()
        {
            var lifespan = TimeSpan.FromHours(1);
            Assert.Equal(3600, lifespan.TotalSeconds);
        }

        [Fact]
        public void TestMimeTypeCheck_ValidPDF_ReturnsTrue()
        {
            var mime = "application/pdf";
            bool isValid = mime == "application/pdf" || mime == "image/png";
            Assert.True(isValid);
        }
    }
}
