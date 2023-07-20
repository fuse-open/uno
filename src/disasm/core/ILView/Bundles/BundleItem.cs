using System.Collections.Generic;
using Uno.Compiler;
using Uno.Compiler.API.Domain.Extensions;
using Uno.Compiler.API.Domain.IL;
using Uno.Disasm.ILView.Commands;
using Uno.Disasm.ILView.Namespaces;

namespace Uno.Disasm.ILView.Bundles
{
    public class BundleItem : FileBase, IFile
    {
        public readonly Dictionary<string, NamespaceItem> Namespaces = new Dictionary<string, NamespaceItem>();
        public readonly ReferenceCollection References = new ReferenceCollection();
        public readonly ResourceCollection Resources = new ResourceCollection();
        public readonly BundleFolderItem SourceFiles;
        public readonly BundleFolderItem ExtensionsFiles;
        public readonly BundleFolderItem BundleFiles;
        public readonly BundleFolderItem ForeignSourceFiles;
        public readonly BundleFolderItem AdditionalFiles;
        public readonly BundleFolderItem StuffFiles;
        public readonly SourceBundle Bundle;

        public override string DisplayName => Bundle.Name;
        public override ILIcon Icon => ILIcon.Project;
        public override Syntax Syntax => Syntax.Stuff;
        public override string FullName => Bundle.Source.FullPath;

        public BundleItem(SourceBundle bundle)
        {
            Bundle = bundle;
            SourceFiles = new BundleFolderItem(bundle, "Source Files");
            ExtensionsFiles = new BundleFolderItem(bundle, "Extensions Files");
            BundleFiles = new BundleFolderItem(bundle, "Bundle Files");
            ForeignSourceFiles = new BundleFolderItem(bundle, "Foreign Source Files");
            AdditionalFiles = new BundleFolderItem(bundle, "Additional Files");
            StuffFiles = new BundleFolderItem(bundle, "Stuff Files");

            if (bundle.Version != null)
                Suffix = "(" + bundle.Version + ")";

            foreach (var reference in bundle.References)
                References.AddChild(new ReferenceItem(reference));
            foreach (var f in bundle.SourceFiles)
                SourceFiles.AddFile(bundle.SourceDirectory, f.UnixPath);
            foreach (var f in bundle.ExtensionsFiles)
                ExtensionsFiles.AddFile(bundle.SourceDirectory, f.UnixPath);
            foreach (var f in bundle.BundleFiles)
                BundleFiles.AddFile(bundle.SourceDirectory, f.UnixPath);
            foreach (var f in bundle.ForeignSourceFiles)
                ForeignSourceFiles.AddFile(bundle.SourceDirectory, f.UnixPath);
            foreach (var f in bundle.AdditionalFiles)
                AdditionalFiles.AddFile(bundle.SourceDirectory, f.UnixPath);
            foreach (var f in bundle.StuffFiles)
                StuffFiles.AddFile(bundle.SourceDirectory, f.UnixPath);

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
            disasm.AppendHeader(Bundle);
            disasm.Append(Contents);
        }

        public void AddResources(IEnumerable<BundleFile> files)
        {
            foreach (var f in files)
                Resources.AddChild(new BundleFileItem(f));
        }
    }
}