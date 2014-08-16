using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web.Http;
using System.Web.Optimization;

namespace DemoPlugin
{
    public static class WebApiConfig
    {
        public static void Register(HttpConfiguration config)
        {
            config.MapHttpAttributeRoutes();
            config.EnableSystemDiagnosticsTracing();

            BundleTable.Bundles.Add(new Bundle("~/Scripts/main-js").IncludeDirectory("~/Scripts", "*.js"));
        }
    }
}
