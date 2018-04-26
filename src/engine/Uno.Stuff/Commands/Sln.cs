using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Stuff.Format;
using Mono.Options;

namespace Stuff.Commands
{
    class Sln : Command
    {
        public override string Name        => "sln";
        public override string Description => "Generates C# .SLN file(s)";

        public override void Help(IEnumerable<string> args)
        {
            WriteUsage("[directory|file|glob ...]");
            WriteLine("By default this will generate all .STUFF-SLN files found in current directory.");

            WriteHead("Available options");
            WriteRow("-a, --all",             "Expand all if/else blocks -- everything is true");
            WriteRow("-f, --force",           "Don't early out if up-to-date, replace solution");
            WriteRow("-o, --out=PATH",        "Specify output filename for .SLN", true);
            WriteRow("-b, --build=INT",       "Specify build number for AssemblyVersion", true);
            WriteRow("-c, --commit=STRING",   "Specify commit number for AssemblyConfiguration", true);
            WriteRow("-n, --version=STRING",  "Specify version number for AssemblyVersion", true);
            WriteRow("-D, --define=STRING",   "Add define");
            WriteRow("-U, --undefine=STRING", "Remove define");
            WriteDefines();
        }

        public override void Execute(IEnumerable<string> args)
        {
            var flags = StuffFlags.None;
            var defines = new List<string>(StuffFile.DefaultDefines);
            string outName = null;
            string buildNumber = null;
            string commitNumber = null;
            string version = null;
            var files = new OptionSet
                {
                    { "a|all", x => flags |= StuffFlags.AcceptAll },
                    { "f|force", x => flags |= StuffFlags.Force },
                    { "o=|out=", x => outName = x },
                    { "b=|build=", x => buildNumber = x },
                    { "c=|commit=", x => commitNumber = x },
                    { "n=|version=", x => version = x },
                    { "D=", defines.Add },
                    { "U=", x => defines.Remove(x) }
                }
                .Parse(args)
                .GetFiles("*.stuff-sln");

            foreach (var file in files)
                new Generator()
                    .Generate(
                        file,
                        flags,
                        defines,
                        outName,
                        buildNumber,
                        commitNumber,
                        version);
        }

        class Generator
        {
            readonly Dictionary<string, string> _assemblyInfo = new Dictionary<string, string>();
            readonly Dictionary<string, string> _nestedProjects = new Dictionary<string, string>();
            readonly Dictionary<string, string> _solutionFolders = new Dictionary<string, string>();
            readonly List<string> _projectDirectories = new List<string>();
            readonly List<string> _projectGuids = new List<string>();
            readonly List<string> _projectNames = new List<string>();
            readonly StringBuilder _sln = new StringBuilder();
            string _relativeHead;
            byte _guids;

            public void Generate(string filename, StuffFlags flags, IEnumerable<string> defines,
                string outName = null, string buildNumber = null, string commitNumber = null, string version = null)
            {
                filename = Path.GetFullPath(filename);
                outName = outName != null
                    ? Path.GetFullPath(outName)
                    : Path.Combine(
                        Path.GetDirectoryName(filename),
                        Path.GetFileNameWithoutExtension(filename) + ".sln");
                _relativeHead = Path.GetDirectoryName(outName) + Path.DirectorySeparatorChar;

                var versionFile = Path.Combine(
                    Path.GetDirectoryName(outName),
                    ".stuff",
                    Path.GetFileNameWithoutExtension(outName),
                    "version");
                var versionValue = commitNumber + "." + buildNumber + "." + version;

                try
                {
                    if (!flags.HasFlag(StuffFlags.Force) &&
                        File.Exists(outName) &&
                        File.Exists(versionFile) &&
                        File.GetLastWriteTimeUtc(filename) <=
                            File.GetLastWriteTimeUtc(versionFile) &&
                        File.ReadAllText(versionFile) == versionValue)
                    {
                        Log.Verbose(filename.Relative() + ": Up-to-date");
                        return;
                    }
                }
                catch (Exception e)
                {
                    Log.Warning(filename.Relative() + ": " + e.Message);
                }

                Emit(versionFile, versionValue, true);
                EmitSolution(filename, outName, flags, defines);
                EmitAssemblyInfo(filename, buildNumber, commitNumber, version);
            }

            void EmitSolution(string filename, string outName, StuffFlags flags, IEnumerable<string> defines)
            {
                _sln.AppendLine();
                _sln.AppendLine("Microsoft Visual Studio Solution File, Format Version 12.00");
                _sln.AppendLine("# Visual Studio 14");
                _sln.AppendLine("VisualStudioVersion = 14.0.24720.0");
                _sln.AppendLine("MinimumVisualStudioVersion = 10.0.40219.1");

                LoadStuff(filename, flags, defines);

                _sln.AppendLine("Global");
                _sln.AppendLine("\tGlobalSection(SolutionConfigurationPlatforms) = preSolution");
                _sln.AppendLine("\t\tDebug|Any CPU = Debug|Any CPU");
                _sln.AppendLine("\t\tRelease|Any CPU = Release|Any CPU");
                _sln.AppendLine("\tEndGlobalSection");
                _sln.AppendLine("\tGlobalSection(ProjectConfigurationPlatforms) = postSolution");

                foreach (var p in _projectGuids)
                {
                    _sln.AppendLine("\t\t" + p + ".Debug|Any CPU.ActiveCfg = Debug|Any CPU");
                    _sln.AppendLine("\t\t" + p + ".Debug|Any CPU.Build.0 = Debug|Any CPU");
                    _sln.AppendLine("\t\t" + p + ".Release|Any CPU.ActiveCfg = Release|Any CPU");
                    _sln.AppendLine("\t\t" + p + ".Release|Any CPU.Build.0 = Release|Any CPU");
                }

                _sln.AppendLine("\tEndGlobalSection");
                _sln.AppendLine("\tGlobalSection(SolutionProperties) = preSolution");
                _sln.AppendLine("\t\tHideSolutionNode = FALSE");
                _sln.AppendLine("\tEndGlobalSection");
                _sln.AppendLine("\tGlobalSection(NestedProjects) = preSolution");

                foreach (var e in _nestedProjects)
                    _sln.AppendLine("\t\t" + e.Key + " = " + e.Value);

                _sln.AppendLine("\tEndGlobalSection");
                _sln.AppendLine("EndGlobal");

                Emit(outName, _sln.ToString());
            }

            void EmitAssemblyInfo(string filename, string buildNumber, string commitNumber, string version)
            {
                if (_assemblyInfo.Count == 0)
                    return;

                string title, description, copyright, company, product, trademark, culture;
                _assemblyInfo.TryGetValue("Title", out title);
                _assemblyInfo.TryGetValue("Description", out description);
                _assemblyInfo.TryGetValue("Copyright", out copyright);
                _assemblyInfo.TryGetValue("Company", out company);
                _assemblyInfo.TryGetValue("Product", out product);
                _assemblyInfo.TryGetValue("Trademark", out trademark);
                _assemblyInfo.TryGetValue("Culture", out culture);

                if (string.IsNullOrEmpty(version))
                    _assemblyInfo.TryGetValue("Version", out version);

                if (product == null)
                    product = Path.GetFileNameWithoutExtension(filename);
                if (company == null)
                    company = "(the company)";
                if (copyright == null)
                    copyright = "Copyright © " + DateTime.Now.Year + " " + company;
                if (version == null)
                    version = "1.0.0";

                // Format version numbers
                var parsedVersion = ParseVersion(version);
                var fileVersion = Math.Max(parsedVersion.Major, 0) + "." +
                                  Math.Max(parsedVersion.Minor, 0) + "." +
                                  Math.Max(parsedVersion.Build, 0) + "." +
                                  (buildNumber ?? "0");

                for (var i = 0; i < _projectDirectories.Count; i++)
                {
                    var dir = _projectDirectories[i];
                    var guid = _projectGuids[i];
                    var info = new StringBuilder();
                    info.AppendLine("using System.Reflection;");
                    info.AppendLine("using System.Runtime.InteropServices;");
                    info.AppendLine();
                    info.AppendLine("// General Information about an assembly is controlled through the following");
                    info.AppendLine("// set of attributes. Change these attribute values to modify the information");
                    info.AppendLine("// associated with an assembly.");
                    info.AppendLine("[assembly: AssemblyTitle(\"" + (title ?? description) + "\")]");
                    info.AppendLine("[assembly: AssemblyDescription(\"" + description + "\")]");
                    info.AppendLine("[assembly: AssemblyConfiguration(\"" + commitNumber + "\")]");
                    info.AppendLine("[assembly: AssemblyCompany(\"" + company + "\")]");
                    info.AppendLine("[assembly: AssemblyProduct(\"" + product + "\")]");
                    info.AppendLine("[assembly: AssemblyCopyright(\"" + copyright + "\")]");
                    info.AppendLine("[assembly: AssemblyTrademark(\"" + trademark + "\")]");
                    info.AppendLine("[assembly: AssemblyCulture(\"" + culture + "\")]");
                    info.AppendLine();
                    info.AppendLine("// Setting ComVisible to false makes the types in this assembly not visible");
                    info.AppendLine("// to COM components.  If you need to access a type in this assembly from");
                    info.AppendLine("// COM, set the ComVisible attribute to true on that type.");
                    info.AppendLine("[assembly: ComVisible(false)]");
                    info.AppendLine();
                    info.AppendLine("// The following GUID is for the ID of the typelib if this project is exposed to COM");
                    info.AppendLine("[assembly: Guid(\"" + guid.Trim('{', '}').ToLowerInvariant() + "\")]");
                    info.AppendLine("// Version information for an assembly consists of the following four values:");
                    info.AppendLine("//");
                    info.AppendLine("//      Major Version");
                    info.AppendLine("//      Minor Version");
                    info.AppendLine("//      Build Number");
                    info.AppendLine("//      Revision");
                    info.AppendLine("//");
                    info.AppendLine("// You can specify all the values or you can default the Build and Revision Numbers");
                    info.AppendLine("// by using the '*' as shown below:");
                    info.AppendLine("// [assembly: AssemblyVersion(\"1.0.* \")]");
                    info.AppendLine("[assembly: AssemblyVersion(\"" + fileVersion + "\")]");
                    info.AppendLine("[assembly: AssemblyFileVersion(\"" + fileVersion + "\")]");
                    info.AppendLine("[assembly: AssemblyInformationalVersion(\"" + version + "\")]");
                    Emit(Path.Combine(dir, "Properties", "AssemblyInfo.cs"), info.ToString());
                }
            }

            Version ParseVersion(string version)
            {
                var str = version;

                if (string.IsNullOrEmpty(str))
                    return new Version();

                // Remove suffix
                var i = str.IndexOf('-');
                if (i != -1)
                {
                    str = str.Substring(0, i);
                    if (string.IsNullOrEmpty(str))
                        return new Version();
                }

                // Check that version string only contains numbers or periods (X.X.X)
                foreach (var c in str)
                    if (!char.IsNumber(c) && c != '.')
                        return new Version();

                try
                {
                    return new Version(str);
                }
                catch
                {
                    Log.Warning("Failed to parse version string {0}", str.Quote());
                    return new Version();
                }
            }

            void Emit(string filename, string text, bool force = false)
            {
                if (!force &&
                    File.Exists(filename) &&
                    File.ReadAllText(filename) == text)
                    return;

                Log.Event(IOEvent.Write, filename);
                Disk.CreateDirectory(Path.GetDirectoryName(filename));
                File.WriteAllText(filename, text);
            }

            void LoadStuff(string filename, StuffFlags flags, IEnumerable<string> defines)
            {
                var solutionDir = Path.GetDirectoryName(filename);

                foreach (var solutionFolder in StuffObject.Load(filename, flags, defines))
                {
                    if (solutionFolder.Key.StartsWith("AssemblyInfo."))
                    {
                        var key = solutionFolder.Key;
                        var property = key.Substring(key.IndexOf('.') + 1);
                        _assemblyInfo[property] = solutionFolder.Value?.ToString();
                        continue;
                    }

                    var folderGuid = GetFolderGuid(solutionFolder.Key);

                    foreach (var projectPath in solutionFolder.Value.Lines()
                            .Select(x => Path.Combine(solutionDir, x)))
                        if (Directory.Exists(projectPath))
                            foreach (var projectFile in Directory.EnumerateFiles(projectPath, "*.csproj"))
                                AddProject(projectFile, folderGuid);
                        else if (File.Exists(projectPath))
                            AddProject(projectPath, folderGuid);
                        else
                            Log.Warning(projectPath.Relative() + ": No C# project(s) found");
                }
            }

            void AddProject(string path, string folderGuid)
            {
                try
                {
                    path = Path.GetFullPath(path);
                    var guid = GetProjectGuid(path);
                    var name = Path.GetFileNameWithoutExtension(path);
                    var relativeName = path;

                    if (relativeName.StartsWith(_relativeHead))
                        relativeName = relativeName.Substring(_relativeHead.Length);

                    _projectGuids.Add(guid);
                    _projectNames.Add(name);
                    _projectDirectories.Add(Path.GetDirectoryName(path));
                    _nestedProjects[guid] = folderGuid;

                    _sln.AppendLine("Project(\"{FAE04EC0-301F-11D3-BF4B-00C04F79EFBC}\") = \"" +
                                    name + "\", \"" + relativeName.Replace('/', '\\') + "\", \"" + guid + "\"");
                    _sln.AppendLine("EndProject");
                    Log.Event(IOEvent.Include, relativeName);
                }
                catch (Exception e)
                {
                    Log.Error(path.Relative() + ": " + e.Message);
                }
            }

            string GetFolderGuid(string path)
            {
                path = path.Replace('/', Path.DirectorySeparatorChar);

                string guid;
                if (!_solutionFolders.TryGetValue(path, out guid))
                {
                    guid = NewGuid();
                    var parent = Path.GetDirectoryName(path);
                    var name = Path.GetFileName(path);

                    if (!string.IsNullOrEmpty(parent))
                        _nestedProjects[guid] = GetFolderGuid(parent);

                    _solutionFolders[path] = guid;
                    _sln.AppendLine("Project(\"{2150E333-8FDC-42A3-9474-1A3956D46DE8}\") = \"" +
                                    name + "\", \"" + name + "\", \"" + guid + "\"");
                    _sln.AppendLine("EndProject");
                }

                return guid;
            }

            string GetProjectGuid(string projectFile)
            {
                const string begin = "<ProjectGuid>";
                const string end = "</ProjectGuid>";

                var xml = File.ReadAllText(projectFile);
                var a = xml.IndexOf(begin, StringComparison.Ordinal);

                if (a == -1)
                    throw new FormatException(begin + " not found");

                var b = xml.IndexOf(end, a + begin.Length, StringComparison.Ordinal);

                if (b == -1)
                    throw new FormatException(end + " not found");

                return xml.Substring(a + begin.Length, b - a - begin.Length);
            }

            string NewGuid()
            {
                var bytes = new byte[]
                {
                    12, 34, 56, 78, 91, 111, 112, 123, 134, 145,
                    156, 178, 191, 211, 212, _guids++
                };

                return ("{" + new Guid(bytes) + "}").ToUpperInvariant();
            }
        }
    }
}
