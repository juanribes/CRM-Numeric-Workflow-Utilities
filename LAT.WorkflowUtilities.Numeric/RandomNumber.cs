using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Workflow;
using System;
using System.Activities;

namespace LAT.WorkflowUtilities.Numeric
{
    public class RandomNumber : CodeActivity
    {
        [RequiredArgument]
        [Input("Max Value")]
        public InArgument<int> MaxValue { get; set; }

        [OutputAttribute("Generated Number")]
        public OutArgument<int> GeneratedNumber { get; set; }

        protected override void Execute(CodeActivityContext executionContext)
        {
            ITracingService tracer = executionContext.GetExtension<ITracingService>();

            try
            {
                int maxValue = MaxValue.Get(executionContext);

                if (maxValue < 1)
                    maxValue = 1;

                Random random = new Random();
                int generatedNumber = random.Next(maxValue);

                GeneratedNumber.Set(executionContext, generatedNumber);
            }
            catch (Exception ex)
            {
                tracer.Trace("Exception: {0}", ex.ToString());
            }
        }
    }
}
