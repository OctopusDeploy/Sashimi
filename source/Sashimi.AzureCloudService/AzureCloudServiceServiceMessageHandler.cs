using System;
using System.Collections.Generic;
using Octopus.Diagnostics;
using Octostache;
using Sashimi.AzureCloudService.Endpoints;
using Sashimi.Server.Contracts.Accounts;
using Sashimi.Server.Contracts.Endpoints;
using Sashimi.Server.Contracts.ServiceMessages;

namespace Sashimi.AzureCloudService
{
    class AzureCloudServiceServiceMessageHandler : ICreateTargetServiceMessageHandler
    {
        readonly ILog logger;

        public AzureCloudServiceServiceMessageHandler(ILog logger)
        {
            this.logger = logger;
        }

        public string AuditEntryDescription => "Azure Cloud Service Target";

        public string ServiceMessageName => AzureCloudServiceServiceMessageNames.CreateTargetName;

        public Endpoint BuildEndpoint(IDictionary<string, string> messageProperties, VariableDictionary variables,
            Func<string, string> accountIdResolver, Func<string, string> certificateIdResolver,
            Func<string, string> workerPoolIdResolver, Func<string, AccountType> accountTypeResolver)
        {
            if (messageProperties == null) throw new ArgumentNullException(nameof(messageProperties));
            if (variables == null) throw new ArgumentNullException(nameof(variables));
            if (accountIdResolver == null) throw new ArgumentNullException(nameof(accountIdResolver));

            // TODO should this be getting the account id as an Azure specific scoped variable

            var endpoint = new AzureCloudServiceEndpoint();

            var accountIdOrName = GetAccountIdOrNameFromAction(messageProperties, variables);
            if (!string.IsNullOrWhiteSpace(accountIdOrName))
            {
                var resolvedAccountId = accountIdResolver(accountIdOrName);
                if (resolvedAccountId == null)
                {
                    var message = $"Account with Id / Name, {accountIdOrName}, not found.";
                    logger.Error(message);
                    throw new Exception(message);
                }

                endpoint.AccountId = resolvedAccountId;
            }

            messageProperties.TryGetValue(AzureCloudServiceServiceMessageNames.AzureCloudServiceNameAttribute,
                out var cloudServiceName);
            messageProperties.TryGetValue(AzureCloudServiceServiceMessageNames.AzureStorageAccountAttribute,
                out var storageAccountName);

            endpoint.CloudServiceName = cloudServiceName;
            endpoint.StorageAccountName = storageAccountName;
            endpoint.Slot = GetSlot(messageProperties);
            endpoint.SwapIfPossible = GetSwap(messageProperties);
            endpoint.UseCurrentInstanceCount = GetUseCurrentInstance(messageProperties);

            return endpoint;
        }

        static string GetAccountIdOrNameFromAction(IDictionary<string, string> messageProperties,
            VariableDictionary variables)
        {
            messageProperties.TryGetValue(AzureCloudServiceServiceMessageNames.AccountIdOrNameAttribute, out var accountId);

            if (string.IsNullOrWhiteSpace(accountId))
            {
                accountId = variables.Get(SpecialVariables.Action.Azure.AccountId);
            }

            return accountId;
        }

        static bool GetSwap(IDictionary<string, string> messageProperties)
        {
            var propertyValue = messageProperties[AzureCloudServiceServiceMessageNames.SwapAttribute];
            return string.IsNullOrWhiteSpace(propertyValue) ||
                   !propertyValue.Equals("deploy", StringComparison.OrdinalIgnoreCase);
        }

        static string GetSlot(IDictionary<string, string> messageProperties)
        {
            var propertyValue = messageProperties[AzureCloudServiceServiceMessageNames.AzureDeploymentSlotAttribute];
            if (!string.IsNullOrEmpty(propertyValue) &&
                propertyValue.Equals("production", StringComparison.OrdinalIgnoreCase))
            {
                return AzureCloudServiceEndpointDeploymentSlot.Production;
            }

            return AzureCloudServiceEndpointDeploymentSlot.Staging;
        }

        static bool GetUseCurrentInstance(IDictionary<string, string> messageProperties)
        {
            var propertyValue = messageProperties[AzureCloudServiceServiceMessageNames.InstanceCountAttribute];
            return string.IsNullOrEmpty(propertyValue) ||
                   !propertyValue.Equals("configuration", StringComparison.OrdinalIgnoreCase);
        }

        internal static class AzureCloudServiceServiceMessageNames
        {
            public const string CreateTargetName = "create-azurecloudservicetarget";
            public const string AccountIdOrNameAttribute = "account";
            public const string AzureCloudServiceNameAttribute = "azureCloudServiceName";
            public const string AzureStorageAccountAttribute = "azureStorageAccount";
            public const string AzureDeploymentSlotAttribute = "azureDeploymentSlot";
            public const string SwapAttribute = "swap";
            public const string InstanceCountAttribute = "instanceCount";
        }

        internal static class AzureCloudServiceEndpointDeploymentSlot
        {
            public const string Staging = "Staging";
            public const string Production = "Production";
        }
    }
}