using System.Threading.Tasks;

namespace Spike
{
    interface IStep
    {
        
    }
    
    internal interface IStep<TResult> : IStep
    {
        public Task<TResult> Execute(IContext context);
    }
}