using Octopus.Data.Model;

namespace Sashimi.Server.Contracts.Accounts.Temp
{
    public class AzureServicePrincipalAccountDetails : IAccountDetails
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

        public string SubscriptionNumber { get;  }

        public string ClientId { get;  }

        public string TenantId { get;  }

        public SensitiveString Password { get;  }

        public string AzureEnvironment { get;  }
        public string ResourceManagementEndpointBaseUri { get;  }
        public string ActiveDirectoryEndpointBaseUri { get;  }
    }
}