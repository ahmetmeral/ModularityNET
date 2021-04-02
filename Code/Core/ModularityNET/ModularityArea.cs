using Apiks.Modularity.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ModularityNET
{
    public class ModularityArea
    {
        public AreaOptions Options { get; set; }
        public List<ModularityModule> Modules { get; set; }
        public bool IsDefaultArea { get; set; }

        public ModularityArea(AreaOptions options, bool isDefaultArea = false)
        {
            Options = options;
            Modules = new List<ModularityModule>();
            IsDefaultArea = isDefaultArea;
        }

        public void AddModule(ModularityModule module)
        {
            if(Modules.Count(m=>m.Options.Id == module.Options.Id) == 0)
            {
                Modules.Add(module);
            }
        }
    }
}
