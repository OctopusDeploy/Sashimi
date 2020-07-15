using System;
using System.Collections.Generic;
using FluentAssertions;
using NSubstitute;
using NUnit.Framework;
using Octopus.Diagnostics;
using Octostache;
using Sashimi.AzureCloudService.Endpoints;
using Sashimi.Server.Contracts.Endpoints;
using Sashimi.Server.Contracts.ServiceMessages;
using AzureCloudServiceServiceMessageNames =
    Sashimi.AzureCloudService.AzureCloudServiceServiceMessageHandler.AzureCloudServiceServiceMessageNames;
using AzureCloudServiceEndpointDeploymentSlot =
    Sashimi.AzureCloudService.AzureCloudServiceServiceMessageHandler.AzureCloudServiceEndpointDeploymentSlot;

namespace Sashimi.AzureCloudService.Tests
{
    [TestFixture]
    public class AzureCloudServiceServiceMessageHandlerFixture
    {
        ICreateTargetServiceMessageHandler serviceMessageHandler;

        [SetUp]
        public void SetUp()
        {
            serviceMessageHandler = new AzureCloudServiceServiceMessageHandler(Substitute.For<ILog>());
        }

        [Test]
        public void Ctor_Properties_ShouldBeInitializedProperly()
        {
            serviceMessageHandler.AuditEntryDescription.Should().Be("Azure Cloud Service Target");
            serviceMessageHandler.ServiceMessageName.Should().Be(AzureCloudServiceServiceMessageNames.CreateTargetName);
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
        //                case AzureCloudServiceServiceMessageNames.AccountIdOrNameAttribute:
        //                    result.Messages.Should().ContainSingle(m => m == "Account Id or Name is missing");
        //                    break;
        //                case AzureCloudServiceServiceMessageNames.AzureCloudServiceNameAttribute:
        //                    result.Messages.Should().ContainSingle(m => m == "Azure Cloud Service Name is missing");
        //                    break;
        //                case AzureCloudServiceServiceMessageNames.AzureStorageAccountAttribute:
        //                    result.Messages.Should().ContainSingle(m => m == "Azure Storage Account is missing");
        //                    break;
        //            }
        //        }
        //    }
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
            Action action = () => serviceMessageHandler.BuildEndpoint(null, new VariableDictionary(), _ => "", null);

            action.Should().Throw<ArgumentNullException>();
        }

        [Test]
        public void BuildEndpoint_WhenVariableDictionaryIsNull_ShouldThrowArgumentNullException()
        {
            Action action = () => serviceMessageHandler.BuildEndpoint(new Dictionary<string, string>(), null, _ => "", null);

            action.Should().Throw<ArgumentNullException>();
        }

        [Test]
        public void BuildEndpoint_WhenAccountIdResolverIsNull_ShouldThrowArgumentNullException()
        {
            Action action = () => serviceMessageHandler.BuildEndpoint(new Dictionary<string, string>(), new VariableDictionary(), null, null);

            action.Should().Throw<ArgumentNullException>();
        }

        [Test]
        public void BuildEndpoint_WhenAbleToResolveAccountIdUsingAccountIdOrNameAttribute_ShouldNotTryToResolveUsingAccountIdInVariables()
        {
            var messageProperties = GetMessageProperties();
            var variableDict = GetVariableDictionaryForBuildingEndpoint();

            const string accountId = "Accounts-1";
            string ResolveAccountId(string key)
            {
                if (key == messageProperties[AzureCloudServiceServiceMessageNames.AccountIdOrNameAttribute]) 
                    return accountId;

                return null;
            }

            var endpoint = serviceMessageHandler.BuildEndpoint(messageProperties, variableDict, ResolveAccountId,
                null);

            AssertAzureCloudServiceEndpoint(endpoint, new ExpectedEndpointValues
            {
                AccountId = accountId,
                CloudServiceName = messageProperties[AzureCloudServiceServiceMessageNames.AzureCloudServiceNameAttribute],
                StorageAccountName = messageProperties[AzureCloudServiceServiceMessageNames.AzureStorageAccountAttribute],
                SwapIfPossible = false,
                Slot = AzureCloudServiceEndpointDeploymentSlot.Production,
                UseCurrentInstanceCount = false
            });
        }

        [Test]
        [Ignore("Will address later")]
        public void BuildEndpoint_WhenUnableToResolveAccountIdUsingAccountIdOrNameAttribute_ShouldTryToResolveUsingAccountIdInVariables()
        {
            var messageProperties = GetMessageProperties();
            var variableDict = GetVariableDictionaryForBuildingEndpoint();

            const string accountId = "Accounts-3";
            string ResolveAccountId(string key)
            {
                if (key == messageProperties[AzureCloudServiceServiceMessageNames.AccountIdOrNameAttribute])
                    return null;
                if (key == variableDict[SpecialVariables.Action.Azure.AccountId])
                    return accountId;

                return null;
            }

            var endpoint = serviceMessageHandler.BuildEndpoint(messageProperties, variableDict, ResolveAccountId,
                null);

            AssertAzureCloudServiceEndpoint(endpoint, new ExpectedEndpointValues
            {
                AccountId = accountId,
                CloudServiceName = messageProperties[AzureCloudServiceServiceMessageNames.AzureCloudServiceNameAttribute],
                StorageAccountName = messageProperties[AzureCloudServiceServiceMessageNames.AzureStorageAccountAttribute],
                SwapIfPossible = false,
                Slot = AzureCloudServiceEndpointDeploymentSlot.Production,
                UseCurrentInstanceCount = false
            });
        }

        [Test]
        [TestCase(null)]
        [TestCase("")]
        [TestCase("    ")]
        [TestCase("DoNotDeploy")]
        public void BuildEndpoint_WhenSwapPropertyIsNullOrWhiteSpaceOrNotDeploy_ShouldReturnEndpointWithSwapPropertyAsTrue(
                string swapValue)
        {
            var messageProperties = GetMessageProperties();
            messageProperties[AzureCloudServiceServiceMessageNames.SwapAttribute] = swapValue;
            const string accountId = "Accounts-2";

            var endpoint = serviceMessageHandler.BuildEndpoint(messageProperties,
                GetVariableDictionaryForBuildingEndpoint(), _ => accountId,
                null);

            AssertAzureCloudServiceEndpoint(endpoint, new ExpectedEndpointValues
            {
                AccountId = accountId,
                CloudServiceName = messageProperties[AzureCloudServiceServiceMessageNames.AzureCloudServiceNameAttribute],
                StorageAccountName = messageProperties[AzureCloudServiceServiceMessageNames.AzureStorageAccountAttribute],
                SwapIfPossible = true,
                Slot = AzureCloudServiceEndpointDeploymentSlot.Production,
                UseCurrentInstanceCount = false
            });
        }

        [Test]
        [TestCase("deploy")]
        [TestCase("depLoY")]
        public void BuildEndpoint_WhenSwapPropertyIsDeploy_ShouldReturnEndpointWithSwapPropertyAsFalse(string swapValue)
        {
            var messageProperties = GetMessageProperties();
            messageProperties[AzureCloudServiceServiceMessageNames.SwapAttribute] = swapValue;
            const string accountId = "Accounts-2";

            var endpoint = serviceMessageHandler.BuildEndpoint(messageProperties,
                GetVariableDictionaryForBuildingEndpoint(), _ => accountId,
                null);

            AssertAzureCloudServiceEndpoint(endpoint, new ExpectedEndpointValues
            {
                AccountId = accountId,
                CloudServiceName = messageProperties[AzureCloudServiceServiceMessageNames.AzureCloudServiceNameAttribute],
                StorageAccountName = messageProperties[AzureCloudServiceServiceMessageNames.AzureStorageAccountAttribute],
                SwapIfPossible = false,
                Slot = AzureCloudServiceEndpointDeploymentSlot.Production,
                UseCurrentInstanceCount = false
            });
        }

        [Test]
        [TestCase(null)]
        [TestCase("")]
        [TestCase("stage")]
        public void BuildEndpoint_WhenSlotPropertyIsNullOrEmptyOrNotProduction_ShouldReturnEndpointWithSlotPropertyAsStaging(
                string slotValue)
        {
            var messageProperties = GetMessageProperties();
            messageProperties[AzureCloudServiceServiceMessageNames.AzureDeploymentSlotAttribute] = slotValue;
            const string accountId = "Accounts-2";

            var endpoint = serviceMessageHandler.BuildEndpoint(messageProperties,
                GetVariableDictionaryForBuildingEndpoint(), _ => accountId,
                null);

            AssertAzureCloudServiceEndpoint(endpoint, new ExpectedEndpointValues
            {
                AccountId = accountId,
                CloudServiceName = messageProperties[AzureCloudServiceServiceMessageNames.AzureCloudServiceNameAttribute],
                StorageAccountName = messageProperties[AzureCloudServiceServiceMessageNames.AzureStorageAccountAttribute],
                SwapIfPossible = false,
                Slot = AzureCloudServiceEndpointDeploymentSlot.Staging,
                UseCurrentInstanceCount = false
            });
        }

        [Test]
        [TestCase("production")]
        [TestCase("ProduCtion")]
        public void BuildEndpoint_WhenSlotPropertyIsProduction_ShouldReturnEndpointWithSlotPropertyAsProduction(
            string slotValue)
        {
            var messageProperties = GetMessageProperties();
            messageProperties[AzureCloudServiceServiceMessageNames.AzureDeploymentSlotAttribute] = slotValue;
            const string accountId = "Accounts-2";

            var endpoint = serviceMessageHandler.BuildEndpoint(messageProperties,
                GetVariableDictionaryForBuildingEndpoint(), _ => accountId,
                null);

            AssertAzureCloudServiceEndpoint(endpoint, new ExpectedEndpointValues
            {
                AccountId = accountId,
                CloudServiceName = messageProperties[AzureCloudServiceServiceMessageNames.AzureCloudServiceNameAttribute],
                StorageAccountName = messageProperties[AzureCloudServiceServiceMessageNames.AzureStorageAccountAttribute],
                SwapIfPossible = false,
                Slot = AzureCloudServiceEndpointDeploymentSlot.Production,
                UseCurrentInstanceCount = false
            });
        }

        [Test]
        [TestCase(null)]
        [TestCase("")]
        [TestCase("NotConfiguration")]
        public void BuildEndpoint_WhenInstanceCountPropertyIsNullOrEmptyOrNotConfiguration_ShouldReturnEndpointWithUseCurrentInstanceCountPropertyAsTrue(
                string instanceCountValue)
        {
            var messageProperties = GetMessageProperties();
            messageProperties[AzureCloudServiceServiceMessageNames.InstanceCountAttribute] = instanceCountValue;
            const string accountId = "Accounts-2";

            var endpoint = serviceMessageHandler.BuildEndpoint(messageProperties,
                GetVariableDictionaryForBuildingEndpoint(), _ => accountId,
                null);

            AssertAzureCloudServiceEndpoint(endpoint, new ExpectedEndpointValues
            {
                AccountId = accountId,
                CloudServiceName = messageProperties[AzureCloudServiceServiceMessageNames.AzureCloudServiceNameAttribute],
                StorageAccountName = messageProperties[AzureCloudServiceServiceMessageNames.AzureStorageAccountAttribute],
                SwapIfPossible = false,
                Slot = AzureCloudServiceEndpointDeploymentSlot.Production,
                UseCurrentInstanceCount = true
            });
        }

        [Test]
        [TestCase("configuration")]
        [TestCase("ConfigurAtion")]
        public void BuildEndpoint_WhenInstanceCountPropertyIsConfiguration_ShouldReturnEndpointWithUseCurrentInstanceCountPropertyAsFalse(
                string instanceCountValue)
        {
            var messageProperties = GetMessageProperties();
            messageProperties[AzureCloudServiceServiceMessageNames.InstanceCountAttribute] = instanceCountValue;
            const string accountId = "Accounts-2";

            var endpoint = serviceMessageHandler.BuildEndpoint(messageProperties,
                GetVariableDictionaryForBuildingEndpoint(), _ => accountId,
                null);

            AssertAzureCloudServiceEndpoint(endpoint, new ExpectedEndpointValues
            {
                AccountId = accountId,
                CloudServiceName = messageProperties[AzureCloudServiceServiceMessageNames.AzureCloudServiceNameAttribute],
                StorageAccountName = messageProperties[AzureCloudServiceServiceMessageNames.AzureStorageAccountAttribute],
                SwapIfPossible = false,
                Slot = AzureCloudServiceEndpointDeploymentSlot.Production,
                UseCurrentInstanceCount = false
            });
        }

        static void AssertAzureCloudServiceEndpoint(Endpoint actualEndpoint,
            ExpectedEndpointValues expectedEndpointValues)
        {
            actualEndpoint.Should().BeOfType<AzureCloudServiceEndpoint>();
            var cloudServiceEndpoint = (AzureCloudServiceEndpoint) actualEndpoint;
            cloudServiceEndpoint.AccountId.Should().Be(expectedEndpointValues.AccountId);
            cloudServiceEndpoint.CloudServiceName.Should().Be(expectedEndpointValues.CloudServiceName);
            cloudServiceEndpoint.StorageAccountName.Should().Be(expectedEndpointValues.StorageAccountName);
            cloudServiceEndpoint.Slot.Should().Be(expectedEndpointValues.Slot);
            cloudServiceEndpoint.SwapIfPossible.Should().Be(expectedEndpointValues.SwapIfPossible);
            cloudServiceEndpoint.UseCurrentInstanceCount.Should()
                .Be(expectedEndpointValues.UseCurrentInstanceCount);
        }

        static IEnumerable<IDictionary<string, string>> AllPossibleInvalidServiceMessagePropertyCombination()
        {
            yield return new Dictionary<string, string>
            {
                {AzureCloudServiceServiceMessageNames.AccountIdOrNameAttribute, null},
                {AzureCloudServiceServiceMessageNames.AzureCloudServiceNameAttribute, null},
                {AzureCloudServiceServiceMessageNames.AzureStorageAccountAttribute, null}
            };

            yield return new Dictionary<string, string>
            {
                {AzureCloudServiceServiceMessageNames.AccountIdOrNameAttribute, "Accounts-1"},
                {AzureCloudServiceServiceMessageNames.AzureCloudServiceNameAttribute, null},
                {AzureCloudServiceServiceMessageNames.AzureStorageAccountAttribute, null}
            };

            yield return new Dictionary<string, string>
            {
                {AzureCloudServiceServiceMessageNames.AccountIdOrNameAttribute, "Accounts-1"},
                {AzureCloudServiceServiceMessageNames.AzureCloudServiceNameAttribute, "CloudService"},
                {AzureCloudServiceServiceMessageNames.AzureStorageAccountAttribute, null}
            };

            yield return new Dictionary<string, string>
            {
                {AzureCloudServiceServiceMessageNames.AccountIdOrNameAttribute, "Accounts-1"},
                {AzureCloudServiceServiceMessageNames.AzureCloudServiceNameAttribute, null},
                {AzureCloudServiceServiceMessageNames.AzureStorageAccountAttribute, "Storage"}
            };

            yield return new Dictionary<string, string>
            {
                {AzureCloudServiceServiceMessageNames.AccountIdOrNameAttribute, null},
                {AzureCloudServiceServiceMessageNames.AzureCloudServiceNameAttribute, "CloudService"},
                {AzureCloudServiceServiceMessageNames.AzureStorageAccountAttribute, null}
            };

            yield return new Dictionary<string, string>
            {
                {AzureCloudServiceServiceMessageNames.AccountIdOrNameAttribute, null},
                {AzureCloudServiceServiceMessageNames.AzureCloudServiceNameAttribute, "CloudService"},
                {AzureCloudServiceServiceMessageNames.AzureStorageAccountAttribute, "Storage"}
            };

            yield return new Dictionary<string, string>
            {
                {AzureCloudServiceServiceMessageNames.AccountIdOrNameAttribute, null},
                {AzureCloudServiceServiceMessageNames.AzureCloudServiceNameAttribute, null},
                {AzureCloudServiceServiceMessageNames.AzureStorageAccountAttribute, "Storage"}
            };
        }

        static IDictionary<string, string> GetMessageProperties()
        {
            return new Dictionary<string, string>
            {
                {AzureCloudServiceServiceMessageNames.AccountIdOrNameAttribute, "Accounts-1"},
                {AzureCloudServiceServiceMessageNames.AzureCloudServiceNameAttribute, "CloudService"},
                {AzureCloudServiceServiceMessageNames.AzureStorageAccountAttribute, "AzureStorage"},
                {AzureCloudServiceServiceMessageNames.AzureDeploymentSlotAttribute, "production"},
                {AzureCloudServiceServiceMessageNames.InstanceCountAttribute, "configuration"},
                {AzureCloudServiceServiceMessageNames.SwapAttribute, "deploy"},
            };
        }

        static VariableDictionary GetVariableDictionaryForBuildingEndpoint()
        {
            return new VariableDictionary {{SpecialVariables.Action.Azure.AccountId, "Accounts-2"}};
        }

        class ExpectedEndpointValues
        {
            public string AccountId { get; set; }
            public string CloudServiceName { get; set; }
            public string StorageAccountName { get; set; }
            public string Slot { get; set; }
            public bool SwapIfPossible { get; set; }
            public bool UseCurrentInstanceCount { get; set; }
        }
    }
}