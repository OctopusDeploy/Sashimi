using Autofac;
using Sashimi.Server.Contracts.Endpoint;

namespace Sashimi.AzureWebApp
{
    public class AzureWebAppModule : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            builder.RegisterType<AzureWebAppDeploymentTargetTypeProvider>().As<IDeploymentTargetTypeProvider>().SingleInstance();
        }
    }
}