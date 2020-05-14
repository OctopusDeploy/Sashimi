using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Amazon.CloudFormation;
using Amazon.CloudFormation.Model;
using Amazon.Runtime;
using Calamari.Aws.Exceptions;
using Calamari.Aws.Integration.CloudFormation;
using NuGet;
using Octopus.CoreUtilities;
using Octopus.CoreUtilities.Extensions;
using StackStatus = Calamari.Aws.Deployment.Conventions.StackStatus;

namespace Calamari.Aws.Deployment.CloudFormation
{

    public interface ICloudFormationService
    {
        Task DeleteByStackArn(StackArn stackArn, bool waitForCompletion);

        Task ExecuteChangeSet(StackArn stackArn, ChangeSetArn changeSetArn, bool waitForCompletion);

        Task OutputVariables(IVariables variables, bool hasOutputs = true);
    }

    public class CloudFormationService : CloudFormationServiceBase, ICloudFormationService
    {
        readonly IAmazonCloudFormation amazonCloudFormationClient;

        public CloudFormationService(ILog log, IAmazonCloudFormation amazonCloudFormationClient)
            : base(log)
        {
            this.amazonCloudFormationClient = amazonCloudFormationClient;
        }

        public async Task ExecuteChangeSet(StackArn stackArn, ChangeSetArn changeSetArn, bool waitForCompletion)
        {
            Guard.NotNull(stackArn, $"'{nameof(stackArn)}' cannot be null.");
            Guard.NotNull(changeSetArn, $"'{nameof(changeSetArn)}' cannot be null.");

            var response = await amazonCloudFormationClient.DescribeChangeSetAsync(stackArn, changeSetArn);
            
            if (!response.Changes.Any())
            {
                await amazonCloudFormationClient.DeleteChangeSetAsync(new DeleteChangeSetRequest
                {
                    ChangeSetName = changeSetArn.Value,
                    StackName = stackArn.Value
                });

                log.Info("No changes need to be performed.");
                return;
            }

            await WaitAndExecuteChangeSet(stackArn, changeSetArn);

            if (waitForCompletion)
            {
                await WithAmazonServiceExceptionHandling(async () =>
                    await amazonCloudFormationClient.WaitForStackToComplete(CloudFormationDefaults.StatusWaitPeriod, stackArn, LogAndThrowRollbacks(amazonCloudFormationClient, stackArn))
                );
            }
        }

        async Task<RunningChangeSet> WaitAndExecuteChangeSet(StackArn stack, ChangeSetArn changeSet)
        {
            try
            {
                var changes = await amazonCloudFormationClient.WaitForChangeSetCompletion(CloudFormationDefaults.StatusWaitPeriod,
                    new RunningChangeSet(stack, changeSet));


                if (changes.Status == ChangeSetStatus.FAILED)
                {
                    throw new UnknownException($"The changeset failed to create.\n{changes.StatusReason}");
                }

                await amazonCloudFormationClient.ExecuteChangeSetAsync(new ExecuteChangeSetRequest
                {
                    ChangeSetName = changeSet.Value,
                    StackName = stack.Value
                });

                return new RunningChangeSet(stack, changeSet);
            }
            catch (AmazonCloudFormationException exception) when (exception.ErrorCode == "AccessDenied")
            {
                throw new PermissionException(
                    "The AWS account used to perform the operation does not have the required permission to execute the changeset.\n" +
                    "Please ensure the current account has permission to perfrom action 'cloudformation:ExecuteChangeSet'.\n" +
                    exception.Message + "\n");
            }
            catch (AmazonServiceException exception)
            {
                LogAmazonServiceException(exception);
                throw;
            }
        }

        public Task OutputVariables(IVariables variables, bool hasOutputs = true)
        {
            Guard.NotNull(variables, $"'{nameof(variables)}' cannot be null.");

            var stackArn = new StackArn(variables.Get(AwsSpecialVariables.CloudFormation.StackName));

            return GetAndPipeOutputVariablesWithRetry(() =>
                    WithAmazonServiceExceptionHandling(async () => (await QueryStackAsync(amazonCloudFormationClient, stackArn)).ToMaybe()), 
                variables,
            true,
                CloudFormationDefaults.RetryCount,
                CloudFormationDefaults.StatusWaitPeriod,
                hasOutputs);
        }

        static async Task<(IReadOnlyList<VariableOutput> result, bool success)> GetOutputVariables(
            Func<Task<Maybe<Stack>>> query, bool hasOutputs)
        {
            List<VariableOutput> ConvertStackOutputs(Stack stack) =>
                stack.Outputs.Select(p => new VariableOutput(p.OutputKey, p.OutputValue)).ToList();

            return (await query()).Select(ConvertStackOutputs)
                .Map(result => (result: result.SomeOr(new List<VariableOutput>()), success: hasOutputs || result.Some()));
        }

        void PipeOutputs(IVariables variables, IEnumerable<VariableOutput> outputs)
        {
            foreach (var output in outputs)
            {
                SetOutputVariable(variables, output.Name, output.Value);
            }
        }

        async Task GetAndPipeOutputVariablesWithRetry(Func<Task<Maybe<Stack>>> query, IVariables variables, bool wait, int retryCount, TimeSpan waitPeriod, bool hasOutputs)
        {
            for (var retry = 0; retry < retryCount; ++retry)
            {
                var (result, success) = await GetOutputVariables(query, hasOutputs);
                if (success || !wait)
                {
                    PipeOutputs(variables, result);
                    break;
                }

                // Wait for a bit for and try again
                await Task.Delay(waitPeriod);
            }
        }

        public async Task DeleteByStackArn(StackArn stackArn, bool waitForCompletion)
        {
            Guard.NotNull(stackArn, $"'{nameof(stackArn)}' cannot be null.");

            if (await amazonCloudFormationClient.StackExistsAsync(stackArn, StackStatus.Completed) != StackStatus.DoesNotExist)
            {
                await WithAmazonServiceExceptionHandling(async () =>
                {
                    await amazonCloudFormationClient.DeleteStackAsync(stackArn);
                });

                log.Info($"Deleted stack called {stackArn.Value} in region {amazonCloudFormationClient.Config.RegionEndpoint.SystemName}");
            }
            else
            {
                log.Info($"No stack called {stackArn.Value} exists in region {amazonCloudFormationClient.Config.RegionEndpoint.SystemName}");
                return;
            }

            if (waitForCompletion)
            {
                await WithAmazonServiceExceptionHandling(async () =>
                {
                    await amazonCloudFormationClient.WaitForStackToComplete(CloudFormationDefaults.StatusWaitPeriod, stackArn,
                        LogAndThrowRollbacks(amazonCloudFormationClient, stackArn, true, false));
                });
            }
        }
    }
}
