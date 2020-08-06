using System;
using System.Collections.Generic;
using Calamari.AzureServiceFabric.Deployment.Conventions;
using Calamari.Common.Commands;
using Calamari.Common.Plumbing.Pipeline;

namespace Calamari.AzureServiceFabric.Commands
{
    [Command("deploy-azure-service-fabric-app", Description = "Extracts and installs an Azure Service Fabric Application")]
    public class DeployAzureServiceFabricAppCommand : PipelineCommand
    {
        protected override IEnumerable<IDeployBehaviour> Deploy(DeployResolver resolver)
        {
            yield return resolver.Create<SubstituteVariablesInAzureServiceFabricPackageBehaviour>();
            yield return resolver.Create<EnsureCertificateInstalledInStoreBehaviour>();
            yield return resolver.Create<DeployAzureServiceFabricAppBehaviour>();
        }
    }
}
