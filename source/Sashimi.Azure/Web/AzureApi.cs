using System;
using System.Net.Http;
using Octopus.Server.Extensibility.Extensions.Infrastructure.Web.Api;

namespace Sashimi.Azure.Web
{
    class AzureApi : RegistersEndpoints
    {
        public const string AzureEnvironmentsPath = "/api/accounts/azureenvironments";

        public AzureApi()
        {
            Add<SecuredAsyncActionInvoker<AzureEnvironmentsListAction>>(HttpMethod.Get.ToString(), AzureEnvironmentsPath);
        }
    }
}
