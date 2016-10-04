using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Workflow;
using System;
using System.Activities;

namespace LAT.WorkflowUtilities.Numeric
{
    public class ToInteger : CodeActivity
    {
        [RequiredArgument]
        [Input("Text To Convert")]
        public InArgument<string> TextToConvert { get; set; }

        [OutputAttribute("Converted Number")]
        public OutArgument<int> ConvertedNumber { get; set; }

        [OutputAttribute("Is Valid Integer")]
        public OutArgument<bool> IsValid { get; set; }

        protected override void Execute(CodeActivityContext executionContext)
        {
            ITracingService tracer = executionContext.GetExtension<ITracingService>();

            try
            {
                string textToConvert = TextToConvert.Get(executionContext);

                if (string.IsNullOrEmpty(textToConvert))
                {
                    IsValid.Set(executionContext, false);
                    return;
                }

                int convertedNumber;
                bool isNumber = int.TryParse(textToConvert, out convertedNumber);

                if (isNumber)
                {
                    ConvertedNumber.Set(executionContext, convertedNumber);
                    IsValid.Set(executionContext, true);
                }
                else
                    IsValid.Set(executionContext, false);
            }
            catch (Exception ex)
            {
                tracer.Trace("Exception: {0}", ex.ToString());
            }
        }
    }
}
