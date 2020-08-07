using System;

namespace Sashimi.Server.Contracts.ActionHandlers
{
    public interface IPackageActionOverride
    {
        ActionOverrideResult ShouldOverride(DeploymentTargetType deploymentTargetType, IActionHandlerContext context);
    }
}