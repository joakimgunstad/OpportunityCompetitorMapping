using System;
using Microsoft.Xrm.Sdk;

namespace SimplePlugin
{
    public class SimplePlugin : IPlugin
    {
        IOrganizationService service;
        IPluginExecutionContext context;
        ITracingService tracingService;

        public void Execute(IServiceProvider serviceProvider)
        {
            context = (IPluginExecutionContext)serviceProvider.GetService(typeof(IPluginExecutionContext));
            service = ((IOrganizationServiceFactory)serviceProvider.GetService(typeof(IOrganizationServiceFactory))).CreateOrganizationService(context.UserId);
            tracingService = (ITracingService)serviceProvider.GetService(typeof(ITracingService));

            /* --------------    Call Functions   ------------------------*/

            tracingService.Trace("You are awesome! Congratulations on the first plugin");


            /* --------------    Call Functions   ------------------------*/

        }
    }
}