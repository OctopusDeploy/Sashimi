﻿using Sashimi.Server.Contracts.ActionHandlers;
using Sashimi.Server.Contracts.Calamari;

namespace Sashimi.NamingIsHard
{
    class MyActionHandler : IActionHandler
    {
        public IActionHandlerResult Execute(IActionHandlerContext context)
        {
            return context.CalamariCommand(new CalamariFlavour("Calamari.NamingIsHard"), "my-command-name")
                .Execute(context.TaskLog);
        }

        public string Id { get; } = SpecialVariables.MyActionHandlerTypeName;
        public string Name { get; } = "NamingIsHard";
        public string Description { get; } = "NamingIsHard";
        public string? Keywords { get; }
        public bool ShowInStepTemplatePickerUI { get; }
        public bool WhenInAChildStepRunInTheContextOfTheTargetMachine { get; }
        public bool CanRunOnDeploymentTarget { get; }
        public ActionHandlerCategory[] Categories { get; } = new ActionHandlerCategory[0];
    }
}