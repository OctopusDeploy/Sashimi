﻿using System.Collections.Generic;
using System.IO;
using Calamari.Azure.CloudServices.Deployment.Conventions;
using Calamari.Commands.Support;
using Calamari.Common.Variables;
using Calamari.Deployment;
using Calamari.Deployment.Conventions;
using Calamari.Integration.FileSystem;
using Calamari.Integration.Processes;

namespace Calamari.Azure.CloudServices
{
    [Command("extract-cspkg", Description = "Extracts an Azure cloud-service package (.cspkg)")]
    public class ExtractAzureCloudServicePackageCommand : ICommand
    {
        readonly ILog log;
        readonly IVariables variables;
        string packageFile;
        string destinationDirectory;

        public ExtractAzureCloudServicePackageCommand(ILog log, IVariables variables)
        {
            this.log = log;
            this.variables = variables;
        }

        public int Execute()
        {
            packageFile = variables.Get(AzureSpecialVariables.CloudServicePackagePath);

            if (!File.Exists(packageFile))
            {
                throw new CommandException("Could not find package file: " + packageFile);
            }    
            
            destinationDirectory = variables.Get(KnownVariables.OriginalPackageDirectoryPath, Path.GetDirectoryName(packageFile));

            var fileSystem = new WindowsPhysicalFileSystem();

            var conventions = new List<IConvention>
            {
                new EnsureCloudServicePackageIsCtpFormatConvention(fileSystem),
                new ExtractAzureCloudServicePackageConvention(log, fileSystem),
            };

            var deployment = new RunningDeployment(packageFile, variables);
            var conventionRunner = new ConventionProcessor(deployment, conventions);
            deployment.

            return 0;
        }
    }
}