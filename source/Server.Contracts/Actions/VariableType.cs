using Sashimi.Server.Contracts.Actions.Templates;

namespace Sashimi.Server.Contracts.Actions
{
    public class VariableType
    {
        public static VariableType String = new VariableType("String");
        public static VariableType Sensitive = new VariableType("Sensitive", ControlType.Sensitive);
        public static VariableType Certificate = new VariableType("Certificate", ControlType.Certificate, ReferencesOtherDocuments.Yes);
        public static VariableType WorkerPool = new VariableType("WorkerPool", ControlType.WorkerPool, ReferencesOtherDocuments.Yes);
        public static VariableType AmazonWebServicesAccount = new VariableType("AmazonWebServicesAccount", ControlType.AmazonWebServicesAccount, ReferencesOtherDocuments.Yes);
        public static VariableType AzureAccount = new VariableType("AzureAccount", ControlType.AzureAccount, ReferencesOtherDocuments.Yes);

        public VariableType(string name, ControlType controlType = null, ReferencesOtherDocuments referencesOtherDocuments = ReferencesOtherDocuments.No)
        {
            Name = name;
            ControlType = controlType;
            ReferencesOtherDocuments = referencesOtherDocuments;
        }

        public string Name { get; }
        public ControlType ControlType { get; }
        public ReferencesOtherDocuments ReferencesOtherDocuments { get; }
    }
}