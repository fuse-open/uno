using Uno.Compiler.API.Domain.Graphics;

namespace Uno.Compiler.API.Domain.IL
{
    public abstract class Namescope : Entity
    {
        public Namescope Parent { get; private set; }
        public abstract NamescopeType NamescopeType { get; }

        public bool IsRoot => Parent == null;
        public bool IsNamespaceMember => Parent is Namespace;
        public bool IsNestedBlock => Parent is BlockBase;
        public bool IsNestedType => Parent is DataType;
        public BlockBase ParentBlock => Parent as BlockBase;
        public DataType ParentType => Parent as DataType;
        public Namespace ParentNamespace => Parent as Namespace;
        public Namespace FirstNamespace => ParentNamespace ?? Parent.FirstNamespace;
        public Namescope MasterDefinition => NamescopeType == NamescopeType.DataType ? ((DataType) this).MasterDefinition : this;
        public string QualifiedName => Parent != null && !Parent.IsRoot
                                            ? Parent.FullName + "." + UnoName
                                            : UnoName;
        public virtual string FullName => QualifiedName;
        public override EntityType EntityType => EntityType.Namescope;

        public void SetParent(Namescope parent)
        {
            Parent = parent;
        }

        protected Namescope(Source src, Namescope parent, string name)
            : base(src, name)
        {
            Parent = parent;
        }

        public override string ToString()
        {
            return FullName;
        }

        int _uniqueCount;
        public string GetUniqueIdentifier(string prefix)
        {
            return prefix + ++_uniqueCount;
        }
    }
}
