using Octopus.Data.Model;

namespace Sashimi.Server.Contracts.Accounts.Temp
{
    public interface IAmazonWebServicesAccountDetails
    {
        string AccessKey { get; set; }
        SensitiveString SecretKey { get; set; }
    }
}