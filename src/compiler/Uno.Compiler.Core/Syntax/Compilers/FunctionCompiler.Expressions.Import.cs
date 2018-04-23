using System.IO;
using Uno.Compiler.API.Domain.AST.Expressions;
using Uno.Compiler.API.Domain.Graphics;
using Uno.Compiler.API.Domain.IL;
using Uno.Compiler.API.Domain.IL.Expressions;
using Uno.Compiler.API.Plugins;
using Uno.IO;

namespace Uno.Compiler.Core.Syntax.Compilers
{
    public partial class FunctionCompiler
    {
        public Expression CompileImport(AstImport import)
        {
            if (import.Arguments == null)
            {
                var c = Compiler.CompileConstant(import.Importer, Namescope, Essentials.String);
                if (c.IsInvalid) return c;

                var filename = c.ConstantString;
                Compiler.Disk.GetFullPath(import.Importer.Source, ref filename);
                return Compiler.BundleBuilder.AddBundleFile(import.Importer.Source, filename);
            }

            // TODO: Remove deprecated legacy code below
            var importer = Compiler.TryCompileSuffixedObject(Namescope, import.Importer, "Importer", import.Arguments);

            if (importer == null)
                return Expression.Invalid;

            if (importer.ReturnType.Base == null ||
                importer.ReturnType.Base.MasterDefinition != Essentials.Importer_T)
                return Error(importer.Source, ErrorCode.E0000, "Importer must be a type derived directly from " + Essentials.Importer_T.Quote());

            ILVerifier.VerifyConstUsage(import.Source, importer.Constructor, Function);

            Importer plugin;
            if (!Compiler.Plugins.TryGetImporter(importer.ReturnType.MasterDefinition, out plugin))
                return Error(import.Importer.Source, ErrorCode.E2048, "Unsupported importer " + importer.ReturnType.Quote());

            PathFlags flags = 0;
            var isMetaProperty = MetaProperty != null;

            if (isMetaProperty)
                flags |= PathFlags.WarnIfNonExistingPath;
            if (import.Source.IsUnknown || Path.GetExtension(import.Source.FullPath).ToUpperInvariant() != ".UNO")
                flags |= PathFlags.AllowAbsolutePath;

            if (!Compiler.ExpandFilenames(importer, flags))
            {
                if (isMetaProperty)
                {
                    for (int i = 0; i < importer.Arguments.Length; i++)
                    {
                        if (importer.Constructor.Parameters[i].HasAttribute(Essentials.FilenameAttribute))
                        {
                            var c = importer.Arguments[i] as Constant;

                            if (c != null)
                            {
                                var src = c.Source;
                                var filename = c.Value as string;
                                AddReqStatement(new ReqFile(src, filename));
                            }
                        }
                    }
                }

                return Expression.Invalid;
            }

            var types = importer.ReturnType.IsGenericParameterization ?
                importer.ReturnType.GenericArguments :
                null;

            return CompileImplicitCast(
                import.Source, 
                importer.ReturnType.Base.GenericArguments[0],
                plugin.Import(
                    new ImportContext(
                        import.Source, 
                        types, 
                        importer.GetArgumentValues())));
        }
    }
}
