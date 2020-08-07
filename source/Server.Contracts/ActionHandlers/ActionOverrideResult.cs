using System;

namespace Sashimi.Server.Contracts.ActionHandlers
{
    public abstract class ActionOverrideResult
    {
        public static ActionOverrideResult SkipAction()
        {
            return new SkipActionResult();
        }

        public static ActionOverrideResult RedirectToHandler<THandle>() where THandle : IActionHandler
        {
            return new RedirectToHandlerResult(typeof(THandle));
        }

        public static ActionOverrideResult RunDefaultAction()
        {
            return new RunDefaultActionResult();
        }
    }

    public class SkipActionResult : ActionOverrideResult
    {
        internal SkipActionResult()
        {
        }
    }

    public class RunDefaultActionResult : ActionOverrideResult
    {
        internal RunDefaultActionResult()
        {
        }
    }

    public class RedirectToHandlerResult : ActionOverrideResult
    {
        internal RedirectToHandlerResult(Type handler)
        {
            Handler = handler;
        }

        public Type Handler { get; }
    }
}