using Uno.Compiler.API.Domain.IL;

namespace Uno.Compiler.Core.IL.Validation
{
    public class Visibility
    {
        public VisibilityLevel Level;
        public DataType Type;
        public SourceBundle Bundle => Type.Source.Bundle;

        public bool IsVisibleInBundle(SourceBundle bundle)
        {
            return
                Level == VisibilityLevel.Global ||
                (Level == VisibilityLevel.Bundle && Bundle.Equals(bundle)) ||
                (Level == VisibilityLevel.SameTypeOrSubclassOfOrPackage && Bundle.Equals(bundle)) ||
                Bundle.InternalsVisibleTo.Contains(bundle.Name);
        }

        public Visibility(VisibilityLevel level, DataType dt)
        {
            Level = level;
            Type = dt;
        }

        public bool IsVisibile(DataType dt)
        {
            switch (Level)
            {
                case VisibilityLevel.Global:
                    return dt.IsVisibleGlobally();
                case VisibilityLevel.Bundle:
                    return dt.IsVisibleInPackage(Bundle);
                case VisibilityLevel.SameTypeOrSubclassOfOrPackage:
                    return dt.IsVisibleInPackage(Bundle) &&
                           dt.IsVisibleInType(Type) &&
                           dt.IsVisibleInSubclassesOf(Type);
                case VisibilityLevel.SameTypeOrSubclass:
                    return dt.IsVisibleInType(Type) &&
                           dt.IsVisibleInSubclassesOf(Type);
                case VisibilityLevel.SameType:
                    return dt.IsVisibleInType(Type);
                default:
                    return false;
            }
        }

        public override string ToString()
        {
            return Level + " (Type: " + Type + ", Package: " + Bundle + ")";
        }
    }
}