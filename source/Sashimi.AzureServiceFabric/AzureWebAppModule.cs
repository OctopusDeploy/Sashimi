using Autofac;
using Sashimi.Server.Contracts.Endpoint;

namespace Sashimi.AzureServiceFabric
{
    public class AzureServiceFabricModule : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            builder.RegisterType<AzureServiceFabricClusterDeploymentTargetTypeProvider>().As<IDeploymentTargetTypeProvider>().SingleInstance();
        }
    }
}