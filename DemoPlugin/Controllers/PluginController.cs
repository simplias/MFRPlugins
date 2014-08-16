using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;
using BSSPlugin.Services;
using MFR.Client;
using MFR.Client.Context;
using MFR.Client.Controller;
using DemoPlugin.Services;

namespace DemoPlugin.Controllers
{
    public class PluginController : PluginControllerBase
    {
        [Route("Settings")]
        [HttpGet()]
        public Settings GetSettings()
        {
            return new SynchronizationService(PluginContext).GetSettings(RequestedTenant);
        }

        [Route("Settings")]
        [HttpPost()]
        public Settings CreateSettings([FromBody] Settings settings)
        {
            return new SynchronizationService(PluginContext).UpdateSettings(RequestedTenant, settings);
        }

        [Route("Settings")]
        [HttpPut()]
        public Settings UpdateSettings([FromBody] Settings settings)
        {
            return new SynchronizationService(PluginContext).UpdateSettings(RequestedTenant,settings);
        }

        [Route("Settings")]
        [HttpGet()]
        public void InvokeSynchronization()
        {
            new SynchronizationService(PluginContext).InvokeSynchronize(RequestedTenant);
        }
    }
}
