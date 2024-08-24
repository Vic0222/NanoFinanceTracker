using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NanoFinanceTracker.Core.Application.Dtos.Commands
{
    public class AddExpense
    {
        public int Amount { get; set; }
        public string Description { get; set; }
        public string Category { get; set; }
        public DateTimeOffset TransactionDate { get; set; }
    }
}
