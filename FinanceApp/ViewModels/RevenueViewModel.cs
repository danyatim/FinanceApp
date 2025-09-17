using FinanceApp.Models;
using FinanceApp.Services;

namespace FinanceApp.ViewModels;

public partial class RevenueViewModel : DetailViewModel
{
    public RevenueViewModel(ITransactionService tx, IDateRangeService ranges) : base(tx, ranges)
    {
        Title = "Выручка";
    }

    protected override TransactionDirection? DirectionForList => TransactionDirection.Income;
}