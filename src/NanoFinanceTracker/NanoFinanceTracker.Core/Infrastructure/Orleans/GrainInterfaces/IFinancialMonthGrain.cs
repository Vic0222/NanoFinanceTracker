using Orleans;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NanoFinanceTracker.Core.Infrastructure.Orleans.GrainInterfaces
{
    public interface IFinancialMonthGrain : IGrainWithStringKey
    {
        Task AddExpense(int amount, string category, string description, DateTimeOffset transactionDate);

        //Task<int> GetTotalExpenses();
    }
}
