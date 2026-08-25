using MicroServices.Transfer.Data.Context;
using MicroServices.Transfer.Domain.Interfaces;
using MicroServices.Transfer.Domain.Models;

using Microsoft.EntityFrameworkCore;

using System.Collections.Generic;
using System.Threading.Tasks;

namespace MicroServices.Transfer.Data.Repository {
    public class TransferRepository : ITransferRepository {
        private TransferDbContext _transferDbContext;
        public TransferRepository(TransferDbContext transferDbContext) {
            _transferDbContext = transferDbContext;
        }

        public async Task AddAsync(TransferLog transferLog) {
            await _transferDbContext.AddAsync(transferLog);
            await _transferDbContext.SaveChangesAsync();
        }

        public async Task<IEnumerable<TransferLog>> GetTransferLogsAsync() {
            return await _transferDbContext.TransferLogs.ToListAsync();
        }
    }
}
