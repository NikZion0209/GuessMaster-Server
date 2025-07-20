using Microsoft.Extensions.Hosting;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using GuessMaster.Service.Interface;

namespace GuessMaster.Service.Event_Handlers
{
    public class EventHostedService : IHostedService
    {
        private readonly IEnumerable<IEventHandler> _handlers;

        public EventHostedService(IEnumerable<IEventHandler> handlers)
        {
            _handlers = handlers;
        }

        public Task StartAsync(CancellationToken cancellationToken)
        {
            foreach (var handler in _handlers)
                handler.Subscribe();

            return Task.CompletedTask;
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            foreach (var handler in _handlers)
                handler.Unsubscribe();

            return Task.CompletedTask;
        }
    }
}