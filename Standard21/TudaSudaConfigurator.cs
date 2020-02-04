using System;
using System.Collections.Concurrent;
using System.Collections.Generic;

namespace TudaSuda
{
    public class TudaSudaConfigurator
    {
        public void RegisterHub<T>(Action<CommandProcessorIndexer> configuration)
        {
            CommandProcessorIndexer indexer = new CommandProcessorIndexer();
            configuration?.Invoke(indexer);

            TudaSudaHubs.Types.Add(typeof(T), indexer.Types);
            TudaSudaHubs.ConnectedUsers.Add(typeof(T), new ConcurrentDictionary<Guid, HashSet<string>>());
        }

        public void RegisterHub<T>()
        {
            CommandProcessorIndexer indexer = new CommandProcessorIndexer();
            indexer.RegisterLocalCommands();

            TudaSudaHubs.Types.Add(typeof(T), indexer.Types);
            TudaSudaHubs.ConnectedUsers.Add(typeof(T), new ConcurrentDictionary<Guid, HashSet<string>>());
        }
    }
}