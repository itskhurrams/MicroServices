using MicroServices.Banking.Application.Interfaces;
using MicroServices.Banking.Application.Models;
using MicroServices.Banking.Domain.Commands;
using MicroServices.Banking.Domain.Interfaces;
using MicroServices.Banking.Domain.Models;
using MicroServices.Domain.Core.Bus;

using System.Collections.Generic;
using System.Threading.Tasks;

namespace MicroServices.Banking.Application.Services {
    public class AccountService : IAccountService {
        private readonly IAccountRepository _accountRepository;
        private readonly IEventBus _eventBus;

        public AccountService(IAccountRepository accountRepository, IEventBus eventBus) {
            _accountRepository = accountRepository;
            _eventBus = eventBus;
        }
        public Task<IEnumerable<Account>> GetAccountsAsync() {
            return _accountRepository.GetAccountsAsync();
        }

        public Task TransferAsync(AccountTransfer accountTransfer) {
            var transferCommand = new CreateTransferCommand(accountTransfer.FromAccount, accountTransfer.ToAccount, accountTransfer.TransferAmount);
            return _eventBus.SendCommand(transferCommand);
        }
    }
}
