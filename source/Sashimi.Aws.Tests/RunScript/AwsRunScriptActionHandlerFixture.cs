using NSubstitute;
using NSubstitute.Extensions;
using NUnit.Framework;
using Octopus.Diagnostics;
using Octostache;
using Sashimi.Aws.ActionHandler;
using Sashimi.Server.Contracts.ActionHandlers;
using Sashimi.Server.Contracts.CommandBuilders;
using Sashimi.Server.Contracts.DeploymentTools;
using Sashimi.Server.Contracts.Variables;
using Sashimi.Tests.Shared.Server;

namespace Sashimi.Aws.Tests.RunScript
{
    [TestFixture]
    public class AwsRunScriptActionHandlerFixture
    {
        [Test]
        [TestCase(null, true, Description = "No variable provided -> AwsPs should be included")]
        [TestCase(true, true, Description = "Variable says 'yes, use it' -> AwsPs should be included")]
        [TestCase(false, false, Description = "Variable says 'no, dont use it' -> AwsPs should not be included")]
        public void AwsPowerShellToolAdded(bool? variableValue, bool expectedAction)
        {
            var builder = Substitute.For<ICalamariCommandBuilder>();
            builder.ReturnsForAll<ICalamariCommandBuilder>(builder);
            var context = Substitute.For<IActionHandlerContext>();

            var variables = new TestVariableDictionary();
            if (variableValue.HasValue)
                variables.Set(AwsSpecialVariables.Action.Aws.UseBundledAwsPowerShellModules, variableValue.Value.ToString());
            context.Variables.Returns(variables);

            builder.WithAwsTools(context, Substitute.For<ILog>());

            if (expectedAction)
                builder.Received().WithTool(AwsTools.AwsPowershell);
            else
                builder.DidNotReceive().WithTool(AwsTools.AwsPowershell);
        }

        [Test]
        [TestCase(null, true, Description = "No variable provided -> AwsCli should be included")]
        [TestCase(true, true, Description = "Variable says 'yes, use it' -> AwsCli should be included")]
        [TestCase(false, false, Description = "Variable says 'no, dont use it' -> AwsCli should not be included")]
        public void AwsCliToolAdded(bool? variableValue, bool expectedAction)
        {
            var builder = Substitute.For<ICalamariCommandBuilder>();
            builder.ReturnsForAll<ICalamariCommandBuilder>(builder);
            var context = Substitute.For<IActionHandlerContext>();

            var variables = new TestVariableDictionary();
            if (variableValue.HasValue)
                variables.Set(AwsSpecialVariables.Action.Aws.UseBundledAwsCLI, variableValue.Value.ToString());
            context.Variables.Returns(variables);

            builder.WithAwsTools(context, Substitute.For<ILog>());

            if (expectedAction)
                builder.Received(2).WithTool(Arg.Any<IDeploymentTool>());
            else
                builder.DidNotReceive().WithTool(AwsTools.AwsCli);
        }
    }
}
