using System;
using System.Linq;
using Nuke.Common;
using Nuke.Common.CI;
using Nuke.Common.CI.GitHubActions;
using Nuke.Common.Execution;
using Nuke.Common.IO;
using Nuke.Common.ProjectModel;
using Nuke.Common.Tooling;
using Nuke.Common.Tools.DotNet;
using Nuke.Common.Utilities.Collections;
using Octokit;
using static Nuke.Common.EnvironmentInfo;
using static Nuke.Common.IO.FileSystemTasks;
using static Nuke.Common.IO.PathConstruction;

[GitHubActions(
    "continuous",
    GitHubActionsImage.UbuntuLatest,
    On = [GitHubActionsTrigger.Push],
    InvokedTargets = [nameof(Compile)])]
[GitHubActions(
    "tagPush",
    GitHubActionsImage.UbuntuLatest,
    OnPushTags = ["v*"],
    ImportSecrets = [nameof(NuGetApiKey)],
    InvokedTargets = [nameof(PushNugetPackage)])]
class Build : NukeBuild
{
    public static int Main() => Execute<Build>(x => x.Compile);

    [Parameter("Configuration to build - Default is 'Debug' (local) or 'Release' (server)")]
    readonly Configuration Configuration = IsLocalBuild ? Configuration.Debug : Configuration.Release;

    [Parameter("The version to use for the build")] string Version = "0.42.0";
    [Parameter("The version suffix that should be appended to the version")] readonly string VersionSuffix = "";

    [Solution] readonly Solution Solution;

    [Parameter] [Secret] readonly string NuGetApiKey;

    GitHubActions GitHubActions = GitHubActions.Instance;
    
    const string ProjectName = "AskForPermission";
    readonly AbsolutePath PackagesDirectory = RootDirectory / "PackageDirectory";
    readonly AbsolutePath SourceDirectory = RootDirectory / "src";

    Target Compile => t => t
        .Executes(() =>
        {
            DotNetTasks.DotNetBuild(s => s
                .SetProjectFile(Solution.GetProject(ProjectName))
                .SetConfiguration(Configuration)
            );
        });

    Target Pack => t => t
        .DependsOn(Compile)
        .Produces(PackagesDirectory / "*.nupkg")
        .Executes(() =>
        {
            PackagesDirectory.DeleteDirectory();
            Version = string.IsNullOrEmpty(GitHubActions?.RefName) ? Version:  GitHubActions.RefName.TrimStart('v');
            DotNetTasks.DotNetPack(s => s
                .SetProject(SourceDirectory / ProjectName)
                .SetVersionPrefix(Version)
                .SetVersionSuffix(VersionSuffix)
                .SetOutputDirectory(PackagesDirectory)
            );
        });

    Target PushNugetPackage => t => t
        .DependsOn(Pack)
        .Executes(() =>
        {
            DotNetTasks.DotNetNuGetPush(s => s
                .SetTargetPath(PackagesDirectory / "*.nupkg")
                .SetApiKey(NuGetApiKey)
                .SetSource("https://www.nuget.org/"));
        });
}