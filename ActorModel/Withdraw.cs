using System.Threading.Tasks;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.DurableTask;

namespace ActorModel
{
    public static class Withdraw
    {
        [FunctionName(nameof(Withdraw))]
        public static async Task<bool> WithdrawFun(
            [OrchestrationTrigger] IDurableOrchestrationContext ctx
        )
        {
            var input = ctx.GetInput<WithdrawArgs>();
            var account = ctx.CreateEntityProxy<IAccount>(input.Account);

            var wasSuccessful = await account.Withdraw(input.Amount);

            return wasSuccessful;
        }

        private class WithdrawArgs
        {
            public string Account { get; set; }
            public decimal Amount { get; set; }
        }
    }
}