using System;

namespace TudaSuda
{
    public class TudaSudaCommand : Attribute
    {
        public string Route { get; set; }
    }
}