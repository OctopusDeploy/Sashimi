using System.Collections.Generic;

namespace Sashimi.Server.Contracts.ActionHandlers
{
    public interface IActionHandlerWithAccount: IActionHandler
    {
        string[] StepBasedVariableNameForAccountIds { get; }

        bool RequiresAccount(IReadOnlyDictionary<string, string> properties);
        string GetAccountIdOrExpression(IReadOnlyDictionary<string, string> properties);
    }
}