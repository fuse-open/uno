using System.Collections.Generic;
using System.Linq;
using Uno.UX.Markup.Reflection;

namespace Uno.UX.Markup.UXIL
{
    class GlobalResourceCache
    {
        readonly IDataTypeProvider _dataTypeProvider;

        public GlobalResourceCache(IDataTypeProvider dataTypeProvider)
        {
            _dataTypeProvider = dataTypeProvider;

            foreach (var r in _dataTypeProvider.DataTypes.SelectMany(x => x.GlobalResources))
            {
                if (!_handles.ContainsKey(r.GlobalSymbol))
                {
                    _handles.Add(r.GlobalSymbol, new List<Node>());
                }

                var refNode = new ResourceRefNode(FileSourceInfo.Unknown, r.FullPath, null, r.DataType, InstanceType.Global);
                _handles[r.GlobalSymbol].Add(refNode);
                if (r.GlobalSymbol != r.FullPath)
                {
                    if (_fullPaths.ContainsKey(r.FullPath))
                    {
                        throw new System.Exception("Multiple global resources with the key '" + r.FullPath + "' found in the project");
                    }
                    _fullPaths.Add(r.FullPath, refNode);
                }
            }
        }

        readonly Dictionary<string, List<Node>> _handles = new Dictionary<string, List<Node>>();
        public IEnumerable<KeyValuePair<string, List<Node>>> Handles { get { return _handles; } }

        readonly Dictionary<string, Node> _fullPaths = new Dictionary<string, Node>();

        public IEnumerable<Node> TryFindNode(string handle)
        {
            List<Node> res;
            if (_handles.TryGetValue(handle, out res))
            {
                foreach (var e in res) yield return e;
            }

            Node k;
            if (_fullPaths.TryGetValue(handle, out k)) yield return k;
        }

        public void AddGlobalNode(Node n)
        {
            var name = n.Name;
            if (name == null) return;

            if (!_handles.ContainsKey(name))
            {
                _handles.Add(name, new List<Node>());
            }

            _handles[name].Add(n);
            _fullPaths[n.ContainingClass.GeneratedClassName.FullName + "." + name] = n;
        }
    }
}
