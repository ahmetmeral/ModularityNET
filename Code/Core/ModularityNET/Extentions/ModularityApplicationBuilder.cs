using Apiks.Modularity;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.Hosting;
using ModularityNET.Services;
using Newtonsoft.Json;
using System;

namespace ModularityNET.Extentions
{
    public static class ModularityApplicationBuilder
    {
        public static IApplicationBuilder UseModularity(this IApplicationBuilder app)
        {
            var env = app.ApplicationServices.GetRequiredService<IWebHostEnvironment>();
            var manifest = ModularityManifest.Get();

            //app.UseStaticFiles();

            if (manifest.UseEmbeddedFiles)
            {
                app.UseModularityStaticFiles();
            }

            app.UseModularityConfigureApplication();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapDefaultControllerRoute();
                endpoints.MapControllers();

                if (env.IsDevelopment())
                {
                    endpoints.UseModularityInfoEndpoints();
                }
            });

            return app;
        }

        public static void UseModularityStaticFiles(this IApplicationBuilder app, string requestPath = "/wwwroot")
        {
            if (String.IsNullOrWhiteSpace(requestPath))
                throw new ArgumentNullException("Request Path");

            var modularity = ModularityBuilder.Get();

            if (modularity.Areas.Count > 0)
            {
                var env = app.ApplicationServices.GetRequiredService<IWebHostEnvironment>();

                foreach (var area in modularity.Areas)
                {
                    foreach (var module in area.Modules)
                    {
                        if (env.IsDevelopment())
                        {
                            app.UseStaticFiles(new StaticFileOptions()
                            {
                                FileProvider = new EmbeddedFileProvider(module.Assembly, module.Assembly.GetName().Name + ".wwwroot"),
                                RequestPath = new PathString(requestPath),
                                //??bu compress olayı problem oluşturabilir kontrol edeceğiz
                                HttpsCompression = Microsoft.AspNetCore.Http.Features.HttpsCompressionMode.Compress,
                                OnPrepareResponse = (context) =>
                                {
                                    var headers = context.Context.Response.GetTypedHeaders();
                                    headers.CacheControl = new Microsoft.Net.Http.Headers.CacheControlHeaderValue
                                    {
                                        NoCache = true,
                                        NoStore = true,
                                        MaxAge = TimeSpan.FromDays(-1)
                                    };
                                }
                            });
                        }
                        else
                        {
                            app.UseStaticFiles(new StaticFileOptions()
                            {
                                FileProvider = new EmbeddedFileProvider(module.Assembly, module.Assembly.GetName().Name + ".wwwroot"),
                                RequestPath = new PathString(requestPath),
                                HttpsCompression = Microsoft.AspNetCore.Http.Features.HttpsCompressionMode.Compress,
                                OnPrepareResponse = (context) =>
                                {
                                    var headers = context.Context.Response.GetTypedHeaders();
                                    headers.CacheControl = new Microsoft.Net.Http.Headers.CacheControlHeaderValue
                                    {
                                        Public = true,
                                        MaxAge = TimeSpan.FromDays(30)
                                    };
                                }
                            });
                        }
                    }
                }
            }
        }

        public static void UseModularityInfoEndpoints(this IEndpointRouteBuilder endpoints)
        {
            endpoints.MapGet("/apiks/modularity", async context =>
            {
                var modularity = ModularityBuilder.Get();

                var settings = new JsonSerializerSettings
                {
                    ReferenceLoopHandling = Newtonsoft.Json.ReferenceLoopHandling.Ignore
                };

                var data = JsonConvert.SerializeObject(modularity, Formatting.Indented, settings);

                await context.Response.WriteAsync(data);
            });
        }

        public static void UseModularityConfigureApplication(this IApplicationBuilder app)
        {
            var env = app.ApplicationServices.GetRequiredService<IWebHostEnvironment>();
            var startups = app.ApplicationServices.GetServices<IModularityStartup>();

            foreach (var startup in startups)
            {
                startup.Configure(app, env);
            }
        }
    }
}
