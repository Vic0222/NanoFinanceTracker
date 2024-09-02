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
                FinanceMonthId = this.GetPrimaryKeyString(),
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
            (int year, int month, string userId) = ParseFinanceMonthId(this.GetPrimaryKeyString());

            Guard.IsGreaterThan(year, 0, "FinanceMonth - Year");
            Guard.IsBetween(month, 1, 12, "FinanceMonth - Month");
            Guard.IsNotNullOrEmpty(userId, "FinanceMonth - UserId");

            var state = new FinanceMonthState()
            {
                Year = year,
                Month = month,
                UserId = userId
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
        private static (int year, int month, string userId) ParseFinanceMonthId(string id)
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
