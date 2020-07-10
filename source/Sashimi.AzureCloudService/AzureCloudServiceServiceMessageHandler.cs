using System;
using System.Collections.Generic;
using Octostache;
using Sashimi.Azure.Common.Extensions;
using Sashimi.AzureCloudService.Endpoints;
using Sashimi.Server.Contracts;
using Sashimi.Server.Contracts.Accounts;
using Sashimi.Server.Contracts.Endpoints;
using Sashimi.Server.Contracts.ServiceMessages;

namespace Sashimi.AzureCloudService
{
    class AzureCloudServiceServiceMessageHandler : ICreateTargetServiceMessageHandler
    {
        public string AuditEntryDescription => "Azure Cloud Service Target";

        public string ServiceMessageName => AzureCloudServiceServiceMessageNames.CreateTargetName;

        public AccountType[] SupportedAccountTypes { get; } = {AccountTypes.AzureSubscriptionAccountType};

        public DeploymentTargetType DeploymentTargetType =>
            AzureCloudServiceEndpoint.AzureCloudServiceDeploymentTargetType;

        public ServiceMessageValidationResult IsServiceMessageValid(IDictionary<string, string> messageProperties,
            VariableDictionary variables)
        {
            var accountIsValid =
                messageProperties.ContainsPropertyWithValue(AzureCloudServiceServiceMessageNames
                    .AccountIdOrNameAttribute);
            var cloudServiceNameIsValid =
                messageProperties.ContainsPropertyWithValue(AzureCloudServiceServiceMessageNames
                    .AzureCloudServiceNameAttribute);
            var storageAccountIsValid =
                messageProperties.ContainsPropertyWithValue(AzureCloudServiceServiceMessageNames
                    .AzureStorageAccountAttribute);

            if (!(accountIsValid && cloudServiceNameIsValid && storageAccountIsValid))
            {
                var messages = new List<string>();
                if (!accountIsValid) messages.Add("Account Id or Name is missing");
                if (!cloudServiceNameIsValid) messages.Add("Azure Cloud Service Name is missing");
                if (!storageAccountIsValid) messages.Add("Azure Storage Account is missing");
                return ServiceMessageValidationResult.Invalid(messages);
            }

            return ServiceMessageValidationResult.Valid;
        }

        public Endpoint BuildEndpoint(IDictionary<string, string> messageProperties, VariableDictionary variables,
            Func<string, string> accountIdResolver, Func<string, string> certificateIdResolver)
        {
            // TODO should this be getting the account id as an Azure specific scoped variable
            var accountId =
                accountIdResolver(messageProperties[AzureCloudServiceServiceMessageNames.AccountIdOrNameAttribute]) ??
                accountIdResolver(variables[SpecialVariables.Action.Azure.AccountId]);
            var endpoint = new AzureCloudServiceEndpoint
            {
                AccountId = accountId,
                CloudServiceName =
                    messageProperties[AzureCloudServiceServiceMessageNames.AzureCloudServiceNameAttribute],
                StorageAccountName =
                    messageProperties[AzureCloudServiceServiceMessageNames.AzureStorageAccountAttribute],
                Slot = GetSlot(messageProperties),
                SwapIfPossible = GetSwap(messageProperties),
                UseCurrentInstanceCount = GetUseCurrentInstance(messageProperties)
            };

            return endpoint;
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

        static class AzureCloudServiceServiceMessageNames
        {
            public const string CreateTargetName = "create-azurecloudservicetarget";
            public const string AccountIdOrNameAttribute = "account";
            public const string AzureCloudServiceNameAttribute = "azureCloudServiceName";
            public const string AzureStorageAccountAttribute = "azureStorageAccount";
            public const string AzureDeploymentSlotAttribute = "azureDeploymentSlot";
            public const string SwapAttribute = "swap";
            public const string InstanceCountAttribute = "instanceCount";
        }

        static class AzureCloudServiceEndpointDeploymentSlot
        {
            public const string Staging = "Staging";
            public const string Production = "Production";
        }
    }
}