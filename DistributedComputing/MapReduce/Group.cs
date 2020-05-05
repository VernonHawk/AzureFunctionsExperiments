using System.Collections.Generic;
using System.Linq;

namespace DistributedComputing.MapReduce
{
    public static class Grouper
    {
        public static IList<Group<KeyT, ValueT>> GroupResults<KeyT, ValueT>(
            IEnumerable<IList<Result<KeyT, ValueT>>> results
        ) =>
            results.SelectMany(r => r)
                   .GroupBy(r => r.Key)
                   .Select(
                       group => new Group<KeyT, ValueT>(
                           key: group.Key,
                           values: group.Select(r => r.Value).ToList()
                       )
                   )
                   .ToList();
    }

    public class Group<KeyT, ValueT>
    {
        public KeyT Key { get; set; }
        public IList<ValueT> Values { get; set; }

        public Group(KeyT key, IList<ValueT> values) => (Key, Values) = (key, values);
    }
}