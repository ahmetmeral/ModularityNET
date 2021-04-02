using Microsoft.AspNetCore.Authorization;
using ModularityNET.Attributes;
using ModularityNET.Services;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace ModularityNET
{
    public class ModularityContext : IModularityContext
    {
        public ModularityArea DefaultArea { get; set; }
        public List<ModularityArea> Areas { get; set; }
        public List<ModularityPlugin> Plugins { get; set; }
        public Dictionary<string, string> RequestPrefix { get; set; }
        public List<string> AuthorizeByDefaultNamespaces { get; set; }
        public string ContentDirectory { get; set; }

        private List<string> Files { get; set; }

        public ModularityContext()
        {
            Areas = new List<ModularityArea>();
            Plugins = new List<ModularityPlugin>();
            RequestPrefix = new Dictionary<string, string>();
            AuthorizeByDefaultNamespaces = new List<string>();
            Files = new List<string>();
        }

        public bool TryAddGetFileAlreadyLoad(string fileName)
        {
            fileName = Path.GetFileNameWithoutExtension(fileName);

            if (Files.Contains(fileName))
            {
                return true;
            }

            Files.Add(fileName);

            return false;
        }

        public bool ExistFile(string fileName)
        {
            fileName = Path.GetFileNameWithoutExtension(fileName);

            return Files.Contains(fileName);
        }

        public void AddArea(ModularityArea area)
        {
            if (Areas.Count(a => a.Options.Name == area.Options.Name) == 0)
            {
                Areas.Add(area);
            }
        }

        internal void Build()
        {
            foreach (var area in Areas)
            {
                if (area.Options.AuthorizeByDefault)
                {
                    AuthorizeByDefaultNamespaces.AddRange(area.Options.Modules);
                }

                if (!String.IsNullOrWhiteSpace(area.Options.RequestPathPrefix))
                {
                    foreach (var module in area.Options.Modules)
                    {
                        if (!RequestPrefix.ContainsKey(module))
                        {
                            RequestPrefix.Add(module, area.Options.RequestPathPrefix);
                        }
                    }
                }

                //module assembly check.
                foreach (var module in area.Modules)
                {
                    if (!module.HasAssembly)
                    {
                        throw new Exception($"Module {module.Options.Id} Assembly is null");
                    }
                }
            }
        }

        public bool ShouldApplyActionAuthorizeConvention(string controllerAssemblyName, IReadOnlyList<object> controllerRouteAttributes)
        {
            if (String.IsNullOrWhiteSpace(controllerAssemblyName))
            {
                throw new ArgumentNullException("ShouldApplyActionAuthorizeConvention controllerAssemblyName is empty");
            }

            if (AuthorizeByDefaultNamespaces.Count > 0)
            {
                var isAuthorizationDefined = !controllerRouteAttributes.Any(x => x.GetType() == typeof(AuthorizeAttribute)) && !controllerRouteAttributes.Any(x => x.GetType() == typeof(AllowAnonymousAttribute));

                if (isAuthorizationDefined)
                {
                    if (AuthorizeByDefaultNamespaces.Contains(controllerAssemblyName))
                    {
                        return true;
                    }
                }

                return false;
            }

            return false;
        }

        public string GetControllerRoutePrefix(string controllerAssemblyName, IReadOnlyList<object> controllerRouteAttributes)
        {
            if (String.IsNullOrWhiteSpace(controllerAssemblyName))
            {
                throw new ArgumentNullException("ShouldApplyActionAuthorizeConvention controllerAssemblyName is empty");
            }

            if (RequestPrefix.Count == 0)
            {
                return string.Empty;
            }

            if (!controllerRouteAttributes.Any(x => x.GetType() == typeof(ConventionIgnoreAttribute)))
            {
                foreach (var keyValuePair in RequestPrefix)
                {
                    if (controllerAssemblyName == keyValuePair.Key)
                    {
                        return keyValuePair.Value;//return request prefix
                    }
                }
            }
            return string.Empty;
        }
    }
}
