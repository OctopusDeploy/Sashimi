﻿using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Calamari.Tests.Shared;
using FluentAssertions;
using NSubstitute;
using NUnit.Framework;
using Octopus.Data.Model;
using Octopus.Server.Extensibility.Extensions.Infrastructure.Web.Api;

namespace Sashimi.Aws.Accounts.Tests
{
    [TestFixture]
    public class AmazonWebServicesAccountVerifierFixture
    {
        [Test]
        public void Verify_ShouldUseTheHttpClientProvidedFromTheOctopusHttpClientFactory()
        {
            var httpMessageHandler = new TestHttpClientHandler();
            var awsHttpClientFactory = new AwsHttpClientFactory(new Lazy<IOctopusHttpClientFactory>(
                                                                                                    () => GetOctopusHttpClientFactory(httpMessageHandler)));
            var verifier = new AmazonWebServicesAccountVerifier(awsHttpClientFactory);

            verifier.Verify(new AmazonWebServicesAccountDetails
                            {
                                AccessKey = ExternalVariables.Get(ExternalVariable.AwsAcessKey),
                                SecretKey = ExternalVariables.Get(ExternalVariable.AwsSecretKey).ToSensitiveString()
                            },
                            CancellationToken.None);

            httpMessageHandler.RequestLog.Should().ContainSingle(r => r.RequestUri.AbsoluteUri == "https://sts.amazonaws.com/");
        }

        static IOctopusHttpClientFactory GetOctopusHttpClientFactory(HttpMessageHandler httpMessageHandler)
        {
            var httpClientFactory = Substitute.For<IOctopusHttpClientFactory>();
            httpClientFactory.CreateClient().Returns(new HttpClient(httpMessageHandler));

            return httpClientFactory;
        }

        class TestHttpClientHandler : HttpClientHandler
        {
            public TestHttpClientHandler()
            {
                RequestLog = new List<HttpRequestMessage>();
            }

            public IList<HttpRequestMessage> RequestLog { get; }

            protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
            {
                RequestLog.Add(request);

                return base.SendAsync(request, cancellationToken);
            }
        }
    }
}