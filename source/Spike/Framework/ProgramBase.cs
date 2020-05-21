using System;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Autofac;

namespace Spike
{
    class ProgramBase
    {
        protected async Task<int> Run(string[] args)
        {
            var command = new MyCommand();

            command.Build(new Variables());

            var steps = command.Steps;
            var builder = new ContainerBuilder();
            RegisterServices(builder);
            var context = new MyContext();
            var executeMethod = typeof(ProgramBase).GetMethod(nameof(Execute), 2, BindingFlags.Instance | BindingFlags.NonPublic, null, CallingConventions.Any, new[] {typeof(IContext), typeof(ILifetimeScope)}, new ParameterModifier[0]);
            var methods = typeof(ProgramBase).GetMethods(BindingFlags.Instance | BindingFlags.NonPublic);
            ILifetimeScope container = builder.Build();
            Type stepType;
            Object result = null;
            do
            {
                stepType = steps.Dequeue();

                container = container.BeginLifetimeScope(b =>
                {
                    if (result != null)
                    {
                        b.RegisterInstance(result).AsSelf();
                    }

                    b.RegisterType(stepType);

                });

                var r = stepType.GetInterfaces().Where(type => type.IsGenericType && type.GetGenericTypeDefinition() == typeof(IStep<>)).Single().GetGenericArguments()[0];
                var genericMethod = executeMethod.MakeGenericMethod(stepType, r);
                result = await (dynamic)genericMethod.Invoke(this, new object[] {context, container});

            } while (steps.Count > 0);

            return 0;
        }

        Task<TResult> Execute<TStep, TResult>(IContext context, ILifetimeScope container) where TStep: IStep<TResult>
        {
            var step = container.Resolve<TStep>();
            
            return step.Execute(context);
        } 

        protected virtual void RegisterServices(ContainerBuilder builder)
        {
            
        }
    }
}