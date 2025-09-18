using FinanceApp.Services;

namespace FinanceApp.ViewModels;

public partial class ProfitViewModel : DetailViewModel
{
    public ProfitViewModel(ITransactionService tx, IDateRangeService ranges, IReferenceService refs) : base(tx, ranges, refs)
    {
        Title = "Прибыль";
    }

    protected override Models.TransactionDirection? DirectionForList => null;
}