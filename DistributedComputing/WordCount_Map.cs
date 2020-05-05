using System.Collections.Generic;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.DurableTask;
using Microsoft.Extensions.Logging;

namespace DistributedComputing
{
    public class WordCountMap
    {
        [FunctionName(nameof(WordCountMap))]
        public static IList<string> Run([ActivityTrigger] IList<string> lines, ILogger log)
        {
            log.LogInformation($"Got {lines.Count} lines.");

            return lines;
        }
    }
}