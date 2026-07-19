using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace CVerify.API.Modules.Shared.System.Extensions
{
    /// <summary>
    /// Provides utility extension methods for collection manipulation, safe merging, and concurrent mapping operations.
    /// </summary>
    public static class CollectionExtensions
    {
        /// <summary>
        /// Splits the elements of a sequence into chunks of size at most <paramref name="size"/>.
        /// </summary>
        /// <typeparam name="T">The type of the elements of source.</typeparam>
        /// <param name="source">An IEnumerable whose elements to chunk.</param>
        /// <param name="size">The maximum size of each chunk.</param>
        /// <returns>An IEnumerable of IEnumerable that contains the elements of the input sequence split into chunks.</returns>
        public static IEnumerable<IEnumerable<T>> Partition<T>(this IEnumerable<T> source, int size)
        {
            if (source == null)
            {
                throw new ArgumentNullException(nameof(source));
            }

            if (size <= 0)
            {
                throw new ArgumentException("Chunk size must be greater than zero.", nameof(size));
            }

            return PartitionIterator(source, size);
        }

        private static IEnumerable<IEnumerable<T>> PartitionIterator<T>(IEnumerable<T> source, int size)
        {
            using (var enumerator = source.GetEnumerator())
            {
                while (enumerator.MoveNext())
                {
                    yield return GetChunk(enumerator, size);
                }
            }
        }

        private static IEnumerable<T> GetChunk<T>(IEnumerator<T> enumerator, int size)
        {
            var chunk = new List<T>(size) { enumerator.Current };
            for (int i = 1; i < size && enumerator.MoveNext(); i++)
            {
                chunk.Add(enumerator.Current);
            }
            return chunk;
        }

        /// <summary>
        /// Merges a secondary dictionary into a primary dictionary. In case of duplicate keys,
        /// a custom resolver function can decide which value to keep.
        /// </summary>
        /// <typeparam name="TKey">The type of keys in the dictionary.</typeparam>
        /// <typeparam name="TValue">The type of values in the dictionary.</typeparam>
        /// <param name="primary">The main dictionary receiving values.</param>
        /// <param name="secondary">The dictionary to merge into the primary.</param>
        /// <param name="resolveConflict">A callback to resolve values when keys duplicate. If null, secondary value overwrites primary value.</param>
        /// <returns>A new dictionary containing the merged key-value pairs.</returns>
        public static IDictionary<TKey, TValue> SafeMerge<TKey, TValue>(
            this IDictionary<TKey, TValue> primary,
            IDictionary<TKey, TValue> secondary,
            Func<TValue, TValue, TValue> resolveConflict = null)
        {
            if (primary == null) throw new ArgumentNullException(nameof(primary));
            if (secondary == null) return primary.ToDictionary(k => k.Key, v => v.Value);

            var result = primary.ToDictionary(k => k.Key, v => v.Value);

            foreach (var kvp in secondary)
            {
                if (result.TryGetValue(kvp.Key, out var existingValue))
                {
                    result[kvp.Key] = resolveConflict != null 
                        ? resolveConflict(existingValue, kvp.Value) 
                        : kvp.Value;
                }
                else
                {
                    result[kvp.Key] = kvp.Value;
                }
            }

            return result;
        }

        /// <summary>
        /// Maps a collection asynchronously using a maximum concurrency limit.
        /// Useful for throttling third-party API queries or heavy tasks.
        /// </summary>
        /// <typeparam name="TSource">The type of source elements.</typeparam>
        /// <typeparam name="TResult">The type of result elements.</typeparam>
        /// <param name="source">The collection to process.</param>
        /// <param name="mapper">The async mapping function.</param>
        /// <param name="maxConcurrency">The maximum concurrent tasks allowed.</param>
        /// <param name="cancellationToken">An optional cancellation token.</param>
        /// <returns>An array containing the mapped results.</returns>
        public static async Task<TResult[]> ParallelMapAsync<TSource, TResult>(
            this IEnumerable<TSource> source,
            Func<TSource, Task<TResult>> mapper,
            int maxConcurrency,
            CancellationToken cancellationToken = default)
        {
            if (source == null) throw new ArgumentNullException(nameof(source));
            if (mapper == null) throw new ArgumentNullException(nameof(mapper));
            if (maxConcurrency <= 0) throw new ArgumentException("Concurrency limit must be greater than zero.", nameof(maxConcurrency));

            using (var semaphore = new SemaphoreSlim(maxConcurrency))
            {
                var tasks = source.Select(async item =>
                {
                    await semaphore.WaitAsync(cancellationToken).ConfigureAwait(false);
                    try
                    {
                        return await mapper(item).ConfigureAwait(false);
                    }
                    finally
                    {
                        semaphore.Release();
                    }
                });

                return await Task.WhenAll(tasks).ConfigureAwait(false);
            }
        }
    }
}
