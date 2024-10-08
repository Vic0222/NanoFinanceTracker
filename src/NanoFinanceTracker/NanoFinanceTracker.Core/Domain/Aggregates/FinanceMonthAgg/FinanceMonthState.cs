﻿namespace NanoFinanceTracker.Core.Domain.Aggregates.FinanceMonthAgg
{
    public class FinanceMonthState
    {
        public string UserId { get; set; } = string.Empty;

        public string Account { get; set; } = string.Empty;

        public int Month { get; set; }

        public int Year { get; set; }

        public decimal Balance { get; set; }

        public decimal TotalExpense { get; set; }

        public decimal TotalIncome { get; set; }

        public List<FinancialTransaction> FinancialTransactions { get; set; } = new List<FinancialTransaction>();

        public void Apply(ExpenseAdded @event)
        {
            FinancialTransactions.Add(new FinancialTransaction { 
                Account = @event.Account ?? Account,
                Amount = @event.Amount,
                Category = @event.Category,
                CreatedAt = @event.CreatedAt,
                Description = @event.Description,
                TransactionDate = @event.TransactionDate,
                TransactionType = TransactionType.Expense.ToString(),
            });

            TotalExpense += @event.Amount;
            Balance -= @event.Amount;

        }

        public void Apply(IncomeAdded @event)
        {
            FinancialTransactions.Add(new FinancialTransaction
            {
                Account = @event.Account ?? Account,
                Amount = @event.Amount,
                Category = @event.Category,
                CreatedAt = @event.CreatedAt,
                Description = @event.Description,
                TransactionDate = @event.TransactionDate,
                TransactionType = TransactionType.Income.ToString(),
            });

            TotalIncome += @event.Amount;
            Balance += @event.Amount;
        }
    }

    public enum TransactionType
    {
        Income,
        Expense
    }

    public class FinancialTransaction
    {
        public string Account { get; set; } = string.Empty;

        public DateTimeOffset TransactionDate { get; set; }

        public string TransactionType { get; set; } = string.Empty;

        public decimal Amount { get; set; }

        public string Category { get; set; } = string.Empty;

        public string Description { get; set; } = string.Empty;

        public DateTimeOffset CreatedAt { get; set; } = DateTimeOffset.Now;
    }
}
