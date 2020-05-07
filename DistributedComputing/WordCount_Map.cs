using System;
using System.Collections.Generic;
using System.Linq;
using DistributedComputing.MapReduce;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.DurableTask;

namespace DistributedComputing
{
    public static class WordCountMap
    {
        [FunctionName(nameof(WordCountMap))]
        public static IList<Result<string, int>>
            WordCount_Map([ActivityTrigger] IList<string> lines) =>
            lines.SelectMany(ToTerms).Select(ToMapReduceResult).ToList();

        private const string Separators =
            " ,?!.…:;—-+/*^=~@#№%&§<>|\\(){}[]_'’‘`\"“”\n\t\r $£";

        private static readonly IReadOnlyCollection<string>
            Redundancies = new HashSet<string>
            {
                "t",
                "m",
                "s",
                "re",
                "ve",
                "th"
            };

        private static IEnumerable<string> ToTerms(string text) =>
            ToWords(text)
                .Select(word => word.ToLower())
                .Where(word => !Redundancies.Contains(word));

        private static IEnumerable<string> ToWords(string text, string separators = Separators) =>
            text.Split(separators.ToCharArray(), StringSplitOptions.RemoveEmptyEntries);

        private static Result<string, int> ToMapReduceResult(string word) =>
            new Result<string, int>(word, 1);
    }
}