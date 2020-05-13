using System.Threading.Tasks;
using Amazon.CloudFormation;
using Calamari.Aws.Integration.CloudFormation;
using StackStatus = Calamari.Aws.Deployment.Conventions.StackStatus;

namespace Calamari.Aws.Deployment.CloudFormation
{

    public interface ICloudFormationService
    {
        Task DeleteByStackArn(StackArn stackArn, bool waitForCompletion);
    }

    public class CloudFormationService : CloudFormationServiceBase, ICloudFormationService
    {
        readonly IAmazonCloudFormation amazonCloudFormationClient;

        public CloudFormationService(ILog log, IAmazonCloudFormation amazonCloudFormationClient)
            : base(log)
        {
            this.amazonCloudFormationClient = amazonCloudFormationClient;
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
