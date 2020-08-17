using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Uno.Compiler;
using Uno.Configuration;
using Uno.IO;
using Uno.Logging;
using Uno.Macros;

namespace Uno.ProjectFormat
{
    public class Project
    {
        string _fullPath;
        UnoConfig _config;
        readonly UnoprojDocument _doc;
        readonly List<ProjectReference> _flattenedProjects = new List<ProjectReference>();
        readonly List<IncludeItem> _flattenedItems = new List<IncludeItem>();
        bool _flattenedItemsDirty = true;
        bool _flattenedProjectsDirty = true;

        public string Name
        {
            get
            {
                SourceValue property;
                if (_doc.Properties.TryGetValue("Name", out property))
                    return property.String;

                var name = Path.GetFileNameWithoutExtension(_fullPath);
                // Support anonymous project files (.unoproj)
                if (string.IsNullOrWhiteSpace(name))
                    name = Path.GetFileName(Path.GetDirectoryName(_fullPath));
                return name;
            }
        }

        public string FullPath
        {
            get { return _fullPath; }
            set { _fullPath = value.ToFullPath(); _config = null; }
        }

        public string RootDirectory => Path.GetDirectoryName(_fullPath);

        public Source Source => new Source(_fullPath);
        public UnoConfig Config => UnoConfig.GetUpToDate(_config) ?? (_config = UnoConfig.Get(_fullPath));

        public IReadOnlyList<PackageReference> PackageReferences => (IReadOnlyList<PackageReference>)_doc.OptionalPackages ?? new PackageReference[0];
        public IReadOnlyList<ProjectReference> ProjectReferences => GetFlattenedProjects();
        public IReadOnlyList<SourceValue> InternalsVisibleTo => (IReadOnlyList<SourceValue>)_doc.OptionalInternalsVisibleTo ?? new SourceValue[0];
        public IReadOnlyList<IncludeItem> IncludeItems => (IReadOnlyList<IncludeItem>)_doc.Includes ?? new IncludeItem[0];
        public IReadOnlyList<SourceValue> ExcludeItems => (IReadOnlyList<SourceValue>)_doc.OptionalExcludes ?? new SourceValue[0];
        public IEnumerable<FileItem> AllFiles => GetFlattenedItems().Where(x => x.Type != IncludeItemType.Folder).Select(x => new FileItem(x.Value, x.Condition));
        public IEnumerable<FileItem> BundleFiles => GetFlattenedItems().Where(x => x.Type == IncludeItemType.Bundle).Select(x => new FileItem(x.Value, x.Condition));
        public IEnumerable<FileItem> SourceFiles => GetFlattenedItems().Where(x => x.Type == IncludeItemType.Source).Select(x => new FileItem(x.Value, x.Condition));
        public IEnumerable<FileItem> ExtensionsFiles => GetFlattenedItems().Where(x => x.Type == IncludeItemType.Extensions).Select(x => new FileItem(x.Value, x.Condition));
        public IEnumerable<FileItem> StuffFiles => GetFlattenedItems().Where(x => x.Type == IncludeItemType.Stuff).Select(x => new FileItem(x.Value, x.Condition));
        public IEnumerable<FileItem> UXFiles => GetFlattenedItems().Where(x => x.Type == IncludeItemType.UX).Select(x => new FileItem(x.Value, x.Condition));
        public IEnumerable<FileItem> AdditionalFiles => GetFlattenedItems().Where(x => x.Type == IncludeItemType.File).Select(x => new FileItem(x.Value, x.Condition));
        public IEnumerable<ForeignItem> ForeignSourceFiles => GetFlattenedItems().Where(x => IncludeItem.IsForeignIncludeType(x.Type)).Select(x => new ForeignItem(x.Value, x.ForeignSourceKind, x.Condition));
        public IEnumerable<FileItem> Folders => GetFlattenedItems().Where(x => x.Type == IncludeItemType.Folder).Select(x => new FileItem(x.Value, x.Condition));
        public IEnumerable<FileItem> FuseJSFiles => GetFlattenedItems().Where(x => x.Type == IncludeItemType.FuseJS).Select(x => new FileItem(x.Value, x.Condition));

        public Dictionary<string, SourceValue> MutableProperties => _doc.Properties;
        public List<PackageReference> MutablePackageReferences => _doc.OptionalPackages ?? (_doc.OptionalPackages = new List<PackageReference>());
        public List<SourceValue> MutableInternalsVisibleTo => _doc.OptionalInternalsVisibleTo ?? (_doc.OptionalInternalsVisibleTo = new List<SourceValue>());

        public List<ProjectReference> MutableProjectReferences
        {
            get { _flattenedProjectsDirty = true; return _doc.OptionalProjects ?? (_doc.OptionalProjects = new List<ProjectReference>()); }
        }

        public List<IncludeItem> MutableIncludeItems
        {
            get { _flattenedItemsDirty = true; return _doc.Includes; }
        }

        public List<SourceValue> MutableExcludeItems
        {
            get { _flattenedItemsDirty = true; return _doc.OptionalExcludes ?? (_doc.OptionalExcludes = new List<SourceValue>()); }
        }

        public string BuildDirectory => Path.Combine(RootDirectory, GetString("BuildDirectory"));
        public string CacheDirectory => Path.Combine(RootDirectory, GetString("CacheDirectory"));

        public string OutputDirectory
        {
            get
            {
                // Avoid Path.Combine() and backslash because property may contain macros
                var outputDir = GetString("OutputDirectory");
                return Path.IsPathRooted(outputDir)
                    ? outputDir
                    : RootDirectory + "/" + outputDir;
            }
        }

        public string GetOutputDirectory(object configuration, object target)
        {
            return OutputDirectory
                .Replace("@(Configuration)", configuration.ToString())
                .Replace("@(Target)", target.ToString())
                .UnixToNative();
        }

        public string Version
        {
            get { return GetString("Version") ?? "0.0.0"; }
            set { MutableProperties["Version"] = value; }
        }

        public string BuildCondition => GetString("BuildCondition");
        public bool UnoCoreReference => GetBool("UnoCoreReference");
        public bool IsTransitive => GetBool("IsTransitive");

        public static Project Load(string filename)
        {
            filename = filename.ToFullPath();
            return new Project(filename, UnoprojParser.Parse(filename));
        }

        public Project(string filename)
            : this(filename.ToFullPath(), new UnoprojDocument())
        {
        }

        Project(string fullPath, UnoprojDocument document)
        {
            _fullPath = fullPath;
            _doc = document;

            foreach (var e in PropertyDefinitions.RenamedItems)
            {
                if (_doc.Properties.ContainsKey(e.Key) && !_doc.Properties.ContainsKey(e.Value))
                {
                    _doc.Properties[e.Value] = _doc.Properties[e.Key];
                    _doc.Properties.Remove(e.Key);

                    var newProperties = new Dictionary<string, SourceValue>();

                    foreach (var p in _doc.Properties)
                        if (!string.IsNullOrEmpty(p.Value.String))
                            newProperties[p.Key] = new SourceValue(p.Value.Source, p.Value.String.Replace("$(" + e.Key + ")", "$(" + e.Value + ")"));

                    foreach (var p in newProperties)
                        _doc.Properties[p.Key] = p.Value;
                }
            }
        }

        public void Save()
        {
            UnoprojSerializer.Serialize(_doc, _fullPath);
        }

        public void Save(Stream stream)
        {
            UnoprojSerializer.Serialize(_doc, stream);
        }

        public void InvalidateItems()
        {
            _flattenedItemsDirty = true;
        }

        public void FlattenItems(Log log)
        {
            // Copy flattened items before clearing
            var flattenedItems = GetFlattenedItems(log, true).ToArray();

            MutableIncludeItems.Clear();
            MutableIncludeItems.AddRange(flattenedItems);
            MutableIncludeItems.Sort();
        }

        public void GlobItems(Log log, IEnumerable<string> patterns, bool exludeItems = false)
        {
            // Copy flattened items before clearing
            var flattenedItems = GetFlattenedItems(log, true).ToArray();
            var includeItems = patterns.Select(x => new IncludeItem(Source, IncludeItemType.Glob, x));

            MutableIncludeItems.Clear();
            MutableIncludeItems.AddRange(flattenedItems);
            IncludeGlobber.FindItems(this, includeItems, ExcludeItems, MutableIncludeItems, log, false, exludeItems);
            MutableIncludeItems.Sort();
        }

        public IReadOnlyList<ProjectReference> GetFlattenedProjects(Log log = null, bool force = false)
        {
            if (_flattenedProjectsDirty || force)
            {
                _flattenedProjects.Clear();
                if (_doc.OptionalProjects != null)
                    ProjectGlobber.FindItems(this, _doc.OptionalProjects, ExcludeItems, _flattenedProjects, log, !force);
                _flattenedProjectsDirty = false;
            }

            return _flattenedProjects;
        }

        public IReadOnlyList<IncludeItem> GetFlattenedItems(Log log = null, bool force = false)
        {
            if (_flattenedItemsDirty || force)
            {
                _flattenedItems.Clear();
                IncludeGlobber.FindItems(this, IncludeItems, ExcludeItems, _flattenedItems, log, !force);
                _flattenedItemsDirty = false;
            }

            return _flattenedItems;
        }

        public void AddDefaults()
        {
            if (_doc.OptionalPackages == null)
                _doc.OptionalPackages = new List<PackageReference>();

            if (_doc.OptionalProjects == null)
                _doc.OptionalProjects = new List<ProjectReference>();

            if (_doc.OptionalInternalsVisibleTo == null)
                _doc.OptionalInternalsVisibleTo = new List<SourceValue>();

            if (_doc.OptionalExcludes == null)
                _doc.OptionalExcludes = new List<SourceValue>();

            foreach (var e in PropertyDefinitions.Items)
                if (!_doc.Properties.ContainsKey(e.Key))
                    _doc.Properties.Add(e.Key, new SourceValue(Source, e.Value.Item2));
        }

        public void RemoveDefaults()
        {
            if (_doc.OptionalPackages != null &&
                _doc.OptionalPackages.Count == 0)
                _doc.OptionalPackages = null;

            if (_doc.OptionalProjects != null &&
                _doc.OptionalProjects.Count == 0)
                _doc.OptionalProjects = null;

            if (_doc.OptionalInternalsVisibleTo != null &&
                _doc.OptionalInternalsVisibleTo.Count == 0)
                _doc.OptionalInternalsVisibleTo = null;

            if (_doc.OptionalExcludes != null &&
                _doc.OptionalExcludes.Count == 0)
                _doc.OptionalExcludes = null;

            foreach (var key in _doc.Properties.Keys.ToArray())
                if (PropertyDefinitions.Items.ContainsKey(key) &&
                    Expand(PropertyDefinitions.Items[key].Item2, Log.Null) ==
                        Expand(_doc.Properties[key].String, Log.Null))
                    _doc.Properties.Remove(key);
        }

        public Dictionary<string, SourceValue> GetProperties(Log log)
        {
            var result = new Dictionary<string, SourceValue>
            {
                { "Name", new SourceValue(Source, Name) },
                { "Identifier", TryGetProperty("Identifier") },
                { "QIdentifier", TryGetProperty("QIdentifier") },
                { "ProjectDirectory", TryGetProperty("ProjectDirectory") },
            };

            foreach (var e in _doc.Properties)
                result[e.Key] = new SourceValue(e.Value.Source, GetString(e.Key, log));

            foreach (var e in PropertyDefinitions.Items)
            {
                if (!result.ContainsKey(e.Key))
                {
                    var defaultValue = GetString(e.Key, log);

                    if (!string.IsNullOrEmpty(defaultValue))
                        result[e.Key] = new SourceValue(Source, defaultValue);
                }
            }

            return result;
        }

        public bool GetBool(string name, Log log = null, bool def = false)
        {
            return GetProperty(name, log, def, bool.Parse);
        }

        public string GetString(string name, Log log = null)
        {
            try
            {
                return Expand(TryGetProperty(name), log);
            }
            catch (Exception e)
            {
                var message = "Failed to expand $(" + name + "): " + e.Message;

                if (log == null)
                    throw new SourceException(GetSource(name), message, e);

                log.Error(GetSource(name), null, message);
                return null;
            }
        }

        string Expand(string value, Log log = null)
        {
            return MacroParser.Expand(
                Source, value, true, log,
                (s, n, c) => GetString(n, (Log)c),
                "$(", ')');
        }

        public T GetProperty<T>(string name, Log log, T def, Func<string, T> convert)
        {
            var str = GetString(name, log);

            try
            {
                if (!string.IsNullOrEmpty(str))
                    return convert(GetString(name, log));
            }
            catch (Exception e)
            {
                log?.Error(GetSource(name), null, "Failed to parse $(" + name + "): " + e.Message);
            }

            return def;
        }

        string TryGetProperty(string name)
        {
            SourceValue val;
            if (_doc.Properties.TryGetValue(name, out val))
                return val.String;

            Tuple<PropertyType, string> def;
            if (PropertyDefinitions.Items.TryGetValue(name, out def))
                return def.Item2;

            switch (name)
            {
                case "Name":
                    return Name;
                case "Identifier":
                    return TryGetProperty("Name").ToIdentifier();
                case "QIdentifier":
                    return TryGetProperty("Name").ToIdentifier(true);
                case "ProjectDirectory":
                    return RootDirectory.NativeToUnix();
            }

            return null;
        }

        Source GetSource(string name)
        {
            SourceValue val;
            if (_doc.Properties.TryGetValue(name, out val))
                return val.Source;

            return Source;
        }

        public SourcePackage CreateSourcePackage(bool isStartup = false)
        {
            var upk = new SourcePackage(
                Name,
                Version,
                FullPath,
                RootDirectory,
                CacheDirectory,
                SourcePackageFlags.Project | (
                    IsTransitive
                        ? SourcePackageFlags.Transitive
                        : 0) | (
                    isStartup
                        ? SourcePackageFlags.Startup
                        : 0),
                BuildCondition);

            upk.AdditionalFiles.AddRange(AdditionalFiles);
            upk.BundleFiles.AddRange(BundleFiles);
            upk.SourceFiles.AddRange(SourceFiles);
            upk.ExtensionsFiles.AddRange(ExtensionsFiles);
            upk.ForeignSourceFiles.AddRange(ForeignSourceFiles);
            upk.StuffFiles.AddRange(StuffFiles);
            upk.UXFiles.AddRange(UXFiles);

            foreach (var p in InternalsVisibleTo)
                upk.InternalsVisibleTo.Add(p.String);

            return upk;
        }

        public override string ToString()
        {
            return Name;
        }
    }
}
