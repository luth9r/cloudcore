namespace CloudCore.Services
{
    public interface ITransactionService
    {
        public Task ExecuteInTransactionAsync(Func<CancellationToken, Task>[] actions, CancellationToken cancellationToken = default);

        public Task ExecuteInTransactionWithCompensationAsync((Func<CancellationToken, Task> action, Func<CancellationToken, Task>? compensation)[] actions, CancellationToken cancellationToken = default);
    }
}
