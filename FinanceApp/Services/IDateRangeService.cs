using FinanceApp.Models;

namespace FinanceApp.Services;

public interface IDateRangeService
{
    DateRange CurrentMonth();
    DateRange Year(int year);
}