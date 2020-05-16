using Uno.Compiler.API.Domain.AST.Expressions;
using Uno.Compiler.API.Domain.IL;

namespace Uno.Compiler.Core.Syntax.Compilers
{
    public partial class FunctionCompiler
    {
        public Expression CompileImport(AstImport import)
        {
            var filename = Compiler.CompileConstant(import.Importer, Namescope, Essentials.String).ConstantString;
            if (filename == null)
                return Expression.Invalid;

            Compiler.Disk.GetFullPath(import.Importer.Source, ref filename);
            return Compiler.BundleBuilder.AddFile(import.Importer.Source, filename);
        }
    }
}
