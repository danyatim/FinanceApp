using FinanceApp.Models;

namespace FinanceApp.Services;

public interface IReferenceService
{
    Task<IList<Account>> GetAccountsAsync();
    Task AddAccountAsync(string name);
    Task DeleteAccountAsync(int id);

    Task<IList<Source>> GetSourcesAsync(TransactionDirection? type = null);
    Task AddSourceAsync(string name, TransactionDirection type);
    Task DeleteSourceAsync(int id);
}
