using System;

namespace Sashimi.Server.Contracts.ActionHandlers
{
    public interface IContributeToScriptDeployment
    {
        ActionContributionResult Contribute(DeploymentTargetType deploymentTargetType, IActionHandlerContext context);
    }
}