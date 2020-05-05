using System.Threading.Tasks;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.DurableTask;
using Microsoft.Extensions.Logging;

namespace DistributedComputing
{
    public class WordCountOutput
    {
        [FunctionName(nameof(WordCountOutput))]
        public static async Task<string> Run([ActivityTrigger] string name, ILogger log)
        {
            log.LogInformation($"Saying hello to {name}.");
        
            return $"Hello {name}!";
        }
    }
}