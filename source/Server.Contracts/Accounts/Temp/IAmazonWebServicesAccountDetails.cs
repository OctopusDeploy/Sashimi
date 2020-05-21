using Octopus.Data.Model;

namespace Sashimi.Server.Contracts.Accounts.Temp
{
    public interface IAmazonWebServicesAccountDetails : IAccountDetails
    {
        string AccessKey { get; set; }
        SensitiveString SecretKey { get; set; }
    }
}