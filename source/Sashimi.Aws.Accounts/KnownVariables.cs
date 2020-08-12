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

                public static readonly string AccountId = "Octopus.Action.AwsAccount.Variable";
                public static readonly string UseInstanceRole = "Octopus.Action.AwsAccount.UseInstanceRole";
                public static readonly string AwsRegion = "Octopus.Action.Aws.Region";
                public static readonly string AssumeRole = "Octopus.Action.Aws.AssumeRole";
                public static readonly string AssumedRoleArn = "Octopus.Action.Aws.AssumedRoleArn";
                public static readonly string AssumedRoleSession = "Octopus.Action.Aws.AssumedRoleSession";
                public static readonly string AssumeRoleSessionDurationSeconds = "Octopus.Action.Aws.AssumeRoleSessionDurationSeconds";
                public static readonly string IamCapabilities = "Octopus.Action.Aws.IamCapabilities";
                public static readonly string AssumeRoleExternalId = "Octopus.Action.Aws.AssumeRoleExternalId";
            }
        }
    }
}