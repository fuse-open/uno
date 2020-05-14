using System;
using System.Collections.Generic;
using Uno.Compiler.API.Backends;
using Uno.Compiler.API.Domain;
using Uno.Compiler.API.Domain.Graphics;
using Uno.Compiler.API.Domain.IL;
using Uno.Compiler.API.Domain.IL.Expressions;
using Uno.Compiler.API.Domain.IL.Members;
using Uno.Compiler.API.Domain.IL.Types;

namespace Uno.Compiler.Core.IL.Validation
{
    public partial class ILVerifier : CompilerPass
    {
        Expression _current;
        Expression _parent;

        bool _isInsideObsolete;
        readonly Stack<bool> _insideObsoleteStack = new Stack<bool>();
        readonly Dictionary<Namescope, string> _mangledNames = new Dictionary<Namescope, string>();
        readonly List<MetaStage> _stageStack = new List<MetaStage> {MetaStage.Undefined};
        readonly FinallyVerifier _finallyVerifier;
        readonly Stack<Lambda> _lambdas = new Stack<Lambda>();
        Lambda _lambda => _lambdas.Count > 0 ? _lambdas.Peek() : null;

        public ILVerifier(CompilerPass parent)
            : base(parent)
        {
            _finallyVerifier = new FinallyVerifier(parent);
        }

        void PushObsolete(IEntity entity)
        {
            _insideObsoleteStack.Push(_isInsideObsolete);
            _isInsideObsolete = _isInsideObsolete || entity.HasAttribute(Essentials.ObsoleteAttribute);
        }

        void PopObsolete()
        {
            _isInsideObsolete = _insideObsoleteStack.Pop();
        }

        public void VerifyConstUsage(Source src, Member member, Namescope scope)
        {
            VerifyConstUsage(src, member, scope, IsObsolete(scope));
        }

        public void VerifyConstUsage(Source src, Member member, Function scope)
        {
            scope.AssignAttributes();
            VerifyConstUsage(src, member, 
                scope.DeclaringType, 
                scope.HasAttribute(Essentials.ObsoleteAttribute) ||
                   IsObsolete(scope.DeclaringType));
        }

        void VerifyConstUsage(Source src, Member member, Namescope scope, bool isInsideObsolete)
        {
            member.AssignAttributes();

            if (!VerifyAccessibleEntity(src, member, scope as DataType ?? DataType.Null, scope) ||
                !VerifyObsolete(src, member, isInsideObsolete))
                return;

            for (var dt = member.DeclaringType; dt != null; dt = dt.ParentType)
            {
                dt.AssignAttributes();
                if (!VerifyAccessibleEntity(src, dt, scope as DataType ?? DataType.Null, scope) ||
                    !VerifyObsolete(src, dt, isInsideObsolete))
                    return;
            }
        }

        bool IsObsolete(Namescope scope)
        {
            do
            {
                scope.AssignAttributes();

                var entity = scope as IEntity;
                if (entity != null && entity.HasAttribute(Essentials.ObsoleteAttribute))
                    return true;

                scope = scope.Parent;
            } while (scope != null);

            return false;
        }

        void VerifyProtection(SourceObject e, Modifiers modifiers)
        {
            switch (modifiers & Modifiers.ProtectionModifiers)
            {
                case Modifiers.Private:
                case Modifiers.Protected:
                case Modifiers.Public:
                case Modifiers.Internal:
                case Modifiers.Protected | Modifiers.Internal:
                    break;
                case 0:
                    Log.Error(e.Source, ErrorCode.I4110, "No protection modifiers");
                    break;
                default:
                    Log.Error(e.Source, ErrorCode.E4109, "More than one protection modifier");
                    break;
            }
        }

        void VerifyModifiers(SourceObject e, Modifiers modifiers, Modifiers allowedModifiers)
        {
            if ((modifiers & ~allowedModifiers) != 0)
                Log.Error(e.Source, ErrorCode.E4001, (modifiers & ~allowedModifiers).ToLiteral().Quote() + " is not valid for " + e.Quote());
        }

        bool IsPublicMetaProperty
        {
            get
            {
                if (MetaProperty != null && MetaProperty.IsPublic)
                {
                    for (var cn = Namescope; cn != null; cn = cn.Parent)
                    {
                        switch (cn.NamescopeType)
                        {
                            case NamescopeType.DataType:
                                if (!((DataType) cn).IsPublic)
                                    return false;
                                break;
                            case NamescopeType.BlockBase:
                                if (((BlockBase) cn).BlockType != BlockType.Block ||
                                    !((Block) cn).IsPublic)
                                    return false;
                                break;
                            case NamescopeType.Namespace:
                                return true;
                            default:
                                return false;
                        }
                    }
                }

                return false;
            }
        }

        bool VerifyAccessibleEntity(Source src, IEntity entity)
        {
            if (Environment.IsGeneratingCode)
            {
                if (entity.IsStripped)
                {
                    Log.Error(src, ErrorCode.I0000,
                        entity.Quote() + " is referenced but stripped from export (internal error)");
                    return false;
                }

                if (Backend.Has(TypeOptions.IgnoreProtection))
                    return true;

                if (!entity.IsAccessibleFrom(Type.Source))
                {
                    Log.Error(src, ErrorCode.E0000,
                        entity.Quote() + " is not accessible from " + Function.Quote() + " because " +
                        entity.Package.Name + " isn't referenced by " + Type.Package.Name);
                    return false;
                }
            }

            return VerifyAccessibleEntity(src, entity, Type, ((Entity)Function ?? Type) ?? Block, IsPublicMetaProperty);
        }

        bool VerifyAccessibleEntity(Source src, IEntity entity, DataType type, Entity scope, bool isPublicMetaProperty = false)
        {
            var declType = entity.DeclaringType ?? DataType.Null;

            if (entity.IsProtected)
            {
                var pt = type;

                do
                    if (pt.IsSubclassOf(declType))
                        return true;
                while ((pt = pt.ParentType) != null);
            }

            if (entity.IsPublic ||
                entity.IsInternal && type.Package == entity.Package ||
                declType.MasterDefinition == type.MasterDefinition ||
                type.MasterDefinition.IsChildClassOf(declType.MasterDefinition) ||
                entity.IsInternal && entity.Package.InternalsVisibleTo.Contains(type.Package.Name) ||
                Environment.IsGeneratingCode && type.IsGenerated)
                return true;

            var msg = entity.Quote() + " is not accessible from " + (
                        isPublicMetaProperty
                            ? "public meta property " + MetaProperty.Quote()
                            : (scope != null ? scope.Quote() : "this scope")
                      );
            Log.Error(src, ErrorCode.E4040, msg);
            return false;
        }

        bool VerifyExportableEntity(Source src, IEntity entity)
        {
            if (entity.HasAttribute(Essentials.DontExportAttribute))
            {
                if (MetaProperty != null ||
                    Function == null ||
                    Function is ShaderFunction ||
                    Function.HasAttribute(Essentials.DontExportAttribute))
                    return true;

                var dt = Function.DeclaringType;

                do
                    if (dt.HasAttribute(Essentials.DontExportAttribute))
                        return true;
                while ((dt = dt.ParentType) != null);

                Log.Error(src, ErrorCode.E0000, entity.Quote() + " cannot be used in this context because it is marked with " + Essentials.DontExportAttribute.AttributeString);
                return false;
            }

            return true;
        }

        void VerifyMemberAccess(Source src, Member member)
        {
            if (!VerifyAccessibleEntity(src, member))
                return;
            if (!VerifyExportableEntity(src, member))
                return;
            VerifyDataTypeAccess(src, member.DeclaringType,
                VerifyObsolete(src, member));
        }

        void VerifyMemberAccess(Source src, Expression obj, Member member)
        {
            if (obj != null)
            {
                if (obj is Base && member.IsAbstract)
                    Log.Error(src, ErrorCode.E4051, member.Quote() + " is abstract");
                else if (member.IsStatic)
                    Log.Error(src, ErrorCode.I4049, member.Quote() + " is static");
                else if (!obj.ReturnType.IsSubclassOfOrEqual(member.DeclaringType) && !obj.ReturnType.IsImplementingInterface(member.DeclaringType))
                    Log.Error(src, ErrorCode.E4050, member.Quote() + " is not a member of " + obj.ReturnType.Quote());
            }
            else
                if (!member.IsStatic)
                    Log.Error(src, ErrorCode.I4052, member.Quote() + " is not a static member");

            VerifyMemberAccess(src, member);
        }

        bool VerifyDataTypeAccess(Source src, DataType dt, bool hasObsolete = false)
        {
            if (Type != null && Type.HasTypeParameter(dt) ||
                dt.IsGenericParameter)
                return true;

            if (dt.IsGenericParameterization)
                foreach (var t in dt.GenericArguments)
                    VerifyDataTypeAccess(src, t);

            for (var pt = dt; !hasObsolete && pt != null; pt = pt.ParentType)
                hasObsolete = VerifyObsolete(src, pt);

            if (!VerifyAccessibleEntity(src, dt) ||
                !VerifyExportableEntity(src, dt) ||
                dt.IsArray && !VerifyDataTypeAccess(src, dt.ElementType, hasObsolete))
                return false;

            return !dt.IsNestedType || VerifyDataTypeAccess(src, dt.ParentType);
        }

        void VerifyVariableType(Source src, DataType dt)
        {
            if (dt.IsArray)
                VerifyVariableType(src, dt.ElementType);
            else if (dt.IsFlattenedDefinition)
                Log.Error(src, ErrorCode.I4041, dt.Quote() + " cannot be used as variable because it is a generic type definition");

            if (dt.IsGenericParameterization)
                foreach (var t in dt.GenericArguments)
                    VerifyVariableType(src, t);

            if (dt is FixedArrayType)
            {
                var fat = dt as FixedArrayType;

                if (fat.ElementType is FixedArrayType)
                    Log.Error(src, ErrorCode.E0000, "Invalid 'fixed' array of 'fixed' array");
                else if (fat.OptionalSize == null)
                    Log.Error(src, ErrorCode.E0000, "Unknown size of 'fixed' array");
                else if (!(fat.OptionalSize is Constant || fat.OptionalSize is RuntimeConst))
                    Log.Error(src, ErrorCode.E0000, "'fixed' array size must be a constant or a runtime shader constant");
            }
        }

        void VerifyParameterList(Entity entity, Parameter[] list)
        {
            for (int i = 0; i < list.Length; i++)
            {
                var p = list[i];
                VerifyAttributes(p, p.Attributes);

                foreach (var attr in p.Attributes)
                {
                    DataType type = attr.ReturnType;

                    if (type == Essentials.CallerPackageNameAttribute && p.Type != Essentials.String)
                        Log.Error(attr.Source, ErrorCode.E0000, type.Quote() + " can only be used on string parameters");
                    if (type == Essentials.CallerFilePathAttribute && p.Type != Essentials.String)
                        Log.Error(attr.Source, ErrorCode.E0000, type.Quote() + " can only be used on string parameters");
                    if (type == Essentials.CallerMemberNameAttribute && p.Type != Essentials.String)
                        Log.Error(attr.Source, ErrorCode.E0000, type.Quote() + " can only be used on string parameters");
                    if (type == Essentials.CallerLineNumberAttribute && p.Type != Essentials.Int)
                        Log.Error(attr.Source, ErrorCode.E0000, type.Quote() + " can only be used on int parameters");
                }

                VerifyVariableType(p.Source, p.Type);

                switch (p.Type.TypeType)
                {
                    case TypeType.FixedArray:
                        switch (p.Modifier)
                        {
                            case ParameterModifier.Const:
                            case ParameterModifier.Ref:
                                break;

                            default:
                                Log.Error(p.Source, ErrorCode.E0000, "'fixed' array parameters must specify 'const' or 'ref' modifier");
                                break;
                        }
                        break;

                    default:
                        switch (p.Modifier)
                        {
                            case ParameterModifier.Const:
                                Log.Error(p.Source, ErrorCode.E0000, "Only fixed arrays parameters can specify 'const' modifier");
                                break;

                            case ParameterModifier.This:
                                if (!(entity is Method))
                                    Log.Error(p.Source, ErrorCode.E0000, "'this' parameter modifier can only be used on extension methods");
                                else if (i > 0)
                                    Log.Error(p.Source, ErrorCode.E0000, "'this' parameter modifier can only be specified for the first parameter");

                                break;
                        }
                        break;
                }

                for (int j = 0; j < i; j++)
                    if (p.Name == list[j].Name)
                        Log.Error(p.Source, ErrorCode.E4108, entity.Quote() + " already has a parameter with name " + p.Name.Quote());
            }
        }

        void VerifyArguments(Source src, IParametersEntity member, params Expression[] args)
        {
            if (args.Length != member.Parameters.Length)
                Log.Error(src, ErrorCode.I4042, "Arguments do not match method signature. " + member.Quote() + " requires " + member.Parameters.Length + ", but " + args.Length + " were given");
            else for (int i = 0; i < args.Length; i++)
                if (args[i] is AddressOf && (args[i] as AddressOf).AddressType == AddressType.Ref && member.Parameters[i].Modifier != ParameterModifier.Ref)
                    Log.Error(src, ErrorCode.I0000, "Arguments do not match method signature. Argument #" + (i + 1) + " to " + member.Quote() + " was unexpectedly passed using 'ref' modifier");
                else if (args[i] is AddressOf && (args[i] as AddressOf).AddressType == AddressType.Out && member.Parameters[i].Modifier != ParameterModifier.Out)
                    Log.Error(src, ErrorCode.I0000, "Arguments do not match method signature. Argument #" + (i + 1) + " to " + member.Quote() + " was unexpectedly passed using 'out' modifier");
                else if (args[i] is AddressOf && !member.Parameters[i].IsReference)
                    Log.Error(src, ErrorCode.I0000, "Arguments do not match method signature. Argument #" + (i + 1) + " to " + member.Quote() + " was unexpectedly passed as reference");
                else if (member.Parameters[i].Modifier == ParameterModifier.Ref && !(args[i] is AddressOf && (args[i] as AddressOf).AddressType == AddressType.Ref))
                    Log.Error(src, ErrorCode.I0000, "Arguments do not match method signature. Argument #" + (i + 1) + " to " + member.Quote() + " was not passed using required 'ref' modifier");
                else if (member.Parameters[i].Modifier == ParameterModifier.Out && !(args[i] is AddressOf && (args[i] as AddressOf).AddressType == AddressType.Out))
                    Log.Error(src, ErrorCode.I0000, "Arguments do not match method signature. Argument #" + (i + 1) + " to " + member.Quote() + " was not passed using required 'out' modifier");
                else if (member.Parameters[i].IsReference && !(args[i] is AddressOf))
                    Log.Error(src, ErrorCode.I0000, "Arguments do not match method signature. Argument #" + (i + 1) + " to " + member.Quote() + " was not passed as reference");
                else if (!args[i].ReturnType.Equals(member.Parameters[i].Type))
                    Log.Error(src, ErrorCode.I4043, "Arguments do not match method signature. Argument #" + (i + 1) + " to " + member.Quote() + " has an incompatible type " + args[i].ReturnType.Quote() + ".");
        }

        bool VerifyObsolete(Source s, IEntity e)
        {
            return VerifyObsolete(s, e, _isInsideObsolete);
        }

        bool VerifyObsolete(Source s, IEntity e, bool isInsideObsolete)
        {
            // do not warn about obsoletion on the definition itself, or inside obsolete
            // classes.
            if (Environment.IsGeneratingCode || s == e.Source || isInsideObsolete)
                return true;

            var obsoleteObj = e.TryGetAttributeObject(Essentials.ObsoleteAttribute);
            if (obsoleteObj == null)
                return true;

            // we do not want warnings from auto-generated meta-properties
            if (MetaProperty != null)
                return true;

            // YUCK: We don't have access to the ObsoleteAttribute object itself, so instead
            // we need to parse it's constructor arguments directly from the IL
            //
            // There's three possible constructors:
            //
            // ObsoleteAttribute() : Message(null), IsError(false)
            // ObsoleteAttribute(string message) : Message(message), IsError(false)
            // ObsoleteAttribute(string message, bool isError) : Message(null), IsError(isError)
            //

            string message = null;
            bool isError = false;
            if (obsoleteObj.Arguments.Length > 0 &&
                obsoleteObj.Arguments[0].ExpressionType == ExpressionType.Constant)
            {
                var constant = (Constant)obsoleteObj.Arguments[0];
                if (constant.Value is string)
                    message = (string)constant.Value;
            }
            if (obsoleteObj.Arguments.Length > 1 &&
                obsoleteObj.Arguments[1].ExpressionType == ExpressionType.Constant)
            {
                var constant = (Constant)obsoleteObj.Arguments[1];
                if (constant.Value is bool)
                    isError = (bool)constant.Value;
            }

            var obsoleteMessage = e.MasterDefinition.Quote() + " is obsolete";
            if (message != null)
                obsoleteMessage += ": '" + message + "'";

            if (isError)
                Log.Error(s, ErrorCode.E4140, obsoleteMessage);
            else
                Log.Warning1(s, ErrorCode.W4139, obsoleteMessage);

            return false;
        }

        void VerifyAttributes(SourceObject owner, NewObject[] attributes)
        {
            foreach (var attr in attributes)
            {
                if (!attr.ReturnType.IsSubclassOf(Essentials.Attribute))
                    Log.Error(attr.Source, ErrorCode.E0000, attr.ReturnType.AttributeString + " cannot be used as an attribute because it does not inherit " + Essentials.Attribute.Quote());

                foreach (var arg in attr.Arguments)
                    if (arg.ExpressionType != ExpressionType.Constant)
                        Log.Error(arg.Source, ErrorCode.E0000, "Attribute argument must be constant");

                VerifyAttributeUsage(attr.Source, owner, attr.ReturnType);
            }
        }

        void VerifyAttributeUsage(Source src, SourceObject owner, DataType attribute)
        {
            var targetsObj = attribute.TryGetAttribute(Essentials.AttributeUsageAttribute);

            if (targetsObj != null)
            {
                var targets = (AttributeTargets) (int) targetsObj;

                if (targets.HasFlag(AttributeTargets.All))
                    return;

                if (owner is DataType)
                {
                    var dt = owner as DataType;

                    switch (dt.TypeType)
                    {
                        case TypeType.Class:
                            if (targets.HasFlag(AttributeTargets.Class))
                                return;
                            break;
                        case TypeType.Struct:
                            if (targets.HasFlag(AttributeTargets.Struct))
                                return;
                            break;
                        case TypeType.Enum:
                            if (targets.HasFlag(AttributeTargets.Enum))
                                return;
                            break;
                        case TypeType.Interface:
                            if (targets.HasFlag(AttributeTargets.Interface))
                                return;
                            break;
                        case TypeType.Delegate:
                            if (targets.HasFlag(AttributeTargets.Delegate))
                                return;
                            break;
                    }
                }
                else if (owner is Member)
                {
                    var m = owner as Member;

                    switch (m.Prototype.MemberType)
                    {
                        case MemberType.Literal:
                            if (targets.HasFlag(AttributeTargets.Field))
                                return;
                            break;
                        case MemberType.Field:
                            if (targets.HasFlag(AttributeTargets.Field) ||
                                targets.HasFlag(AttributeTargets.Property) && ((Field)m.Prototype).DeclaringMember is Property ||
                                targets.HasFlag(AttributeTargets.Event) && ((Field)m.Prototype).DeclaringMember is Event)
                                return;
                            break;
                        case MemberType.Method:
                            if (targets.HasFlag(AttributeTargets.Method) ||
                                targets.HasFlag(AttributeTargets.Property) && ((Method)m.Prototype).DeclaringMember is Property ||
                                targets.HasFlag(AttributeTargets.Event) && ((Method)m.Prototype).DeclaringMember is Event)
                                return;
                            break;

                        case MemberType.Cast:
                        case MemberType.Operator:
                        case MemberType.Finalizer:
                            if (targets.HasFlag(AttributeTargets.Method))
                                return;
                            break;
                        case MemberType.Constructor:
                            if (targets.HasFlag(AttributeTargets.Constructor))
                                return;
                            break;
                        case MemberType.Property:
                            if (targets.HasFlag(AttributeTargets.Property))
                                return;
                            break;
                        case MemberType.Event:
                            if (targets.HasFlag(AttributeTargets.Event))
                                return;
                            break;
                    }
                }
                else if (owner is Parameter)
                    if (targets.HasFlag(AttributeTargets.Parameter))
                        return;

                Log.Error(src, ErrorCode.E0000, "Attribute " + attribute.AttributeString + " can only be used on " + targets.ToLiteral().Quote());
            }
            else if (attribute.Base != null)
                VerifyAttributeUsage(src, owner, attribute.Base);
        }
    }
}
