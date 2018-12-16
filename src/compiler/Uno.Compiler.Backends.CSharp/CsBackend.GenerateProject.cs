using System;
using System.Linq;
using Uno.IO;

namespace Uno.Compiler.Backends.CSharp
{
    public partial class CsBackend
    {
        const string SolutionGuid = "9A3B76BE-04CA-42B8-857C-FD2BB1832788";
        const string ProjectGuid = "BE6FB7C9-4F00-4A1B-BEBD-59869A4AF387";

        void GenerateProject()
        {
            var projFilename = "Uno.Runtime.Core.csproj";
            var slnFilename = "Uno.Runtime.Core.sln";

            using (var w = Disk.CreateBufferedText(Environment.Combine(projFilename), NewLine.CrLf))
            {
                w.WriteLine("\uFEFF<?xml version=\"1.0\" encoding=\"utf-8\"?>");
                w.WriteLine(@"<Project ToolsVersion=""14.0"" DefaultTargets=""Build"" xmlns=""http://schemas.microsoft.com/developer/msbuild/2003"">");
                w.WriteLine(@"  <PropertyGroup>");
                w.WriteLine(@"    <Configuration Condition="" '$(Configuration)' == '' "">Debug</Configuration>");
                w.WriteLine(@"    <Platform Condition="" '$(Platform)' == '' "">AnyCPU</Platform>");
                w.WriteLine(@"    <ProjectGuid>{" + ProjectGuid + "}</ProjectGuid>");
                w.WriteLine(@"    <OutputType>Library</OutputType>");
                w.WriteLine(@"    <AppDesignerFolder>Properties</AppDesignerFolder>");
                w.WriteLine(@"    <RootNamespace></RootNamespace>");
                w.WriteLine(@"    <AssemblyName>Uno.Runtime.Core</AssemblyName>");
                w.WriteLine(@"    <TargetFrameworkVersion>v4.5</TargetFrameworkVersion>");
                w.WriteLine(@"    <FileAlignment>512</FileAlignment>");
                w.WriteLine(@"  </PropertyGroup>");
                w.WriteLine(@"  <PropertyGroup Condition="" '$(Configuration)|$(Platform)' == 'Debug|AnyCPU' "">");
                w.WriteLine(@"    <DebugSymbols>true</DebugSymbols>");
                w.WriteLine(@"    <DebugType>full</DebugType>");
                w.WriteLine(@"    <Optimize>false</Optimize>");
                w.WriteLine(@"    <OutputPath>bin\Debug\</OutputPath>");
                w.WriteLine(@"    <DefineConstants>DEBUG;TRACE</DefineConstants>");
                w.WriteLine(@"    <ErrorReport>prompt</ErrorReport>");
                w.WriteLine(@"    <WarningLevel>4</WarningLevel>");
                w.WriteLine(@"    <AllowUnsafeBlocks>true</AllowUnsafeBlocks>");
                w.WriteLine(@"    <NoWarn>CS0067</NoWarn>");
                w.WriteLine(@"  </PropertyGroup>");
                w.WriteLine(@"  <PropertyGroup Condition="" '$(Configuration)|$(Platform)' == 'Release|AnyCPU' "">");
                w.WriteLine(@"    <DebugType>pdbonly</DebugType>");
                w.WriteLine(@"    <Optimize>true</Optimize>");
                w.WriteLine(@"    <OutputPath>bin\Release\</OutputPath>");
                w.WriteLine(@"    <DefineConstants>TRACE</DefineConstants>");
                w.WriteLine(@"    <ErrorReport>prompt</ErrorReport>");
                w.WriteLine(@"    <WarningLevel>4</WarningLevel>");
                w.WriteLine(@"    <AllowUnsafeBlocks>true</AllowUnsafeBlocks>");
                w.WriteLine(@"    <NoWarn>CS0067</NoWarn>");
                w.WriteLine(@"  </PropertyGroup>");
                w.WriteLine(@"  <ItemGroup>");
                w.WriteLine(@"    <Reference Include=""System"" />");
                w.WriteLine(@"    <Reference Include=""System.Core"" />");
                w.WriteLine(@"    <Reference Include=""Microsoft.CSharp"" />");
                w.WriteLine(@"  </ItemGroup>");
                w.WriteLine(@"  <Import Project=""..\..\GlobalAssemblyInfo.targets"" Condition=""Exists('..\..\GlobalAssemblyInfo.targets')"" />");
                w.WriteLine(@"  <ItemGroup>");

                var sourceFiles = SourceFiles.Select(x => x.Replace('/', '\\')).ToList();
                sourceFiles.Sort(StringComparer.InvariantCulture);

                foreach (var f in sourceFiles)
                    w.WriteLine("    <Compile Include=\"" + f + "\" />");

                w.WriteLine(@"  </ItemGroup>");
                w.WriteLine("  <Import Project=\"$(MSBuildToolsPath)\\Microsoft.CSharp.targets\" />");
                w.Write("</Project>");
            }

            using (var w = Disk.CreateBufferedText(Environment.Combine(slnFilename), NewLine.CrLf))
            {
                w.WriteLine("Microsoft Visual Studio Solution File, Format Version 12.00");
                w.WriteLine("# Visual Studio 2013");
                w.WriteLine("Project(\"{" + SolutionGuid + "}\") = \"" + Environment.ExpandSingleLine("@(Project.Name)") + "\", \"" + projFilename + "\", \"{" + ProjectGuid + "}\"");

                w.WriteLine("EndProject");
                w.WriteLine("Global");
                w.WriteLine("    GlobalSection(SolutionConfigurationPlatforms) = preSolution");
                w.WriteLine("        Debug|Any CPU = Debug|Any CPU");
                w.WriteLine("        Release|Any CPU = Release|Any CPU");
                w.WriteLine("    EndGlobalSection");
                w.WriteLine("    GlobalSection(ProjectConfigurationPlatforms) = postSolution");
                w.WriteLine("        {" + ProjectGuid + "}.Debug|Any CPU.ActiveCfg = Debug|Any CPU");
                w.WriteLine("        {" + ProjectGuid + "}.Debug|Any CPU.Build.0 = Debug|Any CPU");
                w.WriteLine("        {" + ProjectGuid + "}.Release|Any CPU.ActiveCfg = Release|Any CPU");
                w.WriteLine("        {" + ProjectGuid + "}.Release|Any CPU.Build.0 = Release|Any CPU");
                w.WriteLine("    EndGlobalSection");
                w.WriteLine("    GlobalSection(SolutionProperties) = preSolution");
                w.WriteLine("        HideSolutionNode = FALSE");
                w.WriteLine("    EndGlobalSection");
                w.WriteLine("EndGlobal");
            }
        }
    }
}
