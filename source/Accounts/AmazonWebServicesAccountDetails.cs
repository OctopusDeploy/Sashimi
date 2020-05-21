using Octopus.Data.Model;
using Sashimi.Server.Contracts.Accounts;
using Sashimi.Server.Contracts.Accounts.Temp;

namespace Sashimi.Accounts
{
    public class AmazonWebServicesAccountDetails : IAccountDetails, IAmazonWebServicesAccountDetails
    {
        public AmazonWebServicesAccountDetails(string accessKey, SensitiveString secretKey)
        {
            AccessKey = accessKey;
            SecretKey = secretKey;
        }

        public string AccessKey { get; set; }
        public SensitiveString SecretKey { get; set; }
    }
}