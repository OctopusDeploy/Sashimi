using System.Threading.Tasks;
using Amazon.IdentityManagement;
using Amazon.S3;
using Amazon.SecurityToken;
using Calamari.CloudAccounts;

namespace Calamari.Aws.Util
{
    public class AmazonClientFactory : IAmazonClientFactory
    {
        readonly IAwsEnvironmentFactory environmentFactory;

        public AmazonClientFactory(IAwsEnvironmentFactory environmentFactory)
        {
            this.environmentFactory = environmentFactory;
        }
        
        public async Task<IAmazonS3> CreateS3Client()
        {
            var environment = await GetEnvironment();
            return new AmazonS3Client(environment.AwsCredentials, environment.AsClientConfig<AmazonS3Config>());
        }

        public async Task<IAmazonIdentityManagementService> CreateIdentityManagementService()
        {
            var environment = await GetEnvironment();
            return new AmazonIdentityManagementServiceClient(environment.AwsCredentials,
                environment.AsClientConfig<AmazonIdentityManagementServiceConfig>());
        }

        public async Task<IAmazonSecurityTokenService> CreateSecurityTokenService()
        {
            var environment = await GetEnvironment();
            return new AmazonSecurityTokenServiceClient(environment.AwsCredentials,
                environment.AsClientConfig<AmazonSecurityTokenServiceConfig>());
        }
        
        private async Task<AwsEnvironmentGeneration> GetEnvironment()
        {
            var environment = await environmentFactory.Create();
            return environment;
        }
    }
}