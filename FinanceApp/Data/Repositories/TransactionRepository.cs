using FinanceApp.Models;
using SQLite;
using System;
using System.Globalization;
using System.Security.Principal;

namespace FinanceApp.Data.Repositories;

public class TransactionRepository
{
    private readonly IDatabase _database;
    private SQLiteAsyncConnection Conn => _database.GetConnection();

    public TransactionRepository(IDatabase database) => _database = database;

    public async Task<int> InsertAsync(Transaction t)
    {
        await _database.EnsureCreatedAsync();
        return await Conn.InsertAsync(t);
    }

    public async Task<int> UpdateAsync(Transaction t)
    {
        await _database.EnsureCreatedAsync();
        return await Conn.UpdateAsync(t);
    }

    public async Task<int> DeleteAsync(Transaction t)
    {
        await _database.EnsureCreatedAsync();
        return await Conn.DeleteAsync(t);
    }

    public async Task<List<Transaction>> GetByRangeAsync(DateRange range, TransactionDirection? dir = null, string? account = null, string? source = null)
    {
        await _database.EnsureCreatedAsync();

        var query = Conn.Table<Transaction>()
            .Where(t => t.Date >= range.From && t.Date <= range.To);

        if (dir.HasValue) query = query.Where(t => t.Direction == dir.Value);
        if (!string.IsNullOrWhiteSpace(account)) query = query.Where(t => t.Account == account);
        if (!string.IsNullOrWhiteSpace(source)) query = query.Where(t => t.Source == source);

        return await query.OrderByDescending(t => t.Date).ToListAsync();
    }

    public async Task<decimal> SumAsync(DateRange range, TransactionDirection dir, string? account = null, string? source = null)
    {
        await _database.EnsureCreatedAsync();

        var sql = @"
        SELECT IFNULL(SUM(Amount), 0)
        FROM Transactions
        WHERE Date BETWEEN ? AND ?
          AND Direction = ?
          {0} {1};";

        var byAccount = string.IsNullOrWhiteSpace(account) ? "" : "AND Account = ?";
        var bySource = string.IsNullOrWhiteSpace(source) ? "" : "AND Source  = ?";
        sql = string.Format(sql, byAccount, bySource);

        var args = new List<object> { range.From, range.To, (int)dir };
        if (!string.IsNullOrWhiteSpace(account)) args.Add(account!);
        if (!string.IsNullOrWhiteSpace(source)) args.Add(source!);

        return await Conn.ExecuteScalarAsync<decimal>(sql, args.ToArray());
    }

    public async Task<List<TopItem>> SumBySourceAsync(DateRange range, TransactionDirection dir, string? account = null)
    {
        await _database.EnsureCreatedAsync();

        var sql = @"
        SELECT Source, IFNULL(SUM(Amount), 0) AS Total
        FROM Transactions
        WHERE Date BETWEEN ? AND ?
          AND Direction = ?
          {0}
        GROUP BY Source;";

        var byAccount = string.IsNullOrWhiteSpace(account) ? "" : "AND Account = ?";
        sql = string.Format(sql, byAccount);

        var args = new List<object> { range.From, range.To, (int)dir };
        if (!string.IsNullOrWhiteSpace(account)) args.Add(account!);

        var rows = await Conn.QueryAsync<(string Source, decimal Total)>(sql, args.ToArray());
        return [.. rows.Select(r => new TopItem
        {
            Name = r.Source,
            Summ = r.Total
        })];
    }

    public async Task<List<(DateTime Bucket, decimal Sum)>> GroupedSumAsync(
        DateRange range, TransactionDirection? dir, Models.TimeGrouping grouping)
    {
        await _database.EnsureCreatedAsync();

        string bucketExpr = grouping switch
        {
            Models.TimeGrouping.Daily => "date(Date)",
            Models.TimeGrouping.Weekly => "strftime('%Y-W%W', Date)",
            Models.TimeGrouping.Monthly => "strftime('%Y-%m-01', Date)",
            Models.TimeGrouping.Yearly => "strftime('%Y-01-01', Date)",
            _ => "date(Date)"
        };

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

        var rows = await Conn.QueryAsync<GroupRow>(sql, [.. args]);

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
        int year = int.Parse(parts[0]);
        int week = int.Parse(parts[1]);
        var jan4 = new DateTime(year, 1, 4);
        int delta = DayOfWeek.Monday - jan4.DayOfWeek;
        var week1 = jan4.AddDays(delta);
        return week1.AddDays((week - 1) * 7); // фикс off-by-one
    }

    private class GroupRow
    {
        public string Bucket { get; set; } = "";
        public decimal Total { get; set; }
    }
}
