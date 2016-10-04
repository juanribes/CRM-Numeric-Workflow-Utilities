using Microsoft.VisualStudio.TestTools.UnitTesting;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Workflow;
using Moq;
using System;
using System.Activities;
using System.Collections.Generic;

namespace LAT.WorkflowUtilities.Numeric.Tests
{
	[TestClass]
	public class RandomNumberBetweenTests
	{
		#region Class Constructor
		private readonly string _namespaceClassAssembly;
		public RandomNumberBetweenTests()
		{
			//[Namespace.class name, assembly name] for the class/assembly being tested
			//Namespace and class name can be found on the class file being tested
			//Assembly name can be found under the project properties on the Application tab
			_namespaceClassAssembly = "LAT.WorkflowUtilities.Numeric.RandomNumberBetween" + ", " + "LAT.WorkflowUtilities.Numeric";
		}
		#endregion
		#region Test Initialization and Cleanup
		// Use ClassInitialize to run code before running the first test in the class
		[ClassInitialize()]
		public static void ClassInitialize(TestContext testContext) { }

		// Use ClassCleanup to run code after all tests in a class have run
		[ClassCleanup()]
		public static void ClassCleanup() { }

		// Use TestInitialize to run code before running each test 
		[TestInitialize()]
		public void TestMethodInitialize() { }

		// Use TestCleanup to run code after each test has run
		[TestCleanup()]
		public void TestMethodCleanup() { }
		#endregion

		[TestMethod]
		public void SameValue()
		{
			//Target
			Entity targetEntity = null;

			//Input parameters
			var inputs = new Dictionary<string, object>
			{
				{ "MinValue", 2 },
				{ "MaxValue", 2 }
			};

			//Expected value(s)
			const int expected = 2;

			//Invoke the workflow
			var output = InvokeWorkflow(_namespaceClassAssembly, ref targetEntity, inputs);

			//Test(s)
			Assert.AreEqual(expected, (int)output["GeneratedNumber"]);
		}

		[TestMethod]
		public void MinLessThanZero()
		{
			//Target
			Entity targetEntity = null;

			//Input parameters
			var inputs = new Dictionary<string, object>
			{
				{ "MinValue", -2 },
				{ "MaxValue", 1 }
			};

			//Invoke the workflow
			var output = InvokeWorkflow(_namespaceClassAssembly, ref targetEntity, inputs);

			//Test(s)
			Assert.IsTrue((int)output["GeneratedNumber"] >= 0 && (int)output["GeneratedNumber"] <= 1);
		}

		[TestMethod]
		public void MaxLessThanZero()
		{
			//Target
			Entity targetEntity = null;

			//Input parameters
			var inputs = new Dictionary<string, object>
			{
				{ "MinValue", 1 },
				{ "MaxValue", -1 }
			};

			//Expected value(s)
			const int expected = 1;

			//Invoke the workflow
			var output = InvokeWorkflow(_namespaceClassAssembly, ref targetEntity, inputs);

			//Test(s)
			Assert.AreEqual(expected, (int)output["GeneratedNumber"]);
		}

		[TestMethod]
		[ExpectedException(typeof(InvalidPluginExecutionException), "Max Value must be greater than Min Value.")]
		public void MaxLessThanMin()
		{
			//Target
			Entity targetEntity = null;

			//Input parameters
			var inputs = new Dictionary<string, object>
			{
				{ "MinValue", 5 },
				{ "MaxValue", 2 }
			};

			//Invoke the workflow
			InvokeWorkflow(_namespaceClassAssembly, ref targetEntity, inputs);
		}

		[TestMethod]
		public void ValidValues()
		{
			//Target
			Entity targetEntity = null;

			//Input parameters
			var inputs = new Dictionary<string, object>
			{
				{ "MinValue", 1 },
				{ "MaxValue", 3 }
			};

			//Invoke the workflow
			var output = InvokeWorkflow(_namespaceClassAssembly, ref targetEntity, inputs);

			//Test(s)
			Assert.IsTrue((int)output["GeneratedNumber"] >= 1 && (int)output["GeneratedNumber"] <= 3);
		}

		/// <summary>
		/// Invokes the workflow.
		/// </summary>
		/// <param name="name">Namespace.Class, Assembly</param>
		/// <param name="target">The target entity</param>
		/// <param name="inputs">The workflow input parameters</param>
		/// <returns>The workflow output parameters</returns>
		private static IDictionary<string, object> InvokeWorkflow(string name, ref Entity target, Dictionary<string, object> inputs)
		{
			var testClass = Activator.CreateInstance(Type.GetType(name)) as CodeActivity;

			var serviceMock = new Mock<IOrganizationService>();
			var factoryMock = new Mock<IOrganizationServiceFactory>();
			var tracingServiceMock = new Mock<ITracingService>();
			var workflowContextMock = new Mock<IWorkflowContext>();

			IOrganizationService service = serviceMock.Object;

			//Mock workflow Context
			var workflowUserId = Guid.NewGuid();
			var workflowCorrelationId = Guid.NewGuid();
			var workflowInitiatingUserId = Guid.NewGuid();

			//Workflow Context Mock
			workflowContextMock.Setup(t => t.InitiatingUserId).Returns(workflowInitiatingUserId);
			workflowContextMock.Setup(t => t.CorrelationId).Returns(workflowCorrelationId);
			workflowContextMock.Setup(t => t.UserId).Returns(workflowUserId);
			var workflowContext = workflowContextMock.Object;

			//Organization Service Factory Mock
			factoryMock.Setup(t => t.CreateOrganizationService(It.IsAny<Guid>())).Returns(service);
			var factory = factoryMock.Object;

			//Tracing Service - Content written appears in output
			tracingServiceMock.Setup(t => t.Trace(It.IsAny<string>(), It.IsAny<object[]>())).Callback<string, object[]>(MoqExtensions.WriteTrace);
			var tracingService = tracingServiceMock.Object;

			//Parameter Collection
			ParameterCollection inputParameters = new ParameterCollection { { "Target", target } };
			workflowContextMock.Setup(t => t.InputParameters).Returns(inputParameters);

			//Workflow Invoker
			var invoker = new WorkflowInvoker(testClass);
			invoker.Extensions.Add(() => tracingService);
			invoker.Extensions.Add(() => workflowContext);
			invoker.Extensions.Add(() => factory);

			return invoker.Invoke(inputs);
		}
	}
}
