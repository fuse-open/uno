using System.Collections.Generic;
using Uno.Compiler;
using Uno.Compiler.API.Domain.Extensions;
using Uno.Compiler.API.Domain.IL;
using Uno.Disasm.ILView.Commands;
using Uno.Disasm.ILView.Namespaces;

namespace Uno.Disasm.ILView.Packages
{
    public class PackageItem : FileBase, IFile
    {
        public readonly Dictionary<string, NamespaceItem> Namespaces = new Dictionary<string, NamespaceItem>();
        public readonly ReferenceCollection References = new ReferenceCollection();
        public readonly ResourceCollection Resources = new ResourceCollection();
        public readonly PackageFolderItem SourceFiles;
        public readonly PackageFolderItem ExtensionsFiles;
        public readonly PackageFolderItem BundleFiles;
        public readonly PackageFolderItem ForeignSourceFiles;
        public readonly PackageFolderItem AdditionalFiles;
        public readonly PackageFolderItem StuffFiles;
        public readonly SourcePackage Package;

        public override string DisplayName => Package.Name;
        public override ILIcon Icon => ILIcon.Project;
        public override Syntax Syntax => Syntax.Stuff;
        public override string FullName => Package.Source.FullPath;

        public PackageItem(SourcePackage upk)
        {
            Package = upk;
            SourceFiles = new PackageFolderItem(upk, "Source Files");
            ExtensionsFiles = new PackageFolderItem(upk, "Extensions Files");
            BundleFiles = new PackageFolderItem(upk, "Bundle Files");
            ForeignSourceFiles = new PackageFolderItem(upk, "Foreign Source Files");
            AdditionalFiles = new PackageFolderItem(upk, "Additional Files");
            StuffFiles = new PackageFolderItem(upk, "Stuff Files");

            if (upk.Version != null)
                Suffix = "(" + upk.Version + ")";

            foreach (var reference in upk.References)
                References.AddChild(new ReferenceItem(reference));
            foreach (var f in upk.SourceFiles)
                SourceFiles.AddFile(upk.SourceDirectory, f.UnixPath);
            foreach (var f in upk.ExtensionsFiles)
                ExtensionsFiles.AddFile(upk.SourceDirectory, f.UnixPath);
            foreach (var f in upk.BundleFiles)
                BundleFiles.AddFile(upk.SourceDirectory, f.UnixPath);
            foreach (var f in upk.ForeignSourceFiles)
                ForeignSourceFiles.AddFile(upk.SourceDirectory, f.UnixPath);
            foreach (var f in upk.AdditionalFiles)
                AdditionalFiles.AddFile(upk.SourceDirectory, f.UnixPath);
            foreach (var f in upk.StuffFiles)
                StuffFiles.AddFile(upk.SourceDirectory, f.UnixPath);

            SourceFiles.Collapse();
            ExtensionsFiles.Collapse();
            BundleFiles.Collapse();
            ForeignSourceFiles.Collapse();
            AdditionalFiles.Collapse();
            StuffFiles.Collapse();

            AddChild(References);
            AddChild(Resources);
            AddChild(SourceFiles);
            AddChild(ExtensionsFiles);
            AddChild(BundleFiles);
            AddChild(ForeignSourceFiles);
            AddChild(AdditionalFiles);
            AddChild(StuffFiles);
        }

        internal NamespaceItem GetNamespace(Namespace ns)
        {
            var fullName = ns.ToString();

            NamespaceItem result;
            if (!Namespaces.TryGetValue(fullName, out result))
            {
                result = new NamespaceItem(ns);
                Namespaces.Add(fullName, result);
                AddChild(result);
            }

            return result;
        }

        protected override void Disassemble(Disassembler disasm)
        {
            disasm.AppendHeader(Package);
            disasm.Append(Contents);
        }

        public void AddResources(IEnumerable<BundleFile> files)
        {
            foreach (var f in files)
                Resources.AddChild(new BundleFileItem(f));
        }
    }
}