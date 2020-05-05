using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.DurableTask;
using Microsoft.Extensions.Logging;

namespace DistributedComputing
{
    public class WordCountMap
    {
        [FunctionName(nameof(WordCountMap))]
        public static IList<MapResultEntry>
            Run([ActivityTrigger] IList<string> lines) =>
            lines.SelectMany(ToTerms).Select(ToMapResult).ToList();

        private const string Separators =
            " ,?!.…:;—-+/*^=~@#№%&§<>|\\(){}[]_'’‘`\"“”\n\t\r $£";

        private static readonly IReadOnlyCollection<string>
            Redundancies = new HashSet<string> {"m", "s", "re", "ve", "th"};

        private static IEnumerable<string> ToTerms(string text) =>
            ToWords(text).Where(word => !Redundancies.Contains(word));

        private static IEnumerable<string> ToWords(string text, string separators = Separators) =>
            text.Split(separators.ToCharArray(), StringSplitOptions.RemoveEmptyEntries);

        private static MapResultEntry ToMapResult(string word) => new MapResultEntry(word, 1);
    }

    public class MapResultEntry
    {
        public string Word { get; set; }
        public int Count { get; set; }

        public MapResultEntry(string word, int count) => (Word, Count) = (word, count);

        public override string ToString() => $"{Word}: {Count}";
    }
}