using System.Collections.Generic;
using Octopus.Data.Model;
using Sashimi.Server.Contracts.Accounts;
using Sashimi.Server.Contracts.ServiceMessages;

namespace Sashimi.Azure.Accounts
{
    class AzureServicePrincipalServiceMessageHandler : ICreateAccountDetailsServiceMessageHandler
    {
        public string AuditEntryDescription => "Azure Service Principal account";
        public string ServiceMessageName => CreateAzureAccountServiceMessagePropertyNames.Name;
        public AccountDetails CreateAccountDetails(IDictionary<string, string> messageProperties)
        {
            messageProperties.TryGetValue(CreateAzureAccountServiceMessagePropertyNames.SubscriptionAttribute,
                out var subscriptionNumber);
            messageProperties.TryGetValue(CreateAzureAccountServiceMessagePropertyNames.ServicePrincipal.ApplicationAttribute,
                out var clientId);
            messageProperties.TryGetValue(CreateAzureAccountServiceMessagePropertyNames.ServicePrincipal.PasswordAttribute,
                out var password);
            messageProperties.TryGetValue(CreateAzureAccountServiceMessagePropertyNames.ServicePrincipal.TenantAttribute,
                out var tenantId);

            var details = new AzureServicePrincipalAccountDetails
            {
                SubscriptionNumber = subscriptionNumber,
                ClientId = clientId,
                Password = password.ToSensitiveString(),
                TenantId = tenantId
            };

            if (messageProperties.TryGetValue(CreateAzureAccountServiceMessagePropertyNames.ServicePrincipal.EnvironmentAttribute, out var environment) &&
                string.IsNullOrWhiteSpace(environment))
            {
                messageProperties.TryGetValue(CreateAzureAccountServiceMessagePropertyNames.ServicePrincipal.BaseUriAttribute,
                    out var baseUri);
                messageProperties.TryGetValue(CreateAzureAccountServiceMessagePropertyNames.ServicePrincipal.ResourceManagementBaseUriAttribute,
                    out var resourceManagementBaseUri);

                details.AzureEnvironment = environment;
                details.ActiveDirectoryEndpointBaseUri = baseUri;
                details.ResourceManagementEndpointBaseUri = resourceManagementBaseUri;
            }
            else
            {
                details.AzureEnvironment = string.Empty;
                details.ActiveDirectoryEndpointBaseUri = string.Empty;
                details.ResourceManagementEndpointBaseUri = string.Empty;
            }

            return details;
        }
    }
}