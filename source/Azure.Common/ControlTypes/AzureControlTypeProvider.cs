using Sashimi.Azure.Common.Variables;
using Sashimi.Server.Contracts.Actions.Templates;
using Sashimi.Server.Contracts.Variables;

namespace Sashimi.Azure.Common.ControlTypes
{
    class AzureControlTypeProvider : IControlTypeProvider
    {
        public ControlType ControlType => AzureControlType.AzureAccount;
        public VariableType VariableType => AzureVariableType.AzureAccount;
    }
}