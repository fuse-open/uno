using System.Collections.Generic;
using System.Linq;
using Uno.UX.Markup.Reflection;

namespace Uno.UX.Markup.UXIL
{
    public class NameTableEntry
    {
        public readonly string Name;
        public readonly Node Node;

        public NameTableEntry(string name, Node n)
        {
            Name = name;
            Node = n;
        }
    }


    public class NameTableNode: ObjectNode
    {
        readonly DocumentScope _scope;
        readonly IDataType _nameTableType;

        public NameTableEntry[] Entries { get; }

        public override IDataType MemberSource => _nameTableType;

        public NameTableNode ParentTable
        {
            get;
            private set;
        }

        public NameTableNode(FileSourceInfo source, HashSet<UXPropertyClass> propClasses, ClassNode self, NameTableNode parentTable, IDataType nameTableType, DocumentScope scope, IEnumerable<RawProperty> rawProperties)
            : base(source, "__g_nametable", nameTableType, InstanceType.Local, rawProperties)
        {
            ParentTable = parentTable;
            _scope = scope;
            _nameTableType = nameTableType;
			Entries = _scope.NodesIncludingRoot.Where(x => x.Scope == scope && x.Name != null && x.InstanceType == InstanceType.Local).Select(x => new NameTableEntry(x.Name, x)).ToArray();

            if (self != null)
            {
                var selfProp = TryFindBindableProperty(self, "This");
                selfProp.Bind(self);

                var propsProp = TryFindBindableProperty(self, "Properties");
                foreach (var p in self.DeclaredUXProperties)
                {
                    var pr = self.Properties.First(x => x.Facet.Name == p.Name);
                    propsProp.Bind(self, pr);
                    propClasses.Add(new UXPropertyClass(pr, self));
                }
                    
            }

            var objs = TryFindBindableProperty(scope.ContainingClass, "Objects");
			BindEntries(objs);
        }

		void BindEntries(BindableProperty objs)
		{
			foreach (var e in Entries)
				objs.Bind(e.Node);
		}

        public override Reflection.IDataType DeclaredType => _nameTableType;
    }
}
