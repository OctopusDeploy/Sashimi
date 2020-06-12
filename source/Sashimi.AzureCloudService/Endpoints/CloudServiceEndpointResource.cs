using System.ComponentModel.DataAnnotations;
using Octopus.Data.Resources.Attributes;
using Sashimi.Server.Contracts.Endpoints;

#nullable disable
namespace Sashimi.AzureCloudService.Endpoints
{
    public class CloudServiceEndpointResource : EndpointResource
    {
        public override CommunicationStyle CommunicationStyle => CommunicationStyle.AzureCloudService;

        [Trim]
        [Writeable]
        [Required(ErrorMessage = "Please specify an account.")]
        public string AccountId { get; set; }

        [Trim]
        [Writeable]
        [Required(ErrorMessage = "Please specify the cloud service name.")]
        public string CloudServiceName { get; set; }

        [Trim]
        [Writeable]
        [Required(ErrorMessage = "Please specify a storage acccount.")]
        public string StorageAccountName { get; set; }

        [Trim]
        [Writeable]
        public string Slot { get; set; }

        [Writeable]
        public bool SwapIfPossible { get; set; }

        [Writeable]
        public bool UseCurrentInstanceCount { get; set; }

        [Trim]
        [Writeable]
        public string DefaultWorkerPoolId { get; set; }
    }
}