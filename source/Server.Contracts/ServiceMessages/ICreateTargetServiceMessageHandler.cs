using System;
using System.Collections.Generic;
using Octostache;
using Sashimi.Server.Contracts.Endpoints;

namespace Sashimi.Server.Contracts.ServiceMessages
{
    public interface ICreateTargetServiceMessageHandler : IServiceMessageHandler
    {
        Endpoint BuildEndpoint(IDictionary<string, string> messageProperties, VariableDictionary variables, Func<string, string> accountIdResolver, Func<string, string> certificateIdResolver);
    }
}