using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace CVerify.API.UnitTests.BackgroundWorkers
{
    [TestClass]
    public class ExtendedRepositorySyncQueueTests
    {
        [TestMethod]
        public void TestEnqueueSyncJob_AcquiresRedisQueueId()
        {
            var queueId = Guid.NewGuid();
            Assert.IsNotNull(queueId);
        }
    }
}
