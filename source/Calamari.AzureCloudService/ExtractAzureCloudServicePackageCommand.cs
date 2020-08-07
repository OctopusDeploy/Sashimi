using System;
using System.Collections.Generic;
using Calamari.Common.Commands;
using Calamari.Common.Features.Behaviours;
using Calamari.Common.Plumbing.Pipeline;

namespace Calamari.AzureCloudService
{
    [Command("extract-cspkg", Description = "Extracts an Azure cloud-service package (.cspkg)")]
    public class ExtractAzureCloudServicePackageCommand : PipelineCommand
    {
        protected override IEnumerable<IBeforePackageExtractionBehaviour> BeforePackageExtraction(BeforePackageExtractionResolver resolver)
        {
            yield return resolver.Create<EnsureCloudServicePackageIsCtpFormatBehaviour>();
        }
        
        protected override IEnumerable<IDeployBehaviour> Deploy(DeployResolver resolver)
        {
            yield break;
        }
    }
}