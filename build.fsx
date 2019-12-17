#r "paket: groupref build //"
#load ".fake/build.fsx/intellisense.fsx"

open Fake.Core
open Fake.DotNet
open Fake.DotNet.NuGet.Restore
open Fake.IO
open Fake.IO.FileSystemOperators
open Fake.IO.Globbing.Operators
open Fake.Core.TargetOperators
open Fake.DotNet.Testing

let version = "1.0.0"
let solutionFile = "./src/MemDumpHost.sln"
let buildMode =
    DotNet.BuildConfiguration.fromString
        (Environment.environVarOrDefault "buildMode" "Release")

Target.create "Clean"
    (fun _ ->
    !!"./src/**/bin" ++ "./src/**/obj" ++ "./artifacts" |> Shell.cleanDirs)
Target.create "AssemblyInfo" (fun _ ->
    let getAssemblyInfoAttributes =
        [ AssemblyInfo.Version version
          AssemblyInfo.FileVersion version ]

    let getProjectDetails projectPath =
        let projectName =
            System.IO.Path.GetFileNameWithoutExtension(projectPath)
        (projectPath, projectName, System.IO.Path.GetDirectoryName(projectPath),
         getAssemblyInfoAttributes)

    !!"src/**/*.csproj"
    |> Seq.map getProjectDetails
    |> Seq.iter (fun (projFileName, _, folderName, attributes) ->
           match projFileName with
           | proj when proj.EndsWith("csproj") ->
               AssemblyInfoFile.createCSharp (folderName </> "AssemblyInfo.cs")
                   attributes
           | _ -> ()))
Target.create "AnnounceVersion"
    (fun _ ->
    Trace.log (sprintf "##teamcity[buildNumber '%s-{build.number}']" version))
Target.create "Build"
    (fun _ ->
    solutionFile
    |> DotNet.build (fun opts -> { opts with Configuration = buildMode }))
Target.create "Test" (fun _ ->
    solutionFile
    |> DotNet.test (fun opts ->
           { opts with Configuration = buildMode
                       NoBuild = true
                       Blame = true }))
Target.create "Pack" (fun _ ->
    let args =
        { MSBuild.CliArguments.Create() with NoLogo = true
                                             Properties =
                                                 [ "PackageVersion", version ] }
    solutionFile
    |> DotNet.pack (fun opts ->
           { opts with Configuration = buildMode
                       MSBuildParams = args
                       NoBuild = true
                       OutputPath = Some "./artifacts" }))
Target.create "All" ignore
"Clean" ==> "AssemblyInfo" ==> "AnnounceVersion" ==> "Build" ==> "Test"
==> "Pack" ==> "All"
Target.runOrDefault "All"
