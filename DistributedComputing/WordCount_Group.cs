using System.Collections.Generic;
using System.Linq;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.DurableTask;

namespace DistributedComputing
{
    public class WordCountGroup
    {
        [FunctionName(nameof(WordCountGroup))]
        public static IList<Group<string, int>> WordCount_Group(
            [ActivityTrigger] IList<MapResult<string, int>>[] mapResults
        ) =>
            GroupMapResults(mapResults);

        private static IList<Group<KeyT, ValueT>> GroupMapResults<KeyT, ValueT>(
            IList<MapResult<KeyT, ValueT>>[] mapResults
        ) =>
            mapResults.SelectMany(r => r)
                      .GroupBy(r => r.Key)
                      .Select(
                          group => new Group<KeyT, ValueT>(
                              group.Key,
                              group.Select(r => r.Value).ToList()
                          )
                      )
                      .ToList();

        public class Group<KeyT, ValueT>
        {
            public KeyT Key { get; set; }
            public IList<ValueT> Values { get; set; }

            public Group(KeyT key, IList<ValueT> values) => (Key, Values) = (key, values);
        }
    }
}