using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DistributedComputing.MapReduce;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.DurableTask;

namespace DistributedComputing
{
    public static class WordCount
    {
        [FunctionName(nameof(WordCount))]
        public static async Task<List<string>> WordCountOrchestrator(
            [OrchestrationTrigger] IDurableOrchestrationContext context
        )
        {
            var input = context.GetInput<WordCountInput>();

            var lines = input.Content.Split(
                separator: new[] {'\r', '\n'},
                options: StringSplitOptions.RemoveEmptyEntries
            );

            var batchParams = Batching.GetBatchParams(lines.Length);

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
    }
}