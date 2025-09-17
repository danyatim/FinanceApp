using FinanceApp.Services;

namespace FinanceApp.ViewModels;

public partial class ProfitViewModel : DetailViewModel
{
    public ProfitViewModel(ITransactionService tx, IDateRangeService ranges) : base(tx, ranges)
    {
        Title = "Прибыль";
    }

    protected override Models.TransactionDirection? DirectionForList => null;
}