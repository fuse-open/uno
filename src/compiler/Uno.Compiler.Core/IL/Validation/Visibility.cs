using Uno.Compiler.API.Domain.IL;

namespace Uno.Compiler.Core.IL.Validation
{
    public class Visibility
    {
        public VisibilityLevel Level;
        public DataType Type;
        public SourcePackage Package => Type.Source.Package;

        public bool IsVisibleInPackage(SourcePackage package)
        {
            return
                Level == VisibilityLevel.Global ||
                (Level == VisibilityLevel.Package && Package.Equals(package)) ||
                (Level == VisibilityLevel.SameTypeOrSubclassOfOrPackage && Package.Equals(package)) ||
                Package.InternalsVisibleTo.Contains(package.Name);
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
                case VisibilityLevel.Package:
                    return dt.IsVisibleInPackage(Package);
                case VisibilityLevel.SameTypeOrSubclassOfOrPackage:
                    return dt.IsVisibleInPackage(Package) &&
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
            return Level + " (Type: " + Type + ", Package: " + Package + ")";
        }
    }
}