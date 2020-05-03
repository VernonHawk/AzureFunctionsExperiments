using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Azure.Documents;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Host;
using Microsoft.Extensions.Logging;

namespace ServiceGlue
{
    public static class Emailer
    {
        [FunctionName("Emailer")]
        public static async Task RunAsync(
            [CosmosDBTrigger(
                databaseName: "databaseName",
                collectionName: "collectionName",
                ConnectionStringSetting = ""
            )]
            IReadOnlyList<Document> input,
            ILogger log
        )
        {
            if (input != null && input.Count > 0)
            {
                log.LogInformation("Documents modified " + input.Count);
                log.LogInformation("First document Id " + input[0].Id);
                
            }
        }
    }
}