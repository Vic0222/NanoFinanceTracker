using CommunityToolkit.Diagnostics;
using Microsoft.Extensions.Logging;
using NanoFinanceTracker.Core.Application.Dtos.Commands;
using NanoFinanceTracker.Core.Application.Dtos.Views;
using NanoFinanceTracker.Core.Domain.DomainInteraces;
using NanoFinanceTracker.Core.Framework.Orleans.GrainInterfaces;
using Orleans;
using Orleans.EventSourcing;
using Orleans.EventSourcing.CustomStorage;
using Orleans.Serialization.Codecs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NanoFinanceTracker.Core.Domain.Aggregates.FinancialMonthAgg
{
    public class FinancialMonth : JournaledGrain<FinancialMonthState, IFinancialMonthEvent>, IAggregateRoot, IFinancialMonthGrain, ICustomStorageInterface<FinancialMonthState, IFinancialMonthEvent>
    {
        private readonly IAggregateRepository _aggregateRepository;
        private readonly ILogger<FinancialMonth> _logger;

        public FinancialMonth(IAggregateRepository aggregateRepository, ILogger<FinancialMonth> logger)
        {
            _aggregateRepository = aggregateRepository;
            _logger = logger;
        }

        public Task<FinancialMonthView> GetStateView()
        {
            var view = new FinancialMonthView()
            {
                Year = State.Year,
                Month = State.Month,
                Balance = State.Balance,
                TotalExpense = State.TotalExpense,
                TotalIncome = State.TotalIncome,
                UserId = State.UserId
            };
            return Task.FromResult(view);
        }

        public async Task AddExpense(AddExpenseCommand command)
        {
            RaiseEvent(new ExpenseAdded() {
                FinancialMonthId = this.GetPrimaryKeyString(),
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
                FinancialMonthId = this.GetPrimaryKeyString(),
                Amount = command.Amount,
                Category = command.Category,
                Description = command.Description,
                TransactionDate = command.TransactionDate,
                CreatedAt = DateTimeOffset.Now
            });
            await ConfirmEvents();
        }

        public async Task<bool> ApplyUpdatesToStorage(IReadOnlyList<IFinancialMonthEvent> updates, int expectedVersion)
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

        public async Task<KeyValuePair<int, FinancialMonthState>> ReadStateFromStorage()
        {
            (int year, int month, string userId) = ParseFinancialMonthId(this.GetPrimaryKeyString());

            Guard.IsGreaterThan(year, 0, "FinancialMonth - Year");
            Guard.IsBetween(month, 1, 12, "FinancialMonth - Month");
            Guard.IsNotNullOrEmpty(userId, "FinancialMonth - UserId");

            var state = new FinancialMonthState()
            {
                Year = year,
                Month = month,
                UserId = userId
            };

            var events = await _aggregateRepository.LoadEventsAsync<IFinancialMonthEvent>(this.GetPrimaryKeyString());
            if (!events.Any())
            {
                return new KeyValuePair<int, FinancialMonthState>(0, state);
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
            return new KeyValuePair<int, FinancialMonthState>((int)version, state);
        }
        private static (int year, int month, string userId) ParseFinancialMonthId(string id)
        {
            
            if (id.Length < 9)
            {
                return (0, 0, string.Empty);
            }

            //sample id 2024-08-id
            var idSpan = id.AsSpan();
            var strYear = idSpan.Slice(0, 4);
            var strMonth = idSpan.Slice(5, 2);
            var userId = idSpan.Slice(8, idSpan.Length - 8).ToString();

            int.TryParse(strYear, out int year);    
            int.TryParse(strMonth, out int month);

            return (year, month, userId);
        }
    }

    
}
