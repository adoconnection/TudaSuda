using System;
using System.Linq;
using System.Reflection;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;

namespace TudaSuda
{
    public static class TudaSudaStartup
    {
        public static void UseTudaSuda(this IApplicationBuilder app, Action<TudaSudaConfigurator> configure)
        {
            TudaSudaConfigurator configurator = new TudaSudaConfigurator();
            configure?.Invoke(configurator);

            app.UseSignalR(routes =>
            {
                foreach (Type key in TudaSudaHubs.Types.Keys)
                {
                    MethodInfo mapHubMethod = routes.GetType().GetMethods().First( m => m.Name == "MapHub" );

                    MethodInfo makeGenericMethod = mapHubMethod.MakeGenericMethod(key);
                    makeGenericMethod.Invoke(routes, new object[] { new PathString("/" + key.Name) });
                }
            });
        }

        public static void AddTudaSuda(this IServiceCollection services)
        {
            services.AddSignalR(options =>
            {
                //options.JsonSerializerSettings.DateFormatHandling = DateFormatHandling.IsoDateFormat;
                //options.JsonSerializerSettings.DateTimeZoneHandling = DateTimeZoneHandling.Utc;
            });
        }
    }
}