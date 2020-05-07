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
            [ActivityTrigger] WordCountOutputArgs args,
            [Blob("word-count/output/word-count.txt", FileAccess.Write)]
            TextWriter output
        )
        {
            var wordCount = string.Join(
                separator: '\n',
                values: args.Results.Select(res => $"{res.Key}:{res.Value}")
            );

            await output.WriteAsync(wordCount);
        }
    }

    public class WordCountOutputArgs
    {
        public string FileName { get; set; }
        public Result<string, int>[] Results { get; set; }

        public WordCountOutputArgs(string fileName, Result<string, int>[] results) =>
            (FileName, Results) = (fileName, results);
    }
}