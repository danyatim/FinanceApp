using FinanceApp.Models;

namespace FinanceApp.Services;

public interface ITransactionService
{
    Task AddAsync(Transaction t);
    Task<List<Transaction>> GetAsync(DateRange range, TransactionDirection? dir = null, string? account = null, string? source = null);
    Task<decimal> SumAsync(DateRange range, TransactionDirection dir, string? account = null, string? source = null);
    Task<List<(DateTime Bucket, decimal Sum)>> SeriesAsync(DateRange range, TransactionDirection? dir, TimeGrouping g);
    Task<Dictionary<string, decimal>> SumBySourceAsync(DateRange range, TransactionDirection dir, string? account = null);
    Task<decimal> ProfitAsync(DateRange range);
}