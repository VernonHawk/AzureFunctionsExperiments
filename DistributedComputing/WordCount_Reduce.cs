using System.Threading.Tasks;
using DistributedComputing.MapReduce;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.DurableTask;

namespace DistributedComputing
{
    public static class WordCountReduce
    {
        [FunctionName(nameof(WordCountReduce))]
        public static async Task<string> WordCount_Reduce(
            [ActivityTrigger] Group<string, int> group
        )
        {
            return $"Hello !";
        }
    }
}