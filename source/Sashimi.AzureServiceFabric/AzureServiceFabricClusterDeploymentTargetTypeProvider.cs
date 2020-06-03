using System;
using Sashimi.Server.Contracts;
using Sashimi.Server.Contracts.Endpoint;

namespace Sashimi.AzureServiceFabric
{
    class AzureServiceFabricClusterDeploymentTargetTypeProvider : IDeploymentTargetTypeProvider
    {
        public DeploymentTargetType DeploymentTargetType => AzureServiceFabricClusterEndpoint.AzureServiceFabricClusterDeploymentTargetType;
        public Type DomainType => typeof(AzureServiceFabricClusterEndpoint);

        public Type ApiType => typeof(ServiceFabricEndpointResource);
    }
}