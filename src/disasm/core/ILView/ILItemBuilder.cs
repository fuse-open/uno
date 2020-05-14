using System.Collections.Generic;
using Uno.Build;
using Uno.Compiler;
using Uno.Compiler.API;
using Uno.Compiler.API.Backends;
using Uno.Compiler.API.Domain.Extensions;
using Uno.Compiler.API.Domain.Graphics;
using Uno.Compiler.API.Domain.IL;
using Uno.Disasm.ILView.Members;
using Uno.Disasm.ILView.Namespaces;
using Uno.Disasm.ILView.Packages;

namespace Uno.Disasm.ILView
{
    public class ILItemBuilder
    {
        readonly Dictionary<SourcePackage, PackageItem> _packageMap = new Dictionary<SourcePackage, PackageItem>();
        readonly BuildItem _buildItem;

        public ILItemBuilder(BuildItem buildItem)
        {
            _buildItem = buildItem;
        }

        public void Build(BuildResult build)
        {
            Build(build.IL);
            Build(build, build.Compiler.Environment, build.Compiler.Data.Extensions);

            if (build.Compiler.Backend.BuildType == BuildType.Executable &&
                build.Entrypoint != null)
                GetPackage(build.Compiler.Input.Package)
                    .GetNamespace(build.IL)
                    .AddChild(new FunctionItem(build.Entrypoint));

            GetPackage(build.Compiler.Input.Package)
                .AddResources(build.Compiler.Data.Extensions.BundleFiles);

            ExpandItems();
        }

        void Build(BuildResult build, IEnvironment env, ExtensionRoot root)
        {
            if (root == null)
                return;

            // Find typed output files
            foreach (var p in root.Properties)
            {
                if (p.Key.EndsWith(".TargetDirectory"))
                {
                    var key = p.Key.Replace(".TargetDirectory", "");
                    var containingDir = env.GetOutputPath(p.Key);
                    var folder = new PackageFolderItem(containingDir, key.Replace("File", " Files"));

                    foreach (var f in build.Compiler.Environment.Enumerate(key))
                        folder.AddFile(containingDir, f.String);

                    _buildItem.Folders.Add(folder);
                }
            }
        }

        void Build(Namespace root)
        {
            if (root == null)
                return;

            foreach (var ns in root.Namespaces)
                Build(ns);
            foreach (var block in root.Blocks)
                Build(GetPackage(block.Source.Package).GetNamespace(root), block);
            foreach (var dt in root.Types)
                Build(GetPackage(dt.Source.Package).GetNamespace(root), dt);
        }

        void Build(ILItem parent, DataType dt)
        {
            var item = AddType(parent, dt);
            foreach (var it in item.Type.NestedTypes)
                Build(parent, it);
            if (item.Type.Block != null)
                foreach (var it in item.Type.Block.NestedBlocks)
                    Build(parent, it);
        }

        void Build(ILItem parent, Block block)
        {
            parent.AddChild(new BlockItem(block));
            foreach (var it in block.NestedBlocks)
                Build(parent, it);
        }

        static TypeItem AddType(ILItem root, DataType dt)
        {
            var item = new TypeItem(dt);

            if (dt.IsGenericDefinition && dt.GenericParameterizations.Count > 0)
            {
                var ptItems = new ParameterizationCollection(dt.GenericParameterizations);

                for (int i = 0; i < ptItems.Children.Count; i++)
                {
                    var ptRoot = ptItems.Children[i] as TypeItem;

                    if (ptRoot != null)
                    {
                        var pt = (DataType)ptRoot.Object;

                        if (pt.IsParameterizedDefinition)
                        {
                            ptItems.Children.RemoveAt(i);
                            ptRoot.AddChild(new DefinitionCollection(dt));
                            ptRoot.AddChild(ptItems);
                            item = ptRoot;
                            break;
                        }
                    }
                }
            }

            foreach (var c in root.Children)
            {
                var t = c as TypeItem;
                if (t != null &&
                    t.Type.Parent == item.Type.Parent &&
                    t.Type.Name == item.Type.Name)
                {
                    t.Overloads.AddChild(item);
                    return item;
                }
            }

            root.AddChild(item);
            return item;
        }

        PackageItem GetPackage(SourcePackage upk)
        {
            PackageItem result;
            if (!_packageMap.TryGetValue(upk, out result))
            {
                result = new PackageItem(upk);
                _packageMap.Add(upk, result);
                _buildItem.Packages.Add(result);
            }

            return result;
        }

        void ExpandItems()
        {
            foreach (var packageItem in _packageMap.Values)
            {
                foreach (var nsItem in packageItem.Namespaces.Values)
                {
                    foreach (var child in nsItem.Children)
                    {
                        var typeItem = child as TypeItem;
                        if (typeItem == null)
                            continue;

                        int count = typeItem.Overloads.Children.Count;
                        if (count > 0)
                            typeItem.Suffix = "(+" + count + " overload" + (count != 1 ? "s" : null) + ")";
                    }
                }

                packageItem.Expand();
            }

            _buildItem.Expand();
        }
    }
}
