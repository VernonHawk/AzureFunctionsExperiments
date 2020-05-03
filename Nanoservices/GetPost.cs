using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;

namespace Nanoservices
{
    public static class GetPost
    {
        [FunctionName("GetPost")]
        public static IActionResult RunAsync(
            [HttpTrigger(
                authLevel: AuthorizationLevel.Function,
                "get",
                Route = "posts/{author}/{id}"
            )]
            HttpRequest req,
            [CosmosDB(
                databaseName: "Study",
                collectionName: "Posts",
                ConnectionStringSetting = "PostsDBConnectionString",
                PartitionKey = "{author}",
                Id = "{id}"
            )]
            Post? post,
            string id,
            ILogger log
        )
        {
            log.LogInformation($"GetPost {post}");

            if (post == null)
            {
                return new NotFoundObjectResult(
                    new {message = $"Couldn't find a post with id {id}"}
                );
            }

            return new OkObjectResult(post);
        }
    }
}