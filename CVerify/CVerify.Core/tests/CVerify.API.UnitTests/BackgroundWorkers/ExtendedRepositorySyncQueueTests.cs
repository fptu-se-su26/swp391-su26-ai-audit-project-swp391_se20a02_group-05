using System;
using Xunit;

namespace CVerify.API.UnitTests.BackgroundWorkers
{
    public class ExtendedRepositorySyncQueueTests
    {
        [Fact]
        public void TestEnqueueSyncJob_AcquiresRedisQueueId()
        {
            var queueId = Guid.NewGuid();
        }
    }
}
