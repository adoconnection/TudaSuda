using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.SignalR;

namespace TudaSuda
{
    public class TudaSudaTransmitter<T> where T : TudaSudaHub
    {
        private readonly IHubContext<T> hubContext;

        public TudaSudaTransmitter(IHubContext<T> hubContext)
        {
            this.hubContext = hubContext;
        }

        public Task Transmit(AppResponse response)
        {
            if (response is AppConnectionResponse)
            {
                return this.TransmitApp(response as AppConnectionResponse);
            }

            if (response is AppUsersResponse)
            {
                return this.TransmitApp(response as AppUsersResponse);
            }
            
            if (response is AppGroupResponse)
            {
                return this.TransmitApp(response as AppGroupResponse);
            }

            throw new NotSupportedException();
        }

        private Task TransmitApp(AppConnectionResponse response)
        {
            IClientProxy clientProxy = this.hubContext.Clients.Client(response.ConnectionId);

            return clientProxy.SendAsync("Receive", new
            {
                name = response.Name,
                commandGuid = response.CommandGuid,
                data = response.Data
            });
        }

        private Task TransmitApp(AppGroupResponse response)
        {
            IClientProxy clientProxy = this.hubContext.Clients.Client(response.GroupGuid.ToString());

            return clientProxy.SendAsync("Receive", new
            {
                name = response.Name,
                commandGuid = response.CommandGuid,
                data = response.Data
            });
        }

        private Task TransmitApp(AppUsersResponse response)
        {
            IList<Task> tasks = new List<Task>();

            foreach (Guid guid in response.UserGuids)
            {
                HashSet<string> connections = TudaSudaHubs.ConnectedUsers[typeof(T)].GetOrAdd(guid, x => new HashSet<string>());

                foreach (string connection in connections)
                {
                    tasks.Add(this.hubContext.Clients.Client(connection).SendAsync("Receive", new
                    {
                        name = response.Name,
                        commandGuid = response.CommandGuid,
                        data = response.Data
                    }));
                }
            }

            return Task.WhenAll(tasks);
        }
    }
}