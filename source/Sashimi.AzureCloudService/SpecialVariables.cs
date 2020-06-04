namespace Sashimi.AzureCloudService
{
    //TODO: This is duplicated from Server while we sort out a way for Sashimi to contribute special variables.
    static class SpecialVariables
    {
        public static class Action
        {
            public static class Azure
            {
                public static readonly string CloudServiceName = "Octopus.Action.Azure.CloudServiceName";

                public static readonly string StorageAccountName = "Octopus.Action.Azure.StorageAccountName";

                public static readonly string Slot = "Octopus.Action.Azure.Slot";

                public static readonly string SwapIfPossible = "Octopus.Action.Azure.SwapIfPossible";

                public static readonly string UseCurrentInstanceCount = "Octopus.Action.Azure.UseCurrentInstanceCount";
            }
        }
    }
}