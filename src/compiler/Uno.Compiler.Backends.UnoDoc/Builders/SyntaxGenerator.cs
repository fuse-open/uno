using System;
using Uno.Compiler.API.Domain.Graphics;
using Uno.Compiler.API.Domain.IL;
using Uno.Compiler.API.Domain.IL.Members;
using Uno.Compiler.Backends.UnoDoc.Builders.Syntax;

namespace Uno.Compiler.Backends.UnoDoc.Builders
{
    public class SyntaxGenerator : ISyntaxGenerator
    {
        private readonly IExportableCheck _exportableCheck;
        private readonly IEntityNaming _entityNaming;

        public SyntaxGenerator(IExportableCheck exportableCheck, IEntityNaming entityNaming)
        {
            if (exportableCheck == null)
            {
                 throw new ArgumentNullException(nameof(exportableCheck));
            }
            if (entityNaming == null)
            {
                 throw new ArgumentNullException(nameof(entityNaming));
            }

            _exportableCheck = exportableCheck;
            _entityNaming = entityNaming;
        }

        public string BuildUnoSyntax(IEntity entity, IEntity context = null)
        {
            return GetSyntaxImplementationFor(entity).BuildUnoSyntax(entity, context);
        }

        public string BuildUxSyntax(IEntity entity)
        {
            return GetSyntaxImplementationFor(entity).BuildUxSyntax(entity);
        }

        private ISyntaxGenerator GetSyntaxImplementationFor(IEntity entity)
        {
            if (entity is Block)
            {
                return new BlockSyntaxGenerator(_exportableCheck, _entityNaming);
            }
            if (entity is DataType)
            {
                return new DataTypeSyntaxGenerator(_exportableCheck, _entityNaming);
            }
            if (entity is Member)
            {
                return new MemberSyntaxGenerator(_exportableCheck, _entityNaming);
            }
            if (entity is Namespace)
            {
                return new NamespaceSyntaxGenerator(_exportableCheck, _entityNaming);
            }

            throw new ArgumentException("Unable to determine syntax implementation for " + entity.GetType().FullName);
        }
    }
}