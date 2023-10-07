using System.Collections.Generic;
using Uno.Build;
using Uno.Compiler;
using Uno.Compiler.API;
using Uno.Compiler.API.Domain.Extensions;
using Uno.Compiler.API.Domain.Graphics;
using Uno.Compiler.API.Domain.IL;
using Uno.Disasm.ILView.Members;
using Uno.Disasm.ILView.Namespaces;
using Uno.Disasm.ILView.Bundles;

namespace Uno.Disasm.ILView
{
    public class ILItemBuilder
    {
        readonly Dictionary<SourceBundle, BundleItem> _bundleMap = new Dictionary<SourceBundle, BundleItem>();
        readonly BuildItem _buildItem;

        public ILItemBuilder(BuildItem buildItem)
        {
            _buildItem = buildItem;
        }

        public void Build(BuildResult build)
        {
            Build(build.IL);
            Build(build, build.Compiler.Environment, build.Compiler.Data.Extensions);

            if (build.Entrypoint != null)
                GetBundle(build.Compiler.Input.Bundle)
                    .GetNamespace(build.IL)
                    .AddChild(new FunctionItem(build.Entrypoint));

            GetBundle(build.Compiler.Input.Bundle)
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
                if (p.Key.EndsWith(".targetDirectory"))
                {
                    var key = p.Key.Replace(".targetDirectory", "");
                    var containingDir = env.GetOutputPath(p.Key);
                    var folder = new BundleFolderItem(containingDir, key.Replace("File", " files"));

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
                Build(GetBundle(block.Source.Bundle).GetNamespace(root), block);
            foreach (var dt in root.Types)
                Build(GetBundle(dt.Source.Bundle).GetNamespace(root), dt);
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

        BundleItem GetBundle(SourceBundle bundle)
        {
            BundleItem result;
            if (!_bundleMap.TryGetValue(bundle, out result))
            {
                result = new BundleItem(bundle);
                _bundleMap.Add(bundle, result);
                _buildItem.Bundles.Add(result);
            }

            return result;
        }

        void ExpandItems()
        {
            foreach (var bundleItem in _bundleMap.Values)
            {
                foreach (var nsItem in bundleItem.Namespaces.Values)
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

                bundleItem.Expand();
            }

            _buildItem.Expand();
        }
    }
}
