using Apiks.Modularity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ApplicationModels;
using ModularityNET.Services;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text; 

namespace ModularityNET.Convention
{
    //https://www.stevejgordon.co.uk/customising-asp-net-mvc-core-behaviour-with-an-iapplicationmodelconvention
    public class ModularityRoutePrefixConvention : IControllerModelConvention
    {
        private IModularityContext Modularity = null; 

        public ModularityRoutePrefixConvention(IModularityContext modularity)
        {
            Modularity = modularity;
        }

        public void Apply(ControllerModel controller)
        {
            //var ns = controller.ControllerType.Assembly.GetName().Name;
            var ns = Path.GetFileNameWithoutExtension(controller.ControllerType.Assembly.ManifestModule.Name);
            string prefix = Modularity.GetControllerRoutePrefix(ns, controller.Attributes);

            if (!String.IsNullOrWhiteSpace(prefix)) 
            {
                var _routePrefix = new AttributeRouteModel(new RouteAttribute(prefix));
                
                foreach (var selector in controller.Selectors)
                {
                    //tanımlanmış bir route attribute var ..onunla cobmine ediyoruz..
                    if (selector.AttributeRouteModel != null)
                    {
                        selector.AttributeRouteModel = AttributeRouteModel.CombineAttributeRouteModel(_routePrefix, selector.AttributeRouteModel);
                    }
                    else
                    {
                        //yoksada direkt olarak ekliyoruz.
                        selector.AttributeRouteModel = _routePrefix;
                    }
                }
            }
        }
    }
}
