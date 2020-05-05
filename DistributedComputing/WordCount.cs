using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DistributedComputing.MapReduce;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.DurableTask;
using Microsoft.Extensions.Logging;

namespace DistributedComputing
{
    public static class WordCount
    {
        [FunctionName(nameof(WordCount))]
        public static async Task<List<string>> WordCountOrchestrator(
            [OrchestrationTrigger] IDurableOrchestrationContext context,
            ILogger log
        )
        {
            var input = context.GetInput<WordCountInput>();

            var lines = input.Content.Split(
                separator: new[] {'\r', '\n'},
                options: StringSplitOptions.RemoveEmptyEntries
            );

            var batchParams = GetBatchParams(lines.Length);

            var mapResults = await Task.WhenAll(
                Enumerable.Range(start: 0, count: batchParams.BatchesAmount)
                          .Select(
                              batchNum =>
                                  context.CallActivityAsync<IList<Result<string, int>>>(
                                      functionName: nameof(WordCountMap),
                                      input: lines
                                             .Skip(batchNum * batchParams.BatchSize)
                                             .Take(batchParams.BatchSize)
                                             .ToList()
                                  )
                          )
            );

            var groups =
                await context.CallActivityAsync<IList<Group<string, int>>>(
                    functionName: nameof(WordCountGroup),
                    input: mapResults
                );

            var reduceResults = await Task.WhenAll(
                groups.Select(
                    group => context.CallActivityAsync<Result<string, int>>(
                        functionName: nameof(WordCountReduce),
                        input: group
                    )
                )
            );

            foreach (var res in reduceResults.Take(100))
            {
                Console.WriteLine(res);
            }

            return new List<string>();
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