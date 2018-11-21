using System.Collections.Generic;
using System.Linq;
using Uno.Compiler.API.Backends;
using Uno.Compiler.API.Domain;
using Uno.Compiler.API.Domain.IL;
using Uno.Compiler.API.Domain.IL.Members;
using Uno.Compiler.API.Domain.IL.Types;

namespace Uno.Compiler.Core.IL.Validation
{
    public partial class ILVerifier
    {
        public override bool Begin(DataType dt)
        {
            if (dt.CanLink || dt.Package.IsVerified)
                return false;

            var validModifiers = (
                    dt.IsNestedType
                        ? Modifiers.NestedType
                        : Modifiers.ParentType
                ) | (
                    dt.IsClass
                        ? Modifiers.ClassModifiers :
                    dt.IsInterface
                        ? Modifiers.InterfaceModifiers
                        : 0);

            PushObsolete(dt);
            VerifyAttributes(dt, dt.Attributes);
            VerifyProtection(dt, dt.Modifiers);
            VerifyModifiers(dt, dt.Modifiers, validModifiers);

            if (dt.IsIntrinsic && dt.BuiltinType == 0)
                Log.Error(dt.Source, ErrorCode.E4112, "Type cannot be declared 'intrinsic' because it is not an intrinsic type");

            if (dt.IsGenericDefinition)
                VerifyGenericConstraints(dt);

            var oldType = Type;
            Type = dt;

            if (dt.Base != null)
            {
                if (dt.Base.HasAttribute(Essentials.TargetSpecificTypeAttribute))
                    Log.Error(dt.Source, ErrorCode.E0000, "Cannot use a target specific type as base class");
                else if (dt.Base.IsSealed)
                    Log.Error(dt.Source, ErrorCode.E4009, "Cannot use a sealed class as base class");
                else if (dt.Base.IsStatic)
                    Log.Error(dt.Source, ErrorCode.E0000, "Cannot use a static class as base class");
                else if (!dt.Base.IsClass && !dt.IsEnum)
                    Log.Error(dt.Source, ErrorCode.E0000, "Cannot use a " + dt.Base.TypeType.ToString().ToLower() + " as base class");
                else
                    VerifyDataTypeAccess(dt.Source, dt.Base);

                var visibility = dt.GetVisibility();

                if (dt.IsDelegate)
                {
                    VerifyVisibility(dt, visibility, dt.ReturnType);
                    foreach (var p in dt.Parameters)
                        VerifyVisibility(dt, visibility, p.Type);
                }
                else
                    VerifyVisibility(dt, visibility, dt.Base);
            }
            else
            {
                if (dt != Essentials.Object && !dt.IsStatic &&
                    !dt.HasAttribute(Essentials.TargetSpecificTypeAttribute))
                    Log.Error(dt.Source, ErrorCode.I4008, "Non-static types must have a base class");
            }

            if (dt.Initializer != null)
                OnInitializer(dt.Initializer, dt);
            if (dt.Finalizer != null)
                OnFinalizer(dt.Finalizer, dt);

            foreach (var f in dt.EnumerateFunctions())
                OnFunction(f, dt);
            foreach (var f in dt.EnumerateFields())
                OnField(f, dt);

            foreach (var f in dt.Literals)
                OnLiteral(f, dt);
            foreach (var f in dt.Properties)
                OnProperty(f, dt);
            foreach (var f in dt.Events)
                OnEvent(f, dt);
            foreach (var c in dt.Constructors)
                OnConstructor(c, dt);

            foreach (var m in dt.EnumerateMembers())
            {
                // Verify that name is not the same as enclosing type
                if (m.Name == dt.Name)
                    Log.Error(m.Source, ErrorCode.E0000, m.UnoName.Quote() + ": Member names cannot be the same as their enclosing types");

                var visibility = m.GetVisibility();
                VerifyVisibility(m, visibility, m.ReturnType);

                if (m is ParametersMember)
                    foreach (var p in (m as ParametersMember).Parameters)
                        VerifyVisibility(m, visibility, p.Type);
            }

            switch (dt.TypeType)
            {
                case TypeType.Enum:
                {
                    if (!dt.Base.IsIntegralType)
                        Log.Error(dt.Source, ErrorCode.E0000, "Enum base type must be integral");
                    break;
                }
                case TypeType.Delegate:
                {
                    var delegateType = dt as DelegateType;
                    VerifyVariableType(delegateType.Source, delegateType.ReturnType);
                    VerifyParameterList(delegateType, delegateType.Parameters);

                    // TODO: Verify visibility of return type and parameter types

                    foreach (var m in dt.EnumerateMembers())
                        if (!m.IsGenerated)
                            Log.Error(m.Source, ErrorCode.E0000, "Delegate types cannot contain members");

                    break;
                }
                case TypeType.Struct:
                {
                    // Verify that struct does not define default constructor or copy constructor
                    foreach (var c in dt.Constructors)
                        if (c.Parameters.Length == 0)
                            Log.Error(c.Source, ErrorCode.E4037, dt.Quote() + " cannot define a default constructor because it is a struct");
                        else if (c.Parameters.Length == 1 && c.Parameters[0].Type.Equals(dt))
                            Log.Error(c.Source, ErrorCode.E4038, dt.Quote() + " cannot define a copy constructor because it is a struct");

                    // Verify that structs have no circular layout inlinings
                    // TODO: This implementation assumes only one field ever can cause a circular reference WTF?
                    var cycleField = ContainsCircularStructs(dt as StructType, new HashSet<StructType>());
                    if (cycleField != null)
                        Log.Error(cycleField.Source, ErrorCode.E4129, "Struct member " + cycleField.UnoName.Quote() + " causes a cycle in the struct layout");
                    break;
                }
                case TypeType.Class:
                {
                    if (dt.IsStatic)
                    {
                        if (dt.Base != null)
                            Log.Error(dt.Source, ErrorCode.E4005, "Static classes cannot specify a base class");
                        if (dt.IsAbstract)
                            Log.Error(dt.Source, ErrorCode.E4006, "Static classes cannot be declared 'abstract'");
                        if (dt.IsSealed)
                            Log.Error(dt.Source, ErrorCode.E4007, "Static classes cannot be declared 'sealed'");

                        foreach (var m in dt.EnumerateMembers())
                            if (m.MemberType != MemberType.Literal && !m.IsStatic)
                                Log.Error(m.Source, ErrorCode.E4013, "Non-static members are not allowed in static class");
                    }
                    else if (!dt.IsAbstract)
                    {
                        var abstractMembers = new HashSet<Member>();

                        for (var bt = dt.Base; bt != null; bt = bt.Base)
                            foreach (var f in bt.EnumerateFunctions())
                                if (f.IsAbstract)
                                    abstractMembers.Add(f.MasterDefinition);

                        for (var bt = dt; abstractMembers.Count > 0 && bt != null; bt = bt.Base)
                            foreach (var f in bt.EnumerateFunctions())
                                if ((f as Method)?.OverriddenMethod != null)
                                    abstractMembers.Remove((f as Method).OverriddenMethod.MasterDefinition);

                        foreach (var f in abstractMembers)
                            Log.Error(dt.Source, ErrorCode.E4036, dt.Quote() + " does not implement abstract member " + f.Quote());
                    }
                    break;
                }
            }

            // Verify that all interfaces are implemented
            if (!dt.IsInterface)
            {
                for (int i = 0; i < dt.Interfaces.Length; i++)
                {
                    var it = dt.Interfaces[i];

                    if (BaseTypeImplementsInterfaceTypeRecursive(dt.Base, it))
                        continue;

                    foreach (var f in it.EnumerateFunctions())
                    {
                        var m = f as Method;

                        if (m != null && !dt.InterfaceMethods.ContainsKey(m)
                                && !m.HasAttribute(Essentials.OptionalAttribute))
                            Log.Error(dt.Source, ErrorCode.E0000, dt.Quote() + " does not implement interface member " + f.Quote());
                    }
                }

                foreach (var e in dt.InterfaceMethods)
                {
                    var decl = e.Key;
                    var impl = e.Value;

                    if (impl.ImplementedMethod == null && !impl.IsPublic)
                        Log.Error(impl.Source, ErrorCode.E0000, "Method implementing interface method " + decl.Quote() + " must be public");
                    if (impl.IsStatic)
                        Log.Error(impl.Source, ErrorCode.E0000, "Method implementing interface method " + decl.Quote() + " cannot be static");
                    if (!impl.CompareParameters(decl))
                        Log.Error(impl.Source, ErrorCode.E0000, "Signature does not match interface declaration " + decl.Quote());
                    if (!impl.ReturnType.Equals(decl.ReturnType) && !(impl.ImplementedMethod == null && impl.ReturnType.IsImplementingInterface(decl.ReturnType)))
                        Log.Error(impl.Source, ErrorCode.E0000, "Return type does not match interface declaration " + decl.Quote());
                    if (!dt.Interfaces.Contains(decl.DeclaringType))
                        Log.Error(impl.Source, ErrorCode.I0000, dt.Quote() + " does not implement interface " + decl.DeclaringType.Quote());

                    if (decl.IsGenericDefinition)
                    {
                        var implDef = impl;

                        if (!implDef.IsGenericDefinition)
                            implDef = implDef.GenericDefinition;

                        for (int i = 0; i < decl.GenericParameters.Length; i++)
                        {
                            var baseParam = decl.GenericParameters[i];
                            var thisParam = implDef.GenericParameters[i];

                            if (!GenericConstraintsEquals(baseParam, thisParam))
                                Log.Error(thisParam.Source, ErrorCode.E0000, "The constraints for type parameter " + thisParam.Name.Quote() + " of method " + impl.Quote() + " must match the constraints for type parameter " + baseParam.Quote() + " of interface method " + decl.Quote() + ". Consider using an explicit interface implementation instead.");
                        }
                    }
                }
            }

            Type = oldType;
            return true;
        }

        public override void End(DataType dt)
        {
            PopObsolete();
        }

        bool GenericConstraintsEquals(GenericParameterType a, GenericParameterType b)
        {
            if (a.Base != b.Base ||
                a.Interfaces.Length != b.Interfaces.Length)
                return false;

            for (int i = 0; i < a.Interfaces.Length; i++)
                if (a.Interfaces[i] != b.Interfaces[i])
                    return false;

            if (a.ConstraintType != b.ConstraintType ||
                a.Constructors.Count != b.Constructors.Count)
                return false;

            return true;
        }

        // TODO: This is not correct. Remove this and fix dt.InterfaceTypes
        bool BaseTypeImplementsInterfaceTypeRecursive(DataType bt, InterfaceType it)
        {
            if (bt == null)
                return false;

            foreach (var ii in bt.Interfaces)
                if (ii == it)
                    return true;

            return BaseTypeImplementsInterfaceTypeRecursive(bt.Base, it);
        }

        Field ContainsCircularStructs(StructType st, HashSet<StructType> visitedTypes)
        {
            foreach (var f in st.Fields)
            {
                if (!f.IsStatic && f.ReturnType is StructType)
                {
                    var ft = f.ReturnType as StructType;

                    if (visitedTypes.Contains(ft))
                        return f;

                    visitedTypes.Add(f.ReturnType as StructType);

                    if (ContainsCircularStructs(f.ReturnType as StructType, visitedTypes) != null)
                        return f;

                    visitedTypes.Remove(f.ReturnType as StructType);
                }
            }

            return null;
        }

        void VerifyGenericConstraints(DataType dt)
        {
            if (dt.IsSubclassOf(Essentials.Attribute))
                Log.Error(dt.Source, ErrorCode.E0000, "Generic type cannot inherit " + Essentials.Attribute.Quote());

            foreach (var gt in dt.GenericParameters)
            {
                if (gt.ConstraintType != GenericConstraintType.Class)
                {
                    if (gt.Constructors.Count > 0)
                        Log.Error(gt.Constructors[0].Source, ErrorCode.E0000, "'new()' constraint requires 'class' constraint");

                    if (gt.Base != Essentials.Object)
                        Log.Error(gt.Source, ErrorCode.E0000, "base type constraint requires 'class' constraint");
                }
            }

            foreach (var pt in dt.GenericParameterizations)
            {
                if (!Environment.IsGeneratingCode)
                    if (pt.Fields.Count != dt.Fields.Count ||
                        pt.Methods.Count != dt.Methods.Count ||
                        pt.Properties.Count != dt.Properties.Count ||
                        pt.Events.Count != dt.Events.Count ||
                        pt.Constructors.Count != dt.Constructors.Count)
                        Log.Error(pt.Source, ErrorCode.I0000, "IL integrity error in " + pt.Quote());

                if (pt.GenericArguments.Length != dt.GenericParameters.Length)
                {
                    Log.Error(pt.Source, ErrorCode.I0000, "pt.GenericTypeArguments.Length != dt.GenericTypeParameters.Length");
                    continue;
                }

                for (int i = 0; i < pt.GenericArguments.Length; i++)
                {
                    var gt = dt.GenericParameters[i];
                    var at = pt.GenericArguments[i];

                    switch (gt.ConstraintType)
                    {
                        case GenericConstraintType.Class:
                            if (!at.IsReferenceType)
                                Log.Error(pt.Source, ErrorCode.E0000, at.VerboseName.Quote() + " cannot be used as argument for " + gt.Quote() + " because it is not a reference type");
                            else if (!at.IsSubclassOfOrEqualConstrained(gt.Base, pt))
                                Log.Error(pt.Source, ErrorCode.E0000, at.VerboseName.Quote() + " cannot be used as argument for " + gt.Quote() + " because it does not inherit " + gt.Base.Quote());
                            break;

                        case GenericConstraintType.Struct:
                            if (!at.IsValueType)
                                Log.Error(pt.Source, ErrorCode.E0000, at.VerboseName.Quote() + " cannot be used as argument for " + gt.Quote() + " because it is not a value type");
                            break;
                    }

                    var defCtor = at.TryGetDefaultConstructor();

                    if (gt.Constructors.Count > 0 && (
                            defCtor == null ||
                            !defCtor.IsPublic && (
                                !Environment.IsGeneratingCode ||
                                !Backend.Has(TypeOptions.IgnoreProtection)
                            )
                        ))
                        Log.Error(pt.Source, ErrorCode.E0000, at.VerboseName.Quote() + " cannot be used as argument for " + gt.Quote() + " because it does not provide a public parameterless constructor");

                    if (!Environment.IsGeneratingCode)
                        foreach (var it in gt.Interfaces)
                            if (!at.IsImplementingInterface(it))
                                Log.Error(pt.Source, ErrorCode.E0000, at.VerboseName.Quote() + " cannot be used as argument for " + gt.Quote() + " because it does not implement the interface " + it.Quote());
                }
            }
        }

        void VerifyVisibility(SourceObject owner, Visibility visibility, DataType dt)
        {
            if (Backend.Has(TypeOptions.IgnoreProtection) &&
                Environment.IsGeneratingCode)
                return;

            if (VerifyAccessibleEntity(owner.Source, dt) &&
                !visibility.IsVisibile(dt))
                Log.Error(owner.Source, ErrorCode.E4128, dt.Quote() + " is less accessible than " + owner.Quote());
            else if (dt.IsGenericParameterization)
                foreach (var pt in dt.GenericArguments)
                    VerifyVisibility(owner, visibility, pt);
        }
    }
}
