using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Calamari.Tests.Shared;
using Microsoft.IdentityModel.Clients.ActiveDirectory;
using NSubstitute;
using NUnit.Framework;
using Octopus.Data.Model;
using Octopus.Server.Extensibility.Extensions.Infrastructure.Web.Api;

namespace Sashimi.Azure.Accounts.Tests
{
    [TestFixture]
    public class AzureServicePrincipalAccountVerifierFixture
    {
        [Test]
        public void Verify_ShouldSuccessWithValidCredential()
        {
            var httpMessageHandler = new TestHttpClientHandler();
            var accountDetails = new AzureServicePrincipalAccountDetails
            {
                SubscriptionNumber = ExternalVariables.Get(ExternalVariable.AzureSubscriptionId),
                ClientId = ExternalVariables.Get(ExternalVariable.AzureSubscriptionClientId),
                TenantId = ExternalVariables.Get(ExternalVariable.AzureSubscriptionTenantId),
                Password = ExternalVariables.Get(ExternalVariable.AzureSubscriptionPassword).ToSensitiveString()
            };

            Assert.DoesNotThrow(() => accountDetails.CreateResourceManagementClient(httpMessageHandler));
        }

        [Test]
        public void Verify_ShouldFailWithWrongCredential()
        {
            var httpMessageHandler = new TestHttpClientHandler();
            var accountDetails = new AzureServicePrincipalAccountDetails
            {
                SubscriptionNumber = ExternalVariables.Get(ExternalVariable.AzureSubscriptionId),
                ClientId = ExternalVariables.Get(ExternalVariable.AzureSubscriptionClientId),
                TenantId = ExternalVariables.Get(ExternalVariable.AzureSubscriptionTenantId),
                Password = ExternalVariables.Get(ExternalVariable.AzureSubscriptionPassword).ToSensitiveString()
            };
            accountDetails.Password = "invalid password".ToSensitiveString();
            Assert.That(() => accountDetails.CreateResourceManagementClient(httpMessageHandler), Throws.TypeOf<Microsoft.IdentityModel.Clients.ActiveDirectory.AdalServiceException>());
        }

        [Test]
        public void Verify_ShouldNotRetrieveTokenFromCache()
        {
            var httpMessageHandler = new TestHttpClientHandler();
            var accountDetails = new AzureServicePrincipalAccountDetails
            {
                SubscriptionNumber = ExternalVariables.Get(ExternalVariable.AzureSubscriptionId),
                ClientId = ExternalVariables.Get(ExternalVariable.AzureSubscriptionClientId),
                TenantId = ExternalVariables.Get(ExternalVariable.AzureSubscriptionTenantId),
                Password = ExternalVariables.Get(ExternalVariable.AzureSubscriptionPassword).ToSensitiveString()
            };

            // Verify with a valid token once to fill the cache
            using var cleanClient = accountDetails.CreateResourceManagementClient(httpMessageHandler);
            cleanClient.ResourceGroups.ListWithHttpMessagesAsync().GetAwaiter().GetResult();

            accountDetails.Password = "invalid password".ToSensitiveString();
            accountDetails.InvalidateTokenCache(httpMessageHandler);
            Assert.That(() => accountDetails.CreateResourceManagementClient(httpMessageHandler), Throws.TypeOf<Microsoft.IdentityModel.Clients.ActiveDirectory.AdalServiceException>());
        }

        [Test]
        public void Verify_ShouldNotCacheClientCredentials()
        {
            var httpMessageHandler = new TestHttpClientHandler();
            var accountDetails = new AzureServicePrincipalAccountDetails
            {
                SubscriptionNumber = ExternalVariables.Get(ExternalVariable.AzureSubscriptionId),
                ClientId = ExternalVariables.Get(ExternalVariable.AzureSubscriptionClientId),
                TenantId = ExternalVariables.Get(ExternalVariable.AzureSubscriptionTenantId),
                Password = ExternalVariables.Get(ExternalVariable.AzureSubscriptionPassword).ToSensitiveString()
            };

            var clientFactory = Substitute.For<IOctopusHttpClientFactory>();
            clientFactory.HttpClientHandler.Returns(httpMessageHandler);

            var verifier = new AzureServicePrincipalAccountVerifier(new Lazy<IOctopusHttpClientFactory>(clientFactory));
            verifier.Verify(accountDetails);

            accountDetails.Password = "InvalidPassword".ToSensitiveString();
            verifier.Verify(accountDetails);
        }
    }

    class TestHttpClientHandler : HttpClientHandler
    {
        public TestHttpClientHandler()
        {
            RequestLog = new List<HttpRequestMessage>();
        }

        IList<HttpRequestMessage> RequestLog { get; }

        protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            RequestLog.Add(request);

            return base.SendAsync(request, cancellationToken);
        }
    }
}