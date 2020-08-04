using System.Collections.Generic;

namespace Sashimi.Server.Contracts.Endpoints
{
    public interface IEndpointWithCertificates
    {
        IReadOnlyCollection<string> CertificateIds { get; }
    }
}