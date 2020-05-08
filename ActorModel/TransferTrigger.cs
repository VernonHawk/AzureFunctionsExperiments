using System.IO;
using System.Threading.Tasks;
using System.Web.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.DurableTask;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Newtonsoft.Json;

namespace ActorModel
{
    public static class TransferTrigger
    {
        [FunctionName(nameof(TransferTrigger))]
        public static async Task<IActionResult> TransferTriggerFun(
            [HttpTrigger(AuthorizationLevel.Function, "post", Route = "transfer")]
            HttpRequest req,
            [DurableClient] IDurableOrchestrationClient client
        )
        {
            var data = JsonConvert.DeserializeObject<RequestBody?>(
                await new StreamReader(req.Body).ReadToEndAsync()
            );

            if (data?.FromAccount is null || data.ToAccount is null || data.Amount is null)
            {
                return new BadRequestErrorMessageResult(
                    "The required parameters were not specified"
                );
            }

            var id = await client.StartNewAsync(nameof(Transfer), data);

            return new OkObjectResult(new {orchestrationId = id});
        }

        private class RequestBody
        {
            public string? FromAccount { get; set; }
            public string? ToAccount { get; set; }
            public decimal? Amount { get; set; }
        }
    }
}