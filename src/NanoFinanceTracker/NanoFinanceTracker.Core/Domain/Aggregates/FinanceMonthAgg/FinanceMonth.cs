using CommunityToolkit.Diagnostics;
using Microsoft.CodeAnalysis.VisualBasic.Syntax;
using Microsoft.Extensions.Logging;
using NanoFinanceTracker.Core.Application.Dtos.Commands;
using NanoFinanceTracker.Core.Application.Dtos.Views;
using NanoFinanceTracker.Core.Domain.DomainInteraces;
using NanoFinanceTracker.Core.Framework.Orleans.GrainInterfaces;
using Npgsql.Internal;
using Orleans;
using Orleans.EventSourcing;
using Orleans.EventSourcing.CustomStorage;
using Orleans.Serialization.Codecs;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NanoFinanceTracker.Core.Domain.Aggregates.FinanceMonthAgg
{
    public class FinanceMonth : JournaledGrain<FinanceMonthState, IFinanceMonthEvent>, IAggregateRoot, IFinanceMonthGrain, ICustomStorageInterface<FinanceMonthState, IFinanceMonthEvent>
    {
        private readonly IAggregateRepository _aggregateRepository;
        private readonly ILogger<FinanceMonth> _logger;

        public FinanceMonth(IAggregateRepository aggregateRepository, ILogger<FinanceMonth> logger)
        {
            _aggregateRepository = aggregateRepository;
            _logger = logger;
        }

        public Task<FinanceMonthView> GetStateView()
        {
            var view = new FinanceMonthView()
            {
                Year = State.Year,
                Month = State.Month,
                Account = State.Account,
                Balance = State.Balance,
                TotalExpense = State.TotalExpense,
                TotalIncome = State.TotalIncome,
                UserId = State.UserId
            };
            return Task.FromResult(view);
        }

        public Task<List<FinancialTransactionView>> GetFinancialTransactions()
        {
            var breakDown = State.FinancialTransactions
                .Select(ft => new FinancialTransactionView
                {
                    Account = ft.Account,
                    Amount = ft.Amount,
                    Category = ft.Category,
                    CreatedAt = ft.CreatedAt,
                    Description = ft.Description,
                    TransactionDate = ft.TransactionDate,
                    TransactionType = ft.TransactionType
                }).ToList();

            return Task.FromResult(breakDown);
        }

        public async Task AddExpense(AddExpenseCommand command)
        {
            RaiseEvent(new ExpenseAdded() {
                FinanceMonthId = this.GetPrimaryKeyString(),
                Year = State.Year,
                Month = State.Month,
                UserId = State.UserId,
                Account = State.Account,
                Amount = command.Amount,
                Category = command.Category,
                Description = command.Description,
                TransactionDate = command.TransactionDate,
                CreatedAt = DateTimeOffset.Now
            });
            await ConfirmEvents();
        }

        public async Task AddIncome(AddIncomeCommand command)
        {
            RaiseEvent(new IncomeAdded()
            {
                FinanceMonthId = this.GetPrimaryKeyString(),
                Year = State.Year,
                Month = State.Month,
                UserId = State.UserId,
                Account = State.Account,
                Amount = command.Amount,
                Category = command.Category,
                Description = command.Description,
                TransactionDate = command.TransactionDate,
                CreatedAt = DateTimeOffset.Now
            });
            await ConfirmEvents();
        }

        public async Task<bool> ApplyUpdatesToStorage(IReadOnlyList<IFinanceMonthEvent> updates, int expectedVersion)
        {
            try
            {
                await _aggregateRepository.StoreAsync(this.GetPrimaryKeyString(), expectedVersion, updates);
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed saving events for {GraindId}", this.GetPrimaryKeyString());
                throw;
            }
        }

        public async Task<KeyValuePair<int, FinanceMonthState>> ReadStateFromStorage()
        {
            (int year, int month, string userId, string account) = ParseFinanceMonthId(this.GetPrimaryKeyString());

            Guard.IsGreaterThan(year, 0, "FinanceMonth - Year");
            Guard.IsBetween(month, 1, 12, "FinanceMonth - Month");
            Guard.IsNotNullOrEmpty(userId, "FinanceMonth - UserId");
            Guard.IsNotNullOrEmpty(account, "FinanceMonth - Account");

            var state = new FinanceMonthState()
            {
                Year = year,
                Month = month,
                UserId = userId,
                Account = account
            };

            var events = await _aggregateRepository.LoadEventsAsync<IFinanceMonthEvent>(this.GetPrimaryKeyString());
            if (!events.Any())
            {
                return new KeyValuePair<int, FinanceMonthState>(0, state);
            }

            var orderedEvents = events.OrderBy(e => e.version).ToList();

            foreach (var @event in orderedEvents)
            {
                switch (@event.@event)
                {
                    case ExpenseAdded expenseAdded:
                        state.Apply(expenseAdded);
                        break;
                    case IncomeAdded incomeAdded:
                        state.Apply(incomeAdded);
                        break;
                    default:
                        throw new ApplicationException($"Unrecognized event. {@event.@event.GetType().Name}");
                }
            }

            long version = orderedEvents.LastOrDefault().version;
            return new KeyValuePair<int, FinanceMonthState>((int)version, state);
        }


        public static (int year, int month, string userId, string account) ParseFinanceMonthId(string id)
        {

            if (string.IsNullOrEmpty(id) || id[0] != '#')
            {
                return (0, 0, string.Empty, string.Empty);
            }



            //sample id #4-2024#2-08#2-id#7-account
        
            int dataIndex = 0;
            char previousMark = '#';
            StringBuilder dataLengthString = new StringBuilder();
            int dataLength = 0;

            string strYear = string.Empty;
            string strMonth = string.Empty;
            string userId = string.Empty;
            string account = string.Empty;

            const char startDelimiter = '#';
            const char middleDelimiter = '-';

            for (int i = 0; i < id.Length; i++)
            {
                char current = id[i];
                if (previousMark == middleDelimiter && current != startDelimiter)
                {
                    break;
                }
                if (current == startDelimiter)
                {
                    previousMark = startDelimiter;
                    continue;
                }
                if (previousMark == startDelimiter && current == middleDelimiter)
                {
                    dataLength = int.Parse(dataLengthString.ToString());
                    dataLengthString.Clear();
                    var data = id.Substring(i + 1, dataLength);
                    i = i + dataLength;
                    switch (dataIndex)
                    {
                        case 0:
                            strYear = data ;
                            break;
                        case 1:
                            strMonth = data;
                            break;
                        case 2:
                            userId = data;
                            break;
                        case 3:
                            account = data;
                            break;
                        default:
                            break;
                    }
                    dataIndex++;
                    previousMark = middleDelimiter;
                    continue;
                }
                if (previousMark == startDelimiter)
                {
                    if (char.IsNumber(current))
                    {
                        dataLengthString.Append(current);
                    }
                    continue;
                }

            }

            

            int.TryParse(strYear, out int year);
            int.TryParse(strMonth, out int month);

            return (year, month, userId, account);
        }
    }

    
}
