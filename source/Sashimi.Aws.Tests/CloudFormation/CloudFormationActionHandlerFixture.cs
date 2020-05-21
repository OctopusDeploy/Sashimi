using System.Collections.Generic;
using System.IO;
using FluentAssertions;
using NUnit.Framework;
using Sashimi.Aws.ActionHandler;
using Sashimi.Tests.Shared;
using Sashimi.Tests.Shared.Server;

namespace Sashimi.Aws.Tests.CloudFormation
{
    public class CloudFormationActionHandlerFixture
    {
        readonly string StackRole = "arn:aws:iam::968802670493:role/e2e_buckets";
        readonly string TransformIncludeLocation = "s3://octopus-e2e-tests/permanent/tags.json";
        readonly string Region = "us-east-1";
        readonly string pathToPackage = TestEnvironment.GetTestPath(@"Packages\CloudFormationS3.1.0.0.nupkg");

        string StackName = string.Empty;

        [SetUp]
        public void Setup()
        {
            StackName = $"E2ETestStack-{UniqueName.Generate()}";
        }

        [Test]
        public void RunCloudFormation_InlineSourceWithoutParameters()
        {
            var resourceName = @"e2e-test-aws-cf-" + UniqueName.Short();

            string template = File.ReadAllText(Path.Combine(TestEnvironment.GetTestPath(), "CloudFormation", "package-withoutparameters", "template.json"))
                .Replace("e2e-test-aws-cf-@UNIQUEID", resourceName);
            TestActionHandlerResult result = ActionHandlerTestBuilder.Create<AwsRunCloudFormationActionHandler, Calamari.Aws.Program>()
                    .WithArrange(context =>
                    {
                        WithRunCloudFormationVariables(context, StackName);
                        context.WithAwsTemplateInlineSource();
                        context.WithCloudFormationTemplate(template, null);
                        context.Variables.Add(AwsSpecialVariables.Action.Aws.WaitForCompletion, bool.TrueString);
                    })
                    .Execute(assertWasSuccess:false);

            result.WasSuccessful.Should().BeTrue();
            
            // TODO: validate output parameter values
            // RunInlineScript(VerifyOutputsStepName,
            //                     $"$outputVar = $OctopusParameters[\"Octopus.Action[{DeployAwsCloudFormationStackStepName}].Output.AwsOutputs[OutputName]\"]" + Environment.NewLine +
            //                     $"if (-not($outputVar -match \"{resourceName}\")) {{ throw \"Expected OutputName output variable to be '{resourceName}', but was '$outputVar'\"}}"
            //                 ),
        }

        [Test]
        public void RunCloudFormation_InlineSourceWithParameters()
        {
            string VariableParameterValue = "e2e-test-aws-cf-var-" + UniqueName.Short();
            string VariableParameterName = "NameVarParam";
            //string VariableParameterOutput = "OutputWithVariableParam";
            string OctopusVariableName = "NameInVariable";
            string PlainParameterValue = "e2e-test-aws-cf-plain-" + UniqueName.Short();
            const string PlainParameterName = "NamePlainParam";
            //const string PlainParameterOutput = "OutputWithPlainParam";
            string template = File.ReadAllText(Path.Combine(TestEnvironment.GetTestPath(), "CloudFormation", "package-withparameters", "template.yaml"));
            var parameterValues = $"[{{\"ParameterKey\": \"{VariableParameterName}\", \"ParameterValue\": \"#{{{OctopusVariableName}}}\"}},"
                                  + $"{{\"ParameterKey\": \"{PlainParameterName}\", \"ParameterValue\": \"{PlainParameterValue}\"}}]";

            var result = ActionHandlerTestBuilder.Create<AwsRunCloudFormationActionHandler, Calamari.Aws.Program>()
                .WithArrange(context =>
                {
                    WithRunCloudFormationVariables(context, StackName);
                    context.WithAwsTemplateInlineSource();
                    context.WithCloudFormationTemplate(template, parameterValues);
                    context.Variables.Add(AwsSpecialVariables.Action.Aws.WaitForCompletion, bool.TrueString);
                    context.Variables.Add(AwsSpecialVariables.Action.Aws.CloudFormation.TemplateParameters, parameterValues);
                })
                .Execute(assertWasSuccess:false);
            
            // TODO: validate output parameter values
            // result.OutputVariables.ContainsKey()
            
            // RunInlineScript(VerifyOutputsStepName + "1",
            //     $"$outputVar = $OctopusParameters[\"Octopus.Action[{DeployAwsCloudFormationStackStepName}].Output.AwsOutputs[{VariableParameterOutput}]\"]" + Environment.NewLine +
            //     $"if (-not($outputVar -match \"{VariableParameterValue}\")) {{ throw \"Expected '{VariableParameterOutput}' output variable to be '{VariableParameterValue}', but was '$outputVar'\"}}"
            // ),
            // RunInlineScript(VerifyOutputsStepName + "2",
            //     $"$outputVar = $OctopusParameters[\"Octopus.Action[{DeployAwsCloudFormationStackStepName}].Output.AwsOutputs[{PlainParameterOutput}]\"]" + Environment.NewLine +
            //     $"if (-not($outputVar -match \"{PlainParameterValue}\")) {{ throw \"Expected '{PlainParameterOutput}' output variable to be '{PlainParameterValue}', but was '$outputVar'\"}}"
            // ),
        }

        [Test]
        public void RunCloudFormation_PackageWithoutParameters()
        {
            var uniqueId = UniqueName.Short();
            var resourceName = @"e2e-test-aws-cf-" + uniqueId;

            // TODO: may need to package this template.json file
            var sourcePackageFolder = Path.Combine(TestEnvironment.GetTestPath(), "CloudFormation", "package-withoutparameters", "template.json");

            var result = ActionHandlerTestBuilder.Create<AwsRunCloudFormationActionHandler, Calamari.Aws.Program>()
                .WithArrange(context =>
                {
                    WithRunCloudFormationVariables(context, StackName);
                    context.WithAwsTemplatePackageSource();
                    context.WithCloudFormationTemplate(sourcePackageFolder, null);
                    context.Variables.Add(AwsSpecialVariables.Action.Aws.WaitForCompletion, bool.TrueString);
                })
                .Execute(assertWasSuccess: false);
            
            // TODO: validate output parameter values
            // result.OutputVariables.ContainsKey()
            // RunInlineScript(VerifyOutputsStepName,
            //     $"$outputVar = $OctopusParameters[\"Octopus.Action[{DeployAwsCloudFormationStackStepName}].Output.AwsOutputs[OutputName]\"]" + Environment.NewLine +
            //     $"if (-not($outputVar -match \"{resourceName}\")) {{ throw \"Expected OutputName output variable to be '{resourceName}', but was '$outputVar'\"}}"
            // ),
        }

        [Test]
        public void RunCloudFormation_PackageWithParameters()
        {
            string uniqueId = UniqueName.Short();
            string variableParameterValue = "e2e-test-aws-cf-var-" + uniqueId;
            //string VariableParameterOutput = "OutputWithVariableParam";
            //string OctopusVariableName = "NameInVariable";

            string plainParameterValue = "e2e-test-aws-cf-plain-" + uniqueId;
            //string PlainParameterOutput = "OutputWithPlainParam";
            
            var sourcePackageFolder = Path.Combine(TestEnvironment.GetTestPath(), "CloudFormation", "package-withparameters");
            
            var parametersFile = Path.Combine(TestEnvironment.GetTestPath(), "CloudFormation", "parameters.json");
            string template = File.ReadAllText(parametersFile).Replace("e2e-test-aws-cf-plain-@UNIQUEID", plainParameterValue);
            File.WriteAllText(parametersFile, template);
            
            var result = ActionHandlerTestBuilder.Create<AwsRunCloudFormationActionHandler, Calamari.Aws.Program>()
                .WithArrange(context =>
                {
                    WithRunCloudFormationVariables(context, StackName);
                    context.WithAwsTemplatePackageSource();
                    context.WithCloudFormationTemplate(sourcePackageFolder, parametersFile);
                    context.Variables.Add(AwsSpecialVariables.Action.Aws.WaitForCompletion, bool.TrueString);
                })
                .Execute(assertWasSuccess: false);
            
            // TODO: validate output parameter values
            // result.OutputVariables.ContainsKey()
            // RunInlineScript(VerifyOutputsStepName + "1",
            //     $"$outputVar = $OctopusParameters[\"Octopus.Action[{DeployAwsCloudFormationStackStepName}].Output.AwsOutputs[{VariableParameterOutput}]\"]" + Environment.NewLine +
            //     $"if (-not($outputVar -match \"{variableParameterValue}\")) {{ throw \"Expected '{VariableParameterOutput}' output variable to be '{variableParameterValue}', but was '$outputVar'\"}}"
            // ),
            // RunInlineScript(VerifyOutputsStepName + "2",
            //     $"$outputVar = $OctopusParameters[\"Octopus.Action[{DeployAwsCloudFormationStackStepName}].Output.AwsOutputs[{PlainParameterOutput}]\"]" + Environment.NewLine +
            //     $"if (-not($outputVar -match \"{plainParameterValue}\")) {{ throw \"Expected '{PlainParameterOutput}' output variable to be '{plainParameterValue}', but was '$outputVar'\"}}"
            // ),
        }
        
        [Test]
        public void RunCloudFormation_ChangeSet()
        {
            var bucketName = $"cfe2e-{UniqueName.Generate()}";

            // create bucket
            CreateBucket(StackName, bucketName);

            // remove bucket tags
            var (changeSetId, stackId) = RemoveBucketTag(StackName, bucketName);

            // apply remove bucket tags
            ApplyChangeSet(changeSetId, stackId, bucketName);

            // create bucket again
            CreateBucketAgain(StackName, bucketName);
        }

        [TearDown]
        public void TearDown()
        {
            DeleteStack(StackName);
        }

        void DeleteStack(string stackName)
        {
            ActionHandlerTestBuilder.Create<AwsDeleteCloudFormationActionHandler, Calamari.Aws.Program>()
                .WithArrange(context =>
                {
                    context.Variables.Add("Octopus.Action.Aws.AssumeRole", bool.FalseString);
                    context.Variables.Add("Octopus.Action.Aws.AssumedRoleArn", string.Empty);
                    context.Variables.Add("Octopus.Action.Aws.AssumedRoleSession", string.Empty);
                    context.Variables.Add("Octopus.Action.Aws.Region", Region);
                    context.Variables.Add("Octopus.Action.Aws.CloudFormationStackName", stackName);
                    context.Variables.Add("Octopus.Action.Aws.WaitForCompletion", bool.TrueString);
                    context.WithAwsAccount();
                })
                .Execute();
        }

        void CreateBucketAgain(string stackName, string bucketName)
        {
            ActionHandlerTestBuilder.Create<AwsRunCloudFormationActionHandler, Calamari.Aws.Program>()
                .WithArrange(context =>
                {
                    WithRunCloudFormationVariables(context, stackName);
                    context.Variables.Add("BucketName", bucketName);
                    context.Variables.Add("TransformIncludeLocation", TransformIncludeLocation);
                    context.WithAwsRegion(Region);
                    context.WithAwsTemplatePackageSource();
                    context.WithCloudFormationTemplate("bucket.json", "bucket-parameters.json");
                    context.Variables.Add(AwsSpecialVariables.Action.Aws.WaitForCompletion, bool.TrueString);
                    context.WithStackRole(StackRole);
                    context.WithCloudFormationChangeSets();
                    context.WithPackage(pathToPackage);
                })
                .Execute();
        }

        void ApplyChangeSet(string changeSetId, string stackId, string bucketName)
        {
            ActionHandlerTestBuilder.Create<AwsApplyCloudFormationChangeSetActionHandler, Calamari.Aws.Program>()
                .WithArrange(context =>
                {
                    WithRunCloudFormationVariables(context, stackId);
                    context.Variables.Add("BucketName", bucketName);
                    context.Variables.Add("TransformIncludeLocation", TransformIncludeLocation);
                    context.Variables.Add("Octopus.Action.Aws.WaitForCompletion", bool.TrueString);
                    context.Variables.Add(AwsSpecialVariables.Action.Aws.CloudFormation.Changesets.Arn, changeSetId);
                })
                .Execute();
        }

        (string changeSetId, string stackName) RemoveBucketTag(string stackName, string bucketName)
        {
            string changeSetId = string.Empty;
            string stackId = string.Empty;
            ActionHandlerTestBuilder.Create<AwsRunCloudFormationActionHandler, Calamari.Aws.Program>()
                .WithArrange(context =>
                {
                    WithRunCloudFormationVariables(context, stackName);
                    context.Variables.Add("BucketName", bucketName);
                    context.Variables.Add("TransformIncludeLocation", TransformIncludeLocation);
                    context.WithAwsTemplatePackageSource();
                    context.WithCloudFormationTemplate("bucket.json", "bucket-parameters.json");               
                    context.Variables.Add(AwsSpecialVariables.Action.Aws.WaitForCompletion, bool.TrueString);
                    context.WithStackRole(StackRole);
                    context.WithCloudFormationChangeSets(deferExecution: true);
                    context.WithIamCapabilities(new List<string> {"CAPABILITY_IAM"});
                    context.WithPackage(pathToPackage);
                })
                .WithAssert(result =>
                {
                    result.WasSuccessful.Should().BeTrue();
                    changeSetId = result.OutputVariables["AwsOutputs[ChangesetId]"].Value;
                    stackId = result.OutputVariables["AwsOutputs[StackId]"].Value;
                })
                .Execute();
            
            return (changeSetId, stackId);
        }

        void CreateBucket(string stackName, string bucketName)
        {
            ActionHandlerTestBuilder.Create<AwsRunCloudFormationActionHandler, Calamari.Aws.Program>()
                .WithArrange(context =>
                {
                    context.Variables.Add("BucketName", bucketName);
                    context.Variables.Add("TransformIncludeLocation", TransformIncludeLocation);
                    WithRunCloudFormationVariables(context, stackName);
                    context.WithCloudFormationTemplate("bucket-transform.json", "bucket-parameters.json");
                    context.WithAwsTemplatePackageSource();
                    context.Variables.Add(AwsSpecialVariables.Action.Aws.WaitForCompletion, bool.TrueString);
                    context.WithStackRole(StackRole);
                    context.WithCloudFormationChangeSets();
                    context.WithIamCapabilities(new List<string> {"CAPABILITY_IAM"});
                    context.WithPackage(pathToPackage);
                    
                })
                .Execute();
        }

        void WithRunCloudFormationVariables(TestActionHandlerContext<Calamari.Aws.Program> context, string stackName)
        {
            context.Variables.Add("Octopus.Action.Aws.AssumeRole", bool.FalseString);
            context.Variables.Add("Octopus.Action.Aws.AssumedRoleArn", string.Empty);
            context.Variables.Add("Octopus.Action.Aws.AssumedRoleSession", string.Empty);
            context.Variables.Add(AwsSpecialVariables.Action.Aws.CloudFormation.StackName, stackName);
            context.WithAwsRegion(Region);
            context.WithAwsAccount();
        }
    }
}