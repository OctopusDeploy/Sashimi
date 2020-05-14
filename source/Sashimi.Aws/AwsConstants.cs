using Sashimi.Server.Contracts.ActionHandlers;

namespace Sashimi.Aws
{
    public class AwsConstants
    {
        public static readonly string[] CloudTemplateProviderIds ={ "CloudFormation", "CF", "AWSCloudFormation" };
        public static readonly ActionHandlerCategory AwsActionHandlerCategory = new ActionHandlerCategory("Aws", "AWS", 600);
    }
}