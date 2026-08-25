using MicroServices.Banking.Data.Context;
using MicroServices.Banking.Domain.Interfaces;
using MicroServices.Banking.Domain.Models;

using Microsoft.EntityFrameworkCore;

using System.Collections.Generic;
using System.Threading.Tasks;

namespace MicroServices.Banking.Data.Repository {
    public class AccountRepository : IAccountRepository {
        private BankingDbContext _bankingDbContext;
        public AccountRepository(BankingDbContext bankingDbContext) {
            _bankingDbContext = bankingDbContext;
        }
        public async Task<IEnumerable<Account>> GetAccountsAsync() {
            return await _bankingDbContext.Accounts.ToListAsync();
        }
    }
}
