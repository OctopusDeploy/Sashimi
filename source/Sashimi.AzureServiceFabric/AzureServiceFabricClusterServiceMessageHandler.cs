using System;
using System.Collections.Generic;
using System.Linq;
using Octopus.Data.Model;
using Octostache;
using Sashimi.Azure.Common.Extensions;
using Sashimi.AzureServiceFabric.Endpoints;
using Sashimi.Server.Contracts;
using Sashimi.Server.Contracts.Accounts;
using Sashimi.Server.Contracts.Endpoints;
using Sashimi.Server.Contracts.ServiceMessages;

namespace Sashimi.AzureServiceFabric
{
    class AzureServiceFabricClusterServiceMessageHandler : ICreateTargetServiceMessageHandler
    {
        static readonly string[] SecurityModeSecureClientCertificateAliases =
            {"secureclientcertificate", "clientcertificate", "certificate"};

        static readonly string[] SecurityModeAzureActiveDirectoryAliases = {"aad", "azureactivedirectory",};

        public string AuditEntryDescription => "Azure Service Fabric Target";
        public string ServiceMessageName => AzureServiceFabricServiceMessageNames.CreateTargetName;

        public DeploymentTargetType DeploymentTargetType =>
            AzureServiceFabricClusterEndpoint.AzureServiceFabricClusterDeploymentTargetType;

        public AccountType[] SupportedAccountTypes { get; } = { };

        public ServiceMessageValidationResult IsServiceMessageValid(IDictionary<string, string> messageProperties,
            VariableDictionary variables)
        {
            var messages = new List<string>();
            var securityModeValidation = ValidateSecurityMode(messageProperties);
            messages.AddRange(securityModeValidation.Messages);

            var connectionEndpointIsValid =
                messageProperties.ContainsPropertyWithValue(AzureServiceFabricServiceMessageNames
                    .ConnectionEndpointAttribute);
            var isValid = securityModeValidation.IsValid && connectionEndpointIsValid;
            if (!isValid)
            {
                if (!connectionEndpointIsValid) messages.Add("Connection endpoint is missing");
                ServiceMessageValidationResult.Invalid(messages);
            }

            return ServiceMessageValidationResult.Valid;
        }

        public Endpoint BuildEndpoint(IDictionary<string, string> messageProperties, VariableDictionary variables,
            Func<string, string> accountIdResolver,
            Func<string, string> certificateIdResolver)
        {
            var azureServiceFabricClusterEndpoint = new AzureServiceFabricClusterEndpoint
            {
                SecurityMode = GetSecurityMode(messageProperties),
                ConnectionEndpoint =
                    messageProperties[AzureServiceFabricServiceMessageNames.ConnectionEndpointAttribute]
            };

            switch (azureServiceFabricClusterEndpoint.SecurityMode)
            {
                case AzureServiceFabricSecurityMode.SecureClientCertificate:
                    var certificateId =
                        certificateIdResolver(
                            messageProperties[AzureServiceFabricServiceMessageNames.CertificateIdOrNameAttribute]);
                    if (string.IsNullOrEmpty(certificateId))
                    {
                        var message =
                            $"Certificate with Id / Name {messageProperties[AzureServiceFabricServiceMessageNames.CertificateIdOrNameAttribute]} not found.";
                        throw new Exception(message);
                    }

                    azureServiceFabricClusterEndpoint.ClientCertVariable = certificateId;
                    azureServiceFabricClusterEndpoint.ServerCertThumbprint =
                        messageProperties[AzureServiceFabricServiceMessageNames.CertificateThumbprintAttribute];
                    azureServiceFabricClusterEndpoint.CertificateStoreLocation =
                        messageProperties[AzureServiceFabricServiceMessageNames.CertificateStoreLocationAttribute];
                    azureServiceFabricClusterEndpoint.CertificateStoreName = GetCertificateStoreName(messageProperties);
                    break;
                case AzureServiceFabricSecurityMode.SecureAzureAD:
                    azureServiceFabricClusterEndpoint.ServerCertThumbprint =
                        messageProperties[AzureServiceFabricServiceMessageNames.CertificateThumbprintAttribute];
                    azureServiceFabricClusterEndpoint.AadUserCredentialUsername =
                        messageProperties[AzureServiceFabricServiceMessageNames.ActiveDirectoryUsernameAttribute];
                    azureServiceFabricClusterEndpoint.AadUserCredentialPassword = new SensitiveString(
                        messageProperties[AzureServiceFabricServiceMessageNames.ActiveDirectoryPasswordAttribute]);
                    azureServiceFabricClusterEndpoint.AadCredentialType =
                        AzureServiceFabricCredentialType.UserCredential;
                    break;
            }

            return azureServiceFabricClusterEndpoint;
        }

        static string GetCertificateStoreName(IDictionary<string, string> messageProperties)
        {
            return messageProperties.ContainsPropertyWithValue(AzureServiceFabricServiceMessageNames
                .CertificateStoreNameAttribute)
                ? "My"
                : messageProperties[AzureServiceFabricServiceMessageNames.CertificateStoreNameAttribute];
        }

        static AzureServiceFabricSecurityMode GetSecurityMode(IDictionary<string, string> messageProperties)
        {
            var securityModeValue = messageProperties[AzureServiceFabricServiceMessageNames.SecurityModeAttribute];
            if (SecurityModeSecureClientCertificateAliases.Any(x =>
                securityModeValue.Equals(x, StringComparison.OrdinalIgnoreCase)))
            {
                return AzureServiceFabricSecurityMode.SecureClientCertificate;
            }

            if (SecurityModeAzureActiveDirectoryAliases.Any(x =>
                securityModeValue.Equals(x, StringComparison.OrdinalIgnoreCase)))
            {
                return AzureServiceFabricSecurityMode.SecureAzureAD;
            }

            return AzureServiceFabricSecurityMode.Unsecure;
        }

        static ServiceMessageValidationResult ValidateSecurityMode(IDictionary<string, string> messageProperties)
        {
            var securityMode = GetSecurityMode(messageProperties);

            var isValid = false;
            var messages = new List<string>();

            var certThumbprintIsValid =
                messageProperties.ContainsPropertyWithValue(AzureServiceFabricServiceMessageNames
                    .CertificateThumbprintAttribute);
            var certificateIdOrNameIsValid =
                messageProperties.ContainsPropertyWithValue(AzureServiceFabricServiceMessageNames
                    .CertificateIdOrNameAttribute);
            var activeDirectorUsernameIsValid =
                messageProperties.ContainsPropertyWithValue(AzureServiceFabricServiceMessageNames
                    .ActiveDirectoryUsernameAttribute);
            var activeDirectoryPasswordIsValid =
                messageProperties.ContainsPropertyWithValue(AzureServiceFabricServiceMessageNames
                    .ActiveDirectoryPasswordAttribute);

            switch (securityMode)
            {
                case AzureServiceFabricSecurityMode.Unsecure:
                    isValid = true;
                    break;
                case AzureServiceFabricSecurityMode.SecureClientCertificate:
                    isValid = certThumbprintIsValid && certificateIdOrNameIsValid;
                    if (!isValid)
                    {
                        if (!certThumbprintIsValid) messages.Add("Certificate Thumbprint is missing");
                        if (!certificateIdOrNameIsValid) messages.Add("Certificate Id Or Name is missing");
                    }

                    break;
                case AzureServiceFabricSecurityMode.SecureAzureAD:
                    isValid = certThumbprintIsValid && activeDirectorUsernameIsValid && activeDirectoryPasswordIsValid;
                    if (!isValid)
                    {
                        if (!certThumbprintIsValid) messages.Add("Certificate Thumbprint is missing");
                        if (!activeDirectorUsernameIsValid) messages.Add("Active Directory Username is missing");
                        if (!activeDirectoryPasswordIsValid) messages.Add("Active Directory Password is missing");
                    }

                    break;
            }

            if (!isValid)
            {
                ServiceMessageValidationResult.Invalid(messages);
            }

            return ServiceMessageValidationResult.Valid;
        }

        static class AzureServiceFabricServiceMessageNames
        {
            public const string CreateTargetName = "create-azureservicefabrictarget";
            public const string ConnectionEndpointAttribute = "connectionEndpoint";
            public const string SecurityModeAttribute = "securityMode";
            public const string CertificateThumbprintAttribute = "certificateThumbprint";
            public const string CertificateIdOrNameAttribute = "certificate";
            public const string ActiveDirectoryUsernameAttribute = "activeDirectoryUsername";
            public const string ActiveDirectoryPasswordAttribute = "activeDirectoryPassword";
            public const string CertificateStoreLocationAttribute = "certificateStoreLocation";
            public const string CertificateStoreNameAttribute = "certificateStoreName";
        }
    }
}