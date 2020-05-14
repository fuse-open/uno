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
    public class DataTypeBuilder : Builder
    {
        private readonly ICommentParser _commentParser;

        public DataTypeBuilder(IEntityNaming naming,
                               ISyntaxGenerator syntax,
                               IExportableCheck exportable,
                               AttachedMemberCache attachedMembers,
                               ICommentParser commentParser)
                : base(naming, syntax, exportable, attachedMembers)
        {
            _commentParser = commentParser;
        }

        public void Build(DataType entity, HashSet<DocumentViewModel> target)
        {
            AddDataTypeToTarget(entity, null, target);

            entity.NestedTypes.Where(Exportable.IsExportableAndVisible).ToList().ForEach(type => Build(type, target));
            entity.Swizzlers.Where(Exportable.IsExportableAndVisible).ToList().ForEach(swizzler => AddDataTypeToTarget(swizzler, entity, target));

            var seenMembers = new HashSet<string>();

            ExportMembers(entity.Casts, entity, target, seenMembers);
            ExportMembers(entity.Constructors, entity, target, seenMembers);
            ExportMembers(entity.Fields, entity, target, seenMembers);
            ExportMembers(entity.EnumerateFlattenedFields().ToList(), entity, target, seenMembers);
            ExportMembers(entity.Literals, entity, target, seenMembers);
            ExportMembers(entity.Methods, entity, target, seenMembers);
            ExportMembers(entity.EnumerateFlattenedMethods().ToList(), entity, target, seenMembers);
            ExportMembers(entity.Operators, entity, target, seenMembers);
            ExportMembers(entity.Properties, entity, target, seenMembers);
            ExportMembers(entity.EnumerateFlattenedProperties().ToList(), entity, target, seenMembers);
            ExportMembers(entity.Events, entity, target, seenMembers);
            ExportMembers(entity.EnumerateFlattenedEvents().ToList(), entity, target, seenMembers);

            var isUxEntity = entity.GetUxClassProperties() != null;
            if (isUxEntity)
            {
                ExportAttachedProperties(AttachedMembers.GetAttachedUxProperties(GetTypeAndBaseTypes(entity), entity), entity, target);
                ExportAttachedEvents(AttachedMembers.GetAttachedUxEvents(GetTypeAndBaseTypes(entity), entity), entity, target);
            }

            ExportJsMembers(entity, target);
        }

        public DocumentReferenceViewModel BuildReference(DataType dataType)
        {
            var comment = _commentParser.Read(dataType);
            var id = BuildDataTypeId(dataType, comment);
            var uri = BuildDataTypeUri(dataType);
            var titles = BuildDataTypeIndexTitles(dataType);
            return new DocumentReferenceViewModel(id, uri, titles);
        }

        private void ExportMembers(IEnumerable<Member> members, DataType dataType, HashSet<DocumentViewModel> target, HashSet<string> seenMembers)
        {
            var invisibleMembers = members.Where(e => Exportable.IsExportableAndVisible(e) && !Exportable.IsExportableAndVisible(e.DeclaringType)).ToList();
            if (invisibleMembers.Count > 0)
            {
                throw new Exception($"Found members inside {dataType.FullName} declared on non-exportable parent type: {string.Join(", ", invisibleMembers.Select(e => e.FullName))}");
            }
            var exportableMembers = members.Where(e => Exportable.IsExportableAndVisible(e) && Exportable.IsExportableAndVisible(e.DeclaringType)).ToList();
            exportableMembers.ForEach(member =>
            {
                var id = dataType.GetUri() + "/" + member.GetUriSignature();
                if (!seenMembers.Contains(id))
                {
                    GetMemberBuilder().Build(member, dataType, target);
                    seenMembers.Add(id);
                }
            });
        }

        private void ExportAttachedProperties(IEnumerable<AttachedUxProperty> properties, DataType dataType, HashSet<DocumentViewModel> target)
        {
            properties.ToList().ForEach(prop => GetMemberBuilder().Build(prop, dataType, target));
        }

        private void ExportAttachedEvents(IEnumerable<AttachedUxEvent> events, DataType dataType, HashSet<DocumentViewModel> target)
        {
            events.ToList().ForEach(evt => GetMemberBuilder().Build(evt, dataType, target));
        }

        private void ExportJsMembers(DataType dataType, HashSet<DocumentViewModel> target)
        {
            var parentDataType = dataType.Base;
            while (parentDataType != null)
            {
                ExportJsMembers(parentDataType.Methods, dataType, target);
                ExportJsMembers(parentDataType.Properties, dataType, target);
                ExportJsMembers(parentDataType.Events, dataType, target);
                parentDataType = parentDataType.Base;
            }
        }

        private void ExportJsMembers(IEnumerable<Member> members, DataType dataType, HashSet<DocumentViewModel> target)
        {
            foreach (var member in members)
            {
                var comment = _commentParser.Read(member);
                if (comment?.Attributes?.ScriptMethod == null &&
                    comment?.Attributes?.ScriptProperty == null &&
                    comment?.Attributes?.ScriptEvent == null)
                {
                    continue;
                }

                GetMemberBuilder().Build(member, dataType, target);
            }
        }

        private void AddDataTypeToTarget(DataType dataType, DataType swizzlerParent, HashSet<DocumentViewModel> target)
        {
            var ns = dataType.FindNamespace();
            var titles = BuildDataTypeTitles(dataType);
            var syntax = new SyntaxViewModel(Syntax.BuildUnoSyntax(dataType), Syntax.BuildUxSyntax(dataType));
            var baseType = BuildBaseType(dataType);
            var location = new LocationViewModel(ns.FullName, ns.GetUri(), dataType.Source.Package.Name, dataType.Source.Package.Version);
            var uxProperties = BuildUxProperties(dataType);
            var inheritance = GetInheritance(dataType);
            var parameters = GetParameters(dataType);
            var returns = BuildReturns(dataType);
            var values = BuildValues(dataType);
            var implementedInterfaces = BuildImplementedInterfaces(dataType);
            var attributes = BuildAttributes(dataType);

            DocumentViewModel viewModel;
            if (swizzlerParent == null)
            {
                var comment = _commentParser.Read(dataType);
                var type = GetTypeName(dataType, comment);

                var nsRef = new NamespaceBuilder(Naming, Syntax, Exportable, AttachedMembers, _commentParser).BuildReference(ns);
                viewModel = new DataTypeViewModel(new DocumentIdViewModel(dataType.GetUri(),
                                                                          dataType.Parent.GetUri(),
                                                                          type,
                                                                          dataType.GetModifierNames()),
                                                  new DocumentUriViewModel(dataType.GetUri(), dataType.GetUri(), false),
                                                  titles,
                                                  syntax,
                                                  baseType,
                                                  location,
                                                  inheritance,
                                                  parameters,
                                                  returns,
                                                  uxProperties,
                                                  values,
                                                  new CommentViewModel(comment),
                                                  nsRef,
                                                  implementedInterfaces,
                                                  attributes,
                                                  dataType);
            }
            else
            {
                var uri = swizzlerParent.GetUri() + "/" + dataType.GetUri().Replace("/", "_");
                viewModel = new SwizzlerTypeViewModel(new DocumentIdViewModel(uri, swizzlerParent.GetUri(), "SwizzlerType", dataType.GetModifierNames()),
                                                      new DocumentUriViewModel(uri, dataType.GetUri(), false),
                                                      titles,
                                                      null);
            }
            target.AddIfNotExists(viewModel);
        }

        private List<DataType> GetTypeAndBaseTypes(DataType dataType)
        {
            var types = new List<DataType> { dataType };
            if (dataType.GetId() == ExportConstants.RootDataTypeId)
            {
                return types;
            }

            var currentBaseType = dataType.Base;
            while (currentBaseType != null)
            {
                types.Add(currentBaseType);
                currentBaseType = currentBaseType.Base;
            }

            return types;
        }

        private InheritanceViewModel GetInheritance(DataType dataType)
        {
            if (dataType.GetId() == ExportConstants.RootDataTypeId)
            {
                return null;
            }

            var ascendants = new List<DataType>();
            var currentBaseType = dataType.Base;
            while (currentBaseType != null)
            {
                ascendants.Add(currentBaseType);
                currentBaseType = currentBaseType.Base;
            }

            if (ascendants.Count < 1)
            {
                return null;
            }

            var currentChildren = new List<InheritanceNodeViewModel>();
            var roots = currentChildren;

            // Add ancestors in reverse order
            for (var i = ascendants.Count - 1; i >= 0; i--)
            {
                var node = new InheritanceNodeViewModel(ascendants[i].GetUri(),
                                                        Naming.GetFullIndexTitle(ascendants[i]),
                                                        new List<InheritanceNodeViewModel>(),
                                                        true,
                                                        false,
                                                        false);
                currentChildren.Add(node);
                currentChildren = node.Children;
            }

            // Add current node
            currentChildren.Add(new InheritanceNodeViewModel(dataType.GetUri(),
                                                             Naming.GetFullIndexTitle(dataType),
                                                             new List<InheritanceNodeViewModel>(),
                                                             false,
                                                             false,
                                                             true));

            // Make sure we only have one root here
            if (roots.Count != 1)
            {
                throw new InvalidOperationException("Unexpected number of roots for " + dataType.GetUri() + ": " + roots.Count);
            }
            return new InheritanceViewModel(roots.First());
        }

        private ParametersViewModel GetParameters(DataType dataType)
        {
            var parameters = dataType.GetParametersOrNull();
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
                throw new Exception($"Found {invisibleParams.Count} parameters for type {dataType.FullName} that have non-exportable types: {string.Join(", ", names)}");
            }

            var list = parameters.Select(param => param.Type.IsArray
                                                    ? new ParameterViewModel(param.Name,
                                                                             param.Type.ElementType.GetUri(),
                                                                             param.Type.ElementType.IsVirtualType(),
                                                                             Naming.GetIndexTitle(param.Type.ElementType),
                                                                             Naming.GetFullIndexTitle(param.Type.ElementType))
                                                    : new ParameterViewModel(param.Name,
                                                                             param.Type.GetUri(),
                                                                             param.Type.IsVirtualType(),
                                                                             Naming.GetIndexTitle(param.Type),
                                                                             Naming.GetFullIndexTitle(param.Type)))
                    .ToList();
            return new ParametersViewModel(list);
        }

        private ReturnsViewModel BuildReturns(DataType dataType)
        {
            var delegateType = dataType as DelegateType;
            if (delegateType == null || delegateType.ReturnType.FullName == ExportConstants.VoidTypeName) return null;

            if (!Exportable.IsExportableAndVisible(delegateType.ReturnType))
            {
                throw new Exception($"Found return type for delegate {delegateType.FullName} that have non-exportable return type: {delegateType.ReturnType.FullName}");
            }

            return new ReturnsViewModel(delegateType.GetUri(),
                                        delegateType.IsVirtualType(),
                                        Naming.GetIndexTitle(delegateType.ReturnType),
                                        Naming.GetFullIndexTitle(delegateType.ReturnType));
        }

        private DocumentIdViewModel BuildDataTypeId(DataType dataType, SourceComment comment)
        {
            var id = new DocumentIdViewModel(dataType.GetUri(),
                                             dataType.Parent.GetUri(),
                                             GetTypeName(dataType, comment),
                                             dataType.GetModifierNames());
            return id;
        }

        private DocumentUriViewModel BuildDataTypeUri(DataType dataType)
        {
            return new DocumentUriViewModel(dataType.GetUri(), dataType.GetUri(), false);
        }

        private IndexTitlesViewModel BuildDataTypeIndexTitles(DataType dataType)
        {
            var titles = new IndexTitlesViewModel(Naming.GetIndexTitle(dataType),
                                                  Naming.GetFullIndexTitle(dataType));
            return titles;
        }

        private TitlesViewModel BuildDataTypeTitles(DataType dataType)
        {
            var titles = new TitlesViewModel(Naming.GetPageTitle(dataType),
                                             Naming.GetIndexTitle(dataType),
                                             Naming.GetFullIndexTitle(dataType),
                                             Naming.GetNavigationTitle(dataType),
                                             dataType.QualifiedName);
            return titles;
        }

        private DocumentReferenceViewModel BuildBaseType(DataType dataType)
        {
            return dataType.Base == null
                           ? null
                           : BuildReference(dataType.Base);
        }

        private UxClassPropertiesViewModel BuildUxProperties(DataType dataType)
        {
            var props = dataType.GetUxClassProperties();
            return props == null ? null : new UxClassPropertiesViewModel(props.Namespace, props.NamespaceUri, props.Name);
        }

        private ValuesViewModel BuildValues(DataType dataType)
        {
            if (dataType.TypeType != TypeType.Enum || dataType.Literals == null || dataType.Literals.Count == 0)
            {
                return null;
            }

            var values = dataType.Literals.Select(e =>
            {
                var comment = _commentParser.Read(e);
                return new ValueViewModel(e.GetUri(), Naming.GetIndexTitle(e), new BasicCommentViewModel(comment));
            }).ToList();
            return new ValuesViewModel(values);
        }

        private IEnumerable<InterfaceType> GetAllInterfaces(DataType dataType)
        {
            var result = (dataType.Interfaces ?? new InterfaceType[0]).ToList();
            if (dataType.Base != null)
            {
                result.AddRange(GetAllInterfaces(dataType.Base));
            }

            return result;
        }

        private ImplementedInterfacesViewModel BuildImplementedInterfaces(DataType dataType)
        {
            var result = new List<ImplementedInterfaceViewModel>();

            foreach (var iface in GetAllInterfaces(dataType))
            {
                if (!Exportable.IsExportableAndVisible(iface))
                {
                    continue;
                }

                var comment = _commentParser.Read(iface);
                var id = BuildDataTypeId(iface, comment);
                var uri = BuildDataTypeUri(iface);
                var titles = BuildDataTypeIndexTitles(iface);
                var viewModel = new ImplementedInterfaceViewModel(id, uri, titles, new BasicCommentViewModel(comment));
                result.Add(viewModel);
            }

            return new ImplementedInterfacesViewModel(result);
        }

        private AttributesViewModel BuildAttributes(DataType dataType)
        {
            var models = new List<AttributeViewModel>();

            foreach (var attr in dataType.Attributes ?? new NewObject[0])
            {
                if (!Exportable.IsExportableAndVisible(attr.ReturnType))
                {
                    continue;
                }

                var parameters = attr.Arguments.Select(e => e.ConstantString).ToList();
                var reference = BuildReference(attr.ReturnType);
                models.Add(new AttributeViewModel(reference.Id, reference.Uri, reference.Titles, parameters));
            }

            return new AttributesViewModel(models);
        }

        private MemberBuilder GetMemberBuilder()
        {
            return new MemberBuilder(Naming, Syntax, Exportable, AttachedMembers, _commentParser);
        }

        private static string GetTypeName(DataType dataType, SourceComment comment)
        {
            if (comment.Attributes.ScriptModule != null)
            {
                return "JsModule";
            }
            if (dataType.GetUxClassProperties() != null)
            {
                return "UxClass";
            }
            return dataType.TypeType.ToString("G");
        }
    }
}
