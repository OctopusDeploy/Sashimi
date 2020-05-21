using Octopus.Data.Model;

namespace Sashimi.Server.Contracts.Accounts.Temp
{
    public class AzureAccountDetails : IAccountDetails
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

        public string SubscriptionNumber { get; }
        public string CertificateThumbprint { get; }

        public string AzureEnvironment { get;  }
        public string ServiceManagementEndpointBaseUri { get;  }
        public string ServiceManagementEndpointSuffix { get;  }

        public SensitiveString CertificateBytes { get; }
    }
}