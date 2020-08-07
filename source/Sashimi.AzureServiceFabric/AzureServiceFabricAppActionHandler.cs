﻿using System;
using Octopus.Core.Variables;
using Octopus.CoreUtilities;
using Octopus.Extensibility.Actions.Calamari;
using Octopus.Shared;
using Sashimi.AzureServiceFabric.Endpoints;
using Sashimi.Server.Contracts.ActionHandlers;

namespace Octopus.Server.Orchestration.ServerTasks.Deploy.Steps.Azure
{
    public class AzureServiceFabricAppActionHandler : IActionHandler
    {
        public string Id => SpecialVariables.Action.ServiceFabric.ServiceFabricAppActionTypeName;
        public string Name => "Deploy a Service Fabric App";
        public string Description => "Deploy the contents of a package to a Service Fabric cluster.";
        public string Keywords => null;
        public bool ShowInStepTemplatePickerUI => true;
        public bool WhenInAChildStepRunInTheContextOfTheTargetMachine => false;
        public bool CanRunOnDeploymentTarget => false;
        public ActionHandlerCategory[] Categories => new[] { ActionHandlerCategory.BuiltInStep, ActionHandlerCategories.Azure };

        public IActionHandlerResult Execute(IActionHandlerContext context)
        {
            var isLegacyAction = !string.IsNullOrWhiteSpace(SpecialVariables.Action.ServiceFabric.ConnectionEndpoint);
            // ReSharper disable once InvertIf
            if (!isLegacyAction && context.DeploymentTargetType.Some())
            {
                if (context.DeploymentTargetType.Value != AzureServiceFabricClusterEndpoint.AzureServiceFabricClusterDeploymentTargetType)
                    throw new ControlledFailureException($"The machine {context.DeploymentTargetName.SomeOr("<unknown>")} will not be deployed to because it is not an {AzureServiceFabricClusterEndpoint.AzureServiceFabricClusterDeploymentTargetType.DisplayName} target.");
            }

            return context.CalamariCommand(CalamariFlavours.CalamariAzure, "deploy-azure-service-fabric-app")
                .WithAzureTools(context)
                .WithStagedPackageArgument()
                .WithArgument("extensions", AzureTools.AzureServiceFabric)
                .Execute();
        }
    }
}