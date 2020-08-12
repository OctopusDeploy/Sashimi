using Autofac;
using Octopus.Server.Extensibility.Extensions.Mappings;
using Sashimi.Server.Contracts.Accounts;
using Sashimi.Server.Contracts.Variables;

namespace Sashimi.Aws.Accounts
{
    public class AmazonWebServicesAccountModule : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            builder.RegisterType<AmazonWebServicesAccountTypeProvider>()
                   .As<IAccountTypeProvider>()
                   .As<IContributeMappings>()
                   .As<IContributeWellKnownVariables>()
                   .SingleInstance();
        }
    }
}