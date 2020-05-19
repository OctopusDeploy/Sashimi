using Amazon.IdentityManagement;
using Amazon.SecurityToken;
using Calamari.Aws.Deployment;
using Calamari.Aws.Deployment.CloudFormation;
using Calamari.Aws.Integration.CloudFormation;
using Calamari.Commands.Support;
using Calamari.Deployment;
using Octostache;

namespace Calamari.Aws
{
    [Command(KnownAwsCalamariCommands.Commands.ApplyAwsCloudformationChangeset, Description = "Apply an existing AWS CloudFormation changeset")]
    public class ApplyCloudFormationChangesetCommand: Command
    {
        readonly ICloudFormationService cloudFormationService;

        public ApplyCloudFormationChangeSetCommand(
            ILog log,
            IVariables variables,
            IAmazonSecurityTokenService amazonSecurityTokenService,
            IAmazonIdentityManagementService amazonIdentityManagementService,
            ICloudFormationService cloudFormationService)
            : base (log, variables, amazonSecurityTokenService, amazonIdentityManagementService)
        {
            this.cloudFormationService = cloudFormationService;
        }

        protected override void Execute(RunningDeployment deployment)
        {
            var stackArn = new StackArn(deployment.Variables.Get(AwsSpecialVariables.CloudFormation.StackName));
            var changeSetArn = new ChangeSetArn(deployment.Variables.Get(AwsSpecialVariables.CloudFormation.Changesets.Arn));
            var waitForCompletion = new VariableDictionary().EvaluateTruthy(variables.Get("waitForCompletion"));
            
            cloudFormationService.ExecuteChangeSet(stackArn, changeSetArn, waitForCompletion).GetAwaiter().GetResult();
            cloudFormationService.OutputVariables(variables).GetAwaiter().GetResult();
        }
    }
}