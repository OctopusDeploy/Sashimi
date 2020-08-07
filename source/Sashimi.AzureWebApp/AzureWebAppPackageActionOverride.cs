using System;
using Sashimi.AzureWebApp.Endpoints;
using Sashimi.Server.Contracts;
using Sashimi.Server.Contracts.ActionHandlers;

namespace Sashimi.AzureWebApp
{
    class AzureWebAppPackageActionOverride : IPackageActionOverride
    {
        public ActionOverrideResult ShouldOverride(DeploymentTargetType deploymentTargetType)
        {
            return deploymentTargetType == AzureWebAppEndpoint.AzureWebAppDeploymentTargetType
                ? ActionOverrideResult.RedirectToHandler<AzureWebAppActionHandler>()
                : ActionOverrideResult.RunDefaultAction();
        }
    }
}