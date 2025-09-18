using FinanceApp.Models;
using FinanceApp.Services;

namespace FinanceApp.ViewModels;

public partial class ExpenseViewModel : DetailViewModel
{
    public ExpenseViewModel(ITransactionService tx, IDateRangeService ranges, IReferenceService refs) : base(tx, ranges, refs)
    {
        Title = "Расходы";
    }

    protected override TransactionDirection? DirectionForList => TransactionDirection.Expense;
}