using System;

namespace TudaSuda
{
    public class AppCommandArgs
    {
        public string ConnectionId { get; set; }
        public Guid? UserGuid { get; set; }
        public Guid? CommandGuid { get; set; }
        public dynamic Data { get; set; }
    }
}