﻿using System.Collections.Generic;
using FluentAssertions;
using NUnit.Framework;
using Sashimi.Server.Contracts.ServiceMessages;
using CreateAwsAccountServiceMessagePropertyNames = Sashimi.Aws.Accounts.AmazonWebServicesAccountServiceMessageHandler.CreateAwsAccountServiceMessagePropertyNames;

namespace Sashimi.Aws.Accounts.Tests
{
    [TestFixture]
    public class AmazonWebServicesAccountServiceMessageHandlerFixture
    {
        ICreateAccountDetailsServiceMessageHandler serviceMessageHandler;

        [OneTimeSetUp]
        public void SetUp()
        {
            serviceMessageHandler = new AmazonWebServicesAccountServiceMessageHandler();
        }

        [Test]
        public void Ctor_Properties_ShouldBeInitializedCorrectly()
        {
            serviceMessageHandler.AuditEntryDescription.Should().Be("AWS Account");
            serviceMessageHandler.ServiceMessageName.Should().Be(CreateAwsAccountServiceMessagePropertyNames.Name);
        }

        [Test]
        public void CreateAccountDetails_ShouldCreateDetailsCorrectly()
        {
            var properties = GetMessageProperties();

            var details = serviceMessageHandler.CreateAccountDetails(properties);

            details.Should().BeOfType<AmazonWebServicesAccountDetails>();
            var amazonWebServicesAccountDetails = (AmazonWebServicesAccountDetails)details;
            amazonWebServicesAccountDetails.AccessKey.Should().Be(properties[CreateAwsAccountServiceMessagePropertyNames.AccessKey]);
            amazonWebServicesAccountDetails.SecretKey.Should().Be(properties[CreateAwsAccountServiceMessagePropertyNames.SecretKey]);
        }

        static IDictionary<string, string> GetMessageProperties()
        {
            return new Dictionary<string, string>
            {
                { CreateAwsAccountServiceMessagePropertyNames.AccessKey, "AccessKey" },
                { CreateAwsAccountServiceMessagePropertyNames.SecretKey, "SecretKey" }
            };
        }
    }
}