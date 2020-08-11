using Sashimi.Server.Contracts.Accounts;

namespace Sashimi.Azure.Accounts
{
    public static class AccountTypes
    {
        public static readonly AccountType AzureServicePrincipalAccountType = new AccountType("AzureServicePrincipal");
        public static readonly AccountType AzureSubscriptionAccountType = new AccountType("AzureSubscription");
    }
}