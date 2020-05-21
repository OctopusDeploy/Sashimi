using Octopus.Data.Model;
using Sashimi.Server.Contracts.Accounts;
using Sashimi.Server.Contracts.Accounts.Temp;

namespace Sashimi.Accounts
{
    public class AzureServicePrincipalAccountDetails : IAzureServicePrincipalAccountDetails
    {
        public AzureServicePrincipalAccountDetails(string subscriptionNumber, string clientId, string tenantId, SensitiveString password, string azureEnvironment, string resourceManagementEndpointBaseUri, string activeDirectoryEndpointBaseUri)
        {
            SubscriptionNumber = subscriptionNumber;
            ClientId = clientId;
            TenantId = tenantId;
            Password = password;
            AzureEnvironment = azureEnvironment;
            ResourceManagementEndpointBaseUri = resourceManagementEndpointBaseUri;
            ActiveDirectoryEndpointBaseUri = activeDirectoryEndpointBaseUri;
        }

        public string SubscriptionNumber { get;  set;}

        public string ClientId { get;  set;}

        public string TenantId { get;  set;}

        public SensitiveString Password { get; set; }

        public string AzureEnvironment { get; set; }
        public string ResourceManagementEndpointBaseUri { get;  set;}
        public string ActiveDirectoryEndpointBaseUri { get; set; }
    }
}