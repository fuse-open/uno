using System.Collections.Generic;
using Uno.Compiler.API.Domain;
using Uno.Compiler.API.Domain.AST.Expressions;
using Uno.Compiler.API.Domain.AST.Members;
using Uno.Compiler.API.Domain.Graphics;
using Uno.Compiler.API.Domain.IL;
using Uno.Compiler.API.Domain.IL.Expressions;
using Uno.Compiler.API.Domain.IL.Statements;
using Uno.Compiler.Core.Syntax.Compilers;

namespace Uno.Compiler.Core.Syntax.Builders
{
    public partial class BlockBuilder
    {
        MetaProperty CreateMetaProperty(AstMetaProperty ast, BlockBase parent)
        {
            var src = ast.Name.Source;
            var name = ast.Name.Symbol;
            var dt = DataType.Invalid;
            var visibility = MetaVisibility.Private;

            if (ast.OptionalType == null)
            {
                var mp = _resolver.TryGetMetaProperty(src, parent, parent, name, true);

                if (mp == null || mp.IsInvalid)
                    Log.Error(src, ErrorCode.E3203, "Unable to resolve type -- no unambiguous definition of " + name.Quote() + " was found");
                else
                {
                    dt = mp.ReturnType;

                    if (ast.Visibility != 0)
                        Log.Error(src, ErrorCode.W3202, "Cannot specify visibility on overriding meta property " + name.Quote());
                    else
                        visibility = mp.Visibility;
                }
            }
            else
            {
                dt = _resolver.GetType(parent, ast.OptionalType);
                visibility = ast.Visibility;
                var mp = _resolver.TryGetMetaProperty(src, parent, parent, name, false);

                if (mp != null && !mp.IsInvalid)
                {
                    if (dt.Equals(mp.ReturnType)) {
                        if (_terminalNames.Contains(name))
                            Log.Warning(src, ErrorCode.W3201, "Specifying type of terminal property is redundant");
                        else
                            Log.Warning(src, ErrorCode.W3202, name.Quote() + " is already declared in " + mp.Source);
                    } else {
                        if (_terminalNames.Contains(name))
                            Log.Warning(src, ErrorCode.W0000, name.Quote() + " is also a terminal property with a different type");
                        else
                            Log.Warning(src, ErrorCode.W0000, name.Quote() + " is already declared with a different type in " + mp.Source);
                    }
                }
            }

            var result = new MetaProperty(ast.Name.Source, parent, dt, ast.Name.Symbol, visibility);
            _enqueuedProperties.Add(new KeyValuePair<AstMetaProperty, MetaProperty>(ast, result));

            return result;
        }

        void CompileMetaPropertyDefinitions(AstMetaProperty ast, MetaProperty mp)
        {
            var defs = new MetaDefinition[ast.Definitions.Count];

            for (int i = 0; i < defs.Length; i++)
            {
                var fc = new FunctionCompiler(_compiler, mp);

                foreach (var req in ast.Definitions[i].Requirements)
                {
                    DataType dt = null;

                    if (req.Type != null)
                        dt = _resolver.GetType(mp.Parent, req.Type);

                    fc.AddReqStatement(new ReqProperty(req.Source, req.Name.Symbol, dt, req.Offset, req.Tag));
                }

                var fixedInit = ast.Definitions[i].Value as AstFixedArrayInitializer;
                Statement s;

                if (fixedInit != null)
                    s = fc.CompileFixedArrayDeclaration(mp.Source, mp.ReturnType, mp.Name, fixedInit);
                else
                {
                    s = fc.CompileStatement(ast.Definitions[i].Value);

                    if (s is Expression)
                        s = fc.CompileImplicitCast(mp.Source, mp.ReturnType, s as Expression);
                }

                // Find implicit req statements
                if (s is Expression)
                {
                    var e = s as Expression;
                    new ReqStatementFinder(fc).VisitNullable(ref e);
                }
                else
                    new ReqStatementFinder(fc).VisitNullable(ref s);

                defs[i] = new MetaDefinition(s, ast.Definitions[i].Tags, fc.ReqStatements);
            }

            mp.SetDefinitions(defs);
        }
    }
}
