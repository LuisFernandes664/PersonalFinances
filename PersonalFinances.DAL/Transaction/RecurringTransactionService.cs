using Microsoft.Extensions.DependencyInjection;
using PersonalFinances.BLL.Entities.Models.Transaction;
using PersonalFinances.BLL.Interfaces.Transaction;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace PersonalFinances.BLL.Services.Transaction
{
    public class RecurringTransactionService : IRecurringTransactionService
    {
        private readonly IRecurringTransactionRepository _repository;
        private readonly ITransactionService _transactionService;

        public RecurringTransactionService(
            IRecurringTransactionRepository repository,
            ITransactionService transactionService)
        {
            _repository = repository;
            _transactionService = transactionService;
        }

        public async Task<IEnumerable<RecurringTransactionModel>> GetUserRecurringTransactionsAsync(string userId)
        {
            return await _repository.GetAllByUserAsync(userId);
        }

        public async Task<RecurringTransactionModel> GetRecurringTransactionAsync(string id)
        {
            return await _repository.GetByIdAsync(id);
        }

        public async Task CreateRecurringTransactionAsync(RecurringTransactionModel transaction)
        {
            transaction.StampEntity = Guid.NewGuid().ToString();
            transaction.CreatedAt = DateTime.UtcNow;
            transaction.UpdatedAt = DateTime.UtcNow;

            await _repository.AddAsync(transaction);
        }

        public async Task UpdateRecurringTransactionAsync(RecurringTransactionModel transaction)
        {
            transaction.UpdatedAt = DateTime.UtcNow;
            await _repository.UpdateAsync(transaction);
        }

        public async Task DeleteRecurringTransactionAsync(string id)
        {
            await _repository.DeleteAsync(id);
        }

        public async Task ProcessDueRecurringTransactionsAsync()
        {
            var dueTransactions = await _repository.GetDueTransactionsAsync();

            foreach (var recurringTransaction in dueTransactions)
            {
                try
                {
                    // Criar uma nova transação com base na recorrente
                    var transaction = new TransactionModel
                    {
                        StampEntity = Guid.NewGuid().ToString(),
                        Description = recurringTransaction.Description,
                        UserStamp = recurringTransaction.UserId,
                        Amount = recurringTransaction.Amount,
                        Date = DateTime.Now,
                        Category = recurringTransaction.Category,
                        PaymentMethod = recurringTransaction.PaymentMethod,
                        Recipient = recurringTransaction.Recipient,
                        Status = "confirmed",
                        CreatedAt = DateTime.UtcNow,
                        UpdatedAt = DateTime.UtcNow
                    };

                    // Adicionar a transação
                    await _transactionService.AddTransactionAsync(transaction);

                    // Atualizar a data de último processamento
                    await _repository.UpdateLastProcessedDateAsync(recurringTransaction.StampEntity, DateTime.UtcNow);
                }
                catch (Exception ex)
                {
                    // Logar erro e continuar com as próximas transações
                    Console.WriteLine($"Erro ao processar transação recorrente {recurringTransaction.StampEntity}: {ex.Message}");
                }
            }
        }
    }

    // Implementar um background service para executar o processamento diariamente
    public class RecurringTransactionProcessor : Microsoft.Extensions.Hosting.BackgroundService
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly System.Threading.Timer _timer;

        public RecurringTransactionProcessor(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        protected override async Task ExecuteAsync(System.Threading.CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                // Processar as transações recorrentes
                using (var scope = _serviceProvider.CreateScope())
                {
                    var service = scope.ServiceProvider.GetRequiredService<IRecurringTransactionService>();
                    await service.ProcessDueRecurringTransactionsAsync();
                }

                // Aguardar até o próximo dia
                await Task.Delay(TimeSpan.FromHours(24), stoppingToken);
            }
        }
    }
}