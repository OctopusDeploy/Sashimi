using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Calamari.Tests.Shared;
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
            var clientFactory = Substitute.For<IOctopusHttpClientFactory>();
            clientFactory.HttpClientHandler.Returns(httpMessageHandler);
            
            var verifier = new AzureServicePrincipalAccountVerifier(new Lazy<IOctopusHttpClientFactory>(clientFactory));

            Assert.DoesNotThrow(() => verifier.Verify(accountDetails));
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
            accountDetails.Password = "InvalidPassword".ToSensitiveString();
            
            var clientFactory = Substitute.For<IOctopusHttpClientFactory>();
            clientFactory.HttpClientHandler.Returns(httpMessageHandler);
            
            var verifier = new AzureServicePrincipalAccountVerifier(new Lazy<IOctopusHttpClientFactory>(clientFactory));
            
            Assert.That(() => verifier.Verify(accountDetails), Throws.TypeOf<Microsoft.IdentityModel.Clients.ActiveDirectory.AdalServiceException>());
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
            Assert.That(() => verifier.Verify(accountDetails), Throws.TypeOf<Microsoft.IdentityModel.Clients.ActiveDirectory.AdalServiceException>());
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