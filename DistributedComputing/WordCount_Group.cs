using System.Collections.Generic;
using DistributedComputing.MapReduce;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.DurableTask;

namespace DistributedComputing
{
    public static class WordCountGroup
    {
        [FunctionName(nameof(WordCountGroup))]
        public static IList<Group<string, int>> WordCount_Group(
            [ActivityTrigger] IList<Result<string, int>>[] mapResults
        ) =>
            Grouping.GroupResults(mapResults);
    }
}