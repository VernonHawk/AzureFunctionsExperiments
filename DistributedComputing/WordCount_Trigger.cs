using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.DurableTask;
using Microsoft.Azure.WebJobs.Host;
using Microsoft.Extensions.Logging;

namespace DistributedComputing
{
    public static class WordCountTrigger
    {
        [FunctionName(nameof(WordCountTrigger))]
        public static async Task WordCount_Trigger(
            [BlobTrigger("word-count/{name}")] string file,
            string name,
            [DurableClient] IDurableOrchestrationClient starter,
            ILogger log
        )
        {
            var instanceId = await starter.StartNewAsync(
                orchestratorFunctionName: nameof(WordCount),
                input: new WordCountInput {Name = name, Content = file}
            );

            log.LogInformation($"Started orchestration with ID = {instanceId}.");
        }
    }
}