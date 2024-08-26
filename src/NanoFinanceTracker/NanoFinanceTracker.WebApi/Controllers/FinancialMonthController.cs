using FluentValidation;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using NanoFinanceTracker.Core.Application.Dtos.Commands;
using NanoFinanceTracker.Core.Domain.Aggregates.FinancialMonthAgg;
using NanoFinanceTracker.Core.Framework.Orleans.GrainInterfaces;
using System.ComponentModel.DataAnnotations;
using System.Security.Claims;

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

        [Authorize]
        [HttpGet("{year}/{month}")]
        public async Task<IActionResult> Get(int year, int month)
        {
            var userId = GetUserId();
            if (userId == null)
            {
                return Unauthorized();
            }
            var grain = _clusterClient.GetGrain<IFinancialMonthGrain>(BuildGrainId(year, month, userId));
            
            return Ok(await grain.GetStateView());
        }

        [Authorize]
        [HttpPost("{year}/{month}/expenses")]
        public async Task<IActionResult> PostExpense([FromServices] IValidator<AddExpenseCommand> validator, int year, int month,[FromBody] AddExpenseCommand command)
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
            var grain = _clusterClient.GetGrain<IFinancialMonthGrain>(BuildGrainId(year, month, userId));
            await grain.AddExpense(command);
            return Ok(await grain.GetStateView());
        }

        [Authorize]
        [HttpPost("{year}/{month}/incomes")]
        public async Task<IActionResult> PostIncome([FromServices] IValidator<AddIncomeCommand> validator, int year, int month, [FromBody] AddIncomeCommand command)
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
            var grain = _clusterClient.GetGrain<IFinancialMonthGrain>(BuildGrainId(year, month, userId));
            await grain.AddIncome(command);
            return Ok(await grain.GetStateView());
        }

        private string? GetUserId()
        {
            return User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value;
        }

        private static string BuildGrainId(int year, int month, string userId)
        {
            return $"{year.ToString("0000")}-{month.ToString("00")}-{userId}";
        }
    }
}
