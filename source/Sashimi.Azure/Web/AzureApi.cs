using System;
using System.Net.Http;
using Octopus.Server.Extensibility.Extensions.Infrastructure.Web.Api;

namespace Sashimi.Azure.Web
{
    class AzureApi : RegistersEndpoints
    {
        public const string AzureEnvironmentsPath = "/accounts/azureenvironments";

        public AzureApi()
        {
            Add<AnonymousAsyncActionInvoker<AzureEnvironmentsListAction>>(HttpMethod.Get.ToString(), AzureEnvironmentsPath);
        }
    }
}
