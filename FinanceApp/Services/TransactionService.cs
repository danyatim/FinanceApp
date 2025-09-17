using FinanceApp.Data.Repositories;
using FinanceApp.Models;

namespace FinanceApp.Services;

public class TransactionService : ITransactionService
{
    private readonly TransactionRepository _repo;
    public TransactionService(TransactionRepository repo) => _repo = repo;

    public Task AddAsync(Transaction t) => _repo.InsertAsync(t);

    public Task<List<Transaction>> GetAsync(DateRange range, TransactionDirection? dir = null, string? account = null, string? source = null)
        => _repo.GetByRangeAsync(range, dir, account, source);

    public Task<decimal> SumAsync(DateRange range, TransactionDirection dir, string? account = null, string? source = null)
        => _repo.SumAsync(range, dir, account, source);

    public Task<List<(DateTime Bucket, decimal Sum)>> SeriesAsync(DateRange range, TransactionDirection? dir, TimeGrouping g)
        => _repo.GroupedSumAsync(range, dir, g);

    public Task<Dictionary<string, decimal>> SumBySourceAsync(DateRange range, TransactionDirection dir, string? account = null)
        => _repo.SumBySourceAsync(range, dir, account);

    public async Task<decimal> ProfitAsync(DateRange range)
    {
        var inc = await _repo.SumAsync(range, TransactionDirection.Income);
        var exp = await _repo.SumAsync(range, TransactionDirection.Expense);
        return inc - exp;
    }
}