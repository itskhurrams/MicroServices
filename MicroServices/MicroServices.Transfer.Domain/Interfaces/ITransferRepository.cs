using MicroServices.Transfer.Domain.Models;

using System.Collections.Generic;
using System.Threading.Tasks;

namespace MicroServices.Transfer.Domain.Interfaces {
    public interface ITransferRepository {
        Task<IEnumerable<TransferLog>> GetTransferLogsAsync();
        Task AddAsync(TransferLog transferLog);
    }
}
