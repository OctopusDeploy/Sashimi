namespace Spike
{
    class MyCommand : Command
    {
        public override void Build(IVariables variable)
        {
            Add<Step1>();
            Add<Step2>();
            Add<Step3>();
        }
    }
}