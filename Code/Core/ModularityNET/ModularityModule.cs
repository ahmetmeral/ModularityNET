using Apiks.Modularity.Options;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Text;


namespace ModularityNET
{
    public class ModularityModule
    {
        public ModuleOptions Options { get; set; }

        [JsonIgnore]
        public Assembly Assembly { get; set; }
        /// <summary>
        /// Content Directory Not Assembly..
        /// </summary>
        public string PhysicalFileDirectory { get; set; }

        public bool HasAssembly { get { return Assembly != null; } }

        public ModularityModule() { }

        public ModularityModule(ModuleOptions options, string[] contentDirectory)
        {
            Options = options;
            PhysicalFileDirectory = Path.GetFullPath(Path.Combine(Path.Combine(contentDirectory), options.Id));
        }
    }
}
