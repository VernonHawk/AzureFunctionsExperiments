using System.Linq;
using DistributedComputing.MapReduce;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.DurableTask;

namespace DistributedComputing
{
    public static class WordCountReduce
    {
        [FunctionName(nameof(WordCountReduce))]
        public static Result<string, int> WordCount_Reduce(
            [ActivityTrigger] Group<string, int> group
        ) =>
            new Result<string, int>(key: group.Key, value: group.Values.Sum());
    }
}