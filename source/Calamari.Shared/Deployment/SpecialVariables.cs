namespace Calamari.Deployment
{
    public static class SpecialVariables
    {
        public static class Package
        {
            public static readonly string EnabledFeatures = "Octopus.Action.EnabledFeatures";
        }

        public static class Action
        {
            public const string SkipRemainingConventions = "Octopus.Action.SkipRemainingConventions";
            public const string FailScriptOnErrorOutput = "Octopus.Action.FailScriptOnErrorOutput";

            public static class Aws
            {
                public static readonly string CloudFormationStackName = "Octopus.Action.Aws.CloudFormationStackName";
                public static readonly string CloudFormationTemplate = "Octopus.Action.Aws.CloudFormationTemplate";
                public static readonly string CloudFormationProperties = "Octopus.Action.Aws.CloudFormationProperties";
                public static readonly string AssumeRoleARN = "Octopus.Action.Aws.AssumedRoleArn";
                public static readonly string AccountId = "Octopus.Action.AwsAccount.Variable";
                public static readonly string CloudFormationAction = "Octopus.Action.Aws.CloudFormationAction";
            }
        }
    }
}
