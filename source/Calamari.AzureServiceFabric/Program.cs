using System.Threading.Tasks;
using Autofac;
using Calamari.AzureServiceFabric.Integration;
using Calamari.Common;
using Calamari.Common.Features.Scripting;
using Calamari.Common.Plumbing.Commands;
using Calamari.Common.Plumbing.Logging;

namespace Calamari.AzureServiceFabric
{
    public class Program : CalamariFlavourProgramAsync
    {
        public Program(ILog log) : base(log)
        {
        }

        public static Task<int> Main(string[] args)
        {
            return new Program(ConsoleLog.Instance).Run(args);
        }

        protected override void ConfigureContainer(ContainerBuilder builder, CommonOptions options)
        {
            base.ConfigureContainer(builder, options);
            builder.RegisterType<AzureServiceFabricPowershellScriptWrapper>().As<IScriptWrapper>().SingleInstance();

        }
    }
}