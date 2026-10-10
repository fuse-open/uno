using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Uno.Build.Targets.Utilities;
using Uno.Compiler.API;
using Uno.Compiler.API.Domain.Extensions;
using Uno.Configuration;
using Uno.Diagnostics;
using Uno.Logging;

namespace Uno.Build.Targets.Generators
{
    class XcodeGenerator
    {
        internal static void Configure(IEnvironment env, IEnumerable<BundleFile> bundleFiles, Shell shell)
        {
            {
                // Files
                var publicHeaders = new HashSet<string>(env.GetSet("xcode.publicHeader", true));

                // Xcode (since 27) requires PBXFileReference paths to match the actual
                // capitalization on disk, even on case-insensitive volumes. The build
                // copies all foreign sources/headers into a single output directory, so a
                // casing inconsistency between packages (e.g. one declaring "iOS/..." and
                // another "ios/...") can make the on-disk folder name differ from the path
                // recorded in the pbxproj. Resolve the real on-disk casing here so the
                // generated project always matches the filesystem.
                var realCaseCache = new Dictionary<string, string>(StringComparer.Ordinal);
                var headerDir = env.GetOutputPath("headerDirectory");
                var sourceDir = env.GetOutputPath("sourceDirectory");

                var headers = env.GetSet("headerFile", true)
                    .Distinct()
                    .Select(f => new XcodeFile(
                        "@(headerDirectory)/" + GetRealRelativePath(headerDir, f, realCaseCache),
                        "Headers",
                        "<group>",
                        publicHeaders.Contains(f) ? "{ATTRIBUTES = (Public, ); }" : null))
                    .ToList();

                var sources = env.GetSet("sourceFile", true)
                    .Distinct()
                    .Select(f => new XcodeFile("@(sourceDirectory)/" + GetRealRelativePath(sourceDir, f, realCaseCache), "Sources", "<group>"))
                    .ToList();

                var frameworks = env.GetSet("xcode.framework", true)
                    .Select(f =>
                    {
                        var eframework = Path.HasExtension(f) ? f : (f + ".framework");
                        var isAbsolute = Path.IsPathRooted(eframework);
                        var filename = isAbsolute ? eframework : ("System/Library/Frameworks/" + eframework);
                        var sourceTree = isAbsolute ? "<absolute>" : "SDKROOT";

                        return new XcodeFile(filename, "Frameworks", sourceTree);
                    })
                    .GroupBy(f => f.Path)
                    .Select(x => x.First())
                    .ToList();

                var embeddedFrameworks = env.GetSet("xcode.embeddedFramework", true)
                    .Distinct()
                    .Select(f => new XcodeFile(f, "Embed Frameworks", "<absolute>", "{ATTRIBUTES = (CodeSignOnCopy, RemoveHeadersOnCopy, ); }"))
                    .ToList();

                var allFiles = headers.Concat(sources).Concat(frameworks).Concat(embeddedFrameworks);

                var frameworkDirs = env.GetSet("xcode.framework", true)
                    .Concat(env.GetSet("xcode.embeddedFramework", true))
                    .Where(Path.IsPathRooted)
                    .Select(Path.GetDirectoryName)
                    .Concat(env.GetSet("xcode.frameworkDirectory", true))
                    .Distinct()
                    .Select(dir => dir.QuoteSpace().QuoteSpace());

                {
                    // File references
                    var fileReferences = new StringBuilder();
                    foreach (var f in allFiles)
                        fileReferences.AppendLine(
                            Indent(2) + f.FileReferenceUUID +
                            " /* " + f.Path + " */ = {isa = PBXFileReference; name = " + f.Name.QuoteSpace() +
                            "; path = " + f.Path.QuoteSpace() +
                            "; sourceTree = " + f.SourceTree.QuoteSpace() + "; };");
                    env.Set("pbxproj.PBXFileReferences", fileReferences.ToString().Trim());
                }
                {
                    // Build files
                    var buildFiles = new StringBuilder();
                    foreach (var f in allFiles)
                        buildFiles.AppendLine(
                            Indent(2) + f.BuildFileUUID + " /* " + f.Name + " in " + f.Category +
                            " */ = {isa = PBXBuildFile; fileRef = " + f.FileReferenceUUID + " /* " + f.Name + " */; " +
                            (f.Settings == null ? "" : "settings = " + f.Settings + "; ") + "};");

                    env.Set("pbxproj.PBXBuildFiles", buildFiles.ToString().Trim());
                }
                {
                    // Sources
                    var groupSources = new StringBuilder();
                    var sourcesBuildPhaseFiles = new StringBuilder();
                    foreach (var f in sources)
                    {
                        groupSources.AppendLine(Indent(4) + f.FileReferenceUUID + " /* " + f.Path + " */,");
                        sourcesBuildPhaseFiles.AppendLine(
                            Indent(4) + f.BuildFileUUID + " /* " + f.Name + " in " + f.Category + " */,");
                    }

                    env.Set("pbxproj.PBXGroupSources", groupSources.ToString().Trim());
                    env.Set("pbxproj.PBXSourcesBuildPhaseFiles", sourcesBuildPhaseFiles.ToString().Trim());
                }
                {
                    // Headers
                    var groupHeaders = new StringBuilder();
                    var headersBuildPhaseFiles = new StringBuilder();
                    foreach (var f in headers)
                    {
                        groupHeaders.AppendLine(Indent(4) + f.FileReferenceUUID + " /* " + f.Path + " */,");
                        headersBuildPhaseFiles.AppendLine(
                            Indent(4) + f.BuildFileUUID + " /* " + f.Name + " in " + f.Category + " */,");
                    }

                    env.Set("pbxproj.PBXGroupHeaders", groupHeaders.ToString().Trim());
                    env.Set("pbxproj.PBXHeadersBuildPhaseFiles", headersBuildPhaseFiles.ToString().Trim());
                }
                {
                    // Frameworks
                    var groupFrameworks = new StringBuilder();
                    var frameworksBuildPhaseFiles = new StringBuilder();
                    var embedFrameworksBuildPhaseFiles = new StringBuilder();
                    foreach (var f in frameworks)
                    {
                        groupFrameworks.AppendLine(Indent(4) + f.FileReferenceUUID + " /* " + f.Path + " */,");
                        frameworksBuildPhaseFiles.AppendLine(Indent(4) + f.BuildFileUUID + " /* " + f.Name + " in " + f.Category + " */,");
                    }
                    foreach (var f in embeddedFrameworks)
                    {
                        groupFrameworks.AppendLine(Indent(4) + f.FileReferenceUUID + " /* " + f.Path + " */,");
                        embedFrameworksBuildPhaseFiles.AppendLine(Indent(4) + f.BuildFileUUID + " /* " + f.Name + " in " + f.Category + " */,");
                    }
                    env.Set("pbxproj.PBXGroupFrameworks", groupFrameworks.ToString().Trim());
                    env.Set("pbxproj.PBXFrameworksBuildPhaseFiles", frameworksBuildPhaseFiles.ToString().Trim());
                    env.Set("pbxproj.PBXEmbedFrameworksBuildPhaseFiles", embedFrameworksBuildPhaseFiles.ToString().Trim());

                    env.Set("pbxproj.frameworkDirectories", string.Join(", ", frameworkDirs));
                }
            }

            {
                // Development Team
                var devTeam = env.GetString("project.ios.developmentTeam");
                if (string.IsNullOrEmpty(devTeam))
                    devTeam = UnoConfig.Current.GetString("ios.developmentTeam");
                if (string.IsNullOrEmpty(devTeam))
                    devTeam = FindCodeSigningDevelopmentTeam(shell);
                if (!string.IsNullOrEmpty(devTeam))
                    env.Set("pbxproj.developmentTeam", devTeam.QuoteSpace());
            }

            {
                // ShellScripts
                var buildPhases = new StringBuilder();
                var shellScripts = new StringBuilder();
                foreach (var script in env.GetSet("xcode.shellScript"))
                {
                    var uuid = XcodeFile.CreateUUID();
                    buildPhases.AppendLine(Indent(4) + uuid + " /* ShellScript */,");
                    shellScripts.AppendLine(Indent(2) + uuid +
                        " /* ShellScript */ = {isa = PBXShellScriptBuildPhase; buildActionMask = 2147483647; files = (); inputPaths = (); outputPaths = (); runOnlyForDeploymentPostprocessing = 0; shellPath = /bin/sh; shellScript = " +
                        script.ToLiteral() + "; };");
                }
                env.Set("pbxproj.PBXNativeTargetBuildPhases", buildPhases.ToString().Trim());
                env.Set("pbxproj.PBXShellScriptBuildPhase", shellScripts.ToString().Trim());
            }

            {
                // Include dirs
                var includeDirs = new StringBuilder();
                includeDirs.AppendLine(Indent(5) + "@(headerDirectory),");
                foreach (var e in env.GetSet("includeDirectory", true))
                    includeDirs.AppendLine(Indent(5) + e.QuoteSpace().QuoteSpace() + ",");
                env.Set("pbxproj.includeDirectories", includeDirs.ToString().Trim());
            }

            {
                // Link dirs
                var linkDirs = new StringBuilder();
                foreach (var e in env.GetSet("linkDirectory", true))
                    linkDirs.AppendLine(Indent(5) + e.QuoteSpace().QuoteSpace() + ",");
                env.Set("pbxproj.linkDirectories", linkDirs.ToString().Trim());
            }

            {
                // Link flags
                var linkFlags = new StringBuilder();
                foreach (var e in env.GetSet("linkLibrary", true))
                    linkFlags.Append(" -l" + e);
                env.Set("pbxproj.linkLibraries", linkFlags.ToString().Trim());
            }

            // Bundle files
            foreach (var e in bundleFiles)
                switch (Path.GetExtension(e.SourcePath).ToUpperInvariant())
                {
                    // TODO: We're just guessing here. It should be possible to
                    // catch or tag Fonts as they're added and specifically pick
                    // them up.
                    case ".OTF":
                    case ".TTF":
                        env.Require("fontFile", e.TargetName);
                        break;
                }
        }

        static string Indent(int c) => new string('\t', c);

        /// <summary>
        /// Resolves the actual on-disk casing of a source/header file path relative to its
        /// output directory, so the generated pbxproj matches the filesystem. Falls back to
        /// the declared path if the file or any of its ancestor directories does not exist.
        /// </summary>
        static string GetRealRelativePath(string baseDir, string unixRelativePath, Dictionary<string, string> cache)
        {
            if (string.IsNullOrEmpty(unixRelativePath))
                return unixRelativePath;

            var parts = unixRelativePath.Split('/');
            var current = baseDir;
            var result = new string[parts.Length];

            for (int i = 0; i < parts.Length; i++)
            {
                var part = parts[i];
                var real = part;

                if (!string.IsNullOrEmpty(part) && part != "." && part != ".." && current != null && Directory.Exists(current))
                {
                    if (!cache.TryGetValue(current, out var listing))
                    {
                        try { listing = string.Join("\0", Directory.EnumerateFileSystemEntries(current)); }
                        catch { listing = string.Empty; }
                        cache[current] = listing;
                    }

                    var fallback = (string)null;
                    foreach (var e in listing.Split('\0'))
                    {
                        var name = Path.GetFileName(e);
                        if (string.Equals(name, part, StringComparison.Ordinal))
                        {
                            real = name;
                            fallback = null;
                            break;
                        }
                        if (fallback == null && string.Equals(name, part, StringComparison.OrdinalIgnoreCase))
                            fallback = name;
                    }
                    if (real == part && fallback != null)
                        real = fallback;
                }

                result[i] = real;
                current = current != null ? Path.Combine(current, real) : null;
            }

            return string.Join("/", result);
        }

        static string FindCodeSigningDevelopmentTeam(Shell shell)
        {
            // Can only run on Mac
            if (!OperatingSystem.IsMacOS())
            {
                Log.Default.Warning("Finding a development team for signing failed: This operation is only supported on macOS.");
                return null;
            }

            try
            {
                var res = new DevelopmentTeamExtractor().FindAllDevelopmentTeams(shell);
                return res
                    ?.FirstOrDefault(x =>
                        x.organizationalUnit != null &&
                        x.organizationalUnit.All(char.IsLetterOrDigit))
                    ?.organizationalUnit;
            }
            catch (DevelopmentTeamExtractorFailure e)
            {
                Log.Default.Warning("Finding a development team for signing failed: " + e);
                return null;
            }
        }
    }
}
