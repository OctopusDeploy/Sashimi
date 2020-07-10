using System;
using System.Collections.Generic;
using System.Linq;
using FluentValidation;
using Octopus.Data.Model;
using Octopus.Server.Extensibility.HostServices.Mapping;
using Sashimi.Server.Contracts.Accounts;
using Sashimi.Server.Contracts.ServiceMessages;

namespace Sashimi.Aws.Accounts
{
    class AmazonWebServicesAccountTypeProvider : IAccountTypeProvider
    {
        public AccountDetails CreateViaServiceMessage(IDictionary<string, string> properties)
        {
            properties.TryGetValue(CreateAwsAccountServiceMessagePropertyNames.AccessKey, out var accessKey);
            properties.TryGetValue(CreateAwsAccountServiceMessagePropertyNames.SecretKey, out var secretKey);

            return new AmazonWebServicesAccountDetails
            {
                AccessKey = accessKey,
                SecretKey = secretKey.ToSensitiveString()
            };
        }

        public AccountType AccountType { get; } = AccountTypes.AmazonWebServicesAccountType;
        public Type ModelType { get; } = typeof(AmazonWebServicesAccountDetails);
        public Type ApiType { get; } = typeof(AmazonWebServicesAccountResource);
        public IValidator Validator { get; } = new AmazonWebServicesAccountValidator();
        public IVerifyAccount Verifier { get; } = new AmazonWebServicesAccountVerifier();

        public IEnumerable<(string key, object value)> GetFeatureUsage(IAccountMetricContext context)
        {
            var total = context.GetAccountDetails<AmazonWebServicesAccountDetails>().Count();

            yield return ("amazonwebservicesaccount", total);
        }

        public IServiceMessageHandler? ServiceMessageHandler { get; } = new AmazonWebServicesServiceMessageHandler();

        public void BuildMappings(IResourceMappingsBuilder builder)
        {
            builder.Map<AmazonWebServicesAccountResource, AmazonWebServicesAccountDetails>();
        }

        public static class CreateAwsAccountServiceMessagePropertyNames
        {
            public const string Name = "create-awsaccount";
            public const string SecretKey = "secretKey";
            public const string AccessKey = "accessKey";
        }
    }
}