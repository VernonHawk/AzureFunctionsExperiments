using System;

namespace DistributedComputing.MapReduce
{
    public static class Batching
    {
        public static BatchParams GetBatchParams(int dataSize)
        {
            var batchSizeRaw = Math.Sqrt(dataSize);

            return new BatchParams(
                size: (int) Math.Floor(batchSizeRaw),
                amount: (int) Math.Ceiling(batchSizeRaw)
            );
        }
    }

    public class BatchParams
    {
        public int BatchSize { get; }
        public int BatchesAmount { get; }

        public BatchParams(int size, int amount) => (BatchSize, BatchesAmount) = (size, amount);
    }
}