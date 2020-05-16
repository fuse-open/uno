using Uno.Compiler.API.Domain;
using Uno.Compiler.API.Domain.IL;
using Uno.Compiler.API.Domain.IL.Members;

namespace Uno.Compiler.Core.IL.Validation
{
    public partial class ILVerifier
    {
        public override bool Begin(Function f)
        {
            if (f.CanLink)
                return false;

            if (Environment.IsGeneratingCode &&
                f.Body == null && f.IsExtern && !f.IsAbstract && !f.IsGenerated && Environment.CanExport(f))
                Log.Error(f.Source, ErrorCode.E0000, f.Quote() + " is marked as 'extern', but target does not provide an implementation");

            PushObsolete(f);
            return true;
        }

        public override void End(Function f)
        {
            PopObsolete();
        }

        void OnLiteral(Literal f, DataType dt)
        {
            PushObsolete(f);
            VerifyAttributes(f, f.Attributes);
            VerifyProtection(f, f.Modifiers);
            VerifyModifiers(f, f.Modifiers, Modifiers.LiteralMember);
            VerifyVariableType(f.Source, f.ReturnType);
            PopObsolete();

            if (!dt.IsStruct && !dt.IsClass && !dt.IsEnum)
                Log.Error(dt.Source, ErrorCode.E0000, "'const' literals are only allowed in classes and structs");
        }

        void OnField(Field f, DataType dt)
        {
            PushObsolete(f);
            VerifyAttributes(f, f.Attributes);
            VerifyProtection(f, f.Modifiers);
            VerifyModifiers(f, f.Modifiers, Modifiers.FieldMember);
            VerifyVariableType(f.Source, f.ReturnType);
            PopObsolete();

            if (!f.IsStatic && dt.HasAttribute(Essentials.TargetSpecificTypeAttribute))
                Log.Error(f.Source, ErrorCode.E0000, "Opaque types can only contain static fields");
            else if (f.FieldModifiers.HasFlag(FieldModifiers.Const))
                Log.Error(f.Source, ErrorCode.I4035, "Field cannot be 'const'");
            else if (!dt.IsStruct && !dt.IsClass)
                Log.Error(dt.Source, ErrorCode.I0000, "Only classes and structs can contain fields");
            else if (f.ReturnType.IsFixedArray && !dt.IsClass)
                Log.Error(f.Source, ErrorCode.E0000, "Only classes can contain 'fixed' array fields");
        }

        void OnProperty(Property f, DataType dt)
        {
            PushObsolete(f);
            VerifyAttributes(f, f.Attributes);
            VerifyProtection(f, f.Modifiers);
            VerifyModifiers(f, f.Modifiers, Modifiers.PropertyMember);
            VerifyVariableType(f.Source, f.ReturnType);
            PopObsolete();

            if (f.OverriddenProperty != null)
            {
                if (f.OverriddenProperty.GetMethod != null) {
                    if (f.GetMethod == null)
                        Log.Error(f.Source, ErrorCode.E4130, f.Quote() + " must include a get accessor to override " + f.OverriddenProperty.Quote());
                } else {
                    if (f.GetMethod != null)
                        Log.Error(f.Source, ErrorCode.E4131, f.Quote(".get") + " cannot override " + f.OverriddenProperty.Quote() + " because it does not have an overridable get accessor");
                }

                if (f.OverriddenProperty.SetMethod != null) {
                    if (f.SetMethod == null)
                        Log.Error(f.Source, ErrorCode.E4132, f.Quote() + " must include a set accessor to override " + f.OverriddenProperty.Quote());
                } else {
                    if (f.SetMethod != null)
                        Log.Error(f.Source, ErrorCode.E4133, f.Quote(".set") + " cannot override " + f.OverriddenProperty.Quote() + " because it does not have an overridable set accessor");
                }
            }

            if (!dt.IsStruct && !dt.IsClass && !dt.IsInterface)
                Log.Error(dt.Source, ErrorCode.E0000, "Properties are only allowed in classes, structs and interfaces");
        }

        void OnEvent(Event f, DataType dt)
        {
            PushObsolete(f);
            VerifyAttributes(f, f.Attributes);
            VerifyProtection(f, f.Modifiers);
            VerifyModifiers(f, f.Modifiers, Modifiers.EventMember);
            VerifyVariableType(f.Source, f.ReturnType);
            PopObsolete();

            if (f.OverriddenEvent != null)
            {
                if (f.OverriddenEvent.AddMethod != null) {
                    if (f.AddMethod == null)
                        Log.Error(f.Source, ErrorCode.E4134, f.Quote() + " must include an add accessor to override " + f.OverriddenEvent.Quote());
                } else {
                    if (f.AddMethod != null)
                        Log.Error(f.Source, ErrorCode.E4135, f.Quote(".add") + " cannot override " + f.OverriddenEvent.Quote() + " because it does not have an overridable add accessor");
                }

                if (f.OverriddenEvent.RemoveMethod != null) {
                    if (f.RemoveMethod == null)
                        Log.Error(f.Source, ErrorCode.E4136, f.Quote() + " must include a remove accessor to override " + f.OverriddenEvent.Quote());
                } else {
                    if (f.RemoveMethod != null)
                        Log.Error(f.Source, ErrorCode.E4137, f.Quote(".remove") + " cannot override " + f.OverriddenEvent.Quote() + " because it does not have an overridable remove accessor");
                }
            }

            if (!dt.IsStruct && !dt.IsClass && !dt.IsInterface)
                Log.Error(dt.Source, ErrorCode.E0000, "Events are only allowed in classes, structs and interfaces");
        }

        // TODO: Clean up later
        void OnFunction(Function f, DataType dt)
        {
            if (f.CanLink)
                return;

            if (f != dt.Initializer && f != dt.Finalizer)
            {
                var validModifiers = Modifiers.FunctionMember | (
                        f.IsMethod ? Modifiers.MethodModifiers :
                        f.IsCast ? Modifiers.CastModifiers : 0
                    ) | (
                        dt.IsIntrinsic || dt.IsDelegate || dt.IsEnum
                            ? Modifiers.Intrinsic
                            : 0
                    );

                if (dt.IsValueType || dt.HasAttribute(Essentials.TargetSpecificTypeAttribute))
                    validModifiers &= ~(Modifiers.Abstract | Modifiers.Virtual);

                VerifyProtection(f, f.Modifiers);
                VerifyModifiers(f, f.Modifiers, validModifiers);
            }

            PushObsolete(f);
            VerifyAttributes(f, f.Attributes);
            VerifyVariableType(f.Source, f.ReturnType);
            VerifyParameterList(f, f.Parameters);
            PopObsolete();

            if (!f.HasBody && !f.IsExtern && !f.IsAbstract && !f.IsIntrinsic)
                Log.Error(f.Source, ErrorCode.E4088, "Pure method must be declared 'abstract' or 'extern'");
            else if (f.IsIntrinsic && f.HasBody)
                Log.Error(f.Source, ErrorCode.E0000, "'intrinsic' is not valid for non-pure methods");

            switch (f.MemberType)
            {
                case MemberType.Method:
                {
                    var m = f as Method;

                    if (m.IsGenericDefinition)
                        VerifyGenericConstraints(m.GenericType);

                    if (m.OverriddenMethod != null && !m.IsVirtualOverride)
                        Log.Error(f.Source, ErrorCode.I4090, "Overriding methods must be declared using the 'override' modifier");

                    if (m.IsSealed && !m.IsVirtualOverride)
                        Log.Error(f.Source, ErrorCode.E4107, "Only overriding methods can be declared as 'sealed'");

                    if (m.IsVirtualOverride)
                    {
                        if (f.IsAbstract)
                            Log.Error(f.Source, ErrorCode.E4016, "Cannot be declared both 'abstract' and 'override'");

                        if (f.IsVirtualBase)
                            Log.Error(f.Source, ErrorCode.E4017, "Cannot be declared both 'virtual' and 'override'");

                        if (m.OverriddenMethod == null)
                            Log.Error(f.Source, ErrorCode.E4018, "No suitable method found to override");
                        else if (m.OverriddenMethod != null)
                        {
                            var modifierMask = ~(Modifiers.New | Modifiers.Abstract | Modifiers.Virtual | Modifiers.Override | Modifiers.Sealed | Modifiers.Extern | Modifiers.Generated);

                            if (!f.ReturnType.Equals(m.OverriddenMethod.ReturnType) || !f.CompareParameters(m.OverriddenMethod))
                                Log.Error(f.Source, ErrorCode.E0000, "Signature does not match base class declaration " + m.OverriddenMethod.Quote());

                            if (f.UnoName != m.OverriddenMethod.UnoName || f.Name != m.OverriddenMethod.Name)
                                Log.Error(f.Source, ErrorCode.I4091, "Name does not match base class declaration " + m.OverriddenMethod.Quote());

                            if ((f.Modifiers & modifierMask) != (m.OverriddenMethod.Modifiers & modifierMask))
                                Log.Error(f.Source, ErrorCode.E4022, "Modifiers does not match base class declaration " + m.OverriddenMethod.Quote());

                            if (!m.OverriddenMethod.IsVirtual)
                                Log.Error(f.Source, ErrorCode.E4023, "Cannot override non-virtual method " + m.OverriddenMethod.Quote());
                            if (m.OverriddenMethod.IsSealed)
                                Log.Error(f.Source, ErrorCode.E4093, "Cannot override sealed method " + m.OverriddenMethod.Quote());
                        }
                    }

                    if (m.IsAbstract)
                    {
                        if (f.IsVirtualBase)
                            Log.Error(f.Source, ErrorCode.E4017, "Cannot be declared both 'abstract' and 'virtual'");
                        if (!dt.IsAbstract)
                            Log.Error(f.Source, ErrorCode.E4014, "Abstract member is not allowed in non-abstract class");
                        if (f.HasBody)
                            Log.Error(f.Source, ErrorCode.E4095, "'abstract' is not valid for non-pure methods");
                    }

                    break;
                }
                case MemberType.Cast:
                {
                    if (!f.IsImplicitCast && !f.IsExplicitCast)
                        Log.Error(f.Source, ErrorCode.E4096, "Conversion operators must be declared as 'implicit' or 'explicit'");

                    if (!f.IsPublic || !f.IsStatic)
                        Log.Error(f.Source, ErrorCode.E4097, "Conversion operators must be declared both 'public' and 'static'");

                    if (f.Parameters.Length == 1)
                    {
                        var p = f.Parameters[0];
                        var parameterizedType = TypeBuilder.Parameterize(dt);

                        if (!f.ReturnType.Equals(parameterizedType) && !p.Type.Equals(parameterizedType))
                            Log.Error(f.Source, ErrorCode.E4098, "The return type or parameter type must be " + parameterizedType.Quote() + " for conversion operator");

                        if (f.ReturnType.Equals(p.Type))
                            Log.Error(f.Source, ErrorCode.E4099, "The return type and parameter type cannot be the same type for conversion operator");

                        if (p.Modifier != 0)
                            Log.Error(f.Parameters[0].Source, ErrorCode.E4100, "Conversion operators cannot specify parameter modifiers");
                    }
                    else
                    {
                        Log.Error(f.Source, ErrorCode.E4101, "Conversion operators must specify exactly 1 parameter");
                    }

                    break;
                }
                case MemberType.Operator:
                {
                    if (!f.IsPublic || !f.IsStatic)
                        Log.Error(f.Source, ErrorCode.E4102, "Operators must be declared both 'public' and 'static'");

                    switch (f.Parameters.Length)
                    {
                        case 1:
                        case 2:
                            break;

                        default:
                            Log.Error(f.Source, ErrorCode.E4103, "Operators must specify exactly 1 or 2 parameter(s)");
                            break;
                    }

                    var parameterizedType = TypeBuilder.Parameterize(dt);
                    var foundType = false;

                    foreach (var p in f.Parameters)
                    {
                        if (p.Modifier != 0)
                            Log.Error(p.Source, ErrorCode.E4104, "Operators cannot specify parameter modifiers");

                        if (p.Type.Equals(parameterizedType))
                            foundType = true;
                    }

                    if (!foundType)
                        Log.Error(f.Source, ErrorCode.E4105, "One of the operator parameters must have type " + parameterizedType.Quote());

                    var opString = f.OperatorString;

                    switch (opString)
                    {
                        case "==":
                        case "!=":
                        {
                            var findOp = opString == "=="
                                ? "!="
                                : "==";
                            var foundOp = false;

                            foreach (var o in dt.Operators)
                            {
                                if (o.Symbol == findOp && f.CompareParameters(o) && f.ReturnType.Equals(o.ReturnType))
                                {
                                    foundOp = true;
                                    break;
                                }
                            }

                            if (!foundOp)
                                Log.Error(f.Source, ErrorCode.E0000, f.Quote() + " requires matching " + findOp.Quote() + " to be defined");

                            break;
                        }
                    }

                    break;
                }
            }
        }

        void OnInitializer(Constructor c, DataType dt)
        {
            if (!c.IsStatic)
                Log.Error(c.Source, ErrorCode.I2025, "'static' constructor must be declared 'static'");
            else if ((c.Modifiers & ~(Modifiers.Static | Modifiers.Extern | Modifiers.Generated)) != 0)
                Log.Error(c.Source, ErrorCode.E4106, "'static' constructor can only be declared 'static'");

            if (c.Parameters.Length > 0)
                Log.Error(c.Source, ErrorCode.E4027, "'static' constructors must have an empty parameter list");

            if (!dt.IsStruct && !dt.IsClass)
                Log.Error(c.Source, ErrorCode.E0000, "'static' constructors are only allowed in classes and structs");
        }

        void OnFinalizer(Finalizer c, DataType dt)
        {
            if ((c.Modifiers & ~Modifiers.Extern) != 0)
                Log.Error(c.Source, ErrorCode.E0000, "Modifiers are not valid on finalizers");

            if (c.Parameters.Length > 0)
                Log.Error(c.Source, ErrorCode.E4027, "Finalizers must have an empty parameter list");

            if (!dt.IsClass)
                Log.Error(c.Source, ErrorCode.E0000, "Finalizers are only allowed in classes");
        }

        void OnConstructor(Constructor c, DataType dt)
        {
            if (!dt.IsStruct && !dt.IsClass)
                Log.Error(dt.Source, ErrorCode.E0000, "Constructors are only allowed in classes and structs");
        }
    }
}
