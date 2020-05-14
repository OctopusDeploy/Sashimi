using Autofac;
using Sashimi.Aws.CloudTemplates;
using Sashimi.Server.Contracts.ActionHandlers;
using Sashimi.Server.Contracts.CloudTemplates;

namespace Sashimi.Aws
{
    /// <summary>
    /// This module is expected to register handlers that can parse parameters to return the module for
    /// DynamicForm, as well as any instances of IDeploymentActionDefinition.
    /// </summary>
    public class AwsModule : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            base.Load(builder);

            builder.RegisterType<CloudFormationJsonCloudTemplateHandler>().As<ICloudTemplateHandler>().SingleInstance();
            builder.RegisterType<CloudFormationYamlCloudTemplateHandler>().As<ICloudTemplateHandler>().SingleInstance();

            builder.RegisterAssemblyTypes(ThisAssembly)
                .AssignableTo<IActionHandler>()
                .As<IActionHandler>()
                .AsSelf()
                .InstancePerLifetimeScope();
        }
    }
}
