using Microsoft.Xrm.Sdk;
using System;
// speti_comment: import nuget package
using System.Text.RegularExpressions;

namespace CustomAPILab
{
    /// <summary>
    /// Plugin development guide: https://docs.microsoft.com/powerapps/developer/common-data-service/plug-ins
    /// Best practices and guidance: https://docs.microsoft.com/powerapps/developer/common-data-service/best-practices/business-logic/
    /// </summary>
    public class MatchPlugin : PluginBase
    {
        public MatchPlugin(string unsecureConfiguration, string secureConfiguration)
            : base(typeof(MatchPlugin))
        {
            // TODO: Implement your custom configuration handling
            // https://docs.microsoft.com/powerapps/developer/common-data-service/register-plug-in#set-configuration-data
        }

        // Entry point for custom business logic execution
        protected override void ExecuteDataversePlugin(ILocalPluginContext localPluginContext)
        {
            if (localPluginContext == null)
            {
                throw new ArgumentNullException(nameof(localPluginContext));
            }

            var context = localPluginContext.PluginExecutionContext;

            // speti_comment: These lines get the value from the input parameters passed on the custom API invocation
            string input = (string)context.InputParameters["StringIn"];
            string pattern = (string)context.InputParameters["Pattern"];

            // speti_comment: get the tracing service
            ITracingService tracingService = (ITracingService)localPluginContext.ServiceProvider.GetService(typeof(ITracingService));

            // speti_comment: write the input value into trace
            tracingService.Trace("Provided input: " + input);

            // speti_comment: call the Regex.Match method
            var result = Regex.Match(input, pattern);

            // speti_comment: Write the result to trace
            tracingService.Trace("Matching result: " + result.Success);

            // speti_comment: set the output parameter Matched
            context.OutputParameters["Matched"] = result.Success;


            // TODO: Implement your custom business logic

            // Check for the entity on which the plugin would be registered
            //if (context.InputParameters.Contains("Target") && context.InputParameters["Target"] is Entity)
            //{
            //    var entity = (Entity)context.InputParameters["Target"];

            //    // Check for entity name on which this plugin would be registered
            //    if (entity.LogicalName == "account")
            //    {

            //    }
            //}
        }
    }
}
