using System;

namespace Uno.Compiler.Backends.UnoDoc.Builders
{
    public abstract class Builder
    {
        protected AttachedMemberCache AttachedMembers { get; private set; }
        protected IEntityNaming Naming { get; private set; }
        protected ISyntaxGenerator Syntax { get; private set; }
        protected IExportableCheck Exportable { get; private set; }

        protected Builder(IEntityNaming naming, ISyntaxGenerator syntax, IExportableCheck exportable, AttachedMemberCache attachedMembers)
        {
            if (naming == null)
            {
                throw new ArgumentNullException(nameof(naming));
            }
            if (syntax == null)
            {
                throw new ArgumentNullException(nameof(syntax));
            }
            if (exportable == null)
            {
                throw new ArgumentNullException(nameof(exportable));
            }
            if (attachedMembers == null)
            {
                throw new ArgumentNullException(nameof(attachedMembers));
            }

            Naming = naming;
            Syntax = syntax;
            Exportable = exportable;
            AttachedMembers = attachedMembers;
        }
    }
}