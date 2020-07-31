using System.Collections.Generic;

namespace Sashimi.Server.Contracts.Endpoints
{
    public class IEndpointWithServerCertificates
    {
        IEnumerable<string> ServerCertificateIds { get; }
    }
}