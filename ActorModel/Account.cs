using System.Threading.Tasks;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.DurableTask;
using Newtonsoft.Json;

namespace ActorModel
{
    public interface IAccount
    {
        Task Replenish(decimal amount);

        Task<bool> Withdraw(decimal amount);

        Task<decimal> GetBalance();
    }

    public class Account : IAccount
    {
        public decimal Balance { get; set; } = decimal.Zero;

        public Task Replenish(decimal amount)
        {
            Balance += amount;

            return Task.CompletedTask;
        }

        public Task<bool> Withdraw(decimal amount)
        {
            var canWithdraw = amount <= Balance;

            if (canWithdraw)
            {
                Balance -= amount;
            }

            return Task.FromResult(canWithdraw);
        }

        public Task<decimal> GetBalance() => Task.FromResult(Balance);

        [FunctionName(nameof(Account))]
        public static Task Run([EntityTrigger] IDurableEntityContext ctx) =>
            ctx.DispatchAsync<Account>();
    }
}