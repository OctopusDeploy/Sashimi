using System.Threading.Tasks;
using Amazon.IdentityManagement;
using Amazon.S3;
using Amazon.SecurityToken;

namespace Calamari.Aws.Util
{
    public interface IAmazonClientFactory
    {
        Task<IAmazonS3> CreateS3Client();
        Task<IAmazonIdentityManagementService> CreateIdentityManagementService();
        Task<IAmazonSecurityTokenService> CreateSecurityTokenService();
    }
}