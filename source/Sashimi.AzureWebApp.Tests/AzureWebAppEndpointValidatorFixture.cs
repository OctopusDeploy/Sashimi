using FluentAssertions;
using FluentValidation;
using NUnit.Framework;
using Sashimi.AzureWebApp.Endpoints;
using Sashimi.Tests.Shared.Extensions;

namespace Sashimi.AzureWebApp.Tests
{
    [Parallelizable(ParallelScope.All)]
    public class AzureWebAppEndpointValidatorFixture
    {
        [Test]
        public void Validate_FieldsNotPopulated_Error()
        {
            var sut = new AzureWebAppEndpointValidator();
            var errors = sut.ValidateAndGetErrors(new AzureWebAppEndpoint());

            errors.Should().Contain("'Account' must not be empty.");
            errors.Should().Contain("'Web App' must not be empty.");
            errors.Should().Contain("'Resource Group' must not be empty.");
        }

        [Test]
        public void Validate_FieldsPopulated_NoError()
        {
            var sut = new AzureWebAppEndpointValidator();
            var endpoint = new AzureWebAppEndpoint
            {
                AccountId = "blah", 
                WebAppName = "the webapp", 
                ResourceGroupName = "the group"
            };


            var errors = sut.ValidateAndGetErrors(endpoint);

            errors.Should().BeEmpty();
        }
    }
}