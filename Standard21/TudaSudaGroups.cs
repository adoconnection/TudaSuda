using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.SignalR;

namespace TudaSuda
{
    public class TudaSudaGroups<T> where T : TudaSudaHub
    {
        private readonly IHubContext<T> hubContext;

        public TudaSudaGroups(IHubContext<T> hubContext)
        {
            this.hubContext = hubContext;
        }

        public Task Join(string connectionId, Guid groupGuid)
        {
            return this.hubContext.Groups.AddToGroupAsync(connectionId, groupGuid.ToString());
        }

        public Task Leave(string connectionId, Guid groupGuid)
        {
            return this.hubContext.Groups.RemoveFromGroupAsync(connectionId, groupGuid.ToString());
        }
    }
}