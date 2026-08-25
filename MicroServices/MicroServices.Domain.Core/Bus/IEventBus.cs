using MicroServices.Domain.Core.Commands;
using MicroServices.Domain.Core.Events;

using System.Threading.Tasks;

namespace MicroServices.Domain.Core.Bus {
    public interface IEventBus {
        Task SendCommand<T>(T command) where T : Command;
        Task Publish<T>(T @event) where T : Event;

        Task Subscribe<T, TH>()
            where T : Event
            where TH : IEventHandler<T>;
    }
}

