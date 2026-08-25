using MicroServices.Banking.Application.Interfaces;
using MicroServices.Banking.Application.Models;
using MicroServices.Banking.Domain.Models;

using Microsoft.AspNetCore.Mvc;

using System.Collections.Generic;
using System.Threading.Tasks;

namespace MicroServices.Banking.API.Controllers {
    [ApiController]
    [Route("[controller]")]
    public class BankingController : ControllerBase {
        private readonly IAccountService _accountService;

        public BankingController(IAccountService accountService) {
            _accountService = accountService;
        }

        [HttpGet]
        public Task<IEnumerable<Account>> Get() {
            return _accountService.GetAccountsAsync();
        }
        [HttpPost]
        public async Task<IActionResult> Post([FromBody] AccountTransfer accountTransfer) {
            await _accountService.TransferAsync(accountTransfer);
            return Ok();
        }
    }
}
