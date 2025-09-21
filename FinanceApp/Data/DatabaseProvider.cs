
namespace FinanceApp.Data
{
    public interface IDatabaseProvider
    {
        IDatabase CurrentDatabase { get; }
        Task SwitchProfileAsync(string profileName);
    }

    public class DatabaseProvider : IDatabaseProvider
    {
        private IDatabase? _currentDatabase;
        private readonly SemaphoreSlim _lock = new(1, 1);

        public IDatabase CurrentDatabase => _currentDatabase ?? throw new InvalidOperationException("База данных не инициализирована");

        public async Task SwitchProfileAsync(string profileName)
        {
            await _lock.WaitAsync();
            try
            {
                var newDb = new Database(profileName); // или можно передавать профиль и создавать внутри
                await newDb.EnsureCreatedAsync(); // убедиться, что таблицы созданы
                _currentDatabase = newDb;
            }
            finally
            {
                _lock.Release();
            }
        }

        public async Task InitializeAsync()
        {
            await SwitchProfileAsync("finance");
        }
    }
}
