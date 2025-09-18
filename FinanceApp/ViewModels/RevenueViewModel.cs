using FinanceApp.Models;
using FinanceApp.Services;

namespace FinanceApp.ViewModels;

public partial class RevenueViewModel : DetailViewModel
{
    public RevenueViewModel(ITransactionService tx, IDateRangeService ranges, IReferenceService refs) : base(tx, ranges, refs)
    {
        Title = "Выручка";
    }

    protected override TransactionDirection? DirectionForList => TransactionDirection.Income;
}