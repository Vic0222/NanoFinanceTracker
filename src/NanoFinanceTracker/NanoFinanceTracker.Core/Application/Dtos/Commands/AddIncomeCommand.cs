using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NanoFinanceTracker.Core.Application.Dtos.Commands
{
    [GenerateSerializer]
    public class AddIncomeCommand
    {
        [Id(0)]
        public int Amount { get; set; }
        [Id(1)]
        public string Description { get; set; } = string.Empty;
        [Id(2)]
        public string Category { get; set; } = string.Empty;
        [Id(3)]
        public DateTimeOffset TransactionDate { get; set; }
    }
}
