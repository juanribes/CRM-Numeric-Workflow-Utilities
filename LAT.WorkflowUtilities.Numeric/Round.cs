using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Workflow;
using System;
using System.Activities;

namespace LAT.WorkflowUtilities.Numeric
{
    public class Round : CodeActivity
    {
        [RequiredArgument]
        [Input("Number To Round")]
        public InArgument<decimal> NumberToRound { get; set; }

        [RequiredArgument]
        [Input("Decimal Places")]
        public InArgument<int> DecimalPlaces { get; set; }

        [OutputAttribute("Rounded Number")]
        public OutArgument<decimal> RoundedNumber { get; set; }


        protected override void Execute(CodeActivityContext executionContext)
        {
            ITracingService tracer = executionContext.GetExtension<ITracingService>();

            try
            {
                decimal numberToRound = NumberToRound.Get(executionContext);
                int decimalPlaces = DecimalPlaces.Get(executionContext);

                if (decimalPlaces < 0)
                    decimalPlaces = 0;
				
                decimal roundedNumber = Math.Round(numberToRound, decimalPlaces);

                RoundedNumber.Set(executionContext, roundedNumber);
            }
            catch (Exception ex)
            {
                tracer.Trace("Exception: {0}", ex.ToString());
            }
        }
    }
}
