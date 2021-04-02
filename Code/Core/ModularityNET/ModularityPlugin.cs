using Apiks.Modularity.Options;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;


namespace ModularityNET
{
    public class ModularityPlugin
    {
        public PluginOptions Options { get; set; }

        [JsonIgnore]
        public Assembly Assembly { get; set; }

        public bool HasAssembly { get { return Assembly != null; } }

        public ModularityPlugin() { }

        public ModularityPlugin(PluginOptions options)
        {
            Options = options;
        }
    }
}
