using System;
using Octopus.Server.Extensibility.HostServices.Diagnostics;

namespace Sashimi.Server.Contracts.ActionHandlers
{
    /// <summary>
    /// Implementors of this interface must not keep state so that they can be reusable between steps and deployments
    /// </summary>
    public interface IActionHandler
    {
        string Id { get; }
        string Name { get; }
        string Description { get; }
        string? Keywords { get; }
        bool ShowInStepTemplatePickerUI { get; }
        bool WhenInAChildStepRunInTheContextOfTheTargetMachine { get; }
        bool CanRunOnDeploymentTarget { get; }
        ActionHandlerCategory[] Categories { get; }

        IActionHandlerResult Execute(IActionHandlerContext context, ITaskLog taskLog);
    }

    public interface IActionHandlerForTarget : IActionHandler
    {
        DeploymentTargetType DeploymentTargetType { get; }

        // Note: We have not moved CanRunOnDeploymentTarget into this abstraction as it does not have a correlation with the step being strongly associated
        // Note: with a single deployment target type. An example are the Java steps - the can be run on the target, as the target is usually a VM of some kind.
    }
}