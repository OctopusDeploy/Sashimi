using System;
using System.Collections.Generic;
using Octopus.Diagnostics;
using Octostache;
using Sashimi.AzureWebApp.Endpoints;
using Sashimi.Server.Contracts.Endpoints;
using Sashimi.Server.Contracts.ServiceMessages;

namespace Sashimi.AzureWebApp
{
    class AzureWebAppServiceMessageHandler : ICreateTargetServiceMessageHandler
    {
        readonly ILog logger;

        public AzureWebAppServiceMessageHandler(ILog logger)
        {
            this.logger = logger;
        }

        public string AuditEntryDescription => "Azure Web App Target";
        public string ServiceMessageName => AzureWebAppServiceMessageNames.CreateTargetName;

        public Endpoint BuildEndpoint(IDictionary<string, string> messageProperties, VariableDictionary variables,
            Func<string, string> accountIdResolver, Func<string, string> certificateIdResolver, Func<string, string> workerPoolIdResolver)
        {
            if (messageProperties == null) throw new ArgumentNullException(nameof(messageProperties));
            if (variables == null) throw new ArgumentNullException(nameof(variables));
            if (accountIdResolver == null) throw new ArgumentNullException(nameof(accountIdResolver));

            var endpoint = new AzureWebAppEndpoint();

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

            messageProperties.TryGetValue(AzureWebAppServiceMessageNames.WebAppNameAttribute, out var wepAppName);
            messageProperties.TryGetValue(AzureWebAppServiceMessageNames.ResourceGroupNameAttribute,
                out var resourceGroupName);
            
            endpoint.WebAppName = wepAppName;
            endpoint.ResourceGroupName = resourceGroupName;

            if (messageProperties.TryGetValue(AzureWebAppServiceMessageNames.WebAppSlotNameAttribute, out var webAppSlotName) &&
                !string.IsNullOrWhiteSpace(webAppSlotName))
            {
                endpoint.WebAppSlotName = webAppSlotName;
            }

            return endpoint;
        }

        static string GetAccountIdOrNameFromAction(IDictionary<string, string> messageProperties,
            VariableDictionary variables)
        {
            messageProperties.TryGetValue(AzureWebAppServiceMessageNames.AccountIdOrNameAttribute, out var accountId);
            
            if (string.IsNullOrWhiteSpace(accountId))
            {
                accountId = variables.Get(SpecialVariables.Action.Azure.AccountId);
            }

            return accountId;
        }

        internal static class AzureWebAppServiceMessageNames
        {
            public const string CreateTargetName = "create-azurewebapptarget";
            public const string AccountIdOrNameAttribute = "account";
            public const string WebAppNameAttribute = "webAppName";
            public const string ResourceGroupNameAttribute = "resourceGroupName";
            public const string WebAppSlotNameAttribute = "webAppSlot";
        }
    }
}