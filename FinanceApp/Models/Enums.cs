namespace FinanceApp.Models;

public enum TransactionDirection
{
    Income = 1, // Доход
    Expense = 2 // Расход
}

public enum TimeGrouping
{
    Daily,
    Weekly,
    Monthly,
    Yearly
}

public record DateRange(DateTime From, DateTime To);