//////////////////////////////////////////////////////////////////////
// TOOLS
//////////////////////////////////////////////////////////////////////
#module nuget:?package=Cake.DotNetTool.Module&version=0.4.0
#tool "dotnet:?package=AzureSignTool&version=2.0.17"
#tool "dotnet:?package=GitVersion.Tool&version=5.3.6"
#tool "nuget:?package=TeamCity.Dotnet.Integration&version=1.0.10"
#tool "nuget:?package=NuGet.CommandLine&version=5.9.1"
#addin "nuget:?package=SharpZipLib&version=1.2.0"
#addin "nuget:?package=Cake.Compression&version=0.2.4"
#addin "nuget:?package=Cake.Incubator&version=5.0.1"
#addin "nuget:?package=Cake.FileHelpers&version=4.0.1"

using Path = System.IO.Path;
using IO = System.IO;
using Cake.Common.Xml;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
//////////////////////////////////////////////////////////////////////
// ARGUMENTS
//////////////////////////////////////////////////////////////////////
var target = Argument("target", "Default");
var configuration = Argument("configuration", "Release");
var signFiles = Argument<bool>("sign_files", false);
var signingCertificatePath = Argument("signing_certificate_path", "");
var signingCertificatePassword = Argument("signing_certificate_password", "");
var keyVaultUrl = Argument("AzureKeyVaultUrl", "");
var keyVaultAppId = Argument("AzureKeyVaultAppId", "");
var keyVaultAppSecret = Argument("AzureKeyVaultAppSecret", "");
var keyVaultCertificateName = Argument("AzureKeyVaultCertificateName", "");
///////////////////////////////////////////////////////////////////////////////
// GLOBAL VARIABLES
///////////////////////////////////////////////////////////////////////////////
var publishDir = "./publish/";
var artifactsDir = "./artifacts/";
var localPackagesDir = "../LocalPackages";
var workingDir = "./working";
var projectUrl = "https://github.com/OctopusDeploy/Sashimi/";
var signToolPath = MakeAbsolute(File("./certificates/signtool.exe"));
var packagesFeed = "https://f.feedz.io/octopus-deploy/dependencies/nuget/index.json";

string nugetVersion;
GitVersion gitVersionInfo;

// From time to time the timestamping services go offline, let's try a few of them so our builds are more resilient
var timestampUrls = new string[]
{
    "http://timestamp.digicert.com?alg=sha256",
    "http://timestamp.comodoca.com"
};

///////////////////////////////////////////////////////////////////////////////
// SETUP / TEARDOWN
///////////////////////////////////////////////////////////////////////////////
Setup(context =>
{
    gitVersionInfo = GitVersion(new GitVersionSettings {
        OutputType = GitVersionOutput.Json
    });

    if(BuildSystem.IsRunningOnTeamCity)
        BuildSystem.TeamCity.SetBuildNumber(gitVersionInfo.NuGetVersion);

    nugetVersion = gitVersionInfo.NuGetVersion;

    Information("Building Sashimi v{0}", nugetVersion);
    Information("Informational Version {0}", gitVersionInfo.InformationalVersion);
});

Teardown(context =>
{
    Information("Finished running tasks.");
});

//////////////////////////////////////////////////////////////////////
//  PRIVATE TASKS
//////////////////////////////////////////////////////////////////////

Task("Clean")
    .Does(() =>
{
    CleanDirectory(publishDir);
    CleanDirectory(artifactsDir);
    CleanDirectory(workingDir);
    CleanDirectories("./source/**/bin");
    CleanDirectories("./source/**/obj");
});

Task("Build")
    .IsDependentOn("Clean")
    .Does(() =>
{
    DotNetCoreBuild("./source", new DotNetCoreBuildSettings
    {
        Configuration = configuration,
        ArgumentCustomization = args => args.Append($"/p:Version={nugetVersion}")
    });
});

// Task("Test")
//     .IsDependentOn("Build")
//     .Does(() => {
// 		var projects = GetFiles("./source/**/*Tests.csproj");

//         Parallel.ForEach(projects, project => {
//             DotNetCoreTest(project.FullPath, new DotNetCoreTestSettings
// 			{
// 				Configuration = configuration,
// 				NoBuild = true
// 			});
//         });
//     });

Task("PublishCalamariProjects")
    .IsDependentOn("Build")
    .Does(() => {
        var projects = GetFiles("./source/**/Calamari*.csproj");
		foreach(var project in projects)
        {
            var calamariFlavour = XmlPeek(project, "Project/PropertyGroup/AssemblyName") ?? project.GetFilenameWithoutExtension().ToString();

            var frameworks = XmlPeek(project, "Project/PropertyGroup/TargetFrameworks") ??
                XmlPeek(project, "Project/PropertyGroup/TargetFramework");

            foreach(var framework in frameworks.Split(';'))
            {
                void RunPublish(string runtime, string platform) {
                     DotNetCorePublish(project.FullPath, new DotNetCorePublishSettings
		    	    {
		    	    	Configuration = configuration,
                        OutputDirectory = $"{publishDir}/{calamariFlavour}/{platform}",
                        Framework = framework,
                        Runtime = runtime,
						NoRestore = true
		    	    });

                    SignAndTimestampBinaries(project.FullPath);
                }

                if(framework.StartsWith("netcoreapp"))
                {
                    var runtimes = XmlPeek(project, "Project/PropertyGroup/RuntimeIdentifiers").Split(';');
                    foreach(var runtime in runtimes)
                        RunPublish(runtime, runtime);
                }
                else
                {
                    RunPublish(null, "netfx");
                }
            }
            Verbose($"{publishDir}/{calamariFlavour}");
            if (calamariFlavour.EndsWith(".Tests")) {
                TeamCity.PublishArtifacts($"{publishDir}{calamariFlavour}/**/*=>{calamariFlavour}.zip");
            } else {
                ZipCompress($"{publishDir}{calamariFlavour}", $"{artifactsDir}{calamariFlavour}.zip", 1);
            }
        }
});

Task("PublishSashimiTestProjects")
    .IsDependentOn("Build")
    .Does(() => {
        var projects = GetFiles("./source/**/Sashimi*.Tests.csproj");
		foreach(var project in projects)
        {
            var sashimiFlavour = XmlPeek(project, "Project/PropertyGroup/AssemblyName") ?? project.GetFilenameWithoutExtension().ToString();

            DotNetCorePublish(project.FullPath, new DotNetCorePublishSettings
            {
                Configuration = configuration,
                OutputDirectory = $"{publishDir}/{sashimiFlavour}"
            });

            SignAndTimestampBinaries(project.FullPath);

            Verbose($"{publishDir}/{sashimiFlavour}");
            TeamCity.PublishArtifacts($"{publishDir}{sashimiFlavour}/**/*=>{sashimiFlavour}.zip");
        }
});

Task("PackSashimi")
    .IsDependentOn("PublishSashimiTestProjects")
    .IsDependentOn("PublishCalamariProjects")
    .Does(() =>
{
    SignAndPack("source", new DotNetCorePackSettings {
        Configuration = configuration,
        OutputDirectory = artifactsDir,
        NoBuild = false, // Don't change this flag we need it because of https://github.com/dotnet/msbuild/issues/5566
        IncludeSource = false,
        IncludeSymbols = false,
        ArgumentCustomization = args => args.Append($"/p:Version={nugetVersion}")
    });
});

Task("PublishPackageArtifacts")
    .IsDependentOn("PackSashimi")
    .Does(() =>
{
    var packages = GetFiles($"{artifactsDir}*.{nugetVersion}.nupkg");
    foreach (var package in packages)
    {
        TeamCity.PublishArtifacts(package.FullPath);
    }
});

Task("CopyToLocalPackages")
    // .IsDependentOn("Test")
    .IsDependentOn("PublishPackageArtifacts")
    .WithCriteria(BuildSystem.IsLocalBuild)
    .Does(() =>
{
    CreateDirectory(localPackagesDir);
    CopyFiles(Path.Combine(artifactsDir, $"*.{nugetVersion}.nupkg"), localPackagesDir);
});

Task("Publish")
    // .IsDependentOn("Test")
    .IsDependentOn("PublishPackageArtifacts")
    .WithCriteria(BuildSystem.IsRunningOnTeamCity)
    .Does(() =>
{
    var packages = GetFiles($"{artifactsDir}*.{nugetVersion}.nupkg");
    foreach (var package in packages)
    {
        NuGetPush(package, new NuGetPushSettings {
            Source = packagesFeed,
            ApiKey = EnvironmentVariable("FeedzIoApiKey")
        });
    }
});

Task("Default")
    .IsDependentOn("CopyToLocalPackages")
    .IsDependentOn("Publish");

private void SignAndPack(string project, DotNetCorePackSettings dotNetCorePackSettings){
    Information("SignAndPack project: " + project);

    var binariesFolder = GetDirectories($"{Path.GetDirectoryName(project)}/bin/{configuration}/*");

    foreach(var directory in binariesFolder){
        SignAndTimestampBinaries(directory.ToString());
    }
    
    DotNetCorePack(project, dotNetCorePackSettings);
}

void SignAndTimestampBinaries(string outputDirectory)
{
    // When building locally signing isn't really necessary and it could take up to 3-4 minutes to sign all the binaries
    // as we build for many, many different runtimes so disabling it locally means quicker turn around when doing local development.
    // if (BuildSystem.IsLocalBuild && !signFiles) return;

    Information($"Signing binaries in {outputDirectory}");

    // check that any unsigned libraries, that Octopus Deploy authors, get signed to play nice with security scanning tools
    // refer: https://octopusdeploy.slack.com/archives/C0K9DNQG5/p1551655877004400
    // decision re: no signing everything: https://octopusdeploy.slack.com/archives/C0K9DNQG5/p1557938890227100
     var unsignedExecutablesAndLibraries =
         GetFiles(
            outputDirectory + "/Calamari*.exe",
            outputDirectory + "/Calamari*.dll",
            outputDirectory + "/Sashimi*.exe",
            outputDirectory + "/Sashimi*.dll")
         .Where(f => !HasAuthenticodeSignature(f))
         .ToArray();

    if (String.IsNullOrEmpty(keyVaultUrl) && String.IsNullOrEmpty(keyVaultAppId) && String.IsNullOrEmpty(keyVaultAppSecret) && String.IsNullOrEmpty(keyVaultCertificateName))
    {
      Information("Signing files using signtool and the self-signed development code signing certificate.");
      SignFilesWithSignTool(unsignedExecutablesAndLibraries, signingCertificatePath, signingCertificatePassword);
    }
    else
    {
      Information("Signing files using azuresigntool and the production code signing certificate");
      SignFilesWithAzureSignTool(unsignedExecutablesAndLibraries, keyVaultUrl, keyVaultAppId, keyVaultAppSecret, keyVaultCertificateName);
    }
    TimeStampFiles(unsignedExecutablesAndLibraries);
}

bool HasAuthenticodeSignature(FilePath filePath)
{
    try
    {
        X509Certificate.CreateFromSignedFile(filePath.FullPath);
        return true;
    }
    catch
    {
        return false;
    }
}

void SignFilesWithAzureSignTool(IEnumerable<FilePath> files, string vaultUrl, string vaultAppId, string vaultAppSecret, string vaultCertificateName, string display = "", string displayUrl = "")
{
    var signArguments = new ProcessArgumentBuilder()
        .Append("sign")
        .Append("--azure-key-vault-url").AppendQuoted(vaultUrl)
        .Append("--azure-key-vault-client-id").AppendQuoted(vaultAppId)
        .Append("--azure-key-vault-client-secret").AppendQuotedSecret(vaultAppSecret)
        .Append("--azure-key-vault-certificate").AppendQuoted(vaultCertificateName)
        .Append("--file-digest sha256");

    if (!string.IsNullOrWhiteSpace(display))
    {
        signArguments
        .Append("--description").AppendQuoted(display)
        .Append("--description-url").AppendQuoted(displayUrl);
    }

    foreach (var file in files)
        signArguments.AppendQuoted(file.FullPath);

        var azureSignToolPath = MakeAbsolute(File("./tools/azuresigntool.exe"));

        if (!FileExists(azureSignToolPath))
            throw new Exception($"The azure signing tool was expected to be at the path '{azureSignToolPath}' but wasn't available.");

    Information($"Executing: {azureSignToolPath} {signArguments.RenderSafe()}");
    var exitCode = StartProcess(azureSignToolPath.FullPath, signArguments.Render());
        if (exitCode != 0)
            throw new Exception($"AzureSignTool failed with the exit code {exitCode}.");

    Information($"Finished signing {files.Count()} files.");
}

void SignFilesWithSignTool(IEnumerable<FilePath> files, FilePath certificatePath, string certificatePassword, string display = "", string displayUrl = "")
{
    if (!FileExists(signToolPath))
        throw new Exception($"The signing tool was expected to be at the path '{signToolPath}' but wasn't available.");

    if (!FileExists(certificatePath))
        throw new Exception($"The code-signing certificate was not found at {certificatePath}.");

    Information($"Signing {files.Count()} files using certificate at '{certificatePath}'...");

    var signArguments = new ProcessArgumentBuilder()
        .Append("sign")
        .Append("/fd SHA256")
        .Append("/f").AppendQuoted(certificatePath.FullPath)
        .Append($"/p").AppendQuotedSecret(certificatePassword);

    if (!string.IsNullOrWhiteSpace(display))
    {
        signArguments
            .Append("/d").AppendQuoted(display)
            .Append("/du").AppendQuoted(displayUrl);
    }

    foreach (var file in files)
    {
        signArguments.AppendQuoted(file.FullPath);
    }

    Information($"Executing: {signToolPath} {signArguments.RenderSafe()}");
    var exitCode = StartProcess(signToolPath, new ProcessSettings
    {
        Arguments = signArguments
    });

    if (exitCode != 0)
    {
        throw new Exception($"Signing files failed with the exit code {exitCode}. Look for 'SignTool Error' in the logs.");
    }

    Information($"Finished signing {files.Count()} files.");
}

void TimeStampFiles(IEnumerable<FilePath> files)
{
    if (!FileExists(signToolPath))
    {
        throw new Exception($"The signing tool was expected to be at the path '{signToolPath}' but wasn't available.");
    }

    Information($"Timestamping {files.Count()} files...");

    var timestamped = false;
    foreach (var url in timestampUrls)
    {
        var timestampArguments = new ProcessArgumentBuilder()
            .Append($"timestamp")
            .Append("/tr").AppendQuoted(url)
            .Append("/td").Append("sha256");
            
        foreach (var file in files)
        {
            timestampArguments.AppendQuoted(file.FullPath);
        }

        try
        {
            Information($"Executing: {signToolPath} {timestampArguments.RenderSafe()}");
            var exitCode = StartProcess(signToolPath, new ProcessSettings
            {
                Arguments = timestampArguments
            });

            if (exitCode == 0)
            {
                timestamped = true;
                break;
            }
            else
            {
                throw new Exception($"Timestamping files failed with the exit code {exitCode}. Look for 'SignTool Error' in the logs.");
            }
        }
        catch (Exception ex)
        {
            Warning(ex.Message);
            Warning($"Failed to timestamp files using {url}. Maybe we can try another timestamp service...");
        }
    }

    if (!timestamped)
    {
        throw new Exception($"Failed to timestamp files even after we tried all of the timestamp services we use.");
    }

    Information($"Finished timestamping {files.Count()} files.");
}

//////////////////////////////////////////////////////////////////////
// EXECUTION
//////////////////////////////////////////////////////////////////////
RunTarget(target);
