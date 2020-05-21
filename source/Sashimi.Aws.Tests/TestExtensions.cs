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

        public static void WithCloudFormationTemplate<TCalamariProgram>(this TestActionHandlerContext<TCalamariProgram> context, string template, string templateParametersRaw)
            where TCalamariProgram : CalamariFlavourProgram
        {
            context.Variables.Add(AwsSpecialVariables.Action.Aws.CloudFormation.Template, template);
            context.Variables.Add(AwsSpecialVariables.Action.Aws.CloudFormation.TemplateParametersRaw, templateParametersRaw);
        }

        public static void WithAwsRegion<TCalamariProgram>(this TestActionHandlerContext<TCalamariProgram> context, string region)
            where TCalamariProgram : CalamariFlavourProgram
        {
            context.Variables.Add("Octopus.Action.Aws.Region", region);
        }

        public static void WithAwsTemplatePackageSource<TCalamariProgram>(this TestActionHandlerContext<TCalamariProgram> context)
            where TCalamariProgram : CalamariFlavourProgram
        {
            context.Variables.Add(AwsSpecialVariables.Action.Aws.TemplateSource, AwsSpecialVariables.Action.Aws.TemplateSourceOptions.Package);
        }
        
        public static void WithAwsTemplateInlineSource<TCalamariProgram>(this TestActionHandlerContext<TCalamariProgram> context)
            where TCalamariProgram : CalamariFlavourProgram
        {
            context.Variables.Add(AwsSpecialVariables.Action.Aws.TemplateSource, AwsSpecialVariables.Action.Aws.TemplateSourceOptions.Inline);
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