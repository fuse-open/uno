using System.Collections.Generic;
using System.IO;
using Uno.Compiler.API.Domain.IL;

namespace Uno.Compiler.API.Backends
{
    public abstract class SourceBackend : Backend
    {
        protected int MaxExportNameLength = 72;
        const string RootExportName = "_root";

        protected SourceBackend(ShaderBackend shaderBackend)
            : base(shaderBackend)
        {
        }

        public List<string> GetNamespaceNames(DataType dt)
        {
            return dt.IsNestedType
                ? GetNamespaceNames(dt.ParentType)
                : GetNamespaceNames(dt.ParentNamespace);
        }

        public List<string> GetNamespaceNames(Namespace p)
        {
            var result = new List<string>();

            while (p != null && !p.IsRoot)
            {
                result.Add(p.Name);
                p = p.ParentNamespace;
            }

            result.Reverse();
            return result;
        }

        public string GetExportName(DataType dt, char separator = '.')
        {
            var names = GetNamespaceNames(dt);

            if (names.Count == 0)
                names.Add(RootExportName);

            var index = names.Count;
            do
            {
                var name = dt.Name;

                if (dt.IsGenericDefinition)
                    name += "-" + dt.GenericParameters.Length;
                if (dt.IsGenericParameterization)
                    name += "-" + dt.GenericArguments.Length;

                names.Insert(index, name);
            }
            while ((dt = dt.ParentType) != null);

            var result = string.Join(separator.ToString(), names);

            if (result.Length > MaxExportNameLength)
                result = result.Substring(0, MaxExportNameLength - 9) +
                    "-" + result.GetHashCode().ToString("x");

            return result;
        }

        public string GetExportName(Namespace ns)
        {
            return ns.IsRoot ? RootExportName : ns.FullName;
        }

        protected virtual void ExportBundle(string directory)
        {
            directory = Environment.Combine(directory.UnixToNative());

            foreach (var file in Data.Extensions.BundleFiles)
                Disk.CopyFile(file.SourcePath, Path.Combine(directory, file.TargetName));
        }

        protected virtual void ExportNamespace(Namespace ns)
        {
            foreach (var c in ns.Namespaces)
                ExportNamespace(c);

            foreach (var dt in ns.Types)
                if (!IgnoreType(dt))
                    ExportType(dt);
        }

        protected virtual void ExportType(DataType dt)
        {
        }

        public virtual bool IgnoreType(DataType dt)
        {
            return false;
        }
    }
}
