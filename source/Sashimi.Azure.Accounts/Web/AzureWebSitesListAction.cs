using System;
using System.Collections.Generic;
using System.Linq;
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
    public class AzureWebSitesListAction : AzureWebSiteActionBase, IAccountDetailsEndpoint
    {
        static readonly BadRequestRegistration UnsupportedType = new BadRequestRegistration("Unsupported account type");
        static readonly BadRequestRegistration ManagementCertsUnsupportedType = new BadRequestRegistration("Azure Management Certificate accounts not supported");
        static readonly Extensibility.Extensions.Infrastructure.Web.Api.OctopusJsonRegistration<ICollection<AzureWebSiteResource>> Results = new Extensibility.Extensions.Infrastructure.Web.Api.OctopusJsonRegistration<ICollection<AzureWebSiteResource>>();

        readonly IOctopusHttpClientFactory httpClientFactory;
        public AzureWebSitesListAction(ILog log, IOctopusHttpClientFactory httpClientFactory)
            : base(log)
        {
            this.httpClientFactory = httpClientFactory;
        }

        public string Method => "GET";
        public string Route => "websites";
        public string Description => "Lists the websites associated with an Azure account.";

        public async Task<IOctoResponseProvider> Respond(IOctoRequest request, string accountName, AccountDetails accountDetails)
        {
            if (accountDetails.AccountType == AccountTypes.AzureSubscriptionAccountType)
                return ManagementCertsUnsupportedType.Response();

            if (accountDetails.AccountType != AccountTypes.AzureServicePrincipalAccountType)
                return UnsupportedType.Response();

            var sites = (await GetSites(accountName, (AzureServicePrincipalAccountDetails) accountDetails))
                .OrderBy(x => x.Name).ThenBy(x => x.Region).ToArray();
            return Results.Response(sites);
        }

        Task<List<AzureWebSiteResource>> GetSites(string accountName, AzureServicePrincipalAccountDetails accountDetails)
        {
            return ThrowIfNotSuccess(async () =>
            {
                using (var webSiteClient = accountDetails.CreateWebSiteManagementClient(httpClientFactory.HttpClientHandler))
                {
                    return await webSiteClient.WebApps.ListWithHttpMessagesAsync().ConfigureAwait(false);
                }
            }, response => response.Body
                .Select(site => AzureWebSiteResource.ForResourceManagement(site.Name, site.ResourceGroup, site.Location))
                .ToList(), $"Failed to retrieve list of WebApps for '{accountName}' service principal.");
        }
    }
}