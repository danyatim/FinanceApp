using System.Text.RegularExpressions;

namespace FinanceApp.Services;

public class ProfileService : IProfileService
{
    private const string CurrentProfileKey = "CurrentProfileName";
    private static readonly Regex InvalidCharsRegex = new(@"[^a-zA-Z0-9_\-]+", RegexOptions.Compiled);

    public string? GetCurrentProfileName()
        => Preferences.Get(CurrentProfileKey, null);

    public Task SetCurrentProfileAsync(string name)
    {
        name = Sanitize(name);
        if (string.IsNullOrWhiteSpace(name)) name = "default";
        Preferences.Set(CurrentProfileKey, name);
        return Task.CompletedTask;
    }

    public async Task<IList<string>> GetProfilesAsync()
    {
        var dir = FileSystem.AppDataDirectory;
        var files = Directory.GetFiles(dir, "finance*.sqlite3", SearchOption.TopDirectoryOnly);

        var list = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
        foreach (var f in files)
        {
            var file = Path.GetFileName(f);
            if (file.Equals("finance.sqlite3", StringComparison.OrdinalIgnoreCase))
                list.Add("default");
            else if (file.StartsWith("finance_", StringComparison.OrdinalIgnoreCase))
            {
                var name = file["finance_".Length..^".sqlite3".Length];
                list.Add(name);
            }
        }

        // Если совсем пусто — покажем default как опцию
        if (list.Count == 0) list.Add("default");
        var result = list.ToList();
        result.Sort(StringComparer.OrdinalIgnoreCase);
        return await Task.FromResult(result);
    }

    public async Task CreateProfileAsync(string name)
    {
        name = Sanitize(name);
        if (string.IsNullOrWhiteSpace(name)) name = "default";
        // Просто запомним его как текущий — файл создастся при EnsureCreatedAsync
        await SetCurrentProfileAsync(name);
    }

    public Task DeleteProfileAsync(string name)
    {
        name = Sanitize(name);
        var path = GetDbFilePath(name);
        if (File.Exists(path))
            File.Delete(path);
        // Если удалили текущий — сбросим
        if (string.Equals(GetCurrentProfileName(), name, StringComparison.OrdinalIgnoreCase))
            Preferences.Remove(CurrentProfileKey);
        return Task.CompletedTask;
    }

    public string GetDbFilePath(string name)
    {
        name = Sanitize(name);
        var dir = FileSystem.AppDataDirectory;
        if (string.Equals(name, "default", StringComparison.OrdinalIgnoreCase))
            return Path.Combine(dir, "finance.sqlite3");
        return Path.Combine(dir, $"finance_{name}.sqlite3");
    }

    private static string Sanitize(string? s)
    {
        if (string.IsNullOrWhiteSpace(s)) return "default";
        var trimmed = s.Trim();
        var safe = InvalidCharsRegex.Replace(trimmed, "_");
        return string.IsNullOrWhiteSpace(safe) ? "default" : safe;
    }
}
