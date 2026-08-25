using MicroServices.Banking.Domain.Models;

using System.Collections.Generic;
using System.Threading.Tasks;

namespace MicroServices.Banking.Domain.Interfaces {
    public interface IAccountRepository {
        Task<IEnumerable<Account>> GetAccountsAsync();
    }
}
