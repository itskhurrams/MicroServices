using MediatR;

using MicroServices.Domain.Core.Bus;
using MicroServices.Domain.Core.Commands;
using MicroServices.Domain.Core.Events;

using Microsoft.Extensions.DependencyInjection;

using Newtonsoft.Json;

using RabbitMQ.Client;
using RabbitMQ.Client.Events;

using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace MicroServices.Infrastructure.Bus {
    public sealed class RabbitMQBus : IEventBus, IAsyncDisposable {
        private readonly IMediator _mediator;
        private readonly Dictionary<string, List<Type>> _handlers;
        private readonly List<Type> _eventTypes;
        private readonly IServiceScopeFactory _serviceScopeFactory;
        private readonly RabbitMQOptions _options;
        private readonly SemaphoreSlim _connectionLock = new(1, 1);
        private readonly ConcurrentBag<IChannel> _consumerChannels = new();
        private IConnection _connection;

        public RabbitMQBus(IMediator mediator, IServiceScopeFactory serviceScopeFactory, RabbitMQOptions options) {
            _mediator = mediator;
            _handlers = new Dictionary<string, List<Type>>();
            _eventTypes = new List<Type>();
            _serviceScopeFactory = serviceScopeFactory;
            _options = options;
        }
        public Task SendCommand<T>(T command) where T : Command {
            return _mediator.Send(command);
        }
        public async Task Publish<T>(T @event) where T : Event {
            var connection = await GetConnectionAsync().ConfigureAwait(false);
            using (IChannel channel = await connection.CreateChannelAsync().ConfigureAwait(false)) {
                string eventName = @event.GetType().Name;
                await channel.QueueDeclareAsync(eventName, false, false, false, null).ConfigureAwait(false);
                var message = JsonConvert.SerializeObject(@event);
                var body = Encoding.UTF8.GetBytes(message);
                await channel.BasicPublishAsync("", eventName, body).ConfigureAwait(false);
            }
        }
        public async Task Subscribe<T, TH>()
            where T : Event
            where TH : IEventHandler<T> {
            var eventName = typeof(T).Name;
            var handlerType = typeof(TH);
            if (!_eventTypes.Contains(typeof(T))) {
                _eventTypes.Add(typeof(T));
            }
            if (!_handlers.ContainsKey(eventName)) {
                _handlers.Add(eventName, new List<Type>());
            }
            if (_handlers[eventName].Any(s => s == handlerType)) {
                throw new ArgumentException($"Handler Type {handlerType.Name} already registered for '{eventName}'", nameof(handlerType));
            }
            _handlers[eventName].Add(handlerType);
            await StartBasicConsume<T>().ConfigureAwait(false);
        }

        private async Task<IConnection> GetConnectionAsync() {
            if (_connection is { IsOpen: true }) {
                return _connection;
            }
            await _connectionLock.WaitAsync().ConfigureAwait(false);
            try {
                if (_connection is { IsOpen: true }) {
                    return _connection;
                }
                var connectionFactory = new ConnectionFactory {
                    HostName = _options.HostName,
                    Port = _options.Port,
                    UserName = _options.UserName,
                    Password = _options.Password,
                    VirtualHost = _options.VirtualHost
                };
                _connection = await connectionFactory.CreateConnectionAsync().ConfigureAwait(false);
                return _connection;
            }
            finally {
                _connectionLock.Release();
            }
        }

        private async Task StartBasicConsume<T>() where T : Event {
            var connection = await GetConnectionAsync().ConfigureAwait(false);
            IChannel channel = await connection.CreateChannelAsync().ConfigureAwait(false);
            string eventName = typeof(T).Name;
            await channel.QueueDeclareAsync(eventName, false, false, false, null).ConfigureAwait(false);
            AsyncEventingBasicConsumer consumer = new AsyncEventingBasicConsumer(channel);
            consumer.ReceivedAsync += Consumer_Received;
            await channel.BasicConsumeAsync(eventName, true, consumer).ConfigureAwait(false);
            _consumerChannels.Add(channel);
        }
        private async Task Consumer_Received(object sender, BasicDeliverEventArgs e) {
            string eventName = e.RoutingKey;
            string message = Encoding.UTF8.GetString(e.Body.ToArray());
            await ProcessEvent(eventName, message).ConfigureAwait(false);
        }
        private async Task ProcessEvent(string eventName, string message) {
            if (_handlers.ContainsKey(eventName)) {
                using (var scope = _serviceScopeFactory.CreateScope()) {
                    var subscriptions = _handlers[eventName];
                    foreach (var subscription in subscriptions) {
                        var handler = scope.ServiceProvider.GetService(subscription);
                        if (handler == null) continue;
                        var eventType = _eventTypes.SingleOrDefault(t => t.Name == eventName);
                        var @event = JsonConvert.DeserializeObject(message, eventType);
                        var concretetype = typeof(IEventHandler<>).MakeGenericType(eventType);
                        await (Task)concretetype.GetMethod("Handle").Invoke(handler, new object[] { @event });
                    }
                }

            }
        }

        public async ValueTask DisposeAsync() {
            foreach (var channel in _consumerChannels) {
                if (channel.IsOpen) {
                    await channel.CloseAsync().ConfigureAwait(false);
                }
                channel.Dispose();
            }
            if (_connection is { IsOpen: true }) {
                await _connection.CloseAsync().ConfigureAwait(false);
            }
            _connection?.Dispose();
            _connectionLock.Dispose();
        }
    }
}
