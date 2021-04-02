using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;
using System;
using System.Collections.Generic;
using System.Text;

namespace ModularityNET.Extentions
{
    public static class ModularityProgramBuilder
    {
        public static IHostBuilder ConfigureModularity(this IHostBuilder builder)
        {
            ModularityManifest.Initialize();

            return builder;
        }

        public static IWebHostBuilder ConfigureModularity(this IWebHostBuilder builder)
        {
            ModularityManifest.Initialize();

            return builder;
        }
    }
}
