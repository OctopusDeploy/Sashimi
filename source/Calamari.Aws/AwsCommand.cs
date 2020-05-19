using System.IO;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Amazon.IdentityManagement.Model;
using Amazon.Runtime;
using Amazon.SecurityToken.Model;
using Calamari.Aws.Util;
using Calamari.Commands.Support;
using Calamari.Deployment;
using Octopus.CoreUtilities.Extensions;

namespace Calamari.Aws
{
    public abstract class AwsCommand : ICommand
    {
        static readonly Regex ArnNameRe = new Regex("^.*?/(.+)$");

        protected readonly ILog log;
        protected readonly IVariables variables;
        readonly IAmazonClientFactory amazonClientFactory;

        protected AwsCommand(
            ILog log,
            IVariables variables,
            IAmazonClientFactory amazonClientFactory)
        {
            this.log = log;
            this.variables = variables;
            this.amazonClientFactory = amazonClientFactory;
        }

        public int Execute()
        {
            LogAwsUserInfo().ConfigureAwait(false).GetAwaiter().GetResult();

            var pathToPackage = new PathToPackage(Path.GetFullPath(variables.Get("Octopus.Action.Package.PackageId")));

            Execute(new RunningDeployment(pathToPackage, variables));

            return 0;
        }

        protected abstract void Execute(RunningDeployment deployment);

        async Task LogAwsUserInfo()
        {
            if (variables.IsSet(SpecialVariables.Action.Aws.AssumeRoleARN) ||
                !variables.IsSet(SpecialVariables.Action.Aws.AccountId) ||
                !variables.IsSet(variables.Get(SpecialVariables.Action.Aws.AccountId) +
                                            ".AccessKey"))
            {
                await TryLogAwsUserRole();
            }
            else
            {
                await TryLogAwsUserName();
            }
        }

        async Task TryLogAwsUserName()
        {
            try
            {
                var amazonIdentityManagementService = await amazonClientFactory.CreateIdentityManagementService(); 
                var result = await amazonIdentityManagementService.GetUserAsync(new GetUserRequest());

                log.Info($"Running the step as the AWS user {result.User.UserName}");
            }
            catch (AmazonServiceException)
            {
                // Ignore, we just won't add this to the logs
            }
        }

        async Task TryLogAwsUserRole()
        {
            try
            {
                var amazonSecurityTokenService = await amazonClientFactory.CreateSecurityTokenService();
                (await amazonSecurityTokenService.GetCallerIdentityAsync(new GetCallerIdentityRequest()))
                    // The response is narrowed to the Aen
                    .Map(response => response.Arn)
                    // Try and match the response to get just the role
                    .Map(arn => ArnNameRe.Match(arn))
                    // Extract the role name, or a default
                    .Map(match => match.Success ? match.Groups[1].Value : "Unknown")
                    // Log the output
                    .Tee(role => log.Info($"Running the step as the AWS role {role}"));
            }
            catch (AmazonServiceException)
            {
                // Ignore, we just won't add this to the logs
            }
        }
    }
}