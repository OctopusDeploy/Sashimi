using System;
using System.Collections.Generic;
using System.Linq;
using FluentAssertions;
using NSubstitute;
using NUnit.Framework;
using Octopus.Diagnostics;
using Octostache;
using Sashimi.AzureWebApp.Endpoints;
using Sashimi.Server.Contracts.Endpoints;
using Sashimi.Server.Contracts.ServiceMessages;
using AzureWebAppServiceMessageNames = Sashimi.AzureWebApp.AzureWebAppServiceMessageHandler.AzureWebAppServiceMessageNames;

namespace Sashimi.AzureWebApp.Tests
{
    public class AzureWebAppServiceMessageHandlerFixture
    {
        ICreateTargetServiceMessageHandler serviceMessageHandler;
        ILog logger;

        [SetUp]
        public void SetUp()
        {
            logger = Substitute.For<ILog>();
            serviceMessageHandler = new AzureWebAppServiceMessageHandler(logger);
        }

        [Test]
        public void Ctor_Properties_ShouldBeInitializedProperly()
        {
            serviceMessageHandler.AuditEntryDescription.Should().Be("Azure Web App Target");
            serviceMessageHandler.ServiceMessageName.Should().Be(AzureWebAppServiceMessageNames.CreateTargetName);
        }

        //[Test]
        //public void IsServiceMessageValid_WhenMessagePropertiesIsNull_ShouldThrowArgumentNullException()
        //{
        //    Action action = () => serviceMessageHandler.IsServiceMessageValid(null, new VariableDictionary());

        //    action.Should().Throw<ArgumentNullException>();
        //}

        //[Test]
        //public void IsServiceMessageValid_WhenVariableDictionaryIsNull_ShouldThrowArgumentNullException()
        //{
        //    Action action = () => serviceMessageHandler.IsServiceMessageValid(new Dictionary<string, string>(), null);

        //    action.Should().Throw<ArgumentNullException>();
        //}

        //[Test]
        //[TestCaseSource(nameof(AllPossibleInvalidServiceMessagePropertyCombination))]
        //public void IsServiceMessageValid_WhenAnyMessagePropertyIsMissing_ShouldReturnInvalidResult(
        //    IDictionary<string, string> messageProperties)
        //{
        //    var result = serviceMessageHandler.IsServiceMessageValid(messageProperties, new VariableDictionary());

        //    result.IsValid.Should().BeFalse();
        //    result.Messages.Length.Should().Be(messageProperties.Values.Count(v => v == null));
        //    foreach (var keyValuePair in messageProperties)
        //    {
        //        if (keyValuePair.Value == null)
        //        {
        //            switch (keyValuePair.Key)
        //            {
        //                case AzureWebAppServiceMessageNames.WebAppNameAttribute:
        //                    result.Messages.Should().ContainSingle(m => m == "Web App Name is missing");
        //                    break;
        //                case AzureWebAppServiceMessageNames.ResourceGroupNameAttribute:
        //                    result.Messages.Should().ContainSingle(m => m == "Resource Group Name is missing");
        //                    break;
        //                case AzureWebAppServiceMessageNames.AccountIdOrNameAttribute:
        //                    result.Messages.Should().ContainSingle(m => m == "Account Id or Name is missing");
        //                    break;
        //            }
        //        }
        //    }
        //}

        //[Test]
        //[TestCase("")]
        //[TestCase("    ")]
        //[TestCase(null)]
        //public void IsServiceMessageValid_WhenAccountIdOrNameAttributeIsMissingAndVariableAccountIdIsNullOrWhiteSpace_ShouldReturnInvalidResult(
        //    string variableAccountIdValue)
        //{
        //    var variableDictionary = new VariableDictionary
        //    {
        //        {SpecialVariables.Action.Azure.AccountId, variableAccountIdValue}
        //    };
        //    var messageProperties = GetMessageProperties();
        //    messageProperties[AzureWebAppServiceMessageNames.AccountIdOrNameAttribute] = null;

        //    var result = serviceMessageHandler.IsServiceMessageValid(messageProperties, variableDictionary);

        //    result.IsValid.Should().BeFalse();
        //    result.Messages.Length.Should().Be(messageProperties.Values.Count(v => v == null));
        //    result.Messages.Should().ContainSingle(m => m == "Account Id or Name is missing");
        //}

        //[Test]
        //public void IsServiceMessageValid_WhenAccountIdOrNameAttributeIsMissingButVariableAccountIdIsValid_ShouldReturnValidResult()
        //{
        //    var variableDictionary = new VariableDictionary
        //    {
        //        {SpecialVariables.Action.Azure.AccountId, "Accounts-23"}
        //    };
        //    var messageProperties = GetMessageProperties();
        //    messageProperties[AzureWebAppServiceMessageNames.AccountIdOrNameAttribute] = null;

        //    var result = serviceMessageHandler.IsServiceMessageValid(messageProperties, variableDictionary);

        //    result.IsValid.Should().BeTrue();
        //    result.Messages.Should().BeEmpty();
        //}

        //[Test]
        //public void IsServiceMessageValid_WhenAllPropertiesAreSpecified_ShouldReturnValidResult()
        //{
        //    var result = serviceMessageHandler.IsServiceMessageValid(GetMessageProperties(), new VariableDictionary());

        //    result.IsValid.Should().BeTrue();
        //    result.Messages.Should().BeEmpty();
        //}

        [Test]
        public void BuildEndpoint_WhenMessagePropertiesIsNull_ShouldThrowArgumentNullException()
        {
            Action action = () => serviceMessageHandler.BuildEndpoint(null, new VariableDictionary(), _ => "", null, null);

            action.Should().Throw<ArgumentNullException>();
        }

        [Test]
        public void BuildEndpoint_WhenVariableDictionaryIsNull_ShouldThrowArgumentNullException()
        {
            Action action = () => serviceMessageHandler.BuildEndpoint(new Dictionary<string, string>(), null, _ => "", null, null);

            action.Should().Throw<ArgumentNullException>();
        }

        [Test]
        public void BuildEndpoint_WhenAccountIdResolverIsNull_ShouldThrowArgumentNullException()
        {
            Action action = () => serviceMessageHandler.BuildEndpoint(new Dictionary<string, string>(), new VariableDictionary(), null, null, null);

            action.Should().Throw<ArgumentNullException>();
        }

        [Test]
        public void BuildEndpoint_WhenUnableToGetAccountId_ShouldThrowException()
        {
            var messageProperties = GetMessageProperties();

            Action action = () => serviceMessageHandler.BuildEndpoint(messageProperties, new VariableDictionary(), _ => null, null, null);

            var expectedErrorMessage = $"Account with Id / Name, {messageProperties[AzureWebAppServiceMessageNames.AccountIdOrNameAttribute]}, not found.";
            action.Should().Throw<Exception>().Which.Message.Should().Be(expectedErrorMessage);
            logger.Received(1).Error(Arg.Is(expectedErrorMessage));
        }

        [Test]
        public void BuildEndpoint_WhenAbleToResolveAccountIdUsingAccountIdOrNameAttribute_ShouldNotTryToResolveUsingAccountIdInVariables()
        {
            var messageProperties = GetMessageProperties();
            var variableDict = GetVariableDictionaryForBuildingEndpoint();

            const string accountId = "Accounts-1";
            string ResolveAccountId(string key)
            {
                if (key == messageProperties[AzureWebAppServiceMessageNames.AccountIdOrNameAttribute])
                    return accountId;

                return null;
            }

            var endpoint = serviceMessageHandler.BuildEndpoint(messageProperties, variableDict, ResolveAccountId,
                null, null);

            AssertAzureWebAppEndpoint(endpoint, new ExpectedEndpointValues
            {
                AccountId = accountId,
                WebAppName = messageProperties[AzureWebAppServiceMessageNames.WebAppNameAttribute],
                ResourceGroupName = messageProperties[AzureWebAppServiceMessageNames.ResourceGroupNameAttribute],
                WebAppSlotName = messageProperties[AzureWebAppServiceMessageNames.WebAppSlotNameAttribute]
            });
        }

        [Test]
        [TestCase("")]
        [TestCase(null)]
        public void BuildEndpoint_WhenAccountIdIsNullOrEmptyInMessageProperties_ShouldTryToResolveUsingAccountIdInVariables(
            string accountIdOrNameInMessageProperties)
        {
            var messageProperties = GetMessageProperties();
            messageProperties[AzureWebAppServiceMessageNames.AccountIdOrNameAttribute] = accountIdOrNameInMessageProperties;
            var variableDict = GetVariableDictionaryForBuildingEndpoint();

            const string accountId = "Accounts-12";
            string ResolveAccountId(string key)
            {
                if (key == variableDict[SpecialVariables.Action.Azure.AccountId])
                    return accountId;

                return null;
            }

            var endpoint = serviceMessageHandler.BuildEndpoint(messageProperties, variableDict, ResolveAccountId,
                null, null);

            AssertAzureWebAppEndpoint(endpoint, new ExpectedEndpointValues
            {
                AccountId = accountId,
                WebAppName = messageProperties[AzureWebAppServiceMessageNames.WebAppNameAttribute],
                ResourceGroupName = messageProperties[AzureWebAppServiceMessageNames.ResourceGroupNameAttribute],
                WebAppSlotName = messageProperties[AzureWebAppServiceMessageNames.WebAppSlotNameAttribute]
            });
        }

        [Test]
        public void BuildEndpoint_WhenWebAppSlotNameAttributeIsMissing_ShouldReturnEndpointWithoutWebAppSlotName()
        {
            var messageProperties = GetMessageProperties();
            messageProperties.Remove(AzureWebAppServiceMessageNames.WebAppSlotNameAttribute);
            var variableDict = GetVariableDictionaryForBuildingEndpoint();

            const string accountId = "Accounts-12";
            var endpoint = serviceMessageHandler.BuildEndpoint(messageProperties, variableDict, _ => accountId,
                null, null);

            AssertAzureWebAppEndpoint(endpoint, new ExpectedEndpointValues
            {
                AccountId = accountId,
                WebAppName = messageProperties[AzureWebAppServiceMessageNames.WebAppNameAttribute],
                ResourceGroupName = messageProperties[AzureWebAppServiceMessageNames.ResourceGroupNameAttribute],
                WebAppSlotName = string.Empty
            });
        }

        [Test]
        public void BuildEndpoint_WhenWebAppSlotNameAttributeIsNotMissing_ShouldReturnEndpointWithCorrectWebAppSlotName()
        {
            var messageProperties = GetMessageProperties();
            var variableDict = GetVariableDictionaryForBuildingEndpoint();

            const string accountId = "Accounts-12";
            var endpoint = serviceMessageHandler.BuildEndpoint(messageProperties, variableDict, _ => accountId,
                null, null);

            AssertAzureWebAppEndpoint(endpoint, new ExpectedEndpointValues
            {
                AccountId = accountId,
                WebAppName = messageProperties[AzureWebAppServiceMessageNames.WebAppNameAttribute],
                ResourceGroupName = messageProperties[AzureWebAppServiceMessageNames.ResourceGroupNameAttribute],
                WebAppSlotName = messageProperties[AzureWebAppServiceMessageNames.WebAppSlotNameAttribute]
            });
        }

        static IEnumerable<IDictionary<string, string>> AllPossibleInvalidServiceMessagePropertyCombination()
        {
            yield return new Dictionary<string, string>
            {
                {AzureWebAppServiceMessageNames.WebAppNameAttribute, null},
                {AzureWebAppServiceMessageNames.ResourceGroupNameAttribute, null},
                {AzureWebAppServiceMessageNames.AccountIdOrNameAttribute, null}
            };

            yield return new Dictionary<string, string>
            {
                {AzureWebAppServiceMessageNames.WebAppNameAttribute, "WebApp"},
                {AzureWebAppServiceMessageNames.ResourceGroupNameAttribute, null},
                {AzureWebAppServiceMessageNames.AccountIdOrNameAttribute, null}
            };

            yield return new Dictionary<string, string>
            {
                {AzureWebAppServiceMessageNames.WebAppNameAttribute, "WebApp"},
                {AzureWebAppServiceMessageNames.ResourceGroupNameAttribute, "ResourceGroup"},
                {AzureWebAppServiceMessageNames.AccountIdOrNameAttribute, null}
            };

            yield return new Dictionary<string, string>
            {
                {AzureWebAppServiceMessageNames.WebAppNameAttribute, "WebApp"},
                {AzureWebAppServiceMessageNames.ResourceGroupNameAttribute, null},
                {AzureWebAppServiceMessageNames.AccountIdOrNameAttribute, "AccountId"}
            };

            yield return new Dictionary<string, string>
            {
                {AzureWebAppServiceMessageNames.WebAppNameAttribute, null},
                {AzureWebAppServiceMessageNames.ResourceGroupNameAttribute, "ResourceGroup"},
                {AzureWebAppServiceMessageNames.AccountIdOrNameAttribute, null}
            };

            yield return new Dictionary<string, string>
            {
                {AzureWebAppServiceMessageNames.WebAppNameAttribute, null},
                {AzureWebAppServiceMessageNames.ResourceGroupNameAttribute, "ResourceGroup"},
                {AzureWebAppServiceMessageNames.AccountIdOrNameAttribute, "AccountId"}
            };

            yield return new Dictionary<string, string>
            {
                {AzureWebAppServiceMessageNames.WebAppNameAttribute, null},
                {AzureWebAppServiceMessageNames.ResourceGroupNameAttribute, null},
                {AzureWebAppServiceMessageNames.AccountIdOrNameAttribute, "AccountId"}
            };
        }

        static void AssertAzureWebAppEndpoint(Endpoint actualEndpoint, ExpectedEndpointValues expectedEndpointValues)
        {
            actualEndpoint.Should().BeOfType<AzureWebAppEndpoint>();
            var cloudServiceEndpoint = (AzureWebAppEndpoint)actualEndpoint;
            cloudServiceEndpoint.AccountId.Should().Be(expectedEndpointValues.AccountId);
            cloudServiceEndpoint.ResourceGroupName.Should().Be(expectedEndpointValues.ResourceGroupName);
            cloudServiceEndpoint.WebAppName.Should().Be(expectedEndpointValues.WebAppName);
            cloudServiceEndpoint.WebAppSlotName.Should().Be(expectedEndpointValues.WebAppSlotName);
        }

        static IDictionary<string, string> GetMessageProperties()
        {
            return new Dictionary<string, string>
            {
                {AzureWebAppServiceMessageNames.AccountIdOrNameAttribute, "Accounts-1"},
                {AzureWebAppServiceMessageNames.WebAppNameAttribute, "CloudService"},
                {AzureWebAppServiceMessageNames.ResourceGroupNameAttribute, "AzureStorage"},
                {AzureWebAppServiceMessageNames.WebAppSlotNameAttribute, "production"},
            };
        }

        static VariableDictionary GetVariableDictionaryForBuildingEndpoint()
        {
            return new VariableDictionary { { SpecialVariables.Action.Azure.AccountId, "Accounts-2" } };
        }

        class ExpectedEndpointValues
        {
            public string AccountId { get; set; }
            public string WebAppName { get; set; }
            public string ResourceGroupName { get; set; }
            public string WebAppSlotName { get; set; }
        }
    }
}