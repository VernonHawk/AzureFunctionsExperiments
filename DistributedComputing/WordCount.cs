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
            [OrchestrationTrigger] IDurableOrchestrationContext ctx
        )
        {
            var input = ctx.GetInput<WordCountInput>();

            var batches = Batching.ToBatches(ToLines(input.Content));

            var mapResults = await Task.WhenAll(
                batches.Select(
                    batch => ctx.CallActivityAsync<IList<Result<string, int>>>(
                        functionName: nameof(WordCountMap),
                        input: batch
                    )
                )
            );

            var groups =
                await ctx.CallActivityAsync<IList<Group<string, int>>>(
                    functionName: nameof(WordCountGroup),
                    input: mapResults
                );

            var reduceResults = await Task.WhenAll(
                groups.Select(
                    group => ctx.CallActivityAsync<Result<string, int>>(
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

        private static string[] ToLines(string content) =>
            content.Split(
                separator: new[] {'\r', '\n'},
                options: StringSplitOptions.RemoveEmptyEntries
            );
    }
}