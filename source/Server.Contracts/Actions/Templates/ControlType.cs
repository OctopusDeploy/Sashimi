namespace Sashimi.Server.Contracts.Actions.Templates
{
    public class ControlType
    {
        public static ControlType SingleLineText = new ControlType("SingleLineText");
        public static ControlType MultiLineText = new ControlType("MultiLineText");
        public static ControlType Select = new ControlType("Select");
        public static ControlType Checkbox = new ControlType("Checkbox");
        public static ControlType Sensitive = new ControlType("Sensitive");
        public static ControlType StepName = new ControlType("StepName");
        public static ControlType AzureAccount = new ControlType("AzureAccount");
        public static ControlType Certificate = new ControlType("Certificate");
        public static ControlType WorkerPool = new ControlType("WorkerPool");
        public static ControlType AmazonWebServicesAccount = new ControlType("AmazonWebServicesAccount");

        public ControlType(string name)
        {
            Name = name;
        }

        public string Name { get; }

        public override string ToString()
        {
            return Name;
        }
    }
}