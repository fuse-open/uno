using System.Collections.Generic;
using System.Linq;
using Uno.Compiler.API.Domain.IL;
using Uno.Compiler.Backends.UnoDoc.Builders.Extensions;
using Uno.Compiler.Backends.UnoDoc.ViewModels;
using Uno.Compiler.Backends.UnoDoc.ViewModels.MetaData;

namespace Uno.Compiler.Backends.UnoDoc.Builders.EntityTypes
{
    public class NamespaceBuilder : Builder
    {
        private readonly ICommentParser _commentParser;

        public NamespaceBuilder(IEntityNaming naming,
                                ISyntaxGenerator syntax,
                                IExportableCheck exportable,
                                AttachedMemberCache attachedMembers,
                                ICommentParser commentParser)
                : base(naming, syntax, exportable, attachedMembers)
        {
            _commentParser = commentParser;
        }

        public void Build(Namespace entity, HashSet<DocumentViewModel> target)
        {
            AddNamespaceToTarget(entity, target);

            var exportableTypes = entity.Types.Where(Exportable.IsExportableAndVisible).ToList();
            exportableTypes.ForEach(block => GetDataTypeBuilder().Build(block, target));

            entity.Namespaces.ToList().ForEach(ns => Build(ns, target));
        }

        public DocumentReferenceViewModel BuildReference(Namespace entity)
        {
            var parentUri = entity.ParentNamespace == null || entity.ParentNamespace.Name == ExportConstants.RootNamespaceName ? null : entity.ParentNamespace.GetUri();
            return new DocumentReferenceViewModel(new DocumentIdViewModel(entity.GetUri(), parentUri, "Namespace", new List<string>()),
                                                  new DocumentUriViewModel(entity.GetUri(), entity.GetUri(), false),
                                                  new IndexTitlesViewModel(Naming.GetIndexTitle(entity),
                                                                           Naming.GetFullIndexTitle(entity)));
        }

        private void AddNamespaceToTarget(Namespace ns, HashSet<DocumentViewModel> target)
        {
            var titles = new TitlesViewModel(Naming.GetPageTitle(ns),
                                             Naming.GetIndexTitle(ns),
                                             Naming.GetFullIndexTitle(ns),
                                             Naming.GetNavigationTitle(ns),
                                             ns.QualifiedName);
            var syntax = new SyntaxViewModel(Syntax.BuildUnoSyntax(ns), Syntax.BuildUxSyntax(ns));
            var parentUri = ns.ParentNamespace == null || ns.ParentNamespace.Name == ExportConstants.RootNamespaceName ? null : ns.ParentNamespace.GetUri();

            target.AddIfNotExists(new NamespaceViewModel(new DocumentIdViewModel(ns.GetUri(), parentUri, "Namespace", new List<string>()),
                                                         new DocumentUriViewModel(ns.GetUri(), ns.GetUri(), false),
                                                         titles,
                                                         syntax,
                                                         null));
        }

        private DataTypeBuilder GetDataTypeBuilder()
        {
            return new DataTypeBuilder(Naming, Syntax, Exportable, AttachedMembers, _commentParser);
        }
    }
}