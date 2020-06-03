using System;
using Sashimi.Server.Contracts;
using Sashimi.Server.Contracts.Endpoint;

namespace Sashimi.AzureWebApp
{
    class AzureWebAppDeploymentTargetTypeProvider : IDeploymentTargetTypeProvider
    {
        public DeploymentTargetType DeploymentTargetType => AzureWebAppEndpoint.AzureWebAppDeploymentTargetType;
        public Type DomainType => typeof(AzureWebAppEndpoint);

        public Type ApiType => typeof(AzureWebAppEndpointResource);
    }
}