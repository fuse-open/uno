using System.Text;
using Uno.Compiler.API.Backends.Decompilers;
using Uno.Compiler.API.Domain.IL;
using Uno.Compiler.API.Domain.IL.Members;

namespace Uno.Compiler.Backends.CPlusPlus
{
    class CppDecompiler : Decompiler
    {
        readonly CppBackend _backend;

        public CppDecompiler(CppBackend backend)
        {
            _backend = backend;
        }

        public override SourceWriter CreateWriter(Source src, StringBuilder sb, Function context)
        {
            return new CppWriter(_backend, sb, context);
        }

        public override string GetEntity(Source src, Function context, IEntity entity)
        {
            var member = entity as Member;
            if (member == null)
                throw NotSupported(src);

            return _backend.GetStaticName(member, context?.DeclaringType);
        }

        public override string GetForwardDeclaration(Source src, DataType dt)
        {
            return _backend.GetForwardDeclaration(dt);
        }

        public override string GetInclude(Source src, DataType dt)
        {
            return _backend.GetIncludeFilename(dt);
        }

        public override string GetFunction(Source src, Function context, Function func)
        {
            return _backend.GetFunctionPointer(func, context?.DeclaringType);
        }

        public override string GetType(Source src, Function context, DataType dt)
        {
            return _backend.GetTypeDef(dt, context?.DeclaringType);
        }
    }
}
