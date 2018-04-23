using System;
using Uno.Compiler.API.Domain.IL.Members;

namespace Uno.Compiler.API.Domain.IL
{
    public abstract class Entity : SourceObject, IComparable<Entity>
    {
        protected string _name;
        public virtual string Name => _name ?? UnoName;

        public abstract EntityType EntityType { get; }
        public string UnoName { get; protected set; }

        // Compiler hooks
        public Action<Entity> AssigningAttributes;
        public void AssignAttributes() { ActionQueue.Dequeue(ref AssigningAttributes, this); }

        protected Entity(Source s, string name)
            : base(s)
        {
            UnoName = name;
        }

        public virtual void SetName(string name)
        {
            _name = name;
        }

        public int CompareTo(Entity other)
        {
            var diff = string.Compare(GetName(this), GetName(other), StringComparison.InvariantCulture);
            return diff != 0
                ? diff 
                : string.Compare(ToString(), other.ToString(), StringComparison.InvariantCulture);
        }

        static string GetName(Entity entity)
        {
            return (entity as Method)?.DeclaringMember != null
                ? (entity as Method).DeclaringMember.UnoName
                : entity?.UnoName;
        }

        public override string ToString()
        {
            return UnoName;
        }
    }
}
