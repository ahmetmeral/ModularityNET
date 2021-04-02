using Apiks.Modularity;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.ApplicationModels;
using Microsoft.AspNetCore.Mvc.Authorization;
using ModularityNET.Services;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;  

namespace ModularityNET.Convention
{
    //https://www.stevejgordon.co.uk/customising-asp-net-mvc-core-behaviour-with-an-iapplicationmodelconvention
    public class ModularityAuthorizeByDefaultConvention : IActionModelConvention
    {
        private IModularityContext Modularity = null;

        public ModularityAuthorizeByDefaultConvention(IModularityContext modularity)
        {
            Modularity = modularity;
        }

        public void Apply(ActionModel action)
        {
            var ns = Path.GetFileNameWithoutExtension(action.Controller.ControllerType.Assembly.ManifestModule.Name);

            if (Modularity.ShouldApplyActionAuthorizeConvention(ns, action.Attributes))
            {
                var policy = new AuthorizationPolicyBuilder().RequireAuthenticatedUser().Build();
                action.Filters.Add(new AuthorizeFilter(policy));
            }
        }
    }
}
