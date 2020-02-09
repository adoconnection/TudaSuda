using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.DependencyInjection;

namespace TudaSuda
{
    public abstract class TudaSudaHub : Hub
    {
        private readonly IServiceProvider serviceProvider;

        private static ConcurrentDictionary<string, Guid> ConnectionsIndex { get; } = new ConcurrentDictionary<string, Guid>();

        protected abstract Guid? AuthorizeUser(string identity);
        protected abstract void OnUserDisconnected(Guid userGuid);
        protected abstract void OnUserConnected(Guid userGuid);
        protected abstract void OnAuthorized(Guid userGuid);

        protected TudaSudaHub(IServiceProvider serviceProvider)
        {
            this.serviceProvider = serviceProvider;
        }

        public async Task Send(dynamic message)
        {
            string name = (string)message.name;
            Guid? commandGuid = message.commandGuid;

            if (string.IsNullOrWhiteSpace(name))
            {
                return;
            }
            
            await ExecuteCommand(name, message.data, commandGuid);
        }

        protected Task ExecuteCommand(string name, object data = null, Guid? commandGuid = null)
        {
            IAppCommandProcessor commandProcessor = this.CreateCommandProcessor(name);

            if (commandProcessor == null)
            {
                IClientProxy clientProxy = this.Clients.Client(this.Context.ConnectionId);
                return clientProxy.SendAsync("noProcessor", name);
            }

            Guid? userGuid = null;

            if (ConnectionsIndex.TryGetValue(this.Context.ConnectionId, out Guid guid))
            {
                userGuid = guid;
            }

            return commandProcessor.Process(new AppCommandArgs()
            {
                    ConnectionId = this.Context.ConnectionId,
                    UserGuid = userGuid,
                    CommandGuid = commandGuid,
                    Data = data
            });
        }

        public Task Authorize(string identity)
        {
            if (string.IsNullOrWhiteSpace(identity))
            {
                return Task.CompletedTask;
            }

            Guid? guid = this.AuthorizeUser(identity);

            if (guid == null)
            {
                return this.Clients.Client(this.Context.ConnectionId).SendCoreAsync("notAuthorized", new object[] { });
            }

            if (ConnectionsIndex.TryGetValue(this.Context.ConnectionId, out Guid existingGuid))
            {
                return this.Clients.Client(this.Context.ConnectionId).SendCoreAsync("notAuthorized", new object[]{});
            }
            
            ConnectionsIndex.AddOrUpdate(this.Context.ConnectionId, s => guid.Value, (s, guid1) => guid.Value);

            HashSet<string> connections = TudaSudaHubs.ConnectedUsers[this.GetType()].GetOrAdd(guid.Value, valueFactory => new HashSet<string>());

            lock (connections)
            {
                connections.Add(this.Context.ConnectionId);

                if (connections.Count == 1)
                {
                    this.OnUserConnected(guid.Value);
                }
            }

            this.OnAuthorized(guid.Value);

            return this.Clients.Client(this.Context.ConnectionId).SendCoreAsync("authorized", new object[] { });
        }

        public override Task OnDisconnectedAsync(Exception exception)
        {
            if (!ConnectionsIndex.TryGetValue(this.Context.ConnectionId, out Guid userGuid))
            {
                return Task.CompletedTask;
            }

            HashSet<string> connections = TudaSudaHubs.ConnectedUsers[this.GetType()].GetOrAdd(userGuid, valueFactory => new HashSet<string>());

            lock (connections)
            {
                connections.Remove(this.Context.ConnectionId);

                if (connections.Count == 0)
                {
                    this.OnUserDisconnected(userGuid);
                }
            }

            return base.OnDisconnectedAsync(exception);
        }


        protected IAppCommandProcessor CreateCommandProcessor(string name)
        {
            IDictionary<string, Type> dictionary = TudaSudaHubs.Types[this.GetType()];

            if (!dictionary.ContainsKey(name))
            {
                return null;
            }

            return (IAppCommandProcessor)ActivatorUtilities.CreateInstance(this.serviceProvider, dictionary[name]);
        }
    }
}