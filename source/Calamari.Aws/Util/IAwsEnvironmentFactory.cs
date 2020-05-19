using System.Threading.Tasks;
using Calamari.CloudAccounts;

namespace Calamari.Aws.Util
{
    public interface IAwsEnvironmentFactory
    {
        Task<AwsEnvironmentGeneration> Create();
    }
}