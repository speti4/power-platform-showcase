using System;

using Microsoft.Xrm.Sdk;
using System.Text.RegularExpressions;

namespace D365PackageProject;

public class PreOperationFormatPhoneCreateUpdate : IPlugin
{
    public void Execute(IServiceProvider serviceProvider)
    {
        IPluginExecutionContext context = (IPluginExecutionContext)serviceProvider.GetService(typeof(IPluginExecutionContext));
        // Check the input parameter for Target
        if (!context.InputParameters.ContainsKey("Target"))
            throw new InvalidPluginExecutionException("No target found");

        // get the target entity from the input parameter and then check if its attributes contain telephone1 (Business Phone for Contacts, Phone for Accounts)
        var entity = context.InputParameters["Target"] as Entity;
        if (!entity.Attributes.Contains("telephone1"))
            return;

        //removes all non-numeric characters from the user-provided phone number
        string phoneNumber = (string)entity["telephone1"];
        var formattedNumber = Regex.Replace(phoneNumber, @"[^\d]", "");

        // set telephone1 to the formatted phone number
        entity["telephone1"] = formattedNumber;
    }
}
