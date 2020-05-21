using Octopus.Data.Model;

namespace Sashimi.Server.Contracts.Accounts.Temp
{
    public interface IAzureServicePrincipalAccountDetails : IAccountDetails
    {
        string SubscriptionNumber { get; set; }
        string ClientId { get; set;}
        string TenantId { get; set;}
        SensitiveString Password { get; set;}
        string AzureEnvironment { get; set;}
        string ResourceManagementEndpointBaseUri { get; set;}
        string ActiveDirectoryEndpointBaseUri { get; set;}
    }
}