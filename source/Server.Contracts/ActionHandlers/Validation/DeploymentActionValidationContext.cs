using System.Collections.Generic;
using Octopus.Server.Extensibility.HostServices.Model;

namespace Sashimi.Server.Contracts.ActionHandlers.Validation
{
    public class DeploymentActionValidationContext
    {
        public DeploymentActionValidationContext(IReadOnlyDictionary<string, string> properties, IReadOnlyCollection<PackageReference> packages)
        {
            Properties = properties;
            Packages = packages;
        }
        
        public IReadOnlyDictionary<string, string> Properties { get; }
        public IReadOnlyCollection<PackageReference> Packages { get; }
    }
}