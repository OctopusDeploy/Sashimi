using System;
using System.Collections.Generic;
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
        
        [Test]
        public void ChangeSet()
        {
            var stackName = $"E2ETestStack-{UniqueName.Generate()}";
            var bucketName = $"cfe2e-{UniqueName.Generate()}";

            try
            {
                // create bucket
                CreateBucket(stackName, bucketName);

                // remove bucket tags
                string stackId = null;
                string changeSetId = null;
                (changeSetId, stackId) = RemoveBucket(stackName, bucketName);

                // apply remove bucket tags
                ApplyChangeSet(changeSetId, stackId, bucketName);

                // create bucket again
                CreateBucketAgain(stackName, bucketName);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            finally
            {
                // delete cloud formation
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
        }

        void CreateBucketAgain(string stackName, string bucketName)
        {
            ActionHandlerTestBuilder.Create<AwsRunCloudFormationActionHandler, Calamari.Aws.Program>()
                .WithArrange(context =>
                {
                    context.Variables.Add("BucketName", bucketName);
                    context.Variables.Add("TransformIncludeLocation", TransformIncludeLocation);
                    context.Variables.Add("Octopus.Action.Aws.AssumeRole", bool.FalseString);
                    context.Variables.Add("Octopus.Action.Aws.AssumedRoleArn", string.Empty);
                    context.Variables.Add("Octopus.Action.Aws.AssumedRoleSession", string.Empty);
                    context.Variables.Add("Octopus.Action.Aws.Region", Region);
                    context.Variables.Add(AwsSpecialVariables.Action.Aws.CloudFormation.StackName, stackName);
                    context.Variables.Add(AwsSpecialVariables.Action.Aws.TemplateSource, AwsSpecialVariables.Action.Aws.TemplateSourceOptions.Package);
                    context.Variables.Add(AwsSpecialVariables.Action.Aws.CloudFormation.Template, "bucket.json");
                    context.Variables.Add(AwsSpecialVariables.Action.Aws.CloudFormation.TemplateParametersRaw, "bucket-parameters.json");
                    context.Variables.Add(AwsSpecialVariables.Action.Aws.WaitForCompletion, bool.TrueString);
                    context.WithAwsAccount();
                    context.WithStackRole(StackRole);
                    context.WithCloudFormationChangeSets();
                    context.WithPackage(@"Packages\CloudFormationS3.1.0.0.nupkg");
                })
                .Execute();
        }

        void ApplyChangeSet(string changeSetId, string stackId, string bucketName)
        {
            ActionHandlerTestBuilder.Create<AwsApplyCloudFormationChangeSetActionHandler, Calamari.Aws.Program>()
                .WithArrange(context =>
                {
                    context.Variables.Add("BucketName", bucketName);
                    context.Variables.Add("TransformIncludeLocation", TransformIncludeLocation);
                    context.Variables.Add("Octopus.Action.Aws.AssumeRole", bool.FalseString);
                    context.Variables.Add("Octopus.Action.Aws.AssumedRoleArn", string.Empty);
                    context.Variables.Add("Octopus.Action.Aws.AssumedRoleSession", string.Empty);
                    context.Variables.Add("Octopus.Action.Aws.Region", Region);
                    context.Variables.Add("Octopus.Action.Aws.WaitForCompletion", bool.TrueString);
                    context.Variables.Add(AwsSpecialVariables.Action.Aws.CloudFormation.Changesets.Arn, changeSetId);
                    context.Variables.Add(AwsSpecialVariables.Action.Aws.CloudFormation.StackName, stackId);
                    context.WithAwsAccount();
                })
                .Execute();
        }

        (string changeSetId, string stackName) RemoveBucket(string stackName, string bucketName)
        {
            string changeSetId = string.Empty;
            string stackId = string.Empty;
            ActionHandlerTestBuilder.Create<AwsRunCloudFormationActionHandler, Calamari.Aws.Program>()
                .WithArrange(context =>
                {
                    context.Variables.Add("BucketName", bucketName);
                    context.Variables.Add("TransformIncludeLocation", TransformIncludeLocation);
                    context.Variables.Add("Octopus.Action.Aws.AssumeRole", bool.FalseString);
                    context.Variables.Add("Octopus.Action.Aws.AssumedRoleArn", string.Empty);
                    context.Variables.Add("Octopus.Action.Aws.AssumedRoleSession", string.Empty);
                    context.Variables.Add("Octopus.Action.Aws.Region", Region);
                    context.Variables.Add(AwsSpecialVariables.Action.Aws.CloudFormation.StackName, stackName);
                    context.Variables.Add(AwsSpecialVariables.Action.Aws.TemplateSource, AwsSpecialVariables.Action.Aws.TemplateSourceOptions.Package);
                    context.Variables.Add(AwsSpecialVariables.Action.Aws.CloudFormation.Template, "bucket.json");
                    context.Variables.Add(AwsSpecialVariables.Action.Aws.CloudFormation.TemplateParametersRaw, "bucket-parameters.json");
                    context.Variables.Add(AwsSpecialVariables.Action.Aws.WaitForCompletion, bool.TrueString);
                    context.WithAwsAccount();
                    context.WithStackRole(StackRole);
                    context.WithCloudFormationChangeSets(deferExecution: true);
                    context.WithIamCapabilities(new List<string> {"CAPABILITY_IAM"});
                    context.WithPackage(@"Packages\CloudFormationS3.1.0.0.nupkg");
                })
                .WithAssert(result =>
                {
                    result.WasSuccessful.Should().BeTrue();
                    changeSetId = result.OutputVariables["ChangeSetId"].Value;
                    stackId = result.OutputVariables["StackId"].Value;
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
                    context.Variables.Add("Octopus.Action.Aws.AssumeRole", bool.FalseString);
                    context.Variables.Add("Octopus.Action.Aws.AssumedRoleArn", string.Empty);
                    context.Variables.Add("Octopus.Action.Aws.AssumedRoleSession", string.Empty);
                    context.Variables.Add("Octopus.Action.Aws.Region", Region);
                    context.Variables.Add(AwsSpecialVariables.Action.Aws.CloudFormation.StackName, stackName);
                    context.Variables.Add(AwsSpecialVariables.Action.Aws.TemplateSource, AwsSpecialVariables.Action.Aws.TemplateSourceOptions.Package);
                    context.Variables.Add(AwsSpecialVariables.Action.Aws.CloudFormation.Template, "bucket-transform.json");
                    context.Variables.Add(AwsSpecialVariables.Action.Aws.CloudFormation.TemplateParametersRaw, "bucket-parameters.json");
                    context.Variables.Add(AwsSpecialVariables.Action.Aws.WaitForCompletion, bool.TrueString);
                    context.WithAwsAccount();
                    context.WithStackRole(StackRole);
                    context.WithCloudFormationChangeSets();
                    context.WithIamCapabilities(new List<string> {"CAPABILITY_IAM"});
                    context.WithPackage(@"Packages\CloudFormationS3.1.0.0.nupkg");
                })
                .Execute();
        }
    }
}