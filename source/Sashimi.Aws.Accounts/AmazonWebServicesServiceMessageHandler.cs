using System.Collections.Generic;
using Octopus.Data.Model;
using Sashimi.Server.Contracts.Accounts;
using Sashimi.Server.Contracts.ServiceMessages;

namespace Sashimi.Aws.Accounts
{
    class AmazonWebServicesServiceMessageHandler : ICreateAccountDetailsServiceMessageHandler
    {
        public string AuditEntryDescription => "AWS Account";
        public string ServiceMessageName => CreateAwsAccountServiceMessagePropertyNames.Name;

        public AccountDetails CreateAccountDetails(IDictionary<string, string> messageProperties)
        {
            messageProperties.TryGetValue(CreateAwsAccountServiceMessagePropertyNames.AccessKey, out var accessKey);
            messageProperties.TryGetValue(CreateAwsAccountServiceMessagePropertyNames.SecretKey, out var secretKey);

            return new AmazonWebServicesAccountDetails
            {
                AccessKey = accessKey,
                SecretKey = secretKey.ToSensitiveString()
            };
        }
    }
}
