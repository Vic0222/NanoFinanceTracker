using NanoFinanceTracker.Core.Application.Dtos.Commands;
using NanoFinanceTracker.Core.Application.Dtos.Views;
using NanoFinanceTracker.Core.Domain.Aggregates.FinancialMonthAgg;
using Orleans;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NanoFinanceTracker.Core.Framework.Orleans.GrainInterfaces
{
    public interface IFinancialMonthGrain : IGrainWithStringKey
    {
        Task AddExpense(AddExpenseCommand command);
        Task AddIncome(AddIncomeCommand command);
        Task<FinancialMonthView> GetStateView();
    }
}
