using System.Text;
using Microsoft.Maui.Storage;

namespace FinanceApp.Services;

public interface IProfileService
{
    // Имя текущего профиля (null — не выбран, показываем страницу выбора)
    string? GetCurrentProfileName();
    Task SetCurrentProfileAsync(string name);

    // Список профилей по файлам finance_*.sqlite3 (+ "default", если есть)
    Task<IList<string>> GetProfilesAsync();

    // Создание — фактически просто выбор, файл создастся при EnsureCreatedAsync
    Task CreateProfileAsync(string name);

    // Удаление файла профиля
    Task DeleteProfileAsync(string name);

    // Путь к файлу БД для профиля
    string GetDbFilePath(string name);
}
