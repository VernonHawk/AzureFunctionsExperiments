using System;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.DurableTask;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Azure.WebJobs.Host;
using Microsoft.Extensions.Logging;

namespace DistributedComputing
{
    public static class WordCount
    {
        [FunctionName(nameof(WordCount))]
        public static async Task<List<string>> RunOrchestrator(
            [OrchestrationTrigger] IDurableOrchestrationContext context,
            ILogger log
        )
        {
            var input = context.GetInput<WordCountInput>();

            var lines = input.Content.Split(
                separator: new[] {'\r', '\n'},
                options: StringSplitOptions.RemoveEmptyEntries
            );

            var batchSizeRaw = Math.Sqrt(lines.Length);
            var batchSize = (int) Math.Floor(batchSizeRaw);

            var mapResults = await Task.WhenAll(
                Enumerable.Range(start: 0, count: (int) Math.Ceiling(batchSizeRaw))
                          .Select(
                              batchNum => context.CallActivityAsync<IList<MapResultEntry>>(
                                  functionName: nameof(WordCountMap),
                                  input: lines.Skip(batchNum * batchSize).Take(batchSize).ToList()
                              )
                          )
            );

            foreach (var e in mapResults[0])
            {
                Console.WriteLine(e);
            }


            return new List<string>();
        }
    }
}