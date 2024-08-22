using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using NanoFinanceTracker.Core.Domain.Aggregates.FinancialMonthAgg;
using NanoFinanceTracker.Core.Infrastructure.Orleans.GrainInterfaces;

namespace NanoFinanceTracker.WebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class FinancesController : ControllerBase
    {
        private readonly IClusterClient _clusterClient;

        public FinancesController(IClusterClient clusterClient)
        {
            _clusterClient = clusterClient;
        }

        [HttpGet]
        public async Task<IActionResult> Get()
        {
            var grain = _clusterClient.GetGrain<IFinancialMonthGrain>("2024-08-userid");
            return Ok(grain.GetGrainId());
        }

        [HttpPost]
        public async Task<IActionResult> Post()
        {
            var grain = _clusterClient.GetGrain<IFinancialMonthGrain>("2024-08-userid");
            await grain.AddExpense(100000, "test", "test", DateTimeOffset.Now);
            return Ok(grain.GetGrainId());
        }
    }
}
