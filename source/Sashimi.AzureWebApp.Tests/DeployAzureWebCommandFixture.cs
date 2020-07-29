﻿using System;
using System.IO;
using System.Net.Http;
using System.Threading.Tasks;
using Calamari.AzureWebApp;
using Calamari.Common.Plumbing.FileSystem;
using Calamari.Tests.Shared;
using FluentAssertions;
using Microsoft.Azure.Management.AppService.Fluent;
using Microsoft.Azure.Management.Fluent;
using Microsoft.Azure.Management.ResourceManager.Fluent;
using Microsoft.Azure.Management.ResourceManager.Fluent.Core;
using NUnit.Framework;
using Sashimi.Tests.Shared.Server;
using OperatingSystem = Microsoft.Azure.Management.AppService.Fluent.OperatingSystem;

namespace Sashimi.AzureWebApp.Tests
{
    [TestFixture]
    class DeployAzureWebCommandFixture
    {
        IAppServicePlan appServicePlan;
        IResourceGroup resourceGroup;
        IAzure azure;
        string clientId;
        string clientSecret;
        string tenantId;
        string subscriptionId;

        readonly HttpClient client = new HttpClient();

        [OneTimeSetUp]
        public async Task Setup()
        {
            clientId = ExternalVariables.Get(ExternalVariable.AzureSubscriptionClientId);
            clientSecret = ExternalVariables.Get(ExternalVariable.AzureSubscriptionPassword);
            tenantId = ExternalVariables.Get(ExternalVariable.AzureSubscriptionTenantId);
            subscriptionId = ExternalVariables.Get(ExternalVariable.AzureSubscriptionId);
            var resourceGroupName = SdkContext.RandomResourceName(nameof(DeployAzureWebCommandFixture), 60);

            var credentials = SdkContext.AzureCredentialsFactory.FromServicePrincipal(clientId, clientSecret, tenantId,
                AzureEnvironment.AzureGlobalCloud);

            azure = Microsoft.Azure.Management.Fluent.Azure
                             .Configure()
                             .WithLogLevel(HttpLoggingDelegatingHandler.Level.Basic)
                             .Authenticate(credentials)
                             .WithSubscription(subscriptionId);

            resourceGroup = await azure.ResourceGroups
                .Define(resourceGroupName)
                .WithRegion(Region.USWest)
                .CreateAsync();

            appServicePlan = await azure.AppServices.AppServicePlans
                .Define(SdkContext.RandomResourceName(nameof(DeployAzureWebCommandFixture), 60))
                .WithRegion(resourceGroup.Region)
                .WithExistingResourceGroup(resourceGroup)
                .WithPricingTier(PricingTier.StandardS1)
                .WithOperatingSystem(OperatingSystem.Windows)
                .CreateAsync();
        }

        [OneTimeTearDown]
        public async Task Cleanup()
        {
            if (resourceGroup != null)
            {
                await azure.ResourceGroups.DeleteByNameAsync(resourceGroup.Name);
            }
        }

        [Test]
        public async Task Deploy_WebApp_Ensure_Tools_Are_Configured()
        {
            var webAppName = SdkContext.RandomResourceName(nameof(DeployAzureWebCommandFixture), 60);

            var webApp = await CreateWebApp(webAppName);

            using var tempPath = TemporaryDirectory.Create();
            const string actualText = "Hello World";

            await File.WriteAllTextAsync(Path.Combine(tempPath.DirectoryPath, "index.html"), actualText);
            await File.WriteAllTextAsync(Path.Combine(tempPath.DirectoryPath, "PreDeploy.ps1"), @"
az --version
Get-AzureEnvironment");

            Environment.SetEnvironmentVariable("Test_Calamari_InProc_OutProc_Override", "OutProc");

            ActionHandlerTestBuilder.CreateAsync<AzureWebAppActionHandler, Program>()
                                    .WithArrange(context =>
                                                 {
                                                     AddDefaults(context, webAppName);

                                                     context.WithFilesToCopy(tempPath.DirectoryPath);
                                                 })
                                    .Execute();

            Environment.SetEnvironmentVariable("Test_Calamari_InProc_OutProc_Override", "InProc");
            await AssertContent(webApp.DefaultHostName, actualText);
        }

        Task<IWebApp> CreateWebApp(string webAppName)
        {
            return azure.WebApps
                        .Define(webAppName)
                        .WithExistingWindowsPlan(appServicePlan)
                        .WithExistingResourceGroup(resourceGroup)
                        .WithRuntimeStack(WebAppRuntimeStack.NETCore)
                        .CreateAsync();
        }

        async Task AssertContent(string hostName, string actualText, string rootPath = null)
        {
            var result= await client.GetStringAsync($"https://{hostName}/{rootPath}");

            result.Should().Be(actualText);
        }

        void AddDefaults(TestActionHandlerContext<Program> context, string webAppName)
        {
            context.Variables.Add(AzureAccountVariables.SubscriptionId,
                                  subscriptionId);
            context.Variables.Add(AzureAccountVariables.TenantId,
                                  tenantId);
            context.Variables.Add(AzureAccountVariables.ClientId,
                                  clientId);
            context.Variables.Add(AzureAccountVariables.Password,
                                  clientSecret);
            context.Variables.Add(SpecialVariables.Action.Azure.WebAppName, webAppName);
            context.Variables.Add(SpecialVariables.Action.Azure.ResourceGroupName, resourceGroup.Name);
        }
    }
}