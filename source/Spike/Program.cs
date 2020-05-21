using System.Threading.Tasks;
using Autofac;

namespace Spike
{
    class Program : ProgramBase
    {
        static Task<int> Main(string[] args)
        {
            return new Program().Run(args);
        }

        protected override void RegisterServices(ContainerBuilder builder)
        {
            builder.RegisterType<MyService1>();
        }
    }
}