using Autofac;
using Sashimi.Server.Contracts.Endpoint;

namespace Sashimi.AzureCloudService
{
    public class AzureCloudServiceModule : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            builder.RegisterType<AzureCloudServiceDeploymentTargetTypeProvider>().As<IDeploymentTargetTypeProvider>().SingleInstance();
        }
    }
}