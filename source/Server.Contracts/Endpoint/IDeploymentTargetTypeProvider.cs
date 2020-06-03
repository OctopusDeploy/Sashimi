using System;

namespace Sashimi.Server.Contracts.Endpoint
{
    public interface IDeploymentTargetTypeProvider
    {
        DeploymentTargetType DeploymentTargetType { get; }
        Type DomainType { get; }
        Type ApiType { get; }
    }
}