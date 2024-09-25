using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using NanoFinanceTracker.Core.Projections.AccountProjections;
using System.Security.Claims;

namespace NanoFinanceTracker.WebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AccountsController : ControllerBase
    {
        private readonly IAccountService _accountService;

        public AccountsController(IAccountService accountService)
        {
            _accountService = accountService;
        }

        [HttpGet]
        [Authorize]
        public async Task<IActionResult> GetAccounts(CancellationToken cancellationToken)
        {
            var userId = GetUserId();
            if (userId == null)
            {
                return Unauthorized();
            }
            var accounts = await _accountService.GetUserAccounts(userId, cancellationToken);
            return Ok(accounts);
        }

        private string? GetUserId()
        {
            return User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value;
        }
    }
}
