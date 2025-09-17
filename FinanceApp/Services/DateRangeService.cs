using FinanceApp.Models;

namespace FinanceApp.Services;

public class DateRangeService : IDateRangeService
{
    public DateRange CurrentMonth()
    {
        var now = DateTime.Today;
        var from = new DateTime(now.Year, now.Month, 1);
        return new DateRange(from, now);
    }

    public DateRange Year(int year) => new(new DateTime(year, 1, 1), new DateTime(year, 12, 31));
}