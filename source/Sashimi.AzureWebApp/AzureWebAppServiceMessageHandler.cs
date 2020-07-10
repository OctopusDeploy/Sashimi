using System;
using System.Collections.Generic;
using System.Linq;
using Octopus.Diagnostics;
using Octostache;
using Sashimi.Azure.Accounts;
using Sashimi.Azure.Common.Extensions;
using Sashimi.AzureWebApp.Endpoints;
using Sashimi.Server.Contracts;
using Sashimi.Server.Contracts.Accounts;
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

        public AccountType[] SupportedAccountTypes { get; } = {AccountTypes.AzureServicePrincipalAccountType};

        public DeploymentTargetType DeploymentTargetType => AzureWebAppEndpoint.AzureWebAppDeploymentTargetType;

        public Endpoint BuildEndpoint(IDictionary<string, string> messageProperties, VariableDictionary variables,
            Func<string, string> accountIdResolver, Func<string, string> certificateIdResolver)
        {
            var accountIdOrName = GetAccountIdOrNameFromAction(messageProperties, variables);
            var resolvedAccountId = accountIdResolver(accountIdOrName);
            if (resolvedAccountId == null)
            {
                var message = $"Account with Id / Name, {accountIdOrName}, not found.";
                logger.Error(message);
                throw new Exception(message);
            }

            var endpoint = new AzureWebAppEndpoint
            {
                WebAppName = messageProperties[AzureWebAppServiceMessageNames.WebAppNameAttribute],
                ResourceGroupName = messageProperties[AzureWebAppServiceMessageNames.ResourceGroupNameAttribute],
                AccountId = resolvedAccountId
            };

            if (messageProperties.ContainsPropertyWithValue(AzureWebAppServiceMessageNames.WebAppSlotNameAttribute))
            {
                var slotName = messageProperties[AzureWebAppServiceMessageNames.WebAppSlotNameAttribute];
                endpoint.WebAppSlotName = slotName;
            }

            return endpoint;
        }

        public ServiceMessageValidationResult IsServiceMessageValid(IDictionary<string, string> messageProperties,
            VariableDictionary variables)
        {
            var messages = new List<string>();

            if (!messageProperties.ContainsPropertyWithValue(AzureWebAppServiceMessageNames.WebAppNameAttribute))
                messages.Add("Web App Name is missing");
            if (!messageProperties.ContainsPropertyWithValue(AzureWebAppServiceMessageNames.ResourceGroupNameAttribute))
                messages.Add("Resource Group Name is missing");
            if (!messageProperties.ContainsPropertyWithValue(AzureWebAppServiceMessageNames.AccountIdOrNameAttribute) &&
                string.IsNullOrWhiteSpace(variables[SpecialVariables.Action.Azure.AccountId]))
                messages.Add("Account Id or Name is missing");
            if (messages.Any())
            {
                return ServiceMessageValidationResult.Invalid(messages);
            }

            return ServiceMessageValidationResult.Valid;
        }

        static string GetAccountIdOrNameFromAction(IDictionary<string, string> messageProperties,
            VariableDictionary variables)
        {
            var accountId = messageProperties[AzureWebAppServiceMessageNames.AccountIdOrNameAttribute];

            if (string.IsNullOrEmpty(accountId))
            {
                accountId = variables[SpecialVariables.Action.Azure.AccountId];
            }

            return accountId;
        }

        static class AzureWebAppServiceMessageNames
        {
            public const string CreateTargetName = "create-azurewebapptarget";
            public const string AccountIdOrNameAttribute = "account";
            public const string WebAppNameAttribute = "webAppName";
            public const string ResourceGroupNameAttribute = "resourceGroupName";
            public const string WebAppSlotNameAttribute = "webAppSlot";
        }
    }
}