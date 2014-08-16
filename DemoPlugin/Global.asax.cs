using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;
using System.Web.Mvc;
using System.Web.Routing;
using MFR.Client.Context;
using System.Threading.Tasks;

namespace DemoPlugin
{
    // Note: For instructions on enabling IIS6 or IIS7 classic mode, 
    // visit http://go.microsoft.com/?LinkId=9394801

    public class WebApiApplication : System.Web.HttpApplication
    {
        protected void Application_Start()
        {
            var context = PluginContextProvider.Context;
            WebApiConfig.Register(GlobalConfiguration.Configuration);

            GlobalConfiguration.Configuration.EnsureInitialized();

            Task.Delay(3000).ContinueWith((t) => context.UploadManifest());
        }
    }
}