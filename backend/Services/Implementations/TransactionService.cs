using CloudCore.Data.Context;

namespace CloudCore.Services.Implementations
{
    public class TransactionService(CloudCoreDbContext context, ILogger<TransactionService> logger) : ITransactionService
    {
        public async Task ExecuteInTransactionAsync(Func<CancellationToken, Task>[] actions, CancellationToken cancellationToken = default)
        {
            await using var transaction = await context.Database.BeginTransactionAsync(cancellationToken);
            try
            {
                foreach (var action in actions)
                {
                    await action(cancellationToken);
                }
                await transaction.CommitAsync(cancellationToken);
                logger.LogInformation("Transaction committed successfully");
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync(cancellationToken);
                logger.LogError(ex, "Transaction rolled back due to error");
                throw;
            }
        }

        public async Task ExecuteInTransactionWithCompensationAsync((Func<CancellationToken, Task> action, Func<CancellationToken, Task>? compensation)[] actions, CancellationToken cancellationToken = default)
        {
            await using var transaction = await context.Database.BeginTransactionAsync(cancellationToken);
            var executedActions = new Stack<Func<CancellationToken, Task>>();

            try
            {
                foreach (var (action, compensation) in actions)
                {
                    await action(cancellationToken);

                    if (compensation != null)
                    {
                        executedActions.Push(compensation);
                    }
                }

                await transaction.CommitAsync(cancellationToken);
                logger.LogInformation("Transaction committed successfully");
            }
            catch(Exception ex)
            {
                await transaction.RollbackAsync(cancellationToken);

                while (executedActions.Count > 0)
                {
                    var compensation = executedActions.Pop();
                    try
                    {
                        await compensation(cancellationToken);
                    }
                    catch (Exception compensationEx)
                    {
                        logger.LogError(compensationEx, "Compensation action failed");
                    }
                }

                logger.LogError(ex, "Transaction rolled back due to error");
                throw;
            }
        }
    }
}
