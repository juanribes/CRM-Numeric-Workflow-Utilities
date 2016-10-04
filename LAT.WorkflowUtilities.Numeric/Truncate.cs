using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Workflow;
using System;
using System.Activities;

namespace LAT.WorkflowUtilities.Numeric
{
    public class Truncate : CodeActivity
    {
        [RequiredArgument]
        [Input("Number To Truncate")]
        public InArgument<decimal> NumberToTruncate { get; set; }

        [RequiredArgument]
        [Input("Decimal Places")]
        public InArgument<int> DecimalPlaces { get; set; }

        [OutputAttribute("Truncated Number")]
        public OutArgument<decimal> TruncatedNumber { get; set; }

        protected override void Execute(CodeActivityContext executionContext)
        {
            ITracingService tracer = executionContext.GetExtension<ITracingService>();

            try
            {
                decimal numberToTruncate = NumberToTruncate.Get(executionContext);
                int decimalPlaces = DecimalPlaces.Get(executionContext);

                if (decimalPlaces < 0)
                    decimalPlaces = 0;

                decimal step = (decimal)Math.Pow(10, decimalPlaces);
                int temp = (int)Math.Truncate(step * numberToTruncate);

                decimal truncatedNumber = temp / step;

                TruncatedNumber.Set(executionContext, truncatedNumber);
            }
            catch (Exception ex)
            {
                tracer.Trace("Exception: {0}", ex.ToString());
            }
        }
    }
}
