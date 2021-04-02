using System.Collections.Generic;

namespace ModularityNET.Services
{
    public interface IModularityContext
    {
        List<ModularityArea> Areas { get; set; }
        List<string> AuthorizeByDefaultNamespaces { get; set; }
        ModularityArea DefaultArea { get; set; }
        List<ModularityPlugin> Plugins { get; set; }
        Dictionary<string, string> RequestPrefix { get; set; }

        void AddArea(ModularityArea area);
        bool ExistFile(string fileName);
        bool TryAddGetFileAlreadyLoad(string name);
        string GetControllerRoutePrefix(string controllerAssemblyName, IReadOnlyList<object> controllerRouteAttributes);
        bool ShouldApplyActionAuthorizeConvention(string controllerAssemblyName, IReadOnlyList<object> controllerRouteAttributes);
    }
}