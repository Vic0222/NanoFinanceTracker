namespace NanoFinanceTracker.Core.Domain.Aggregates.FinancialMonthAgg
{
    public interface IFinancialMonthEvent
    {

    }

    public class ExpenseAdded : IFinancialMonthEvent
    {
        public string FinancialMonthId { get; set; } = string.Empty;

        public int Amount { get; set; }

        public string Category { get; set; } = string.Empty;

        public string Description { get; set; } = string.Empty;

        public DateTimeOffset TransactionDate { get; set; }

        public DateTimeOffset CreatedAt { get; set; }

    }

    public class IncomeAdded : IFinancialMonthEvent
    {
        public string FinancialMonthId { get; set; } = string.Empty;

        public int Amount { get; set; }

        public string Category { get; set; } = string.Empty;

        public string Description { get; set; } = string.Empty;

        public DateTimeOffset TransactionDate { get; set; }

        public DateTimeOffset CreatedAt { get; set; }

    }
}
