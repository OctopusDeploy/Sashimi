using System.Collections.Generic;
using Octostache;
using Sashimi.Server.Contracts.Accounts;
using Sashimi.Server.Contracts.ServiceMessages;

namespace Sashimi.Aws.Accounts
{
    class AmazonWebServicesServiceMessageHandler : IServiceMessageHandler
    {
        public string AuditEntryDescription => "AWS Account";
        public string ServiceMessageName => CreateAwsAccountServiceMessagePropertyNames.Name;

        public ServiceMessageValidationResult IsServiceMessageValid(IDictionary<string, string> properties, VariableDictionary variables)
        {
            var secretValid = properties.ContainsPropertyWithValue(CreateAwsAccountServiceMessagePropertyNames.SecretKey);
            var accessValid = properties.ContainsPropertyWithValue(CreateAwsAccountServiceMessagePropertyNames.AccessKey);

            if (!(secretValid && accessValid))
            {
                var messages = new List<string>();
                if (!secretValid) messages.Add("Secret Key is missing or invalid");
                if (!accessValid) messages.Add("Access Key is missing or invalid");

                return ServiceMessageValidationResult.Invalid(messages);
            }

            return ServiceMessageValidationResult.Valid;
        }
    }
}
