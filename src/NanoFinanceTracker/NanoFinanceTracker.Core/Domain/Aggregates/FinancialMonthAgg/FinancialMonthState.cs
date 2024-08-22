namespace NanoFinanceTracker.Core.Domain.Aggregates.FinancialMonthAgg
{
    public class FinancialMonthState
    {
        public string UserId { get; set; } = string.Empty;

        public int Month { get; set; }

        public int Year { get; set; }

        public List<FinancialTransaction> FinancialTransactions { get; set; } = new List<FinancialTransaction>();

        public void Apply(ExpenseAdded @event)
        {
            FinancialTransactions.Add(new FinancialTransaction { 
                Amount = @event.Amount,
                Category = @event.Category,
                CreatedAt = @event.CreatedAt,
                Description = @event.Description,
                TransactionDate = @event.TransactionDate,
                TransactionType = TransactionType.Expense.ToString(),
            });

        }

        public void Apply(IncomeAdded @event)
        {
            FinancialTransactions.Add(new FinancialTransaction
            {
                Amount = @event.Amount,
                Category = @event.Category,
                CreatedAt = @event.CreatedAt,
                Description = @event.Description,
                TransactionDate = @event.TransactionDate,
                TransactionType = TransactionType.Income.ToString(),
            });

        }
    }

    public enum TransactionType
    {
        Income,
        Expense
    }

    public class FinancialTransaction
    {

        public DateTimeOffset TransactionDate { get; set; }

        public string TransactionType { get; set; } = string.Empty;

        public int Amount { get; set; }

        public string Category { get; set; } = string.Empty;

        public string Description { get; set; } = string.Empty;

        public DateTimeOffset CreatedAt { get; set; } = DateTimeOffset.Now;
    }
}
