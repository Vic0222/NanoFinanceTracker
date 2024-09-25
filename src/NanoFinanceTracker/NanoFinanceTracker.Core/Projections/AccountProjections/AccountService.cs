using Marten;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NanoFinanceTracker.Core.Projections.AccountProjections
{
    public interface IAccountService
    {
        Task<IReadOnlyList<Account>> GetUserAccounts(string userId, CancellationToken cancellationToken = default);
    }

    public class AccountService : IAccountService
    {
        private readonly IDocumentSession _documentSession;

        public AccountService(IDocumentSession documentSession)
        {
            _documentSession = documentSession;
        }

        public async Task<IReadOnlyList<Account>> GetUserAccounts(string userId, CancellationToken cancellationToken = default)
        {
            return await _documentSession.Query<Account>().Where(a => a.UserId == userId).ToListAsync(cancellationToken);

        }
    }
}
