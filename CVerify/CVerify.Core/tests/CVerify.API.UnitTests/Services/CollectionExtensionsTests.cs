using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Xunit;
using FluentAssertions;
using CVerify.API.Modules.Shared.System.Extensions;

namespace CVerify.API.UnitTests.Services
{
    public class CollectionExtensionsTests
    {
        [Fact]
        public void Partition_NullSource_ShouldThrowArgumentNullException()
        {
            // Arrange
            IEnumerable<int>? source = null;

            // Act
            Action action = () => source!.Partition(2).ToList();

            // Assert
            action.Should().Throw<ArgumentNullException>();
        }

        [Theory]
        [InlineData(0)]
        [InlineData(-1)]
        public void Partition_InvalidSize_ShouldThrowArgumentException(int size)
        {
            // Arrange
            var source = new List<int> { 1, 2, 3 };

            // Act
            Action action = () => source.Partition(size).ToList();

            // Assert
            action.Should().Throw<ArgumentException>();
        }

        [Fact]
        public void Partition_ValidCollection_ShouldChunkCorrectly()
        {
            // Arrange
            var source = Enumerable.Range(1, 10).ToList();

            // Act
            var chunks = source.Partition(3).Select(c => c.ToList()).ToList();

            // Assert
            chunks.Should().HaveCount(4);
            chunks[0].Should().Equal(1, 2, 3);
            chunks[1].Should().Equal(4, 5, 6);
            chunks[2].Should().Equal(7, 8, 9);
            chunks[3].Should().Equal(10);
        }

        [Fact]
        public void SafeMerge_NullSecondary_ShouldReturnCopyOfPrimary()
        {
            // Arrange
            var primary = new Dictionary<string, string> { { "A", "1" } };

            // Act
            var merged = primary.SafeMerge(null);

            // Assert
            merged.Should().HaveCount(1);
            merged["A"].Should().Be("1");
        }

        [Fact]
        public void SafeMerge_DuplicateKeys_ShouldOverwriteWithSecondaryByDefault()
        {
            // Arrange
            var primary = new Dictionary<string, string> { { "A", "1" }, { "B", "2" } };
            var secondary = new Dictionary<string, string> { { "B", "20" }, { "C", "3" } };

            // Act
            var merged = primary.SafeMerge(secondary);

            // Assert
            merged.Should().HaveCount(3);
            merged["A"].Should().Be("1");
            merged["B"].Should().Be("20");
            merged["C"].Should().Be("3");
        }

        [Fact]
        public void SafeMerge_DuplicateKeysWithResolver_ShouldResolveConflict()
        {
            // Arrange
            var primary = new Dictionary<string, int> { { "A", 1 }, { "B", 2 } };
            var secondary = new Dictionary<string, int> { { "B", 10 }, { "C", 3 } };

            // Act
            var merged = primary.SafeMerge(secondary, (pVal, sVal) => pVal + sVal);

            // Assert
            merged.Should().HaveCount(3);
            merged["A"].Should().Be(1);
            merged["B"].Should().Be(12); // 2 + 10 = 12
            merged["C"].Should().Be(3);
        }

        [Fact]
        public async Task ParallelMapAsync_ShouldMapCorrectly()
        {
            // Arrange
            var source = Enumerable.Range(1, 5).ToList();

            // Act
            var result = await source.ParallelMapAsync(async x =>
            {
                await Task.Delay(10);
                return x * 2;
            }, maxConcurrency: 2);

            // Assert
            result.Should().Equal(2, 4, 6, 8, 10);
        }

        [Fact]
        public async Task ParallelMapAsync_ConcurrencyLimiting_ShouldLimitActiveTasks()
        {
            // Arrange
            var source = Enumerable.Range(1, 10).ToList();
            int activeTasks = 0;
            int maxObservedConcurrency = 0;
            var lockObj = new object();

            // Act
            await source.ParallelMapAsync(async x =>
            {
                int currentActive = Interlocked.Increment(ref activeTasks);
                lock (lockObj)
                {
                    if (currentActive > maxObservedConcurrency)
                    {
                        maxObservedConcurrency = currentActive;
                    }
                }

                await Task.Delay(15);
                Interlocked.Decrement(ref activeTasks);
                return x;
            }, maxConcurrency: 3);

            // Assert
            maxObservedConcurrency.Should().BeLessThanOrEqualTo(3);
        }
    }
}
