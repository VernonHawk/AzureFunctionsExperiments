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
        public static IActionResult GetPostFun(
            [HttpTrigger(
                authLevel: AuthorizationLevel.Function,
                "get",
                Route = "posts/{author}/{id}"
            )]
            HttpRequest req,
            [CosmosDB(
                databaseName: ConnectionParams.DatabaseName,
                collectionName: "Posts",
                ConnectionStringSetting = ConnectionParams.DbConnectionStringSetting,
                PartitionKey = "{author}",
                Id = "{id}"
            )]
            Post? post,
            string id
        ) =>
            post == null
                ? new NotFoundObjectResult(
                    new {message = $"Couldn't find a post with id {id}"}
                )
                : (IActionResult) new OkObjectResult(post);
    }
}