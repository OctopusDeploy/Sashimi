namespace Calamari.Aws
{
    public class Program : CalamariFlavourProgram
    {

        Program() : base(ConsoleLog.Instance)
        {
        }
        
        public static int Main(string[] args)
        {
            return new Program().Run(args);
        }
    }
}