using System;
using Newtonsoft.Json;

namespace TudaSuda
{
    public abstract class AppResponse
    {
        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("commandGuid")]
        public Guid? CommandGuid { get; set; }

        [JsonProperty("data")]
        public dynamic Data { get; set; }
    }
}