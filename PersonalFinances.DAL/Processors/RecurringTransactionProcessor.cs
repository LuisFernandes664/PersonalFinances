using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using PersonalFinances.BLL.Interfaces.Transaction;

namespace PersonalFinances.DAL.Processors
{
    public class RecurringTransactionProcessor : IHostedService
    {
        private readonly IServiceProvider _serviceProvider;
        private Timer _timer;

        public RecurringTransactionProcessor(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        public Task StartAsync(CancellationToken cancellationToken)
        {
            _timer = new Timer(ProcessTransactions, null, TimeSpan.Zero,
                TimeSpan.FromHours(24));
            return Task.CompletedTask;
        }

        private async void ProcessTransactions(object state)
        {
            using (var scope = _serviceProvider.CreateScope())
            {
                var service = scope.ServiceProvider.GetRequiredService<IRecurringTransactionService>();
                await service.ProcessDueRecurringTransactionsAsync();
            }
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            _timer?.Change(Timeout.Infinite, 0);
            return Task.CompletedTask;
        }
    }
}
