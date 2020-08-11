﻿using System;
using System.Collections.Generic;
using FluentAssertions;
using FluentValidation.TestHelper;
using NUnit.Framework;
using Octopus.Server.Extensibility.HostServices.Model;
using Sashimi.Server.Contracts.ActionHandlers.Validation;

namespace Sashimi.AzureCloudService.Tests
{
    public class AzureCloudServiceDeploymentActionValidatorFixture
    {
        AzureCloudServiceActionHandlerValidator validator;

        [SetUp]
        public void Setup()
        {
            validator = new AzureCloudServiceActionHandlerValidator();
        }

        [Test]
        public void Validate_NoAccountNotLegacyMode_NoErrors()
        {
            var context = CreateBareAction(new Dictionary<string, string>());
            var result = validator.TestValidate(context);

            result.Errors.Should().NotContain(f => f.PropertyName == SpecialVariables.Action.Azure.AccountId);
        }

        [Test]
        public void Validate_HasAccountLegacyMode_NoErrors()
        {
            var context = CreateBareAction(new Dictionary<string, string>
            {
                { SpecialVariables.Action.Azure.IsLegacyMode, bool.TrueString },
                { SpecialVariables.Action.Azure.AccountId, "Accounts-1" }
            });
            var result = validator.TestValidate(context);

            result.Errors.Should().NotContain(f => f.PropertyName == SpecialVariables.Action.Azure.AccountId);
        }

        [Test]
        public void Validate_NoAccountLegacyMode_Error()
        {
            var context = CreateBareAction(new Dictionary<string, string>
            {
                { SpecialVariables.Action.Azure.IsLegacyMode, bool.TrueString }
            });
            var result = validator.TestValidate(context);

            result.Errors.Should().Contain(f => f.PropertyName == SpecialVariables.Action.Azure.AccountId);
        }

        static DeploymentActionValidationContext CreateBareAction(Dictionary<string, string> properties)
        {
            return new DeploymentActionValidationContext(SpecialVariables.Action.Azure.CloudServiceActionTypeName,
                                                         properties,
                                                         new List<PackageReference>());
        }
    }
}