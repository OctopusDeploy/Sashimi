using System.Collections.Generic;

namespace Sashimi.Server.Contracts.Variables
{
    public interface IContributeWellKnownVariables
    {
        IEnumerable<WellKnownVariable> GetWellKnownVariables();
    }
}