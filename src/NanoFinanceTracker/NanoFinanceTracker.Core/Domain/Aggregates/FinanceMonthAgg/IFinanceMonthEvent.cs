namespace NanoFinanceTracker.Core.Domain.Aggregates.FinanceMonthAgg
{
    public interface IFinanceMonthEvent
    {

    }

    public class ExpenseAdded : IFinanceMonthEvent
    {
        public string FinanceMonthId { get; set; } = string.Empty;

        public string Account { get; set; } = string.Empty;

        public decimal Amount { get; set; }

        public string Category { get; set; } = string.Empty;

        public string Description { get; set; } = string.Empty;

        public DateTimeOffset TransactionDate { get; set; }

        public DateTimeOffset CreatedAt { get; set; }

    }

    public class IncomeAdded : IFinanceMonthEvent
    {
        public string FinanceMonthId { get; set; } = string.Empty;

        public string Account { get; set; } = string.Empty;

        public decimal Amount { get; set; }

        public string Category { get; set; } = string.Empty;

        public string Description { get; set; } = string.Empty;

        public DateTimeOffset TransactionDate { get; set; }

        public DateTimeOffset CreatedAt { get; set; }

    }
}
