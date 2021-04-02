using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Text;

namespace ModularityNET.Services
{
    public interface IModularityStartup
    {
        int Order { get; }
        void ConfigureServices(IServiceCollection services);
        void Configure(IApplicationBuilder appBuilder, IWebHostEnvironment env);
    }
}
