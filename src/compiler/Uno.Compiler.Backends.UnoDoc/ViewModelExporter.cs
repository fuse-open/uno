using System;
using System.Collections.Generic;
using Uno.Compiler.API;
using Uno.Compiler.API.Domain.IL;
using Uno.Compiler.Backends.UnoDoc.Builders;
using Uno.Compiler.Backends.UnoDoc.Builders.EntityTypes;
using Uno.Compiler.Backends.UnoDoc.ViewModels;

namespace Uno.Compiler.Backends.UnoDoc
{
    public class ViewModelExporter
    {
        private readonly Namespace _rootNamespace;
        private readonly IUtilities _utilities;

        public ViewModelExporter(Namespace rootNamespace, IUtilities utilities)
        {
            
            if (rootNamespace == null)
            {
                throw new ArgumentNullException(nameof(rootNamespace));
            }
            if (utilities == null)
            {
                throw new ArgumentNullException(nameof(utilities));
            }

            _rootNamespace = rootNamespace;
            _utilities = utilities;
        }

        public HashSet<DocumentViewModel> BuildExport()
        {
            var attachedMembers = new AttachedMemberCache(_utilities.FindAllTypes());
            var commentParser = new CommentParser();
            var builder = new NamespaceBuilder(new EntityNaming(),
                                               new SyntaxGenerator(new ExportableCheck(commentParser), new EntityNaming()),
                                               new ExportableCheck(commentParser),
                                               attachedMembers,
                                               commentParser);
            var result = new HashSet<DocumentViewModel>(new DocumentViewModelEqualityComparer());
            _rootNamespace.Namespaces.ForEach(ns => builder.Build(ns, result));
            return result;
        }
    }
}