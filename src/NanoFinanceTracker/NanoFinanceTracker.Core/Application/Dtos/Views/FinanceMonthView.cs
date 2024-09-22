using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NanoFinanceTracker.Core.Application.Dtos.Views
{
    [GenerateSerializer]
    public class FinanceMonthView
    {
        [Id(0)]
        public string UserId { get; set; } = string.Empty;

        [Id(6)]
        public string Account { get; set; } = string.Empty;

        [Id(1)]
        public int Month { get; set; }

        [Id(2)]
        public int Year { get; set; }

        [Id(3)]
        public decimal Balance { get; set; }

        [Id(4)]
        public decimal TotalExpense { get; set; }

        [Id(5)]
        public decimal TotalIncome { get; set; }
    }
}
