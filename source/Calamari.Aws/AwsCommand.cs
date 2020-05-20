using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Amazon.IdentityManagement;
using Amazon.IdentityManagement.Model;
using Amazon.Runtime;
using Amazon.SecurityToken;
using Amazon.SecurityToken.Model;
using Calamari.Aws.Integration.CloudFormation;
using Calamari.Aws.Util;
using Calamari.Commands.Support;
using Octopus.CoreUtilities.Extensions;

namespace Calamari.Aws
{
    public abstract class AwsCommand : ICommand, IDisposable
    {
        static readonly Regex ArnNameRe = new Regex("^.*?/(.+)$");

        protected readonly ILog log;
        protected readonly IVariables variables;

        readonly Lazy<Task<IAmazonIdentityManagementService>> amazonIdentityManagementClient;
        readonly Lazy<Task<IAmazonSecurityTokenService>> amazonSecurityTokenClient;

        protected AwsCommand(
            ILog log,
            IVariables variables,
            IAmazonClientFactory amazonClientFactory)
        {
            this.log = log;
            this.variables = variables;

            amazonIdentityManagementClient = new Lazy<Task<IAmazonIdentityManagementService>>(amazonClientFactory.GetIdentityManagementClient);
            amazonSecurityTokenClient = new Lazy<Task<IAmazonSecurityTokenService>>(amazonClientFactory.GetSecurityTokenClient);
        }

        public int Execute()
        {
            LogAwsUserInfo().ConfigureAwait(false).GetAwaiter().GetResult();

            ExecuteCoreAsync().ConfigureAwait(false).GetAwaiter().GetResult();

            return 0;
        }

        protected abstract Task ExecuteCoreAsync();

        protected void SetOutputVariable(string name, string value)
        {
            log.SetOutputVariable($"AwsOutputs[{name}]", value ?? "", variables);
            log.Info($"Saving variable \"Octopus.Action[{variables["Octopus.Action.Name"]}].Output.AwsOutputs[{name}]\"");
        }

        protected void SetOutputVariables(IReadOnlyCollection<VariableOutput> variableOutputs)
        {
            if (variableOutputs?.Any() != true) return;

            foreach (var variableOutput in variableOutputs)
            {
                SetOutputVariable(variableOutput.Name, variableOutput.Value);
            }
        }

        async Task LogAwsUserInfo()
        {
            if (variables.IsSet(SpecialVariableNames.Aws.AssumeRoleARN) ||
                !variables.IsSet(SpecialVariableNames.Aws.AccountId) ||
                !variables.IsSet(variables.Get(SpecialVariableNames.Aws.AccountId) +
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
                var client = await amazonIdentityManagementClient.Value;

                var result = await client.GetUserAsync(new GetUserRequest());

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
                var client = await amazonSecurityTokenClient.Value;

                (await client.GetCallerIdentityAsync(new GetCallerIdentityRequest()))
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

        public virtual void Dispose()
        {
            if (amazonIdentityManagementClient.IsValueCreated) amazonIdentityManagementClient.Value.Dispose();

            if (amazonSecurityTokenClient.IsValueCreated) amazonSecurityTokenClient.Value.Dispose(); 
        }
    }
}