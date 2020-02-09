using System;
using System.Collections.Generic;
using System.Reflection;

namespace TudaSuda
{
    public class CommandProcessorIndexer
    {
        public IDictionary<string, Type> Types { get; } = new Dictionary<string, Type>();

        public void RegisterLocalCommands(string namespaceFilter = null)
        {
            this.RegisterLocalCommands(Assembly.GetEntryAssembly(), namespaceFilter);
        }

        public void RegisterLocalCommands(Assembly assembly, string namespaceName = null)
        {
            string baseNamespaceName = assembly.GetName().Name ;

            if (!string.IsNullOrWhiteSpace(namespaceName))
            {
                baseNamespaceName += "." + namespaceName;
            }

            baseNamespaceName += ".";

            foreach (Type type in assembly.GetTypes())
            {
                TudaSudaCommand tudaSudaCommand = type.GetCustomAttribute<TudaSudaCommand>();

                if (!type.FullName.StartsWith(baseNamespaceName, StringComparison.InvariantCultureIgnoreCase))
                {
                    continue;
                }

                if (tudaSudaCommand == null)
                {
                    continue;
                }

                if (this.Types.ContainsKey(tudaSudaCommand.Route))
                {
                    continue;
                }

                this.Types.Add(tudaSudaCommand.Route, type);
            }
        }
    }
}