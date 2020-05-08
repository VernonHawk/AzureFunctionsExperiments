using System.Threading.Tasks;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.DurableTask;

namespace ActorModel
{
    public static class Transfer
    {
        [FunctionName(nameof(Transfer))]
        public static async Task<bool> TransferFun(
            [OrchestrationTrigger] IDurableOrchestrationContext ctx
        )
        {
            var input = ctx.GetInput<TransferArgs>();

            var fromEntity = new EntityId(nameof(Account), input.FromAccount);
            var toEntity = new EntityId(nameof(Account), input.ToAccount);

            using (await ctx.LockAsync(fromEntity, toEntity))
            {
                var fromAccount = ctx.CreateEntityProxy<IAccount>(fromEntity);
                var toAccount = ctx.CreateEntityProxy<IAccount>(toEntity);

                var hasEnoughFunds = await fromAccount.Withdraw(input.Amount);

                if (!hasEnoughFunds)
                {
                    return false;
                }

                await toAccount.Replenish(input.Amount);
            }

            return true;
        }

        private class TransferArgs
        {
            public string FromAccount { get; set; }
            public string ToAccount { get; set; }
            public decimal Amount { get; set; }
        }
    }
}