#nullable disable
using Octopus.Server.MessageContracts;
using Octopus.Server.MessageContracts.Attributes;
using Octopus.Server.MessageContracts.Features.Accounts;

namespace Sashimi.Aws.Accounts
{
    class AmazonWebServicesAccountResource : AccountResource
    {
        public override AccountType AccountType => AccountType.AmazonWebServicesAccount;

        [Trim]
        [Writeable]
        public string AccessKey { get; set; }

        [Trim, Writeable]
        public SensitiveValue SecretKey { get; set; }
    }
}