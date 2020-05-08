using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Azure.WebJobs.Extensions.DurableTask;

namespace ActorModel
{
    public static class GetBalance
    {
        [FunctionName(nameof(GetBalance))]
        public static async Task<IActionResult> GetBalanceFun(
            [HttpTrigger(AuthorizationLevel.Function, "get", Route = "{account}/balance")]
            HttpRequest req,
            string account,
            [DurableClient] IDurableEntityClient client
        )
        {
            var entity =
                await client.ReadEntityStateAsync<Account>(
                    new EntityId(entityName: nameof(Account), entityKey: account)
                );

            if (!entity.EntityExists)
            {
                return new NotFoundObjectResult(
                    new {message = $"Couldn't find the account {account}"}
                );
            }

            return new OkObjectResult(new {balance = await entity.EntityState.GetBalance()});
        }
    }
}