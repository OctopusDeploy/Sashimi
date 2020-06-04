using Autofac;
using Sashimi.AzureCloudService.Endpoints;
using Sashimi.Server.Contracts.Endpoints;

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