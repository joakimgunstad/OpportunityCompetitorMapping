using Microsoft.Xrm.Sdk;
using System;

public class SimplePluginOpportunity : IPlugin
{
    public void Execute(IServiceProvider serviceProvider)
    {
        // Obtain services from the service provider
        ITracingService tracingService = (ITracingService)serviceProvider.GetService(typeof(ITracingService));
        IPluginExecutionContext context = (IPluginExecutionContext)serviceProvider.GetService(typeof(IPluginExecutionContext));

        tracingService.Trace("Plugin execution started.");

        try
        {
            // Prevent infinite loop by checking depth
            if (context.Depth > 1)
            {
                tracingService.Trace("Plugin execution skipped due to depth check.");
                return;
            }

            // Validate the target entity
            if (context.InputParameters.Contains("Target") && context.InputParameters["Target"] is Entity targetEntity)
            {
                tracingService.Trace("Target entity is valid.");

                // Check if the specific lookup field 'sopra_existingbankconnection' is modified
                if (targetEntity.Contains("sopra_existingbankconnection"))
                {
                    tracingService.Trace("Field 'sopra_existingbankconnection' is modified.");

                    // Retrieve the lookup value
                    var competitorRef = targetEntity.GetAttributeValue<EntityReference>("sopra_existingbankconnection");

                    if (competitorRef != null)
                    {
                        tracingService.Trace($"Retrieved competitor reference: ID = {competitorRef.Id}, LogicalName = {competitorRef.LogicalName}");

                        // Obtain the organization service
                        var serviceFactory = (IOrganizationServiceFactory)serviceProvider.GetService(typeof(IOrganizationServiceFactory));
                        var service = serviceFactory.CreateOrganizationService(context.UserId);

                        // Associate the competitor with the opportunity
                        tracingService.Trace("Associating the Competitor with the Opportunity using N:N relationship.");

                        // Use the correct relationship schema name
                        string relationshipName = "opportunitycompetitors_association";

                        // Prepare the related entities to associate
                        EntityReferenceCollection relatedEntities = new EntityReferenceCollection
                        {
                            competitorRef
                        };

                        // Perform the association
                        Relationship relationship = new Relationship(relationshipName);
                        service.Associate(targetEntity.LogicalName, targetEntity.Id, relationship, relatedEntities);

                        tracingService.Trace("Association completed successfully.");
                    }
                    else
                    {
                        tracingService.Trace("Field 'sopra_existingbankconnection' does not have a value.");
                    }
                }
                else
                {
                    tracingService.Trace("Field 'sopra_existingbankconnection' is not modified.");
                }
            }
            else
            {
                tracingService.Trace("Target entity is not valid or does not contain the expected fields.");
            }
        }
        catch (Exception ex)
        {
            tracingService.Trace($"An error occurred: {ex.Message}");
            throw new InvalidPluginExecutionException("An error occurred in the SimplePluginOpportunity plugin.", ex);
        }

        tracingService.Trace("Plugin execution completed.");
    }
}
