using System;
using System.Collections.Generic;
using System.IO;
using Calamari.Tests.Shared;
using FluentAssertions;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using NuGet.Protocol;
using NUnit.Framework;
using Sashimi.Aws.ActionHandler;
using Sashimi.Aws.Validation;
using Sashimi.Server.Contracts;
using Sashimi.Tests.Shared;
using Sashimi.Tests.Shared.Server;

namespace Sashimi.Aws.Tests.RunScript
{
    // TODO : move to e2e project, maybe
    public class AwsS3UploadActionHandlerFixture : BaseTest
    {
        [Test]
        public void AwsS3Upload()
        {
            var bucketName = "octopus-e2e-tests";
            var region = "ap-southeast-1";
            var folderPrefix = $"test/{Guid.NewGuid().ToString()}/";
            var path = TestEnvironment.GetTestPath(@"AwsS3Sample\AwsS3Sample.1.0.0.nupkg");

            TestActionHandler<AwsUploadS3ActionHandler, Calamari.Aws.Program>(context =>
            {
                context.Variables.Add(KnownVariables.Action.Packages.PackageId, path);
                context.Variables.Add(KnownVariables.Action.Packages.FeedId, "FeedId");
                // variables
                context.Variables.Add("Octopus.Action.AwsAccount.Variable", "AWSAccount");
                context.Variables.Add("Octopus.Action.Amazon.AccessKey", ExternalVariables.Get(ExternalVariable.AwsAcessKey));
                context.Variables.Add("Octopus.Action.Amazon.SecretKey", ExternalVariables.Get(ExternalVariable.AwsSecretKey));
                context.Variables.Add("Octopus.Action.Aws.AssumeRole", "False");
                context.Variables.Add("Octopus.Action.Aws.AssumedRoleArn", "");
                context.Variables.Add("Octopus.Action.Aws.AssumedRoleSession", "");
                context.Variables.Add("Octopus.Action.Aws.Region", region);
                context.Variables.Add(AwsSpecialVariables.Action.Aws.S3.BucketName, bucketName);
                context.Variables.Add(AwsSpecialVariables.Action.Aws.S3.TargetMode, "EntirePackage");
                context.Variables.Add(
                    AwsSpecialVariables.Action.Aws.S3.PackageOptions,
                    JsonConvert.SerializeObject(new S3PackageProperties
                    {
                        BucketKey = $"{folderPrefix}AwsS3Sample.nupkg",
                        CannedAcl = "private",
                        Metadata = {new KeyValuePair<string, string>("TestKey", "TestValue")},
                        Tags = {new KeyValuePair<string, string>("TestTag", "TestTagValue")},
                        StorageClass = "STANDARD",
                        BucketKeyBehaviour = BucketKeyBehaviourType.Custom
                    }, new JsonSerializerSettings {ContractResolver = new CamelCasePropertyNamesContractResolver()})
                );

                // package refs point to folders in the project 
            },
                result =>
                {
                    
                    result.WasSuccessful.Should().BeTrue();
                });
        }
    }
}