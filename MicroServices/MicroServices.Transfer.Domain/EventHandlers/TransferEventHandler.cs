using MicroServices.Domain.Core.Bus;
using MicroServices.Transfer.Domain.Events;
using MicroServices.Transfer.Domain.Interfaces;
using MicroServices.Transfer.Domain.Models;

using System.Threading.Tasks;

namespace MicroServices.Transfer.Domain.EventHandlers {
    public class TransferEventHandler : IEventHandler<TransferCreatedEvent> {
        private readonly ITransferRepository _transferRepository;
        public TransferEventHandler(ITransferRepository transferRepository) {
            _transferRepository = transferRepository;
        }
        public Task Handle(TransferCreatedEvent @event) {
            return _transferRepository.AddAsync(new TransferLog() {
                AccountFrom = @event.From,
                AccountTo = @event.To,
                TransferAmount = @event.Amount
            });
        }
    }
}
