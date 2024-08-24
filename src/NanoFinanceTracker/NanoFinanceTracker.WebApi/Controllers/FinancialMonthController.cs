using FluentValidation;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using NanoFinanceTracker.Core.Application.Dtos.Commands;
using NanoFinanceTracker.Core.Domain.Aggregates.FinancialMonthAgg;
using NanoFinanceTracker.Core.Framework.Orleans.GrainInterfaces;

namespace NanoFinanceTracker.WebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class FinancialMonthController : ControllerBase
    {
        private readonly IClusterClient _clusterClient;

        public FinancialMonthController(IClusterClient clusterClient)
        {
            _clusterClient = clusterClient;
        }

        [HttpGet("{year}/{month}")]
        public async Task<IActionResult> Get(int year, int month)
        {
            var grain = _clusterClient.GetGrain<IFinancialMonthGrain>(BuildGrainId(year, month));
            
            return Ok(await grain.GetBalance());
        }

        [HttpPost("{year}/{month}/expenses")]
        public async Task<IActionResult> PostExpense([FromServices] IValidator<AddExpense> validator, int year, int month,[FromBody] AddExpense command)
        {
            var validationResult = await validator.ValidateAsync(command);
            if (!validationResult.IsValid)
            {
                validationResult.AddToModelState(ModelState);
                return this.ValidationProblem(ModelState);
            }
            var grain = _clusterClient.GetGrain<IFinancialMonthGrain>(BuildGrainId(year, month));
            await grain.AddExpense(command.Amount, command.Category, command.Description, command.TransactionDate);
            return Ok(await grain.GetBalance());
        }

        [HttpPost("{year}/{month}/incomes")]
        public async Task<IActionResult> PostIncome(int year, int month)
        {
            var grain = _clusterClient.GetGrain<IFinancialMonthGrain>(BuildGrainId(year, month));
            await grain.AddIncome(500000, "test", "test", DateTimeOffset.Now);
            return Ok(await grain.GetBalance());
        }

        private static string BuildGrainId(int year, int month)
        {
            return $"{year.ToString("0000")}-{month.ToString("00")}-userid";
        }
    }
}
