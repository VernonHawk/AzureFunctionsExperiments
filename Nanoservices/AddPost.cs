using System.IO;
using System.Threading.Tasks;
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
            [HttpTrigger(authLevel: AuthorizationLevel.Function, "post", Route = "posts")]
            HttpRequest req,
            [CosmosDB(
                databaseName: "Study",
                collectionName: "Posts",
                ConnectionStringSetting = "PostsDBConnectionString"
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

            if (data.Author == null || data.Body == null || data.Title == null)
            {
                return new BadRequestObjectResult(
                    new {message = "One of the required fields is missing"}
                );
            }

            await collector.AddAsync(
                new Post {Author = data.Author, Body = data.Body, Title = data.Title}
            );

            return new OkResult();
        }
    }
}