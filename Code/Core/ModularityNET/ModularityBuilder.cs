using Apiks.Modularity.Options;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.Loader;
using System.Text;

namespace ModularityNET
{
    public class ModularityBuilder
    {
        private static ModularityContext Context { get; set; } = null;

        public static void Initialize()
        {
            Context = new ModularityContext();

            var modularity = ModularityManifest.Get();


            if (modularity.Default != null && modularity.Default.Running)
            {
                BuildArea(ref modularity, new ModularityArea(modularity.Default, true));
            }

            if (modularity.Areas != null)
            {
                foreach (AreaOptions areaConfig in modularity.Areas)
                {
                    if (!areaConfig.Running)
                    {
                        continue;
                    }

                    BuildArea(ref modularity, new ModularityArea(areaConfig));
                }
            }

            Context.ContentDirectory = Path.Combine(modularity.ContentDirectory.ToArray());
            Context.Build();
        }

        public static ModularityContext Get()
        {
            if (Context == null)
            {
                Initialize();
            }

            return Context;
        }

        private static void BuildArea(ref ModularityManifest modularity, ModularityArea area)
        {
            foreach (string module_id in area.Options.Modules)
            {
                var moduleConfig = modularity.Modules.Where(m => m.Id == module_id).FirstOrDefault();
                //todl: aynı name space in tekrar yüklenmesi durumu olabilir mi?

                if (moduleConfig == null)
                {
                    throw new Exception($"Module not found : {module_id}");
                }

                var directory = GetAssemblyDirectory(moduleConfig, modularity.WorkingDirectory);
                var directoryInfo = new DirectoryInfo(directory);
                var module = new ModularityModule(moduleConfig, modularity.ContentDirectory);

                if (!moduleConfig.Bundle && directoryInfo.Exists)
                {
                    FileSystemInfo[] files = directoryInfo.GetFileSystemInfos("*.dll", SearchOption.AllDirectories);

                    foreach (FileSystemInfo file in files)
                    {
                        //RequestPathPrefix not affected
                        if (Context.TryAddGetFileAlreadyLoad(file.Name))
                        {
                            continue;
                        }

                        Assembly assembly;

                        try
                        {
                            assembly = AssemblyLoadContext.Default.LoadFromAssemblyPath(file.FullName);
                        }
                        catch (FileLoadException)
                        {
                            assembly = Assembly.Load(new AssemblyName(Path.GetFileNameWithoutExtension(file.Name)));

                            if (assembly == null)
                            {
                                throw;
                            }

                            string loadedAssemblyVersion = FileVersionInfo.GetVersionInfo(assembly.Location).FileVersion;
                            string tryToLoadAssemblyVersion = FileVersionInfo.GetVersionInfo(file.FullName).FileVersion;
                           
                            if (tryToLoadAssemblyVersion != loadedAssemblyVersion)
                            {
                                throw new Exception($"Cannot load {file.FullName} {tryToLoadAssemblyVersion} because {assembly.Location} {loadedAssemblyVersion} has been loaded");
                            }
                        }

                        var id = Path.GetFileNameWithoutExtension(assembly.ManifestModule.Name);

                        if (id == moduleConfig.Id)
                        {
                            module.Assembly = assembly;
                        }
                    }
                }
                else
                {
                    module.Assembly = Assembly.Load(new AssemblyName(moduleConfig.Id));
                }

                if (module.Assembly == null)
                {
                    throw new Exception($"Cannot find main assembly for module {moduleConfig.Id}");
                }
                else
                {
                    module.Options.Installed = true;
                }

                area.AddModule(module);
            }

            Context.AddArea(area);

            if (area.IsDefaultArea)
            {
                Context.DefaultArea = area;
            }
        }

        private static string GetAssemblyDirectory(ModuleOptions module, string[] workingDirectory)
        {
            var paths = new List<string>
            {
                AppContext.BaseDirectory
            };

            foreach (var path in workingDirectory)
            {
                if (path == "@Id")
                {
                    paths.Add(module.Id);
                }
                else
                {
                    paths.Add(path);
                }
            }

            return Path.Combine(paths.ToArray());
        }
    }
}
