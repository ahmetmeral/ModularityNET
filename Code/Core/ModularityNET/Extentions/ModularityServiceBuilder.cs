using Apiks.Modularity;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ApplicationParts;
using Microsoft.AspNetCore.Mvc.Razor.RuntimeCompilation;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.FileProviders;
using ModularityNET.Convention;
using ModularityNET.Services;
using System;
using System.Collections.Generic;
using System.Linq;

namespace ModularityNET.Extentions
{
    public static class ModularityServiceBuilder
    {
        public static IServiceCollection AddModularityMvc(this IServiceCollection services)
        {
            var manifest = ModularityManifest.Get();

            services.AddControllersWithViews();
            services.AddRazorPages();

            services
            .AddMvc()
            .SetCompatibilityVersion(Microsoft.AspNetCore.Mvc.CompatibilityVersion.Latest)
            .AddModularity()
            .AddModularityRazorRuntimeCompilation(manifest.IsDevelopment);

            services.AddModularityConventions();
            services.AddModularityEmbeddedFileProviders();
            services.AddModularityConfigureServices();

            return services;
        }

        public static IMvcBuilder AddModularity(this IMvcBuilder mvcBuilder)
        {
            var modularity = ModularityBuilder.Get();

            foreach (var area in modularity.Areas)
            {
                foreach (ModularityModule module in area.Modules)
                {
                    var partFactory = ApplicationPartFactory.GetApplicationPartFactory(module.Assembly);
                    foreach (var part in partFactory.GetApplicationParts(module.Assembly))
                    {
                        mvcBuilder.PartManager.ApplicationParts.Add(part);
                    }

                    var relatedAssemblies = RelatedAssemblyAttribute.GetRelatedAssemblies(module.Assembly, throwOnError: false);
                    foreach (var relatedAssembly in relatedAssemblies)
                    {
                        partFactory = ApplicationPartFactory.GetApplicationPartFactory(relatedAssembly);
                        foreach (var part in partFactory.GetApplicationParts(relatedAssembly))
                        {
                            mvcBuilder.PartManager.ApplicationParts.Add(part);
                        }
                    }
                }
            }

            return mvcBuilder;
        }

        public static IServiceCollection AddModularityConventions(this IServiceCollection services)
        {
            services.Configure<MvcOptions>(options =>
            {
                var modularity = ModularityBuilder.Get();

                if (modularity == null)
                {
                    throw new Exception("Modularity is null in AddMvc Options");
                }

                options.Conventions.Add(new ModularityRoutePrefixConvention(modularity));
                options.Conventions.Add(new ModularityAuthorizeByDefaultConvention(modularity));
            });

            return services;
        }

        public static IMvcBuilder AddModularityRazorRuntimeCompilation(this IMvcBuilder mvcBuilder, bool isDevelopment)
        {
            if (isDevelopment)
            {
                mvcBuilder.AddRazorRuntimeCompilation(options =>
                {
                    var modularity = ModularityBuilder.Get();

                    foreach (var area in modularity.Areas)
                    {
                        foreach (ModularityModule module in area.Modules)
                        {
                            options.FileProviders.Add(new PhysicalFileProvider(module.PhysicalFileDirectory));
                        }
                    }
                });
            }

            return mvcBuilder;
        }

        public static IServiceCollection AddModularityEmbeddedFileProviders(this IServiceCollection services)
        {
            var modularity = ModularityBuilder.Get();

            services.Configure<MvcRazorRuntimeCompilationOptions>(options =>
            {
                foreach (var area in modularity.Areas)
                {
                    foreach (ModularityModule module in area.Modules)
                    {
                        options.FileProviders.Add(new EmbeddedFileProvider(module.Assembly));
                    }
                }
            });

            return services;
        }

        public static IServiceCollection AddModularityConfigureServices(this IServiceCollection services)
        {
            var startupsTempory = new List<IModularityStartup>();
            var modularity = ModularityBuilder.Get();

            foreach (var area in modularity.Areas)
            {
                foreach (var module in area.Modules)
                {
                    var startupType = module.Assembly.GetTypes()
                       .FirstOrDefault(t => typeof(IModularityStartup).IsAssignableFrom(t));

                    if ((startupType != null) && (startupType != typeof(IModularityStartup)))
                    {
                        var startupInstance = (IModularityStartup)Activator.CreateInstance(startupType);
                        startupsTempory.Add(startupInstance);
                    }
                }
            }

            var startups = startupsTempory.OrderBy(s => s.Order).ToList();

            foreach (var startup in startups)
            {
                services.AddSingleton(typeof(IModularityStartup), startup);
                startup.ConfigureServices(services);
            }

            return services;
        }
    }
}
