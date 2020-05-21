using Octopus.Data.Model;

namespace Sashimi.Accounts.Accounts
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