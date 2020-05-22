using Octopus.TinyTypes;

namespace Sashimi.Server.Contracts.Accounts
{
    public class AccountType : CaseInsensitiveTypedString
    {
        public static AccountType UsernamePassword = new AccountType("UsernamePassword");
        public static AccountType SshKeyPair = new AccountType("SshKeyPair");
        public static AccountType Token = new AccountType("Token");
        public static AccountType AzureSubscription = new AccountType("AzureSubscription");
        public static AccountType AzureServicePrincipal = new AccountType("AzureServicePrincipal");
        public static AccountType AmazonWebServicesAccount = new AccountType("AmazonWebServicesAccount");
        public static AccountType AmazonWebServicesRoleAccount = new AccountType("AmazonWebServicesRoleAccount");

        public AccountType(string value) : base(value)
        {
        }
    }
}