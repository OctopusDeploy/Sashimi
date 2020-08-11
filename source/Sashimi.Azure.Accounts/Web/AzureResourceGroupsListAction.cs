using System;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using Microsoft.Azure.Management.ResourceManager;
using Microsoft.Azure.Management.Storage;
using Microsoft.Azure.Management.WebSites;
using Microsoft.IdentityModel.Clients.ActiveDirectory;
using Microsoft.Rest;
using Octopus.Core.Resources;
using Octopus.Diagnostics;
using Octopus.Extensibility.Actions.Sashimi;
using Octopus.Server.Extensibility.Extensions.Infrastructure.Web.Api;
using Sashimi.Azure.Accounts;
using Sashimi.Server.Contracts.Accounts;
using BadRequestRegistration = Octopus.Server.Extensibility.Extensions.Infrastructure.Web.Api.BadRequestRegistration;
using IHttpClientFactory = Microsoft.IdentityModel.Clients.ActiveDirectory.IHttpClientFactory;

namespace Octopus.Server.Web.Api.Actions
{
    public static class AzureServicePrincipalAccountExtensions
    {
        static ServiceClientCredentials Credentials(this AzureServicePrincipalAccountDetails account, HttpMessageHandler handler)
        {
            return new TokenCredentials(GetAuthorizationToken(account, handler));
        }

        public static ResourceManagementClient CreateResourceManagementClient(this AzureServicePrincipalAccountDetails account, HttpClientHandler httpClientHandler)
        {
            return string.IsNullOrWhiteSpace(account.ResourceManagementEndpointBaseUri) ?
                new ResourceManagementClient(account.Credentials(httpClientHandler), httpClientHandler) { SubscriptionId = account.SubscriptionNumber } :
                new ResourceManagementClient(new Uri(account.ResourceManagementEndpointBaseUri), account.Credentials(httpClientHandler), httpClientHandler) { SubscriptionId = account.SubscriptionNumber };
        }

        public static StorageManagementClient CreateStorageManagementClient(this AzureServicePrincipalAccountDetails account, HttpClientHandler httpClientHandler)
        {
            return string.IsNullOrWhiteSpace(account.ResourceManagementEndpointBaseUri) ?
                new StorageManagementClient(account.Credentials(httpClientHandler), httpClientHandler) { SubscriptionId = account.SubscriptionNumber } :
                new StorageManagementClient(new Uri(account.ResourceManagementEndpointBaseUri), account.Credentials(httpClientHandler), httpClientHandler) { SubscriptionId = account.SubscriptionNumber };
        }

        public static WebSiteManagementClient CreateWebSiteManagementClient(this AzureServicePrincipalAccountDetails account, HttpClientHandler httpClientHandler)
        {
            return string.IsNullOrWhiteSpace(account.ResourceManagementEndpointBaseUri) ?
                new WebSiteManagementClient(account.Credentials(httpClientHandler), httpClientHandler) { SubscriptionId = account.SubscriptionNumber } :
                new WebSiteManagementClient(new Uri(account.ResourceManagementEndpointBaseUri), account.Credentials(httpClientHandler), httpClientHandler) { SubscriptionId = account.SubscriptionNumber };
        }

        static string GetAuthorizationToken(AzureServicePrincipalAccountDetails account, HttpMessageHandler handler)
        {
            var adDirectory = "https://login.windows.net/";
            if (!string.IsNullOrWhiteSpace(account.ActiveDirectoryEndpointBaseUri))
            {
                adDirectory = account.ActiveDirectoryEndpointBaseUri;
            }
            var context = new AuthenticationContext(adDirectory + account.TenantId, true, TokenCache.DefaultShared, new HttpClientFactory(handler));

            var resourceManagementEndpointBaseUri = "https://management.core.windows.net/";
            if (!string.IsNullOrWhiteSpace(account.ResourceManagementEndpointBaseUri))
            {
                resourceManagementEndpointBaseUri = account.ResourceManagementEndpointBaseUri;
            }
            var result = context.AcquireTokenAsync(resourceManagementEndpointBaseUri, new ClientCredential(account.ClientId, account.Password?.Value)).GetAwaiter().GetResult();
            return result.AccessToken;
        }

        // We decided to "copy" the impl from https://github.com/AzureAD/azure-activedirectory-library-for-dotnet/blob/af10dd15cdb082bc3dbe14b0c2c6d81f6ca5b541/src/Microsoft.IdentityModel.Clients.ActiveDirectory/Core/Http/HttpClientFactory.cs
        // This was we are setting the same headers as the Azure impl.
        class HttpClientFactory : IHttpClientFactory
        {
            readonly HttpClient client;
            const long MaxResponseContentBufferSizeInBytes = 1048576;

            public HttpClientFactory(HttpMessageHandler handler)
            {
                client = new HttpClient(handler)
                {
                    MaxResponseContentBufferSize = MaxResponseContentBufferSizeInBytes
                };
                client.DefaultRequestHeaders.Accept.Clear();
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            }

            public HttpClient GetHttpClient()
            {
                return client;
            }
        }
    }

    public class AzureResourceGroupsListAction : AzureWebSiteActionBase, IAccountDetailsEndpoint
    {
        static readonly BadRequestRegistration UnsupportedType = new BadRequestRegistration("Unsupported account type");
        static readonly Extensibility.Extensions.Infrastructure.Web.Api.OctopusJsonRegistration<AzureResourceGroupResource[]> Results = new Extensibility.Extensions.Infrastructure.Web.Api.OctopusJsonRegistration<AzureResourceGroupResource[]>();

        readonly IOctopusHttpClientFactory httpClientFactory;

        public AzureResourceGroupsListAction(ILog log, IOctopusHttpClientFactory httpClientFactory) : base(log)
        {
            this.httpClientFactory = httpClientFactory;
        }

        public string Method => "GET";
        public string Route => "resourceGroups";
        public string Description => "Lists the Resource Groups associated with an Azure account.";

        public async Task<IOctoResponseProvider> Respond(IOctoRequest request, string accountName, AccountDetails accountDetails)
        {
            var servicePrincipalAccount = accountDetails as AzureServicePrincipalAccountDetails;
            if (servicePrincipalAccount == null)
            {
                return UnsupportedType.Response();
            }

            var resourceGroups = await RetrieveResourceGroups(accountName, servicePrincipalAccount);
            return Results.Response(resourceGroups);
        }

        Task<AzureResourceGroupResource[]> RetrieveResourceGroups(string accountName, AzureServicePrincipalAccountDetails accountDetails)
        {

            return ThrowIfNotSuccess(async () =>
            {
                using (var armClient = accountDetails.CreateResourceManagementClient(httpClientFactory.HttpClientHandler))
                {
                    return await armClient.ResourceGroups.ListWithHttpMessagesAsync().ConfigureAwait(false);
                }
            }, response =>
            {
                return response.Body
                    .Select(x => new AzureResourceGroupResource {Id = x.Id, Name = x.Name}).ToArray();
            }, $"Failed to retrieve list of Resource Groups for '{accountName}' service principal.");
        }
    }
}