using FinanceApp.Models;
using SQLite;
using System.Globalization;

namespace FinanceApp.Data.Repositories;

public class TransactionRepository
{
    private readonly SQLiteAsyncConnection _db;
    public TransactionRepository(IDatabase database) => _db = database.GetConnection();

    public Task<int> InsertAsync(Transaction t) => _db.InsertAsync(t);
    public Task<int> UpdateAsync(Transaction t) => _db.UpdateAsync(t);
    public Task<int> DeleteAsync(Transaction t) => _db.DeleteAsync(t);

    public Task<List<Transaction>> GetByRangeAsync(DateRange range, TransactionDirection? dir = null, string? account = null, string? source = null)
    {
        var query = _db.Table<Transaction>()
            .Where(t => t.Date >= range.From && t.Date <= range.To);

        if (dir.HasValue) query = query.Where(t => t.Direction == dir.Value);
        if (!string.IsNullOrWhiteSpace(account)) query = query.Where(t => t.Account == account);
        if (!string.IsNullOrWhiteSpace(source)) query = query.Where(t => t.Source == source);

        return query.OrderByDescending(t => t.Date).ToListAsync();
    }

    public async Task<decimal> SumAsync(DateRange range, TransactionDirection dir, string? account = null, string? source = null)
    {
        var list = await GetByRangeAsync(range, dir, account, source);
        return list.Sum(t => t.Amount);
    }

    public async Task<Dictionary<string, decimal>> SumBySourceAsync(DateRange range, TransactionDirection dir, string? account = null)
    {
        var list = await GetByRangeAsync(range, dir, account, null);
        return list.GroupBy(t => t.Source)
                   .ToDictionary(g => g.Key, g => g.Sum(x => x.Amount));
    }

    public async Task<List<(DateTime Bucket, decimal Sum)>> GroupedSumAsync(
    DateRange range, TransactionDirection? dir, Models.TimeGrouping grouping)
    {
        string bucketExpr = grouping switch
        {
            Models.TimeGrouping.Daily => "date(Date)",                 // YYYY-MM-DD
            Models.TimeGrouping.Weekly => "strftime('%Y-W%W', Date)",   // YYYY-Www
            Models.TimeGrouping.Monthly => "strftime('%Y-%m-01', Date)", // 1-е число мес€ца
            Models.TimeGrouping.Yearly => "strftime('%Y-01-01', Date)", // 1-е €нвар€
            _ => "date(Date)"
        };

        // —равниваем date(Date) с date(?) Ч и параметры передаем ISO-датами без времени.
        string dirFilter = dir.HasValue ? " AND Direction = ?" : string.Empty;

        string sql = $@"
        SELECT {bucketExpr} AS Bucket, SUM(Amount) AS Total
        FROM transactions
        WHERE Date IS NOT NULL AND Date <> ''
          AND date(Date) BETWEEN date(?) AND date(?) {dirFilter}
        GROUP BY Bucket
        ORDER BY Bucket ASC;";

        var args = new List<object>
    {
        range.From.ToString("yyyy-MM-dd", CultureInfo.InvariantCulture),
        range.To.ToString("yyyy-MM-dd", CultureInfo.InvariantCulture)
    };
        if (dir.HasValue) args.Add((int)dir.Value);

        var rows = await _db.QueryAsync<GroupRow>(sql, args.ToArray());

        var result = rows
            .Where(r => !string.IsNullOrWhiteSpace(r.Bucket))
            .Select(r =>
            {
                var dt = grouping == Models.TimeGrouping.Weekly
                    ? FirstDateOfIsoWeek(r.Bucket!)
                    : DateTime.ParseExact(r.Bucket!, "yyyy-MM-dd", CultureInfo.InvariantCulture);
                return (dt, r.Total);
            })
            .ToList();

        return result;
    }

    private static DateTime FirstDateOfIsoWeek(string yWeek)
    {
        var parts = yWeek.Split("-W");
        int year = int.Parse(parts[0]); int week = int.Parse(parts[1]);
        var jan4 = new DateTime(year, 1, 4);
        int delta = DayOfWeek.Monday - jan4.DayOfWeek;
        var week1 = jan4.AddDays(delta);
        return week1.AddDays(week * 7);
    }

    private class GroupRow
    {
        public string Bucket { get; set; } = "";
        public decimal Total { get; set; }
    }
}