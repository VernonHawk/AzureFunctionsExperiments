using System.Threading.Tasks;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.DurableTask;
using Newtonsoft.Json;

namespace ActorModel
{
    public interface IAccount
    {
        void Replenish(decimal amount);

        Task<bool> Withdraw(decimal amount);

        Task<decimal> GetBalance();
    }

    [JsonObject(MemberSerialization.OptOut)]
    public class Account : IAccount
    {
        public decimal Balance { get; set; } = decimal.Zero;

        public void Replenish(decimal amount) => Balance += amount;

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