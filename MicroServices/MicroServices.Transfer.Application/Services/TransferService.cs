using MicroServices.Domain.Core.Bus;
using MicroServices.Transfer.Application.Interfaces;
using MicroServices.Transfer.Domain.Interfaces;
using MicroServices.Transfer.Domain.Models;

using System.Collections.Generic;
using System.Threading.Tasks;

namespace MicroServices.Transfer.Application.Services {
    public class TransferService : ITransferService {
        private readonly ITransferRepository _transferRepository;
        private readonly IEventBus _eventBus;

        public TransferService(ITransferRepository transferRepository, IEventBus eventBus) {
            _transferRepository = transferRepository;
            _eventBus = eventBus;
        }
        public Task<IEnumerable<TransferLog>> GetTransferLogsAsync() {
            return _transferRepository.GetTransferLogsAsync();
        }
    }
}
