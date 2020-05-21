using Octopus.Data.Model;

namespace Sashimi.Server.Contracts.Accounts.Temp
{
    public class AmazonWebServicesAccountDetails : IAccountDetails
    {
        public AmazonWebServicesAccountDetails(string accessKey, SensitiveString secretKey)
        {
            AccessKey = accessKey;
            SecretKey = secretKey;
        }

        public string AccessKey { get; }
        public SensitiveString SecretKey { get; }
    }
}