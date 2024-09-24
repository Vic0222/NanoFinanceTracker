using Marten.Events.Projections;
using NanoFinanceTracker.Core.Domain.Aggregates.FinanceMonthAgg;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NanoFinanceTracker.Core.Projections.AccountProjections
{
    public class AccountProjection : MultiStreamProjection<Account, string>
    {
        public AccountProjection()
        {
            Identity<IncomeAdded>(x => $"{x.UserId}-{x.Account}");
            Identity<ExpenseAdded>(x => $"{x.UserId}-{x.Account}");
        }

        public void Apply(IncomeAdded @event, Account view)
        {
            view.Id = $"{@event.UserId}-{@event.Account}";
            view.UserId = @event.UserId;
            view.AccountName = @event.Account;
        }

        public void Apply(ExpenseAdded @event, Account view)
        {
            view.Id = $"{@event.UserId}-{@event.Account}";
            view.UserId = @event.UserId;
            view.AccountName = @event.Account;
        }
    }
}
