using System;
using System.Threading.Tasks;

namespace Spike
{
    public class Step2 : IStep<Step2.Result>
    {
        readonly MyService1 service;

        public Step2(MyService1 service)
        {
            this.service = service;
        }
        
        public async Task<Result> Execute(IContext context)
        {
            await Console.Out.WriteLineAsync("Executing Step2");
            await Console.Out.WriteLineAsync(service.Message);
            
            return new Result {Message = "Hello from Step 2"};
        }

        public class Result
        {
            public string Message { get; set; }
        }
    }
}