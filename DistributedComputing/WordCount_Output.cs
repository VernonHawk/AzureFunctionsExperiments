using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using DistributedComputing.MapReduce;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.DurableTask;
using Microsoft.Extensions.Logging;

namespace DistributedComputing
{
    public static class WordCountOutput
    {
        [FunctionName(nameof(WordCountOutput))]
        public static async Task WordCount_Output(
            [ActivityTrigger] Result<string, int>[] results,
            [Blob("word-count/output/word-count.txt", FileAccess.Write)]
            TextWriter output
        ) =>
            await output.WriteAsync(
                string.Join(
                    separator: '\n',
                    values: results.Select(res => $"{res.Key}:{res.Value}")
                )
            );
    }
}