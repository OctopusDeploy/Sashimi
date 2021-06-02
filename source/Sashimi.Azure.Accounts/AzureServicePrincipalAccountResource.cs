#nullable disable
using System.ComponentModel.DataAnnotations;
using Octopus.Server.MessageContracts;
using Octopus.Server.MessageContracts.Attributes;
using Octopus.Server.MessageContracts.Features.Accounts;

namespace Sashimi.Azure.Accounts
{
    class AzureServicePrincipalAccountResource : AccountResource
    {
        public override AccountType AccountType => AccountType.AzureServicePrincipal;

        [Trim]
        [Writeable]
        [Required(ErrorMessage = "Please provide an Azure subscription ID.")]
        public string SubscriptionNumber { get; set; }

        [Trim]
        [Writeable]
        [NotDocumentReference]
        public string ClientId { get; set; }

        [Trim]
        [Writeable]
        [NotDocumentReference]
        public string TenantId { get; set; }

        [Trim]
        [Writeable]
        public SensitiveValue Password { get; set; }

        [Trim]
        [Writeable]
        public string AzureEnvironment { get; set; }

        [Trim]
        [Writeable]
        public string ResourceManagementEndpointBaseUri { get; set; }

        [Trim]
        [Writeable]
        public string ActiveDirectoryEndpointBaseUri { get; set; }
    }
}