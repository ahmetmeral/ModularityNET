using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Apiks.Modularity.Options
{
    public class AreaOptions
    {
        public string Name { get; set; }
        public bool Running { get; set; }
        public string RequestPathPrefix { get; set; }
        public bool AuthorizeByDefault { get; set; }
        public List<string> Modules { get; set; }
    }
}
