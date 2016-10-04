using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Workflow;
using System;
using System.Activities;

namespace LAT.WorkflowUtilities.Numeric
{
	public class RandomNumberBetween : CodeActivity
	{
		[RequiredArgument]
		[Input("Min Value")]
		public InArgument<int> MinValue { get; set; }

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
				int minValue = MinValue.Get(executionContext);
				int maxValue = MaxValue.Get(executionContext);

				if (minValue < 1)
					minValue = 0;

				if (maxValue < 1)
					maxValue = 1;

				if (maxValue < minValue)
					throw new InvalidPluginExecutionException("Max Value must be greater than Min Value.");

				if (maxValue == minValue)
				{
					GeneratedNumber.Set(executionContext, maxValue);
					return;
				}

				Random random = new Random();
				int generatedNumber = random.Next(minValue, maxValue);

				GeneratedNumber.Set(executionContext, generatedNumber);
			}
			catch (InvalidPluginExecutionException)
			{
				throw;
			}
			catch (Exception ex)
			{
				tracer.Trace("Exception: {0}", ex.ToString());
			}
		}
	}
}
