using MediatR;

using MicroServices.Banking.Application.Interfaces;
using MicroServices.Banking.Application.Services;
using MicroServices.Banking.Data.Repository;
using MicroServices.Banking.Domain.CommandHandlers;
using MicroServices.Banking.Domain.Commands;
using MicroServices.Banking.Domain.Interfaces;
using MicroServices.Domain.Core.Bus;
using MicroServices.Infrastructure.Bus;
using MicroServices.Transfer.Application.Interfaces;
using MicroServices.Transfer.Application.Services;
using MicroServices.Transfer.Data.Repository;
using MicroServices.Transfer.Domain.EventHandlers;
using MicroServices.Transfer.Domain.Events;
using MicroServices.Transfer.Domain.Interfaces;

using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace MicroServices.Infrastructure.IOC {
    public class DependencyContainer {
        public static void RegisterServices(IServiceCollection services, IConfiguration configuration) {
            //Domain Bus
            var rabbitMqSection = configuration.GetSection("RabbitMQ");
            var rabbitMqOptions = new RabbitMQOptions {
                HostName = rabbitMqSection["HostName"] ?? "localhost",
                Port = int.TryParse(rabbitMqSection["Port"], out var port) ? port : 5672,
                UserName = rabbitMqSection["UserName"] ?? "guest",
                Password = rabbitMqSection["Password"] ?? "guest",
                VirtualHost = rabbitMqSection["VirtualHost"] ?? "/"
            };
            services.AddSingleton<IEventBus, RabbitMQBus>(serviceProvider => {
                var mediator = serviceProvider.GetService<IMediator>();
                var scopeFactory = serviceProvider.GetRequiredService<IServiceScopeFactory>();
                return new RabbitMQBus(mediator, scopeFactory, rabbitMqOptions);
            });

            //subscription
            services.AddTransient<TransferEventHandler>();
            services.AddTransient<IEventHandler<TransferCreatedEvent>, TransferEventHandler>();
            //Domain Banking Commands
            services.AddTransient<IRequestHandler<CreateTransferCommand, bool>, TransferCommandHandler>();

            //Application Services
            services.AddTransient<IAccountService, AccountService>();
            services.AddTransient<ITransferService, TransferService>();

            //Data
            services.AddTransient<IAccountRepository, AccountRepository>();
            services.AddTransient<ITransferRepository, TransferRepository>();
        }
    }
}
