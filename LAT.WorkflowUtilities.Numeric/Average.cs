using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Workflow;
using System;
using System.Activities;

namespace LAT.WorkflowUtilities.Numeric
{
    public sealed class Average : CodeActivity
    {
        [RequiredArgument]
        [Input("Number 1")]
        public InArgument<decimal> Number1 { get; set; }

        [RequiredArgument]
        [Input("Number 2")]
        public InArgument<decimal> Number2 { get; set; }

        [RequiredArgument]
        [Input("Round Decimal Places")]
        [Default("-1")]
        public InArgument<int> RoundDecimalPlaces { get; set; }

        [OutputAttribute("Average Value")]
        public OutArgument<decimal> AverageValue { get; set; }

        protected override void Execute(CodeActivityContext executionContext)
        {
            ITracingService tracer = executionContext.GetExtension<ITracingService>();

            try
            {
                decimal number1 = Number1.Get(executionContext);
                decimal number2 = Number2.Get(executionContext);
                int roundDecimalPlaces = RoundDecimalPlaces.Get(executionContext);

                decimal averageValue = ((number1 + number2) / 2);

                if (roundDecimalPlaces != -1)
                    averageValue = Math.Round(averageValue, roundDecimalPlaces);

                AverageValue.Set(executionContext, averageValue);
            }
            catch (Exception ex)
            {
                tracer.Trace("Exception: {0}", ex.ToString());
            }
        }
    }
}