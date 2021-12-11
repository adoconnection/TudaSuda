# TudaSuda
C# SignalR Command/Reply framework

Its like controllers for websockets

Imagine you call this JS code on client side:

```javascript

uplink.send({
    name: 'App/Organization/Sessions/List',
    data:
    {
        organizationGuid: $stateParams.organizationGuid
    }
});

uplink.on('App/Organization/Sessions/List', function(data, commandGuid) {
    
    $scope.sessions = data;
    
});

```

Process it on server like this:
```cs
[TudaSudaCommand(Route = "App/Organization/Sessions/List")]
public class List : AppCommandProcessor
{
    public List(InfraredEntities entities, TudaSudaTransmitter<InfraredAppHub> appHubTransmitter) : base(entities, appHubTransmitter)
    {
    }

    protected override Task Process(AppCommandArgs message)
    {
        Guid? organizationGuid = message?.Data?.organizationGuid;
        
        return this.TransmitConnection(
              "App/Organization/Sessions/List",
              this.Entities.Sessions
                  .Where(s => s.OrganizationGuid == organizationGuid)
                  .Select(SessionFormatter.Details)
                  .ToList()
        );
    }
}
```

with just a few lines of config
```cs
public class Startup
{
    public void ConfigureServices(IServiceCollection services)
    {
        services.AddTudaSuda();
    }

    public void Configure(IApplicationBuilder app, IHostingEnvironment env)
    {
        app.UseTudaSuda();
    }
}
```


Isn't it great? 
Now you can!
