﻿using System;
using Sashimi.Server.Contracts;
using Sashimi.Server.Contracts.ActionHandlers;

namespace Sashimi.AzureScripting
{
    public class AzurePowerShellActionHandler : IActionHandlerWithAccount
    {
        public string Id => "Octopus.AzurePowerShell";
        public string Name => "Run an Azure Script";
        public string Description => "Runs a custom script using an Azure subscription, with the Azure modules loaded by default.";
        public string? Keywords => null;
        public bool ShowInStepTemplatePickerUI => true;
        public bool WhenInAChildStepRunInTheContextOfTheTargetMachine => false;
        public bool CanRunOnDeploymentTarget => false;
        public ActionHandlerCategory[] Categories => new[] { ActionHandlerCategory.BuiltInStep, AzureConstants.AzureActionHandlerCategory, ActionHandlerCategory.Script };
        public string[] StepBasedVariableNameForAccountIds { get; } = {SpecialVariables.Action.Azure.AccountId};

        public IActionHandlerResult Execute(IActionHandlerContext context)
        {
            var syntax = context.Variables.GetEnum(KnownVariables.Action.Script.Syntax, ScriptSyntax.PowerShell);

            var builder = context.CalamariCommand(AzureConstants.CalamariAzure, "run-script")
                                 .WithAzureCLI(context);

            if (syntax == ScriptSyntax.PowerShell)
                builder = builder.WithAzureCmdlets(context);

            var isInPackage = KnownVariableValues.Action.Script.ScriptSource.Package.Equals(context.Variables.Get(KnownVariables.Action.Script.ScriptSource), StringComparison.OrdinalIgnoreCase);
            if (isInPackage)
            {
                builder.WithStagedPackageArgument();
            }

            return builder.Execute();
        }
    }
}