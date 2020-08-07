using Autofac;
using Octopus.Server.Extensibility.Extensions.Infrastructure.Configuration;
using Octopus.Server.Extensibility.Extensions.Mappings;
using Sashimi.AzureWebApp.Endpoints;
using Sashimi.Server.Contracts.ActionHandlers;
using Sashimi.Server.Contracts.ActionHandlers.Validation;
using Sashimi.Server.Contracts.Endpoints;

namespace Sashimi.AzureWebApp
{
       public class AzureWebAppModule : Module
       {
              protected override void Load(ContainerBuilder builder)
              {
                     builder.RegisterType<AzureWebAppDeploymentTargetTypeProvider>()
                            .As<IDeploymentTargetTypeProvider>()
                            .As<IContributeMappings>()
                            .SingleInstance();
                     builder.RegisterType<AzureWebAppHealthCheckActionHandler>()
                            .As<IActionHandler>()
                            .AsSelf()
                            .InstancePerLifetimeScope();
                     builder.RegisterType<AzureWebAppActionHandler>()
                            .As<IActionHandler>()
                            .AsSelf()
                            .InstancePerLifetimeScope();
                     builder.RegisterType<AzureWebAppPackageActionOverride>()
                            .As<IPackageActionOverride>()
                            .InstancePerLifetimeScope();
              }
       }
}