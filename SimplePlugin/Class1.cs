using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Query;
using System;

public class OpportunityCompetitorMapping : IPlugin
{
    public void Execute(IServiceProvider serviceProvider)
    {
        // Obtain the execution context.
        IPluginExecutionContext context = (IPluginExecutionContext)serviceProvider.GetService(typeof(IPluginExecutionContext));

        // Check if the target entity is Opportunity.
        if (context.InputParameters.Contains("Target") && context.InputParameters["Target"] is Entity)
        {
            var targetEntity = (Entity)context.InputParameters["Target"];

            // Ensure the sb1_existingbankconnection field is modified.
            if (targetEntity.Contains("sb1_existingbankconnection"))
            {
                // Obtain services.
                var serviceFactory = (IOrganizationServiceFactory)serviceProvider.GetService(typeof(IOrganizationServiceFactory));
                var service = serviceFactory.CreateOrganizationService(context.UserId);

                // Obtain tracing service for debugging
                ITracingService tracingService = (ITracingService)serviceProvider.GetService(typeof(ITracingService));

                // Check if the lookup has a value.
                var competitorRef = targetEntity.GetAttributeValue<EntityReference>("sb1_existingbankconnection");
                if (competitorRef != null)
                {
                    // Fetch the Competitor record with only the "name" field
                    var competitor = service.Retrieve(competitorRef.LogicalName, competitorRef.Id, new ColumnSet("name"));

                    // Log the fetched name for debugging purposes (optional)
                    var competitorName = competitor.GetAttributeValue<string>("name");
                    tracingService.Trace($"Competitor retrieved: {competitorName}");

                    // No additional logic is required if the subgrid is configured to filter automatically based on the lookup.
                }
            }
        }
    }
}
