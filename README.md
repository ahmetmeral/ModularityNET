# ModularityNET .NET Core Mvc Modularity

Modularity is an open soruce AspNetCore Mvc Modularity library.


## Get Started

> ```Install-Package ModularityNET -Version 0.0.1-alpha-4```

###### modularity.json (Samples/App_Data/Modularity)

```json
{
  "WorkingDirectory": [ "App_Data", "Modules", "@Id", "netcoreapp3.1" ],
  "ContentDirectory": [ "..", "..", "Modules" ],
  "Default": {
    "Name": "Default",
    "Running": true,
    "Modules": [
      "Apiks.Module.App",
      "Apiks.Module.Shared"
    ]
  },
  "Areas": [
    {
      "Name": "accounts",
      "Running": true,
      "AuthorizeByDefault": false,
      "RequestPathPrefix": "accounts",
      "Modules": [ "Apiks.Module.Accounts" ]
    },
    {
      "Name": "auth",
      "Running": true,
      "AuthorizeByDefault": false,
      "RequestPathPrefix": "auth",
      "Modules": [ "Apiks.Module.Auth" ]
    },
    {
      "Name": "api",
      "Running": true,
      "AuthorizeByDefault": false,
      "RequestPathPrefix": "api",
      "Modules": [
        "Apiks.Module.Auth.Api"
      ]
    }
  ],
  "Modules": [
    {
      "Id": "Apiks.Module.App",
      "Name": "App",
      "Version": "1.0.0",
      "Bundle": true
    },
    {
      "Id": "Apiks.Module.Shared",
      "Name": "Shared",
      "Version": "1.0.0",
      "Bundle": true,
      "Installed": false
    },
    {
      "Id": "Apiks.Module.Accounts",
      "Name": "CoreApi",
      "Version": "1.0.0",
      "Bundle": true
    },
    {
      "Id": "Apiks.Module.Auth",
      "Name": "Auth",
      "Version": "1.0.0",
      "Bundle": true
    },
    {
      "Id": "Apiks.Module.Auth.Api",
      "Name": "AuthApi",
      "Version": "1.0.0",
      "Bundle": true
    }
  ]
}
```


###### Program.cs (Samples/Apiks.Web.Core)

```csharp
public class Program
{
    public static void Main(string[] args)
    {
        CreateHostBuilder(args).Build().Run();
    }

    public static IHostBuilder CreateHostBuilder(string[] args) =>
        Host.CreateDefaultBuilder(args)
            //modularity manifest init
            .ConfigureModularity()
            .ConfigureWebHostDefaults(webBuilder =>
            {
                webBuilder.UseStartup<Startup>();
            });
}
```


###### Startup.cs (Samples/Apiks.Web.Core)

```csharp

public class Startup
{
    public IConfiguration Configuration { get; }
    public IWebHostEnvironment HostEnvironment { get; }

    public Startup(IConfiguration configuration, IWebHostEnvironment env)
    {
        Configuration = configuration;
        HostEnvironment = env;
    }

    public void ConfigureServices(IServiceCollection services)
    {
        services.AddModularityMvc(HostEnvironment.IsDevelopment());
    }

    public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
    {
        if (env.IsDevelopment())
        {
            app.UseDeveloperExceptionPage();
        }
        
        app.UseModularityMvc();
    }
}

```