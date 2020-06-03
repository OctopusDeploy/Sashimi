namespace Sashimi.AzureCloudService
{
    //TODO: This is duplicated from Server while we sort out a way for Sashimi to contribute special variables.
    static class SpecialVariables
    {
        public static class Action
        {
            public static class Azure
            {
                public static readonly string UseBundledAzureModules = "OctopusUseBundledAzureModules";

                public static readonly string UseBundledAzureCLI = "OctopusUseBundledAzureCLI";

                public static readonly string OctopusDisableAzureCLI = "OctopusDisableAzureCLI";

                public static readonly string UseBundledAzureModulesLegacy = "Octopus.Action.Azure.UseBundledAzurePowerShellModules";

                // do not reuse this value
                public static readonly string ActionTypeNameForDeprecatedAzureStep = "Octopus.Azure";

                public static readonly string CloudServiceHealthCheckActionTypeName = "Octopus.HealthCheck.AzureCloudService";

                public static readonly string CloudServiceActionTypeName = "Octopus.AzureCloudService";

                public static readonly string WebAppHealthCheckActionTypeName = "Octopus.HealthCheck.AzureWebApp";

                public static readonly string WebAppActionTypeName = "Octopus.AzureWebApp";

                public static readonly string ServiceFabricAppHealthCheckActionTypeName = "Octopus.HealthCheck.AzureServiceFabricApp";

                public static readonly string ServiceFabricAppActionTypeName = "Octopus.AzureServiceFabricApp";

                public static readonly string ServiceFabricPowerShellActionTypeName = "Octopus.AzureServiceFabricPowerShell";

                public static readonly string PowerShellActionTypeName = "Octopus.AzurePowerShell";

                public static readonly string ResourceGroupActionTypeName = "Octopus.AzureResourceGroup";

                public static readonly string AccountId = "Octopus.Action.Azure.AccountId";

                public static readonly string IsLegacyMode = "Octopus.Action.Azure.IsLegacyMode";

                public static readonly string Environment = "Octopus.Action.Azure.Environment";

                public static readonly string ResourceManagementEndPoint = "Octopus.Action.Azure.ResourceManagementEndPoint";
                public static readonly string ActiveDirectoryEndPoint = "Octopus.Action.Azure.ActiveDirectoryEndPoint";
                public static readonly string ServiceManagementEndPoint = "Octopus.Action.Azure.ServiceManagementEndPoint";
                public static readonly string StorageEndPointSuffix = "Octopus.Action.Azure.StorageEndpointSuffix";

                public static readonly string PowershellModulePath = "Octopus.Action.Azure.PowerShellModule";

                public static readonly string PackageExtractionPath = "Octopus.Action.Azure.PackageExtractionPath";

                public static readonly string SubscriptionId = "Octopus.Action.Azure.SubscriptionId";

                public static readonly string ClientId = "Octopus.Action.Azure.ClientId";

                public static readonly string TenantId = "Octopus.Action.Azure.TenantId";

                public static readonly string Password = "Octopus.Action.Azure.Password";

                public static readonly string CertificateThumbprint = "Octopus.Action.Azure.CertificateThumbprint";

                public static readonly string CertificateBytes = "Octopus.Action.Azure.CertificateBytes";

                public static readonly string WebAppName = "Octopus.Action.Azure.WebAppName";

                public static readonly string WebAppSlot = "Octopus.Action.Azure.DeploymentSlot";

                public static readonly string RemoveAdditionalFiles = "Octopus.Action.Azure.RemoveAdditionalFiles";

                public static readonly string PreserveAppData = "Octopus.Action.Azure.PreserveAppData";

                public static readonly string AppOffline = "Octopus.Action.Azure.AppOffline";

                public static readonly string PhysicalPath = "Octopus.Action.Azure.PhysicalPath";

                public static readonly string UseChecksum = "Octopus.Action.Azure.UseChecksum";

                public static readonly string CloudServiceName = "Octopus.Action.Azure.CloudServiceName";

                public static readonly string StorageAccountName = "Octopus.Action.Azure.StorageAccountName";

                public static readonly string Slot = "Octopus.Action.Azure.Slot";

                public static readonly string SwapIfPossible = "Octopus.Action.Azure.SwapIfPossible";

                public static readonly string UseCurrentInstanceCount = "Octopus.Action.Azure.UseCurrentInstanceCount";

                public static readonly string LogExtractedCspkg = "Octopus.Action.Azure.LogExtractedCspkg";

                public static readonly string CloudServiceConfigurationFileRelativePath = "Octopus.Action.Azure.CloudServiceConfigurationFileRelativePath";

                public static readonly string ResourceGroupName = "Octopus.Action.Azure.ResourceGroupName";

                public static readonly string ResourceGroupDeploymentMode = "Octopus.Action.Azure.ResourceGroupDeploymentMode";

                public static readonly string TemplateSource = "Octopus.Action.Azure.TemplateSource";

                public static readonly string ResourceGroupTemplate = "Octopus.Action.Azure.ResourceGroupTemplate";

                public static readonly string ResourceGroupTemplateParameters = "Octopus.Action.Azure.ResourceGroupTemplateParameters";
            }
        }
    }
}