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
        public static ActionHandlerTestBuilder<TCalamariProgram> WithAwsAccount<TCalamariProgram>(this ActionHandlerTestBuilder<TCalamariProgram> builder) 
            where TCalamariProgram : CalamariFlavourProgram
        {
            builder.WithArrange(context => 
            {
                context.Variables.Add(AwsSpecialVariables.Account.AccessKey, ExternalVariables.Get(ExternalVariable.AwsAcessKey));
                context.Variables.Add(AwsSpecialVariables.Account.SecretKey, ExternalVariables.Get(ExternalVariable.AwsSecretKey));    
            });

            return builder;
        }
        
        public static ActionHandlerTestBuilder<TCalamariProgram> WithStackRole<TCalamariProgram>(this ActionHandlerTestBuilder<TCalamariProgram> builder, string stackRole) 
            where TCalamariProgram : CalamariFlavourProgram
        {
            builder.WithArrange(context => context.Variables.Add(AwsSpecialVariables.Action.Aws.CloudFormation.RoleArn, stackRole));
            return builder;
        }
        
        public static ActionHandlerTestBuilder<TCalamariProgram> WithIamCapabilities<TCalamariProgram>(this ActionHandlerTestBuilder<TCalamariProgram> builder, IEnumerable<string> capabilities) 
            where TCalamariProgram : CalamariFlavourProgram
        {
            builder.WithArrange(context => context.Variables.Add(AwsSpecialVariables.Action.Aws.IamCapabilities, JsonConvert.SerializeObject(capabilities)));
            return builder;
        }
        
        public static ActionHandlerTestBuilder<TCalamariProgram> WithCloudFormationChangeSets<TCalamariProgram>(this ActionHandlerTestBuilder<TCalamariProgram> builder, bool generateName = true, bool deferExecution = false) 
            where TCalamariProgram : CalamariFlavourProgram
        {
            builder.WithArrange(context =>
            {
                context.Variables.Add(KnownVariables.Action.EnabledFeatures, AwsSpecialVariables.Action.Aws.CloudFormation.Changesets.Feature);
                context.Variables.Add(AwsSpecialVariables.Action.Aws.CloudFormation.Changesets.Generate, generateName.ToString());
                context.Variables.Add(AwsSpecialVariables.Action.Aws.CloudFormation.Changesets.Defer, deferExecution.ToString());
            });
            return builder;
        }
        
        
    }
}