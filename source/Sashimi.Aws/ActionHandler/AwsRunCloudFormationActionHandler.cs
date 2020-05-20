using Calamari.Aws;
using Sashimi.Aws.CloudFormation.Presets;
using Sashimi.Server.Contracts;
using Sashimi.Server.Contracts.ActionHandlers;
using Sashimi.Server.Contracts.Calamari;

namespace Sashimi.Aws.ActionHandler
{
    /// <summary>
    /// The action handler that prepares a Calamari script execution with
    /// the path set to include the AWS CLI and having AWS credentials
    /// set in the common environment variable paths. It then goes on to
    /// deploy a cloud formation template.
    /// </summary>
    public class AwsRunCloudFormationActionHandler : IActionHandler
    {
        public string Id => AwsActionTypes.RunCloudFormation;
        public string Name => "Deploy an AWS CloudFormation template";
        public string Description => "Creates or updates an AWS CloudFormation stack";
        public string Keywords => null;
        public bool ShowInStepTemplatePickerUI => true;
        public bool WhenInAChildStepRunInTheContextOfTheTargetMachine => false;
        public bool CanRunOnDeploymentTarget => false;
        public ActionHandlerCategory[] Categories => new[] { ActionHandlerCategory.BuiltInStep, AwsConstants.AwsActionHandlerCategory };

        public IActionHandlerResult Execute(IActionHandlerContext context)
        {
            var builder = context.CalamariCommand(CalamariFlavour.CalamariAws, KnownAwsCalamariCommands.Commands.DeployAwsCloudFormation);

            CloudFormationCalamariPresets.TemplatesAndParameters(context.Variables, builder);

            // builder.WithArgument("waitForCompletion", context.Variables.GetFlag(AwsSpecialVariables.Action.Aws.WaitForCompletion, false).ToString());
            // builder.WithArgument("stackName", context.Variables.Get(AwsSpecialVariables.Action.Aws.CloudFormation.StackName, ""));
            // builder.WithArgument("disableRollback", context.Variables.GetFlag(AwsSpecialVariables.Action.Aws.DisableRollback, true).ToString());
            // builder.WithArgument("extensions", CalamariExtensions.Aws);

            return builder.Execute();
        }
    }
}