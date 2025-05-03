using System;
using Microsoft.Xrm.Sdk;

namespace D365PackageProject;

public class PostOperationFormatPhoneOnRetrieveMultiple : IPlugin
{
    public void Execute(IServiceProvider serviceProvider)
    {
        IPluginExecutionContext context =
 (IPluginExecutionContext)serviceProvider.GetService(typeof(IPluginExecutionContext));

        if (context.MessageName.Equals("Retrieve"))
        {
            if (!context.OutputParameters.Contains("BusinessEntity") && context.OutputParameters["BusinessEntity"] is Entity)
                throw new InvalidPluginExecutionException("No business entity found");

            var entity = (Entity)context.OutputParameters["BusinessEntity"];
            if (!entity.Attributes.Contains("telephone1"))
                return;

            if (!long.TryParse(entity["telephone1"].ToString(), out long phoneNumber))
                return;

            var formattedNumber = String.Format("{0:(###) ###-####}", phoneNumber);
            entity["telephone1"] = formattedNumber;
        }
        else if (context.MessageName.Equals("RetrieveMultiple"))
        {
            if (!context.OutputParameters.Contains("BusinessEntityCollection") && context.OutputParameters["BusinessEntityCollection"] is EntityCollection)
                throw new InvalidPluginExecutionException("No business entity collection found");

            var entityCollection = (EntityCollection)context.OutputParameters["BusinessEntityCollection"];
            
            foreach (var entity in entityCollection.Entities)
            {
                if (entity.Attributes.Contains("telephone1") && entity["telephone1"] != null)
                {
                    if (long.TryParse(entity["telephone1"].ToString(), out long phoneNumber))
                    {
                        var formattedNumber = String.Format("{0:(###) ###-####}", phoneNumber);
                        entity["telephone1"] = formattedNumber;
                    }
                }
            }
        }
    }
}
