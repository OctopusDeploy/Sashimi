using System;

namespace Sashimi.Server.Contracts.ActionHandlers
{
    public abstract class ActionContributionResult
    {
        public static ActionContributionResult SkipDeployment()
        {
            return new SkipActionDeploymentContributionResult();
        }

        public static ActionContributionResult RedirectToHandler<THandle>() where THandle : IActionHandler
        {
            return new RedirectToHandlerContributionResult(typeof(THandle));
        }

        public static ActionContributionResult DoDefaultDeployment()
        {
            return new DoDefaultActionDeploymentContributionResult();
        }
    }

    public class SkipActionDeploymentContributionResult : ActionContributionResult
    {
        internal SkipActionDeploymentContributionResult()
        {

        }
    }

    public class DoDefaultActionDeploymentContributionResult : ActionContributionResult
    {
        internal DoDefaultActionDeploymentContributionResult()
        {

        }
    }

    public class RedirectToHandlerContributionResult : ActionContributionResult
    {
        internal RedirectToHandlerContributionResult(Type handler)
        {
            Handler = handler;
        }

        public Type Handler { get; }
    }
}