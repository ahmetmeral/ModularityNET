using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Apiks.Modularity.Options
{
    public class PluginOptions
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string Version { get; set; }
        public bool Bundle { get; set; }
        public bool Installed { get; set; }
        public bool Running { get; set; }
    }
}
