using Uno.Compiler.API.Domain.IL;
using Uno.Compiler.API.Domain.IL.Members;
using Uno.Compiler.API.Domain.IL.Types;

namespace Uno.Compiler.Core.IL.Validation
{
    public static class Visibilities
    {
        public static bool IsVisibleGlobally(this DataType dt)
        {
            return dt.GetVisibility().Level == VisibilityLevel.Global;
        }

        public static bool IsVisibleInPackage(this DataType dt, SourcePackage package)
        {
            return dt.GetVisibility().IsVisibleInPackage(package);
        }

        public static bool IsVisibleInType(this DataType dt, DataType otherType)
        {
            var dtv = dt.GetVisibility();

            switch (dtv.Level)
            {
                case VisibilityLevel.Global:
                    return true;
                case VisibilityLevel.Package:
                    return otherType.Source.Package.Equals(dtv.Package) || 
                        dtv.Package.InternalsVisibleTo.Contains(otherType.Source.Package.Name);
                case VisibilityLevel.SameType:
                    return otherType.MasterDefinition == dt.MasterDefinition || 
                        dt.MasterDefinition.IsChildClassOf(otherType.MasterDefinition);
                case VisibilityLevel.SameTypeOrSubclass:
                    return otherType.MasterDefinition == dt.MasterDefinition || 
                        dt.MasterDefinition.IsChildClassOf(otherType.MasterDefinition) || 
                        dt.IsSubclassOf(otherType) ||
                        dt.IsNestedType && dt.ParentType.IsVisibleInType(otherType);
                case VisibilityLevel.SameTypeOrSubclassOfOrPackage:
                    return otherType.MasterDefinition == dt.MasterDefinition || 
                        dt.MasterDefinition.IsChildClassOf(otherType.MasterDefinition) || 
                        dt.IsSubclassOf(otherType) || 
                        dt.IsNestedType && dt.ParentType.IsVisibleInType(otherType) ||
                        otherType.Source.Package.Equals(dtv.Package) || 
                        dtv.Package.InternalsVisibleTo.Contains(otherType.Source.Package.Name);
                default:
                    return false;
            }
        }

        public static bool IsVisibleInSubclassesOf(this DataType dt, DataType otherType)
        {
            var dtLevel = dt.GetVisibility().Level;
            return
                dt.IsVisibleInType(otherType) &&
                    dtLevel < VisibilityLevel.SameType &&
                    // Disallow exposing internal classes publicly
                    !(otherType.GetVisibility().Level == VisibilityLevel.Global &&
                        dtLevel == VisibilityLevel.Package);
        }

        public static Visibility GetVisibility(this Member m)
        {
            var v = m.DeclaringType.GetVisibility();

            if (m.IsProtected && m.IsInternal)
            {
                if (v.Level < VisibilityLevel.SameTypeOrSubclass)
                {
                    v.Level = VisibilityLevel.SameTypeOrSubclassOfOrPackage;
                    v.Type = m.DeclaringType;
                }
            }
            else if (m.IsInternal)
            {
                if (v.Level < VisibilityLevel.SameTypeOrSubclass)
                {
                    v.Level = VisibilityLevel.Package;
                    v.Type = m.DeclaringType;
                }
            }
            else if (m.IsProtected)
            {
                if (v.Level < VisibilityLevel.SameTypeOrSubclass)
                {
                    v.Level = VisibilityLevel.SameTypeOrSubclass;
                    v.Type = m.DeclaringType;
                }
            }
            else if (m.IsPrivate)
            {
                if (v.Level < VisibilityLevel.SameType)
                {
                    v.Level = VisibilityLevel.SameType;
                    v.Type = m.DeclaringType;
                }
            }

            return v;
        }

        static void AdjustVisibility(DataType dt, ref Visibility v)
        {
            if (dt.IsProtected && dt.IsInternal)
            {
                if (v.Level < VisibilityLevel.SameTypeOrSubclass)
                    v.Level = VisibilityLevel.SameTypeOrSubclassOfOrPackage;
            }
            else if (dt.IsInternal)
            {
                if (v.Level < VisibilityLevel.SameTypeOrSubclass)
                    v.Level = VisibilityLevel.Package;
            }
            else if (dt.IsProtected)
            {
                if (v.Level < VisibilityLevel.SameTypeOrSubclass)
                {
                    v.Level = VisibilityLevel.SameTypeOrSubclass;
                    v.Type = dt.ParentType;
                }
            }
            else if (dt.IsPrivate)
            {
                if (v.Level < VisibilityLevel.SameType)
                {
                    v.Level = VisibilityLevel.SameType;
                    v.Type = dt.ParentType;
                }
            }
        }

        public static Visibility GetVisibility(this DataType dt)
        {
            if (dt is VoidType ||
                dt is GenericParameterType)
                return new Visibility(VisibilityLevel.Global, dt);

            if (dt.IsNamespaceMember)
                return new Visibility(
                    dt.IsPublic
                        ? VisibilityLevel.Global
                        : VisibilityLevel.Package, 
                    dt);


            var v = new Visibility(VisibilityLevel.Global, dt);
            AdjustVisibility(dt, ref v);

            var p = dt.ParentType;
            while (p != null)
            {
                AdjustVisibility(p, ref v);
                p = p.ParentType;
            }

            return v;
        }
    }
}
