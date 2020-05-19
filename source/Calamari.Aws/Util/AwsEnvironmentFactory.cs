using System.Threading.Tasks;
using Calamari.CloudAccounts;

namespace Calamari.Aws.Util
{
    public class AwsEnvironmentFactory : IAwsEnvironmentFactory
    {
        readonly ILog log;
        readonly IVariables variables;

        public AwsEnvironmentFactory(ILog log, IVariables variables)
        {
            this.log = log;
            this.variables = variables;
        }

        public Task<AwsEnvironmentGeneration> Create()
        {
            return AwsEnvironmentGeneration.Create(log, variables);
        }
    }
}