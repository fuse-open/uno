using Uno.Collections;

namespace Uno.UX
{
    public sealed class NameTable
    {
    	public static readonly NameTable Empty = new NameTable(null, new string[0]);

        public readonly string[] Entries;

        List<object> _objects = new List<Object>();
        public IList<object> Objects { get { return _objects; } }

        IList<Property> _properties = new List<Property>();
        public IList<Property> Properties 
        {
        	get { return _properties ?? ParentTable.Properties; }
        	set { _properties = value; }
        }

        public object this[string key]
        {
            get
            {
                for (var i = 0; i < Entries.Length; i++)
                    if (Entries[i] == key) return Objects[i];

                if (ParentTable != null) return ParentTable[key];
                
                return null;
            }
        }

        object _this;
        public object This 
        { 
            get { return _this ?? (ParentTable != null ? ParentTable.This : null); }
            set { _this = value; }
        }

        public readonly NameTable ParentTable;

        public NameTable(NameTable parentTable, string[] entries)
        {
            ParentTable = parentTable;
            Entries = entries;
        }
    }

    /** Makes the UX compiler automatically assign a valid `NameTable` instance to the decorated property. */
    public sealed class UXAutoNameTableAttribute: Attribute {}
    /** Makes the UX compiler automatically assign the full qualified name of the current `ux:Class` to the decorated property. */
    public sealed class UXAutoClassNameAttribute: Attribute {}

    /** When used on an attached property X, this makes the UX compiler automatically assign a valid NameTable instance 
        to the `targetProp` if the property X is assigned a value in UX markup. */
    public sealed class UXAuxNameTableAttribute: Attribute 
    {
        public UXAuxNameTableAttribute(string targetProp) {}
    }
}