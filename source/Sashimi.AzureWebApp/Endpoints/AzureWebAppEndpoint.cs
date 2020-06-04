﻿using System.Collections.Generic;
using Sashimi.Server.Contracts;
using Sashimi.Server.Contracts.Endpoints;
using Sashimi.Server.Contracts.Variables;

namespace Sashimi.AzureWebApp
{
    public class AzureWebAppEndpoint : Endpoint, IEndpointWithAccount, IRunsOnAWorker
    {
        public static readonly DeploymentTargetType AzureWebAppDeploymentTargetType = new DeploymentTargetType("AzureWebApp", "Azure Web Application");

        public override DeploymentTargetType DeploymentTargetType { get; } = AzureWebAppDeploymentTargetType;
        public override string Description => WebAppName;

        public override bool ScriptConsoleSupported => true;

        public string AccountId { get; set; }

        public string WebAppName { get; set; }
        public string ResourceGroupName { get; set; }
        public string WebAppSlotName { get; set; }

        public override IEnumerable<Variable> ContributeVariables()
        {
            yield return new Variable(SpecialVariables.Action.Azure.ResourceGroupName, ResourceGroupName);
            yield return new Variable(SpecialVariables.Action.Azure.WebAppName, WebAppName);
            yield return new Variable(SpecialVariables.Action.Azure.WebAppSlot, WebAppSlotName);
        }

        public override IEnumerable<(string id, DocumentType documentType)> GetRelatedDocuments()
        {
            if (!string.IsNullOrEmpty(AccountId))
                yield return (AccountId, DocumentType.Account);

            if (!string.IsNullOrEmpty(DefaultWorkerPoolId))
                yield return (DefaultWorkerPoolId, DocumentType.WorkerPool);
        }

        public string DefaultWorkerPoolId { get; set; }
    }
}