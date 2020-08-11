// CS8618 Non-nullable field {0} is uninitialized
// Remove this when this class is converted to initialize all required properties via the constructor
#pragma warning disable 8618

namespace Octopus.Core.Resources
{
    public class AzureResourceGroupResource
    {
        public string Id { get; set; }
        public string Name { get; set; }
    }
}