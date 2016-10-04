using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Workflow;
using System;
using System.Activities;

namespace LAT.WorkflowUtilities.Numeric
{
    public sealed class Max : CodeActivity
    {
        [RequiredArgument]
        [Input("Number 1")]
        public InArgument<decimal> Number1 { get; set; }

        [RequiredArgument]
        [Input("Number 2")]
        public InArgument<decimal> Number2 { get; set; }

        [OutputAttribute("Max Value")]
        public OutArgument<decimal> MaxValue { get; set; }

        protected override void Execute(CodeActivityContext executionContext)
        {
            ITracingService tracer = executionContext.GetExtension<ITracingService>();

            try
            {
                decimal number1 = Number1.Get(executionContext);
                decimal number2 = Number2.Get(executionContext);

                decimal maxValue = number1 >= number2 ? number1 : number2;

                MaxValue.Set(executionContext, maxValue);
            }
            catch (Exception ex)
            {
                tracer.Trace("Exception: {0}", ex.ToString());
            }
        }
    }
}