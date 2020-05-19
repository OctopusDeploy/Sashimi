using System;
using Calamari.Commands.Support;
using Calamari.Deployment.Conventions;
using Calamari.Integration.FileSystem;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Calamari.Aws.Deployment.CloudFormation;
using Calamari.Aws.Integration.CloudFormation;
using Calamari.Aws.Integration.CloudFormation.Templates;
using Calamari.Aws.Util;
using Calamari.Common.Util;
using Calamari.Extensions;
using Newtonsoft.Json;
using Octopus.CoreUtilities;
using Octostache;

namespace Calamari.Aws
{
    [Command(KnownAwsCalamariCommands.Commands.DeployAwsCloudFormation, Description = "Creates a new AWS CloudFormation deployment")]
    public class DeployCloudFormationCommand : AwsCommand
    {
        readonly ICalamariFileSystem fileSystem;
        readonly IExtractPackage extractPackage;
        readonly ICloudFormationService cloudFormationService;

        public DeployCloudFormationCommand(
            ILog log,
            IVariables variables,
            ICloudFormationService cloudFormationService,
            IAmazonClientFactory amazonClientFactory,
            ICalamariFileSystem fileSystem,
            IExtractPackage extractPackage)
            : base(log, variables, amazonClientFactory)
        {
            this.cloudFormationService = cloudFormationService;
            this.fileSystem = fileSystem;
            this.extractPackage = extractPackage;
        }

        protected override async Task ExecuteCore()
        {
            var packageFilePath = variables.GetPathToPrimaryPackage(fileSystem, true);
            var stackArn = new StackArn(variables.Get(SpecialVariableNames.Aws.CloudFormation.StackName));
            var roleArn = variables.Get(SpecialVariableNames.Aws.CloudFormation.RoleArn);
            var iamCapabilities = GetValidIamCapabilities();
            var cloudFormationTemplate = GetCloudFormationTemplate(packageFilePath);

            var waitForCompletion = ((VariableDictionary) variables).EvaluateTruthy(variables.Get(SpecialVariableNames.Action.WaitForCompletion));

            extractPackage.ExtractToStagingDirectory(packageFilePath);

            if (IsChangeSetsEnabled())
            {
                var changeSetName = GenerateChangeSetName();
                var result = await cloudFormationService.CreateChangeSet(changeSetName, cloudFormationTemplate, stackArn, roleArn, iamCapabilities);
                SetOutputVariable("ChangesetId", result.ChangeSet.Value);
                SetOutputVariable("StackId", result.Stack.Value);
                variables.Set(SpecialVariableNames.Aws.CloudFormation.ChangeSets.Arn, result.ChangeSet.Value);

                var changes = await cloudFormationService.GetChangeSet(result.Stack, result.ChangeSet);
                SetOutputVariable("ChangeCount", changes.Count.ToString());
                SetOutputVariable("Changes", JsonConvert.SerializeObject(changes, Formatting.Indented));

                if (IsImmediateChangeSetExecution())
                {
                    await cloudFormationService.ExecuteChangeSet(result.Stack, result.ChangeSet, waitForCompletion);

                    SetOutputVariables(await cloudFormationService.GetOutputVariablesByStackArn(result.Stack));
                }
            }
            else
            {
                var isRollbackDisabled = ((VariableDictionary) variables).EvaluateTruthy(variables.Get(SpecialVariableNames.Action.DisableRollBack));

                var stackId = await cloudFormationService.Deploy(cloudFormationTemplate, stackArn, roleArn, iamCapabilities, isRollbackDisabled, waitForCompletion);
                // Take the stackArn ID returned by the create or update events, and save it as an output variable
                SetOutputVariable("StackId", stackId);

                SetOutputVariables(await cloudFormationService.GetOutputVariablesByStackArn(stackArn));
            }
        }

        CloudFormationTemplate GetCloudFormationTemplate(string pathToPackage)
        {
            var templateFile = variables.Get(SpecialVariableNames.Aws.CloudFormation.Template);
            var templateParameterFile = variables.Get(SpecialVariableNames.Aws.CloudFormation.TemplateParameters);
            var isFileInPackage = !string.IsNullOrWhiteSpace(pathToPackage);

            var templateResolver = new TemplateResolver(fileSystem);

            var resolvedTemplate = templateResolver.Resolve(templateFile, isFileInPackage, variables);
            var resolvedParameters = templateResolver.MaybeResolve(templateParameterFile, isFileInPackage, variables);

            if (templateParameterFile != null && !resolvedParameters.Some())
                throw new CommandException("Could not find template parameters file: " + templateParameterFile);

            var parameters = CloudFormationParametersFile.Create(resolvedParameters, fileSystem, variables);

            return CloudFormationTemplate.Create(resolvedTemplate, parameters, fileSystem, variables);
        }

        bool IsImmediateChangeSetExecution()
        {
            return !((VariableDictionary) variables).EvaluateTruthy(variables.Get(SpecialVariableNames.Aws.CloudFormation.ChangeSets.Defer));
        }

        bool IsChangeSetsEnabled()
        {
            return variables.Get(SpecialVariableNames.Package.EnabledFeatures)
                       ?.Contains(SpecialVariableNames.Aws.CloudFormation.ChangeSets.Feature) ?? false;
        }

        string GenerateChangeSetName()
        {
            var name = $"octo-{Guid.NewGuid():N}";

            if (((VariableDictionary)variables).EvaluateTruthy(variables.Get(SpecialVariableNames.Aws.CloudFormation.ChangeSets.Generate)))
            {
                variables.Set(SpecialVariableNames.Aws.CloudFormation.ChangeSets.Name, name);
            }

            log.SetOutputVariableButDoNotAddToVariables("ChangesetName", name);

            return name;
        }

        IReadOnlyCollection<string> GetValidIamCapabilities()
        {
            var iamCapabilities = JsonConvert.DeserializeObject<List<string>>(variables.Get(SpecialVariableNames.Aws.IamCapabilities, "[]"));

            return ExcludeAndLogUnknownIamCapabilities(iamCapabilities);
        }

        IReadOnlyCollection<string> ExcludeAndLogUnknownIamCapabilities(IEnumerable<string> iamCapabilities)
        {
            var (validCapabilities, excludedCapabilities) = iamCapabilities.Aggregate((new List<string>(), new List<string>()), (prev, current) =>
            {
                var (valid, excluded) = prev;

                if (current.IsKnownIamCapability())
                    valid.Add(current);
                else
                    excluded.Add(current);

                return prev;
            });

            if (excludedCapabilities.Any())
            {
                log.Warn($"The following unknown IAM Capabilities have been removed: {string.Join(", ", excludedCapabilities)}");
            }

            return validCapabilities;
        }
    }
}