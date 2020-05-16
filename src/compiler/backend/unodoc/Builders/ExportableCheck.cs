using System.Linq;
using Uno.Compiler.API.Domain.IL;
using Uno.Compiler.API.Domain.IL.Members;
using Uno.Compiler.API.Domain.IL.Types;

namespace Uno.Compiler.Backends.UnoDoc.Builders
{
    public class ExportableCheck : IExportableCheck
    {
        private readonly ICommentParser _commentParser;

        public ExportableCheck(ICommentParser commentParser)
        {
            _commentParser = commentParser;
        }

        public bool IsExportableAndVisible(IEntity entity)
        {
            return IsExportable(entity) && IsVisible(entity);
        }

        private static bool IsExportable(IEntity entity)
        {
            var result = ExportConstants.AllowedPackagePrefixes.Any(prefix => prefix == entity.Source.Package.Name || entity.Source.Package.Name.StartsWith(prefix + "."));
            return result;
        }

        private bool IsVisible(IEntity entity)
        {
            // Regardless of entity visibility, if it's annotated with one of these flags it should be exported regardless:
            //  - @scriptevent
            //  - @scriptmethod
            //  - @scriptmodule
            //  - @scriptproperty
            var comment = new SourceComment();
            var sourceObj = entity as SourceObject;
            if (sourceObj != null)
            {
                comment = _commentParser.Read(sourceObj);
            }

            if (comment.Attributes.ScriptEvent != null ||
                comment.Attributes.ScriptMethod != null ||
                comment.Attributes.ScriptModule != null ||
                comment.Attributes.ScriptProperty != null)
            {
                return true;
            }

            if (entity.IsPrivate || entity.IsInternal)
            {
                return false;
            }

            //If this type is visible but it has a parent type which is not, it should itself not be visible.
            //Note: restricting this to interfaces for now to avoid conflicts
            var @interface = entity as InterfaceType;
            if (@interface != null && @interface.IsNestedType)
            {
                return (@interface.ParentType != null) ? IsVisible(@interface.ParentType) : true;
            }

            var member = entity as Member;
            if (member == null)
            {
                return true;
            }

            // Don't show protected members for sealed data types
            return !member.DeclaringType.IsSealed || !member.IsProtected;
        }
    }
}