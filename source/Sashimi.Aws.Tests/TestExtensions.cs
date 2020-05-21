using System.Collections.Generic;
using Calamari;
using Calamari.Tests.Shared;
using Newtonsoft.Json;
using Sashimi.Server.Contracts;
using Sashimi.Tests.Shared.Server;

namespace Sashimi.Aws.Tests
{
    public static class TestExtensions
    {
        public static void WithAwsAccount<TCalamariProgram>(this TestActionHandlerContext<TCalamariProgram> context) 
            where TCalamariProgram : CalamariFlavourProgram
        {
            context.Variables.Add(AwsSpecialVariables.Account.AccessKey, ExternalVariables.Get(ExternalVariable.AwsCloudFormationAndS3AccessKey));
            context.Variables.Add(AwsSpecialVariables.Account.SecretKey, ExternalVariables.Get(ExternalVariable.AwsCloudFormationAndS3SecretKey));    
        }
        
        public static void WithStackRole<TCalamariProgram>(this TestActionHandlerContext<TCalamariProgram> context, string stackRole) 
            where TCalamariProgram : CalamariFlavourProgram
        {
            context.Variables.Add(AwsSpecialVariables.Action.Aws.CloudFormation.RoleArn, stackRole);
        }
        
        public static void WithIamCapabilities<TCalamariProgram>(this TestActionHandlerContext<TCalamariProgram> context, IEnumerable<string> capabilities) 
            where TCalamariProgram : CalamariFlavourProgram
        {
            context.Variables.Add(AwsSpecialVariables.Action.Aws.IamCapabilities, JsonConvert.SerializeObject(capabilities));
        }

        public static void WithCloudFormationChangeSets<TCalamariProgram>(this TestActionHandlerContext<TCalamariProgram> context, bool generateName = true, bool deferExecution = false)
            where TCalamariProgram : CalamariFlavourProgram
        {
            context.Variables.Add(KnownVariables.Action.EnabledFeatures, AwsSpecialVariables.Action.Aws.CloudFormation.Changesets.Feature);
            context.Variables.Add(AwsSpecialVariables.Action.Aws.CloudFormation.Changesets.Generate, generateName.ToString());
            context.Variables.Add(AwsSpecialVariables.Action.Aws.CloudFormation.Changesets.Defer, deferExecution.ToString());
        }


    }
}