using NanoFinanceTracker.Core.Application.Dtos.Commands;
using NanoFinanceTracker.Core.Application.Dtos.Views;
using NanoFinanceTracker.Core.Domain.Aggregates.FinanceMonthAgg;
using Orleans;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NanoFinanceTracker.Core.Framework.Orleans.GrainInterfaces
{
    public interface IFinanceMonthGrain : IGrainWithStringKey
    {
        Task AddExpense(AddExpenseCommand command);
        Task AddIncome(AddIncomeCommand command);
        Task<List<FinancialTransactionView>> GetFinancialTransactions();
        Task<FinanceMonthView> GetStateView();
    }
}
