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
            [HttpTrigger(AuthorizationLevel.Function, "get", Route = "posts/{id:int}")]
            HttpRequest req,
            int id,
            ILogger log
        )
        {
            log.LogInformation("C# HTTP trigger function processed a request.");

            string name = req.Query["id"];

            return new OkObjectResult($"Hello, {name} {id}");
        }
    }
}