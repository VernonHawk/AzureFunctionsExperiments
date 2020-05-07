using System.Threading.Tasks;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.DurableTask;
namespace DistributedComputing
{
    public static class WordCountTrigger
    {
        [FunctionName(nameof(WordCountTrigger))]
        public static async Task WordCount_Trigger(
            [BlobTrigger("word-count/input/{name}")]
            string file,
            string name,
            [DurableClient] IDurableOrchestrationClient starter
        ) =>
            await starter.StartNewAsync(
                orchestratorFunctionName: nameof(WordCount),
                input: new WordCountInput {Name = name, Content = file}
            );
    }
}