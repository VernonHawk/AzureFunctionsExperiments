using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Common;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace Nanoservices
{
    internal class RequestBody
    {
        public string? Title { get; set; }
        public string? Author { get; set; }
        public string? Body { get; set; }
    }

    public static class AddPost
    {
    [FunctionName("AddPost")]
    public static async Task<IActionResult> RunAsync(
        [HttpTrigger(
            authLevel: AuthorizationLevel.Function,
            "post",
            Route = "posts"
        )]
        HttpRequest req,
        [CosmosDB(
            databaseName: ConnectionParams.DatabaseName,
            collectionName: ConnectionParams.CollectionName,
            ConnectionStringSetting = ConnectionParams.DbConnectionStringSetting
        )]
        IAsyncCollector<Post> collector,
        ILogger log
    )
    {
        var data = JsonConvert.DeserializeObject<RequestBody?>(
            await new StreamReader(req.Body).ReadToEndAsync()
        );

        log.LogInformation($"AddPost {data}");

        if (data == null)
        {
            return new BadRequestObjectResult(
                new {message = "The body of the request is missing"}
            );
        }

        if (new List<string?> {data.Author, data.Body, data.Title}.Any(
            string.IsNullOrWhiteSpace
        ))
        {
            return new BadRequestObjectResult(
                new {message = "One of the required fields is missing"}
            );
        }

        // ...
        await collector.AddAsync(
            new Post {Author = data.Author!, Body = data.Body!, Title = data.Title!}
        );

        return new OkResult();
    }
    }
}