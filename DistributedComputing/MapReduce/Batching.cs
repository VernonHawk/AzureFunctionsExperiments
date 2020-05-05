using System;
using System.Collections.Generic;
using System.Linq;

namespace DistributedComputing.MapReduce
{
    public static class Batching
    {
        public static IEnumerable<List<T>> ToBatches<T>(IList<T> data)
        {
            var batchParams = GetBatchParams(data.Count);

            return Enumerable.Range(start: 0, count: batchParams.BatchesAmount)
                             .Select(
                                 batchNum => data
                                             .Skip(batchNum * batchParams.BatchSize)
                                             .Take(batchParams.BatchSize)
                                             .ToList()
                             );
        }

        private static BatchParams GetBatchParams(int dataSize)
        {
            var batchSizeRaw = Math.Sqrt(dataSize);

            return new BatchParams(
                size: (int) Math.Floor(batchSizeRaw),
                amount: (int) Math.Ceiling(batchSizeRaw)
            );
        }

        private class BatchParams
        {
            public int BatchSize { get; }
            public int BatchesAmount { get; }

            public BatchParams(int size, int amount) => (BatchSize, BatchesAmount) = (size, amount);
        }
    }
}