using FluentValidation;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using NanoFinanceTracker.Core.Application.Dtos.Commands;
using NanoFinanceTracker.Core.Domain.Aggregates.FinanceMonthAgg;
using NanoFinanceTracker.Core.Framework.Orleans.GrainInterfaces;
using System.ComponentModel.DataAnnotations;
using System.Security.Claims;

namespace NanoFinanceTracker.WebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class FinanceMonthController : ControllerBase
    {
        private readonly IClusterClient _clusterClient;

        public FinanceMonthController(IClusterClient clusterClient)
        {
            _clusterClient = clusterClient;
        }

        [Authorize]
        [HttpGet("{account}/{year}/{month}")]
        public async Task<IActionResult> Get(string account, int year, int month)
        {
            var userId = GetUserId();
            if (userId == null)
            {
                return Unauthorized();
            }
            var grain = _clusterClient.GetGrain<IFinanceMonthGrain>(BuildGrainId(year, month, userId, account));
            
            return Ok(await grain.GetStateView());
        }

        [Authorize]
        [HttpGet("{account}/{year}/{month}/transactions")]
        public async Task<IActionResult> GetTransactions(string account, int year, int month)
        {
            var userId = GetUserId();
            if (userId == null)
            {
                return Unauthorized();
            }
            var grain = _clusterClient.GetGrain<IFinanceMonthGrain>(BuildGrainId(year, month, userId, account));

            return Ok(await grain.GetFinancialTransactions());
        }

        [Authorize]
        [HttpPost("{account}/{year}/{month}/expenses")]
        public async Task<IActionResult> PostExpense([FromServices] IValidator<AddExpenseCommand> validator, string account, int year, int month,[FromBody] AddExpenseCommand command)
        {
            var validationResult = await validator.ValidateAsync(command);
            if (!validationResult.IsValid)
            {
                validationResult.AddToModelState(ModelState);
                return this.ValidationProblem(ModelState);
            }
            var userId = GetUserId();
            if (userId == null)
            {
                return Unauthorized();
            }
            var grain = _clusterClient.GetGrain<IFinanceMonthGrain>(BuildGrainId(year, month, userId, account));
            await grain.AddExpense(command);
            return Ok(await grain.GetStateView());
        }

        [Authorize]
        [HttpPost("{account}/{year}/{month}/incomes")]
        public async Task<IActionResult> PostIncome([FromServices] IValidator<AddIncomeCommand> validator, string account, int year, int month, [FromBody] AddIncomeCommand command)
        {
            var validationResult = await validator.ValidateAsync(command);
            if (!validationResult.IsValid)
            {
                validationResult.AddToModelState(ModelState);
                return ValidationProblem(ModelState);
            }
            var userId = GetUserId();
            if (userId == null)
            {
                return Unauthorized();
            }
            var grain = _clusterClient.GetGrain<IFinanceMonthGrain>(BuildGrainId(year, month, userId, account));
            await grain.AddIncome(command);
            return Ok(await grain.GetStateView());
        }

        private string? GetUserId()
        {
            return User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value;
        }

        private static string BuildGrainId(int year, int month, string userId, string account)
        {
            return $"#4-{year.ToString("0000")}#2-{month.ToString("00")}#{userId.Length}-{userId}#{account.Length}-{account.ToLower()}";
        }
    }
}
