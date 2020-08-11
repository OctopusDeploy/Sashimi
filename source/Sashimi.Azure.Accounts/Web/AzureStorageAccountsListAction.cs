using System.Collections.Generic;
using System.Linq;
using Octopus.Core.Resources;
using System.Threading.Tasks;
using Octopus.Diagnostics;
using Octopus.Extensibility.Actions.Sashimi;
using Octopus.Server.Extensibility.Extensions.Infrastructure.Web.Api;
using Sashimi.Azure.Accounts;
using Sashimi.Server.Contracts.Accounts;
using AccountTypes = Sashimi.Azure.Accounts.AccountTypes;
using BadRequestRegistration = Octopus.Server.Extensibility.Extensions.Infrastructure.Web.Api.BadRequestRegistration;

namespace Octopus.Server.Web.Api.Actions
{
    public class AzureStorageAccountsListAction : AzureWebSiteActionBase, IAccountDetailsEndpoint
    {
        static readonly BadRequestRegistration UnsupportedType = new BadRequestRegistration("Account must be an Azure account.");
        static readonly BadRequestRegistration ManagementCertsUnsupportedType = new BadRequestRegistration("This operation is not supported using Azure Management Certificate authentication. Please try again using an Azure Service Account.");
        static readonly Extensibility.Extensions.Infrastructure.Web.Api.OctopusJsonRegistration<ICollection<AzureStorageAccountResource>> Results = new Extensibility.Extensions.Infrastructure.Web.Api.OctopusJsonRegistration<ICollection<AzureStorageAccountResource>>();

        readonly IOctopusHttpClientFactory httpClientFactory;

        public AzureStorageAccountsListAction(ILog log, IOctopusHttpClientFactory httpClientFactory) : base(log)
        {
            this.httpClientFactory = httpClientFactory;
        }

        public string Method => "GET";
        public string Route => "storageAccounts";
        public string Description => "Lists the storage accounts associated with an Azure account.";

        public async Task<IOctoResponseProvider> Respond(IOctoRequest request, string accountName, AccountDetails accountDetails)
        {
            if (accountDetails.AccountType == AccountTypes.AzureServicePrincipalAccountType)
            {
                var storageAccounts = await GetStorageAccountsAsync(accountName, (AzureServicePrincipalAccountDetails) accountDetails);
                return Results.Response(storageAccounts);
            }

            if (accountDetails.AccountType == AccountTypes.AzureSubscriptionAccountType)
                return ManagementCertsUnsupportedType.Response();

            return UnsupportedType.Response();
        }

        Task<AzureStorageAccountResource[]> GetStorageAccountsAsync(string accountName, AzureServicePrincipalAccountDetails accountDetails)
        {
            return ThrowIfNotSuccess(async () =>
                {
                    using (var azureClient = accountDetails.CreateStorageManagementClient(httpClientFactory.HttpClientHandler))
                    {
                        return await azureClient.StorageAccounts.ListWithHttpMessagesAsync();
                    }
                }, response => response.Body
                    .Select(service => new AzureStorageAccountResource {Name = service.Name, Location = service.Location}).ToArray(),
                $"Failed to retrieve list of StorageAccounts for '{accountName}' service principal.");
        }
    }
}