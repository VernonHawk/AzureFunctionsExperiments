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
        public string Title { get; set; } = "";
        public string Author { get; set; } = "";
        public string Body { get; set; } = "";
    }

    public static class AddPost
    {
        [FunctionName("AddPost")]
        public static async Task<IActionResult> RunAsync(
            [HttpTrigger(authLevel: AuthorizationLevel.Function, "post", Route = "posts")]
            HttpRequest req,
            ILogger log
        )
        {
            log.LogInformation("C# HTTP trigger function processed a request.");

            var data = JsonConvert.DeserializeObject<RequestBody>(
                await new StreamReader(req.Body).ReadToEndAsync()
            );

            var post = new Post {Author = data.Author, Body = data.Body, Title = data.Title};

            return new OkObjectResult($"Hello, {post.Author}");
        }
    }
}