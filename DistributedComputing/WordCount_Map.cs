using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.DurableTask;

namespace DistributedComputing
{
    public class WordCountMap
    {
        [FunctionName(nameof(WordCountMap))]
        public static IList<MapResult<string, int>>
            WordCount_Map([ActivityTrigger] IList<string> lines) =>
            lines.SelectMany(ToTerms).Select(ToMapResult).ToList();

        private const string Separators =
            " ,?!.…:;—-+/*^=~@#№%&§<>|\\(){}[]_'’‘`\"“”\n\t\r $£";

        private static readonly IReadOnlyCollection<string>
            Redundancies = new HashSet<string>
            {
                "m",
                "s",
                "re",
                "ve",
                "th"
            };

        private static IEnumerable<string> ToTerms(string text) =>
            ToWords(text).Where(word => !Redundancies.Contains(word));

        private static IEnumerable<string> ToWords(string text, string separators = Separators) =>
            text.Split(separators.ToCharArray(), StringSplitOptions.RemoveEmptyEntries);

        private static MapResult<string, int> ToMapResult(string word) =>
            new MapResult<string, int>(word, 1);
    }

    public class MapResult<KeyT, ValueT>
    {
        public KeyT Key { get; set; }
        public ValueT Value { get; set; }

        public MapResult(KeyT key, ValueT value) => (Key, Value) = (key, value);

        public override string ToString() => $"{Key}: {Value}";
    }
}