﻿using System;
using Amazon.IdentityManagement;
using Amazon.SecurityToken;
using Calamari.Aws.Deployment;
using Calamari.Aws.Deployment.CloudFormation;
using Calamari.Aws.Integration.CloudFormation;
using Calamari.Commands.Support;
using Calamari.Deployment;
using Octostache;

namespace Calamari.Aws
{
    [Command(AwsKnownVariables.Commands.DeleteAwsCloudformation, Description = "Destroy an existing AWS CloudFormation stack")]
    public class DeleteCloudFormationCommand : AwsCommand
    {
        readonly ICloudFormationService cloudFormationService;

        public DeleteCloudFormationCommand(
            ILog log,
            IVariables variables,
            IAmazonSecurityTokenService amazonSecurityTokenService,
            IAmazonIdentityManagementService amazonIdentityManagementService,
            ICloudFormationService cloudFormationService)
            : base(log, variables, amazonSecurityTokenService, amazonIdentityManagementService)
        {
            this.cloudFormationService = cloudFormationService;
        }

        protected override void Execute(RunningDeployment deployment)
        {
            var stackArn = new StackArn(deployment.Variables.Get(AwsSpecialVariables.CloudFormation.StackName));
            var waitForCompletion = new VariableDictionary().EvaluateTruthy(variables.Get("waitForCompletion"));

            cloudFormationService.DeleteByStackArn(stackArn, waitForCompletion).GetAwaiter().GetResult();
        }
    }
}