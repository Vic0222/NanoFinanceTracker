using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NanoFinanceTracker.Core.Application.Dtos.Views
{
    [GenerateSerializer]
    public class FinancialTransactionView
    {
        [Id(0)]
        public string Account { get; set; } = string.Empty;

        [Id(1)]
        public DateTimeOffset TransactionDate { get; set; }

        [Id(2)]
        public string TransactionType { get; set; } = string.Empty;

        [Id(3)]
        public decimal Amount { get; set; }

        [Id(4)]
        public string Category { get; set; } = string.Empty;

        [Id(5)]
        public string Description { get; set; } = string.Empty;

        [Id(6)]
        public DateTimeOffset CreatedAt { get; set; } = DateTimeOffset.Now;

    }
}
