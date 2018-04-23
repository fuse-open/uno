using System.Text;
using Uno.Compiler.API.Domain.IL;
using Uno.Compiler.API.Domain.IL.Members;
using Uno.Compiler.API.Domain.IL.Statements;
using Uno.Compiler.API.Domain.IL.Types;

namespace Uno.Compiler.API.Backends.Decompilers
{
    public class Decompiler
    {
        public bool AllowErrors { get; private set; }

        public Decompiler(bool allowErrors = true)
        {
            AllowErrors = allowErrors;
        }

        public virtual SourceWriter CreateWriter(Source src, StringBuilder sb, Function context)
        {
            throw NotSupported(src);
        }

        public virtual string GetEntity(Source src, Function context, IEntity entity)
        {
            throw NotSupported(src);
        }

        public virtual string GetType(Source src, Function context, DataType dt)
        {
            return GetEntity(src, context, dt);
        }

        public virtual string GetLiteral(Source src, Function context, Literal literal)
        {
            return literal.Value.ToLiteral();
        }

        /* C++ specific */

        public virtual string GetFunction(Source src, Function context, Function func)
        {
            return "(not supported)";
        }

        public virtual string GetForwardDeclaration(Source src, DataType dt)
        {
            return "(not supported)";
        }

        public virtual string GetInclude(Source src, DataType dt)
        {
            return "(not supported)";
        }

        /* Decompiler */

        public string GetScope(Scope scope, Function context, int indent = 0)
        {
            var sb = new StringBuilder();
            using (var w = CreateWriter(scope.Source, sb, context))
            {
                w.Indent(indent);
                w.WriteScope(scope, false, false);
                return sb.ToString();
            }
        }

        public string GetNewArray(Source src, Function context, ArrayType type, Expression[] elements)
        {
            var sb = new StringBuilder();
            using (var w = CreateWriter(src, sb, context))
            {
                w.WriteNewArray(src, type, elements);
                return sb.ToString();
            }
        }

        public string GetLoadElement(Source src, Function context, DataType elementType, Expression obj, Expression index)
        {
            var sb = new StringBuilder();
            using (var w = CreateWriter(src, sb, context))
            {
                w.WriteLoadElement(src, elementType, obj, index);
                return sb.ToString();
            }
        }

        public string GetStoreElement(Source src, Function context, DataType elementType, Expression obj, Expression index, Expression value)
        {
            var sb = new StringBuilder();
            using (var w = CreateWriter(src, sb, context))
            {
                w.WriteStoreElement(src, elementType, obj, index, value);
                return sb.ToString();
            }
        }

        public string GetNewArray(Source src, Function context, ArrayType type, Expression size)
        {
            var sb = new StringBuilder();
            using (var w = CreateWriter(src, sb, context))
            {
                w.WriteNewArray(src, type, size);
                return sb.ToString();
            }
        }

        public string GetNewObject(Source src, Function context, Constructor ctor, Expression[] args)
        {
            var sb = new StringBuilder();
            using (var w = CreateWriter(src, sb, context))
            {
                w.WriteNewObject(src, ctor, args);
                return sb.ToString();
            }
        }

        public string GetLoadField(Source src, Function context, Field field, Expression obj)
        {
            var sb = new StringBuilder();
            using (var w = CreateWriter(src, sb, context))
            {
                w.WriteLoadField(src, field, obj);
                return sb.ToString();
            }
        }

        public string GetStoreField(Source src, Function context, Field field, Expression obj, Expression value)
        {
            var sb = new StringBuilder();
            using (var w = CreateWriter(src, sb, context))
            {
                w.WriteStoreField(src, field, obj, value);
                return sb.ToString();
            }
        }

        public string GetGetProperty(Source src, Function context, Property property, Expression obj, Expression[] args)
        {
            var sb = new StringBuilder();
            using (var w = CreateWriter(src, sb, context))
            {
                w.WriteGetProperty(src, property, obj, args);
                return sb.ToString();
            }
        }

        public string GetSetProperty(Source src, Function context, Property property, Expression obj, Expression[] args, Expression value)
        {
            var sb = new StringBuilder();
            using (var w = CreateWriter(src, sb, context))
            {
                w.WriteSetProperty(src, property, obj, args, value);
                return sb.ToString();
            }
        }

        public string GetCallMethod(Source src, Function context, Method method, Expression obj, Expression[] args)
        {
            var sb = new StringBuilder();
            using (var w = CreateWriter(src, sb, context))
            {
                w.WriteCallMethod(src, method, obj, args);
                return sb.ToString();
            }
        }

        public string GetCallDelegate(Source src, Function context, Expression obj, Expression[] args)
        {
            var sb = new StringBuilder();
            using (var w = CreateWriter(src, sb, context))
            {
                w.WriteCallDelegate(src, obj, args);
                return sb.ToString();
            }
        }

        public string GetTypeOf(Source src, Function context, DataType dt)
        {
            var sb = new StringBuilder();
            using (var w = CreateWriter(src, sb, context))
            {
                w.WriteTypeOf(src, dt);
                return sb.ToString();
            }
        }

        /* Error */

        protected SourceException NotSupported(Source src)
        {
            return new SourceException(src, "Not supported by current backend");
        }
    }
}
