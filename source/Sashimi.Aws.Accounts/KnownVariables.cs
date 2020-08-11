using Sashimi.Server.Contracts.Variables;

namespace Sashimi.Aws.Accounts
{
    public class KnownVariables
    {
        public static class Action
        {
            public static class Aws
            {
                public static readonly WellKnownVariable AccessKey = new WellKnownVariable("Octopus.Action.Amazon.AccessKey");
                public static readonly WellKnownVariable SecretKey = new WellKnownVariable("Octopus.Action.Amazon.SecretKey");
            }
        }
    }
}