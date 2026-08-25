using MicroServices.Transfer.Domain.Models;

using System.Collections.Generic;
using System.Threading.Tasks;

namespace MicroServices.Transfer.Application.Interfaces {
    public interface ITransferService {
        Task<IEnumerable<TransferLog>> GetTransferLogsAsync();
    }
}
