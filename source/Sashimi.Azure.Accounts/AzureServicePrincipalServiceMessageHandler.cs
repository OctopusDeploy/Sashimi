using System.Collections.Generic;
using Octostache;
using Sashimi.Server.Contracts.Accounts;
using Sashimi.Server.Contracts.ServiceMessages;

namespace Sashimi.Azure.Accounts
{
    class AzureServicePrincipalServiceMessageHandler : IServiceMessageHandler
    {
        public string AuditEntryDescription => "Azure Service Principal account";
        public string ServiceMessageName => CreateAzureAccountServiceMessagePropertyNames.Name;

        public ServiceMessageValidationResult IsServiceMessageValid(IDictionary<string, string> messageProperties, VariableDictionary variables)
        {
            var subscriptionAttributeValid = messageProperties.ContainsPropertyWithGuid(CreateAzureAccountServiceMessagePropertyNames.SubscriptionAttribute);
            var applicationAttributeValid = messageProperties.ContainsPropertyWithGuid(CreateAzureAccountServiceMessagePropertyNames.ServicePrincipal.ApplicationAttribute);
            var tenantAttributeValid = messageProperties.ContainsPropertyWithGuid(CreateAzureAccountServiceMessagePropertyNames.ServicePrincipal.TenantAttribute);
            var passwordAttributeValid = messageProperties.ContainsPropertyWithValue(CreateAzureAccountServiceMessagePropertyNames.ServicePrincipal.PasswordAttribute);
            bool isValid = subscriptionAttributeValid && applicationAttributeValid && tenantAttributeValid && passwordAttributeValid;

            var hasEnvironmentConfigured = messageProperties.ContainsPropertyWithValue(CreateAzureAccountServiceMessagePropertyNames.ServicePrincipal.EnvironmentAttribute);
            var baseUriAttributeValid = messageProperties.ContainsPropertyWithValue(CreateAzureAccountServiceMessagePropertyNames.ServicePrincipal.BaseUriAttribute);
            var resourceManagementUriAttributeValid = messageProperties.ContainsPropertyWithValue(CreateAzureAccountServiceMessagePropertyNames.ServicePrincipal.ResourceManagementBaseUriAttribute);
            if (hasEnvironmentConfigured)
            {
                isValid = isValid && baseUriAttributeValid && resourceManagementUriAttributeValid;
            }

            if (!isValid)
            {
                List<string> messages = new List<string>();
                if (!subscriptionAttributeValid) messages.Add("Subscription Id is missing or invalid");
                if (!applicationAttributeValid) messages.Add("Application Id is missing or invalid");
                if (!tenantAttributeValid) messages.Add("Tenant Id is missing or invalid");
                if (!passwordAttributeValid) messages.Add("Password is missing");

                if (hasEnvironmentConfigured)
                {
                    if (!baseUriAttributeValid) messages.Add("AD Endpoint Base Uri is missing");
                    if (!resourceManagementUriAttributeValid) messages.Add("Resource Management Base Uri is missing");
                }

                return ServiceMessageValidationResult.Invalid(messages);
            }

            return ServiceMessageValidationResult.Valid;
        }
    }
}