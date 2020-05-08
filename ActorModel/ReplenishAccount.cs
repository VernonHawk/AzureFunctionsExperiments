using System.IO;
using System.Threading.Tasks;
using System.Web.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Azure.WebJobs.Extensions.DurableTask;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace ActorModel
{
    public static class ReplenishAccount
    {
        [FunctionName(nameof(ReplenishAccount))]
        public static async Task<IActionResult> ReplenishAccountFun(
            [HttpTrigger(AuthorizationLevel.Function, "post", Route = "replenish")]
            HttpRequest req,
            [DurableClient] IDurableEntityClient client
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

            await client.SignalEntityAsync<IAccount>(
                entityKey: data.Account,
                operation: account => account.Replenish(data.Amount.Value)
            );

            return new OkObjectResult(new {message = "Successfully replenished"});
        }

        private class RequestBody
        {
            public string? Account { get; set; }
            public decimal? Amount { get; set; }
        }
    }
}