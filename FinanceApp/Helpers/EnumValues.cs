using FinanceApp.Models;

namespace FinanceApp.Helpers;

public static class TimeGroupingValues
{
    public static Array All => Enum.GetValues(typeof(TimeGrouping));
}

public static class TransactionDirectionValues
{
    public static Array All => Enum.GetValues(typeof(TransactionDirection));
}