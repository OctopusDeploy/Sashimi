using Octopus.Data.Model;
using Sashimi.Server.Contracts.Accounts;
using Sashimi.Server.Contracts.Accounts.Temp;

namespace Sashimi.Accounts
{
    public class AzureAccountDetails : IAzureAccountDetails
    {
        public AzureAccountDetails(string subscriptionNumber, string certificateThumbprint, string azureEnvironment, string serviceManagementEndpointBaseUri, string serviceManagementEndpointSuffix, SensitiveString certificateBytes)
        {
            SubscriptionNumber = subscriptionNumber;
            CertificateThumbprint = certificateThumbprint;
            AzureEnvironment = azureEnvironment;
            ServiceManagementEndpointBaseUri = serviceManagementEndpointBaseUri;
            ServiceManagementEndpointSuffix = serviceManagementEndpointSuffix;
            CertificateBytes = certificateBytes;
        }

        public string SubscriptionNumber { get; set;}
        public string CertificateThumbprint { get; set;}

        public string AzureEnvironment { get; set; }
        public string ServiceManagementEndpointBaseUri { get; set; }
        public string ServiceManagementEndpointSuffix { get; set; }

        public SensitiveString CertificateBytes { get; set; }
    }
}