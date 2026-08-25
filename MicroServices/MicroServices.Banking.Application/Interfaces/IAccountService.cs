using MicroServices.Banking.Application.Models;
using MicroServices.Banking.Domain.Models;

using System.Collections.Generic;
using System.Threading.Tasks;

namespace MicroServices.Banking.Application.Interfaces {
    public interface IAccountService {
        Task<IEnumerable<Account>> GetAccountsAsync();
        Task TransferAsync(AccountTransfer accountTransfer);
    }
}
