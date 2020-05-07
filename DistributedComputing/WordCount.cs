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
        public static async Task WordCountOrchestrator(
            [OrchestrationTrigger] IDurableOrchestrationContext ctx,
            ILogger log
        )
        {
            var input = ctx.GetInput<WordCountInput>();
            log.LogInformation($"Word Count Orchestrator started with {input.Name}.");

            var batches = Batching.ToBatches(ToLines(input.Content));

            var mapResults = await Task.WhenAll(
                batches.Select(
                    batch => ctx.CallActivityAsync<IList<Result<string, int>>>(
                        functionName: nameof(WordCountMap),
                        input: batch
                    )
                )
            );

            log.LogInformation("After mapping");

            var groups = await ctx.CallActivityAsync<IList<Group<string, int>>>(
                functionName: nameof(WordCountGroup),
                input: mapResults
            );

            log.LogInformation("After grouping");

            var reduceResults = await Task.WhenAll(
                groups.Select(
                    group => ctx.CallActivityAsync<Result<string, int>>(
                        functionName: nameof(WordCountReduce),
                        input: group
                    )
                )
            );

            log.LogInformation("After reducing");

            await ctx.CallActivityAsync<string>(
                functionName: nameof(WordCountOutput),
                input: reduceResults
            );

            log.LogInformation("After outputting");
        }

        private static string[] ToLines(string content) =>
            content.Split(
                separator: new[] {'\r', '\n'},
                options: StringSplitOptions.RemoveEmptyEntries
            );
    }
}