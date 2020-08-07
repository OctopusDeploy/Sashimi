using System;
using Sashimi.AzureWebApp.Endpoints;
using Sashimi.Server.Contracts;
using Sashimi.Server.Contracts.ActionHandlers;

namespace Sashimi.AzureWebApp
{
    class AzureWebAppPackageContributor : IContributeToPackageDeployment
    {
        public ActionContributionResult Contribute(DeploymentTargetType deploymentTargetType, IActionHandlerContext context)
        {
            return deploymentTargetType == AzureWebAppEndpoint.AzureWebAppDeploymentTargetType
                ? ActionContributionResult.RedirectToHandler<AzureWebAppActionHandler>()
                : ActionContributionResult.DoDefaultDeployment();
        }
    }
}