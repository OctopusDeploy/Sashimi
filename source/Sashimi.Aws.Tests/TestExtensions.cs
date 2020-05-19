using Calamari;
using Calamari.Tests.Shared;
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
                context.Variables.Add("Octopus.Action.Amazon.AccessKey", ExternalVariables.Get(ExternalVariable.AwsAcessKey));
                context.Variables.Add("Octopus.Action.Amazon.SecretKey", ExternalVariables.Get(ExternalVariable.AwsSecretKey));    
            });

            return builder;
        }
    }
}