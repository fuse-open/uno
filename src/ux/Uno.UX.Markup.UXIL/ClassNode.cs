using System.Collections.Generic;
using Uno.UX.Markup.Reflection;

namespace Uno.UX.Markup.UXIL
{
    public sealed partial class ClassNode: DocumentScope
    {
        public bool IsInnerClass { get; } 

        public IDataType BaseType { get; }

        public bool AutoCtor { get; private set; }

        public bool Simulate { get; private set; }

        public bool IsTest { get; }

        public bool IsAppClass
        {
            get { return ActualIDataTypeImpl.FullName == "Fuse.App" || ActualIDataTypeImpl.FullName == "Fuse.ExportedViews"; }
        }

        internal ClassNode(FileSourceInfo source, bool isInnerClass, string name, IDataType baseType, TypeNameHelper generatedClassName, Vector<float> clearColor, bool autoCtor, bool simulate, bool isTest, IEnumerable<RawProperty> rawProperties)
            : base(source, name, generatedClassName, baseType, clearColor, InstanceType.None, rawProperties)
        {
            IsInnerClass = isInnerClass;
            AutoCtor = autoCtor;
            Simulate = simulate;
            IsTest = isTest;
            BaseType = baseType;
        }

        public override IDataType MemberSource => this;

        public override IDataType DeclaredType => this;

        readonly List<PropertyNode> _propNodes = new List<PropertyNode>();
        public void RegisterUXProperty(PropertyNode propNode)
        {
            _propNodes.Add(propNode);
        }
        public IEnumerable<PropertyNode> DeclaredUXProperties => _propNodes;

		readonly List<DependencyNode> _dependencyNodes = new List<DependencyNode>();
		public void RegisterDependency(DependencyNode dn)
		{
			_dependencyNodes.Add(dn);
		}
		public IEnumerable<DependencyNode> DeclaredDependencies => _dependencyNodes;
    }
}
