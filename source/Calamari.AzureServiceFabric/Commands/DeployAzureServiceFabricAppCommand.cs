using System;
using System.Collections.Generic;
using Calamari.AzureServiceFabric.Deployment.Conventions;
using Calamari.Common.Commands;
using Calamari.Common.Features.Behaviours;
using Calamari.Common.Plumbing.Pipeline;

namespace Calamari.AzureServiceFabric.Commands
{
    [Command("deploy-azure-service-fabric-app", Description = "Extracts and installs an Azure Service Fabric Application")]
    public class DeployAzureServiceFabricAppCommand : PipelineCommand
    {
        protected override IEnumerable<IBeforePackageExtractionBehaviour> BeforePackageExtraction(BeforePackageExtractionResolver resolver)
        {
            yield return resolver.Create<CheckSdkInstalledBehaviour>();
        }

        protected override IEnumerable<IDeployBehaviour> Deploy(DeployResolver resolver)
        {
            yield return resolver.Create<SubstituteVariablesInAzureServiceFabricPackageConvention>();
            yield return resolver.Create<EnsureCertificateInstalledInStoreConvention>();
            yield return resolver.Create<DeployAzureServiceFabricAppConvention>();
        }
    }
}
