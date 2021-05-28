using System;
using System.Collections.Generic;
using System.IO;
using Newtonsoft.Json;
using Apiks.Modularity.Options;

namespace ModularityNET
{
    public class ModularityManifest
    {
        private static ModularityManifest Manifest = null;

        public string[] WorkingDirectory { get; set; }
        public string[] ContentDirectory { get; set; }
        public AreaOptions Default { get; set; }
        public List<AreaOptions> Areas { get; set; }
        public List<ModuleOptions> Modules { get; set; }
        public List<PluginOptions> Plugins { get; set; }
        public bool UseEmbeddedFiles { get; set; } = false;
        public bool IsDevelopment { get; set; }
        public bool Exist { get; set; }
        public Exception Exception { get; set; }

        public static void Initialize()
        {
            Get(true);
        }

        public static ModularityManifest Get(bool forced = false)
        {
            if (Manifest != null && !forced)
            {
                return Manifest;
            }

            string path = Path.Combine(Directory.GetCurrentDirectory(), "App_Data", "Modularity", "modularity.json");//todo: dolduracağız.

            return Get(path, forced);
        }

        public static ModularityManifest Get(string path, bool forced = false)
        {
            try
            {
                if (Manifest != null && !forced)
                {
                    return Manifest;
                }

                var json = File.ReadAllText(path);

                Manifest = JsonConvert.DeserializeObject<ModularityManifest>(json);
                Manifest.Exist = true;

                return Manifest;
            }
            catch (Exception ex)
            {
                Manifest = null;
                return new ModularityManifest { Exist = false, Exception = ex };
            }
        }
    }
}
