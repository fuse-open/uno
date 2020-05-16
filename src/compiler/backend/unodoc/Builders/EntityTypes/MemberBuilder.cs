using System;
using System.Collections.Generic;
using System.Linq;
using Uno.Compiler.API.Domain.IL;
using Uno.Compiler.API.Domain.IL.Expressions;
using Uno.Compiler.API.Domain.IL.Members;
using Uno.Compiler.API.Domain.IL.Types;
using Uno.Compiler.Backends.UnoDoc.Builders.Extensions;
using Uno.Compiler.Backends.UnoDoc.ViewModels;
using Uno.Compiler.Backends.UnoDoc.ViewModels.MetaData;

namespace Uno.Compiler.Backends.UnoDoc.Builders.EntityTypes
{
    public class MemberBuilder : Builder
    {
        private readonly ICommentParser _commentParser;

        public MemberBuilder(IEntityNaming naming,
                             ISyntaxGenerator syntax,
                             IExportableCheck exportable,
                             AttachedMemberCache attachedMembers,
                             ICommentParser commentParser)
                : base(naming, syntax, exportable, attachedMembers)
        {
            _commentParser = commentParser;
        }

        public void Build(Member entity, DataType parentDataType, HashSet<DocumentViewModel> target)
        {
            AddMemberToTarget(entity, parentDataType, target);
        }

        public void Build(AttachedUxProperty property, DataType parentDataType, HashSet<DocumentViewModel> target)
        {
            AddAttachedMemberToTarget(property.Name,
                                      MemberType.Property,
                                      property.UnderlyingMethod,
                                      property.ReturnType,
                                      parentDataType,
                                      target);
        }

        public void Build(AttachedUxEvent evt, DataType parentDataType, HashSet<DocumentViewModel> target)
        {
            AddAttachedMemberToTarget(evt.Name,
                                      MemberType.Event,
                                      evt.UnderlyingMethod,
                                      evt.ReturnType,
                                      parentDataType,
                                      target);
        }

        private Member ResolveOriginalDeclaration(Member member)
        {
            var method = member as Method;
            if (method?.OverriddenMethod != null)
            {
                return ResolveOriginalDeclaration(method.OverriddenMethod);
            }

            var evt = member as Event;
            if (evt?.OverriddenEvent != null)
            {
                return ResolveOriginalDeclaration(evt.OverriddenEvent);
            }

            var property = member as Property;
            if (property?.OverriddenProperty != null)
            {
                return ResolveOriginalDeclaration(property.OverriddenProperty);
            }

            return member;
        }

        private void AddMemberToTarget(Member member, DataType parentDataType, HashSet<DocumentViewModel> target)
        {
            // If this is some kind of overridden member, try to resolve the original declaration as it's the one that's
            // interesting to us
            member = ResolveOriginalDeclaration(member);

            // NOTE: Instead of just using GetUri and GetId directly on the entity to generate the ID, we use the
            // signature variants of the uri/id methods instead and prefix it with the uri and id of the containing
            // data type.
            //
            // This is done to properly support flattening inherited members into the data exchange format used by the
            // compiler backend; while the ToString() method on a type that inherits from Uno.Object without overriding
            // ToString() directly will have the ToString() method listed when using the EnumerateFlattenedMethods()
            // extension method, that method will still belong to Uno.Object and not the type that inherited the method.
            //
            // The data structure requires the type to own its own inherited methods, hence we do it like this.
            var id = parentDataType.GetUri() + "/" + member.GetUriSignature();
            var uri = GetMemberUri(member, parentDataType);

            var comment = _commentParser.Read(member);
            var ns = parentDataType.FindNamespace();
            var titles = new TitlesViewModel(Naming.GetPageTitle(member),
                                             Naming.GetIndexTitle(member),
                                             Naming.GetFullIndexTitle(member),
                                             Naming.GetNavigationTitle(member),
                                             member.FullName);
            var syntax = new SyntaxViewModel(Syntax.BuildUnoSyntax(member, parentDataType), Syntax.BuildUxSyntax(member));
            var location = new LocationViewModel(ns.FullName, ns.GetUri(), member.Source.Package.Name, member.Source.Package.Version);
            var declaredIn = new DataTypeBuilder(Naming, Syntax, Exportable, AttachedMembers, _commentParser).BuildReference(member.DeclaringType);
            var parameters = GetParameters(member);
            var returns = GetReturns(member);
            var uxProperties = GetUxProperties(member);
            var values = BuildValues(member);
            var flags = BuildFlags(member);
            var attributes = BuildAttributes(member);

            var type = GetTypeName(member, comment, uxProperties);
            var isVirtual = uri == null;

            var viewModel = new MemberViewModel(new DocumentIdViewModel(id, parentDataType.GetUri(), type, member.GetModifierNames()),
                                                new DocumentUriViewModel(id, member.GetUri(), isVirtual),
                                                titles,
                                                syntax,
                                                location,
                                                declaredIn,
                                                parameters,
                                                returns,
                                                uxProperties,
                                                values,
                                                flags,
                                                new CommentViewModel(comment),
                                                attributes,
                                                member);
            target.AddIfNotExists(viewModel);
        }

        private void AddAttachedMemberToTarget(string name, MemberType memberType, Method underlyingMethod, DataType returnType, DataType parentDataType, HashSet<DocumentViewModel> target)
        {
            var uxAttribute = underlyingMethod.Attributes.SingleOrDefault(e => e.ReturnType.QualifiedName == ExportConstants.UxAttachedPropertySetterAttributeName ||
                                                                               e.ReturnType.QualifiedName == ExportConstants.UxAttachedEventAdderAttributeName);
            if (uxAttribute == null)
            {
                throw new ArgumentException($"UX attached attribute not found on {underlyingMethod.UnoName}");
            }

            var uxAttributeName = uxAttribute.Arguments.Length == 0 ? null : uxAttribute.Arguments[0].ConstantString;
            if (string.IsNullOrWhiteSpace(uxAttributeName))
            {
                throw new ArgumentException($"UX attached attribute did not contain any on {underlyingMethod.UnoName}");
            }
            uxAttributeName = uxAttributeName.ToLowerInvariant()
                                             .Replace(".", "_");

            var id = parentDataType.GetUri() + "/" + uxAttributeName + "_" + underlyingMethod.GetUriSignature();
            var titles = new TitlesViewModel(Naming.GetPageTitle(underlyingMethod),
                                             uxAttribute.Arguments[0].ConstantString,
                                             uxAttribute.Arguments[0].ConstantString, 
                                             Naming.GetNavigationTitle(underlyingMethod),
                                             Naming.GetFullIndexTitle(underlyingMethod));
            var declaredIn = new DataTypeBuilder(Naming, Syntax, Exportable, AttachedMembers, _commentParser).BuildReference(underlyingMethod.DeclaringType);
            var parameters = GetParameters(underlyingMethod);
            var returns = GetReturns(underlyingMethod, returnType);
            var values = BuildValues(returnType);
            var source = new AttachedMemberSourceViewModel(underlyingMethod.DeclaringType.GetUri(), Naming.GetIndexTitle(underlyingMethod.DeclaringType));
            var comment = _commentParser.Read(underlyingMethod);

            if (!Exportable.IsExportableAndVisible(underlyingMethod.DeclaringType))
            {
                throw new Exception($"Found attached UX member {underlyingMethod.FullName} declared inside non-exportable class {underlyingMethod.DeclaringType.FullName}");
            }

            var viewModel = new AttachedMemberViewModel(new DocumentIdViewModel(id, parentDataType.GetUri(), "AttachedUx" + memberType.ToString("G"), new List<string>()),
                                                        new DocumentUriViewModel(id, underlyingMethod.GetUri(), true),
                                                        titles,
                                                        declaredIn,
                                                        parameters,
                                                        returns,
                                                        values,
                                                        source,
                                                        new CommentViewModel(comment),
                                                        underlyingMethod);
            target.AddIfNotExists(viewModel);
        }

        private static string GetMemberUri(Member member, DataType dataType)
        {
            var property = member as Property;
            var evt = member as Event;

            if (property?.OverriddenProperty != null)
            {
                return null;
            }
            if (evt?.OverriddenEvent != null)
            {
                return null;
            }

            return member.DeclaringType.GetUri() == dataType.GetUri() ? member.GetUri() : null;
        }

        ParameterViewModel GetParameterViewModelForParameter(Parameter param)
        {
            var suffix = "";
            var elementType = param.Type;
            while (elementType.IsArray)
            {
                elementType = elementType.ElementType;
                suffix += "[]";
            }

            return new ParameterViewModel(param.Name,
                                    GetDataTypeUri(elementType),
                                    elementType.IsVirtualType(),
                                    Naming.GetIndexTitle(elementType) + suffix,
                                    Naming.GetFullIndexTitle(elementType) + suffix);
        }

        private ParametersViewModel GetParameters(Member member)
        {
            var parameters = member.GetParametersOrNull();
            if (parameters == null)
            {
                return null;
            }

            var invisibleParams = parameters.Where(e => e.Type.IsArray
                                                          ? !Exportable.IsExportableAndVisible(e.Type.ElementType)
                                                          : !Exportable.IsExportableAndVisible(e.Type))
                                            .ToList();
            if (invisibleParams.Count > 0)
            {
                var names = new List<string>();
                foreach (var p in invisibleParams)
                {
                    var paramType = p.Type.IsArray ? p.Type.ElementType : p.Type;
                    names.Add($"{p.Name} ({paramType.FullName})");
                }
                throw new Exception($"Found {invisibleParams.Count} parameters for member {member.FullName} that have non-exportable types: {string.Join(", ", names)}");
            }

            var list = parameters.Select(param => GetParameterViewModelForParameter(param)).ToList();
            return new ParametersViewModel(list);
        }

        private ReturnsViewModel GetReturns(Member member)
        {
            if (member.ReturnType != null && member.ReturnType.FullName != ExportConstants.VoidTypeName)
            {
                return GetReturns(member, member.ReturnType);
            }
            return null;
        }

        private ReturnsViewModel GetReturns(Member member, DataType returnDataType)
        {
            var dataType = returnDataType.IsArray ? returnDataType.ElementType : returnDataType;
            var suffix = returnDataType.IsArray ? "[]" : "";

            if (!Exportable.IsExportableAndVisible(dataType))
            {
                throw new Exception($"Found return type for member {member.FullName} that have non-exportable return type: {dataType.FullName}"); 
            }

            return new ReturnsViewModel(GetDataTypeUri(dataType),
                                        dataType.IsVirtualType(),
                                        Naming.GetIndexTitle(dataType) + suffix,
                                        Naming.GetFullIndexTitle(dataType) + suffix);
        }

        private UxMemberPropertiesViewModel GetUxProperties(Member member)
        {
            // Only include public members
            if (!member.IsPublic) return null;

            if (member.MemberType == MemberType.Property)
            {
                // Properties must meet one of the following conditions:
                //  - Have either both getter and setter
                //  - Have a getter, and return a type that inherits from IList<T>
                var propertyMember = (Property)member;
                var hasPublicGetter = propertyMember.GetMethod != null && propertyMember.GetMethod.IsPublic;
                var hasPublicSetter = propertyMember.SetMethod != null && propertyMember.SetMethod.IsPublic;
                if (hasPublicGetter && hasPublicSetter)
                {
                    return new UxMemberPropertiesViewModel();
                }

                if (hasPublicGetter && TypeInheritsFromGenericList(propertyMember.ReturnType))
                {
                    return new UxMemberPropertiesViewModel();
                }
            }
            else if (member.MemberType == MemberType.Event)
            {
                return new UxMemberPropertiesViewModel();
            }

            return null;
        }

        private ValuesViewModel BuildValues(Member member)
        {
            if (member.ReturnType == null || member.ReturnType.FullName == ExportConstants.VoidTypeName)
            {
                return null;
            }

            return BuildValues(member.ReturnType);
        }

        private MemberFlagsViewModel BuildFlags(Member member)
        {
            var attrs = member.Attributes ?? new NewObject[0];
            var hasUxContent = attrs.Any(e => e.ReturnType.QualifiedName == ExportConstants.UxContentAttributeName);
            var hasUxPrimary = attrs.Any(e => e.ReturnType.QualifiedName == ExportConstants.UxPrimaryAttributeName);
            var hasUxComponents = attrs.Any(e => e.ReturnType.QualifiedName == ExportConstants.UxComponentsAttributeName);

            return new MemberFlagsViewModel(hasUxContent, hasUxPrimary, hasUxComponents);
        }

        private AttributesViewModel BuildAttributes(Member member)
        {
            var models = new List<AttributeViewModel>();
            foreach (var attr in member.Attributes ?? new NewObject[0])
            {
                if (!Exportable.IsExportableAndVisible(attr.ReturnType))
                {
                    continue;
                }

                var parameters = attr.Arguments.Select(e => e.ConstantString).ToList();
                var reference = new DataTypeBuilder(Naming, Syntax, Exportable, AttachedMembers, _commentParser).BuildReference(attr.ReturnType);
                models.Add(new AttributeViewModel(reference.Id, reference.Uri, reference.Titles, parameters));
            }

            return new AttributesViewModel(models);
        }

        private ValuesViewModel BuildValues(DataType dataType)
        {
            if (dataType.TypeType != TypeType.Enum)
            {
                return null;
            }

            var values = dataType.Literals.Select(e =>
            {
                var comment = _commentParser.Read(e);
                return new ValueViewModel(e.GetUri(),
                                          Naming.GetIndexTitle(e),
                                          new BasicCommentViewModel(comment));
            }).ToList();
            return new ValuesViewModel(values);
        }

        private static string GetDataTypeUri(DataType dataType)
        {
            return dataType.IsGenericParameter ? null : dataType.GetUri();
        }

        private static bool TypeInheritsFromGenericList(DataType dataType)
        {
            if (dataType.GetUri() == ExportConstants.GenericListInterfaceTypeUri)
            {
                return true;
            }
            return dataType.Base != null && TypeInheritsFromGenericList(dataType.Base);
        }

        private static string GetTypeName(Member member, SourceComment comment, UxMemberPropertiesViewModel uxProperties)
        {
            if (comment.Attributes.ScriptEvent != null)
            {
                return "JsEvent";
            }
            if (comment.Attributes.ScriptProperty != null)
            {
                return "JsProperty";
            }
            if (comment.Attributes.ScriptMethod != null)
            {
                return "JsMethod";
            }
            if (uxProperties != null && member.MemberType == MemberType.Event)
            {
                return "UxEvent";
            }
            if (uxProperties != null && member.MemberType == MemberType.Property)
            {
                return "UxProperty";
            }
            if (comment.Attributes.UxProperty && member.MemberType == MemberType.Event)
            {
                return "UxEvent";
            }
            if (comment.Attributes.UxProperty && member.MemberType == MemberType.Property)
            {
                return "UxProperty";
            }

            return member.MemberType.ToString("G");
        }
    }
}
