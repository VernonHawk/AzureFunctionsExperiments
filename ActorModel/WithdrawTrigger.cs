using System.IO;
using System.Threading.Tasks;
using System.Web.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Azure.WebJobs.Extensions.DurableTask;
using Newtonsoft.Json;

namespace ActorModel
{
    public static class WithdrawTrigger
    {
        [FunctionName(nameof(WithdrawTrigger))]
        public static async Task<IActionResult> WithdrawTriggerFun(
            [HttpTrigger(AuthorizationLevel.Function, "post", Route = "withdraw")]
            HttpRequest req,
            [DurableClient] IDurableOrchestrationClient client
        )
        {
            var data = JsonConvert.DeserializeObject<RequestBody?>(
                await new StreamReader(req.Body).ReadToEndAsync()
            );

            if (data?.Account is null || data.Amount is null)
            {
                return new BadRequestErrorMessageResult(
                    "The required parameters were not specified"
                );
            }

            var id = await client.StartNewAsync(nameof(Withdraw), data);

            return new OkObjectResult(new {orchestrationId = id});
        }

        private class RequestBody
        {
            public string? Account { get; set; }
            public decimal? Amount { get; set; }
        }
    }
}