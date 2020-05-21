using System;
using System.Threading.Tasks;

namespace Spike
{
    public class Step1 : IStep<Step1.Result>
    {
        public async Task<Result> Execute(IContext context)
        {
            await Console.Out.WriteLineAsync("Executing Step1");
            return new Result() {Message = "Hello from Step 1"};
        }
        
        


        public class Result
        {
            public string Message { get; set; }
        }
    }
}