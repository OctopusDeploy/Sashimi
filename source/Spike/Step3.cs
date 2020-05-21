using System;
using System.Threading.Tasks;

namespace Spike
{
    public class Step3 : IStep<Step3.Result>
    {
        public class Result
        {
        }

        readonly Step1.Result result1;
        readonly Step2.Result result2;

        public Step3(Step1.Result result1, Step2.Result result2)
        {
            this.result1 = result1;
            this.result2 = result2;
        }
        
        public async Task<Result> Execute(IContext context)
        {
            await Console.Out.WriteLineAsync("Executing Step3");
            await Console.Out.WriteLineAsync(result1.Message);
            await Console.Out.WriteLineAsync(result2.Message);
            return new Result();
        }
    }
}