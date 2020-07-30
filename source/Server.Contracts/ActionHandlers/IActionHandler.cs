using System.Collections.Generic;
using Sashimi.Server.Contracts.ActionHandlers.Validation;

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

        bool RequiresAccount(IReadOnlyDictionary<string, string> properties);
        string GetAccountIdOrExpression(IReadOnlyDictionary<string, string> properties);
        
        IActionHandlerResult Execute(IActionHandlerContext context);
    }
}