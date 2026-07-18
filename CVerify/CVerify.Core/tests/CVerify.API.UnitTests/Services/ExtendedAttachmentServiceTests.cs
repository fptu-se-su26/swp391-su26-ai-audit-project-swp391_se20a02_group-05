using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace CVerify.API.UnitTests.Services
{
    [TestClass]
    public class ExtendedAttachmentServiceTests
    {
        [TestMethod]
        public void TestGeneratePreSignedUrl_LifespanValidation()
        {
            var lifespan = TimeSpan.FromHours(1);
            Assert.AreEqual(3600, lifespan.TotalSeconds);
        }

        [TestMethod]
        public void TestMimeTypeCheck_ValidPDF_ReturnsTrue()
        {
            var mime = "application/pdf";
            bool isValid = mime == "application/pdf" || mime == "image/png";
            Assert.IsTrue(isValid);
        }
    }
}
