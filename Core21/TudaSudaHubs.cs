using System;
using System.Collections.Concurrent;
using System.Collections.Generic;

namespace TudaSuda
{
    public class TudaSudaHubs
    {
        public static IDictionary<Type, IDictionary<string, Type>> Types = new ConcurrentDictionary<Type, IDictionary<string, Type>>();
        public static IDictionary<Type, ConcurrentDictionary<Guid, HashSet<string>>> ConnectedUsers = new ConcurrentDictionary<Type, ConcurrentDictionary<Guid, HashSet<string>>>();
    }
}