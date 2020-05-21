using Octopus.Data.Model;

namespace Sashimi.Server.Contracts.Accounts.Temp
{
    public interface IAzureAccountDetails
    {
        string SubscriptionNumber { get; set; }
        string CertificateThumbprint { get; set; }
        string AzureEnvironment { get; set; }
        string ServiceManagementEndpointBaseUri { get; set; }
        string ServiceManagementEndpointSuffix { get; set; }
        SensitiveString CertificateBytes { get; set; }
    }
}