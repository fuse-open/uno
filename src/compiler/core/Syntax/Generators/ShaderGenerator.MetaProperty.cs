using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Uno.Compiler.API.Domain;
using Uno.Compiler.API.Domain.Graphics;
using Uno.Compiler.API.Domain.IL;
using Uno.Compiler.API.Domain.IL.Expressions;
using Uno.Compiler.API.Domain.IL.Members;
using Uno.Compiler.API.Domain.IL.Statements;
using Uno.Compiler.Core.Syntax.Compilers;
using Uno.Compiler.Core.Syntax.Generators.Passes;

namespace Uno.Compiler.Core.Syntax.Generators
{
    partial class ShaderGenerator
    {
        readonly Dictionary<string, MetaLocation> MetaProperties = new Dictionary<string, MetaLocation>();
        readonly Dictionary<MetaLocation, MetaLocation> PrevProperties = new Dictionary<MetaLocation, MetaLocation>();

        Expression TryGetObject(MetaLocation loc)
        {
            return Path.Nodes[loc.NodeIndex].Object;
        }

        internal MetaProperty GetProperty(MetaLocation loc)
        {
            return (MetaProperty)Path.Nodes[loc.NodeIndex].Block.Members[loc.BlockIndex];
        }

        MetaLocation? TryGetLocation(MetaLocation loc, string name, uint offset)
        {
            // TODO: Needs adjustments

            MetaLocation result;
            if (!MetaProperties.TryGetValue(name, out result))
                 return null;

            if (offset > 0)
            {
                // Find location of offset 1 (First location previous to loc)
                while (result.NodeIndex > loc.NodeIndex || result.NodeIndex == loc.NodeIndex && result.BlockIndex >= loc.BlockIndex)
                    if (!PrevProperties.TryGetValue(result, out result))
                        return null;

                for (int i = 0; i < offset - 1; i++)
                    if (!PrevProperties.TryGetValue(result, out result))
                        return null;
            }

            return result;
        }

        bool IsDefinitionValid(MetaLocation loc, MetaDefinition def)
        {
            foreach (var req in def.Requirements)
            {
                switch (req.Type)
                {
                    case ReqStatementType.File:
                        {
                            if (File.Exists((req as ReqFile).Filename))
                                continue;

                            Path.FailedReqStatements.Add(Tuple.Create(loc, def, req));
                            return false;
                        }

                    case ReqStatementType.Object:
                        {
                            var obj = TryGetObject(loc);
                            var dt = (req as ReqObject).ObjectType;

                            if (obj != null && obj.ReturnType.IsSubclassOfOrEqual(dt))
                                continue;

                            Path.FailedReqStatements.Add(Tuple.Create(loc, def, req));
                            return false;
                        }

                    case ReqStatementType.Property:
                        {
                            var rmp = req as ReqProperty;

                            var rloc = TryGetLocation(loc, rmp.PropertyName, rmp.Offset);
                            if (rloc == null)
                            {
                                Path.FailedReqStatements.Add(Tuple.Create(loc, def, req));
                                return false;
                            }

                            var rdt = GetProperty(rloc.Value).ReturnType;
                            if (rmp.PropertyType != null && !rmp.PropertyType.Equals(rdt))
                            {
                                Path.FailedReqStatements.Add(Tuple.Create(loc, def, req));
                                return false;
                            }

                            var rdef = GetValidDefinition(rloc.Value);
                            if (rdef == null || rmp.Tag != null && !rdef.Tags.Contains(rmp.Tag))
                            {
                                Path.FailedReqStatements.Add(Tuple.Create(loc, def, req));
                                return false;
                            }

                            continue;
                        }
                }

                Path.FailedReqStatements.Add(Tuple.Create(loc, def, req));
                return false;
            }

            return true;
        }

        readonly Dictionary<MetaLocation, MetaDefinition> ValidDefinitions = new Dictionary<MetaLocation, MetaDefinition>();
        readonly List<MetaLocation> DefinitionLocationStack = new List<MetaLocation>();

        MetaDefinition GetValidDefinition(MetaLocation loc)
        {
            MetaDefinition result;
            if (ValidDefinitions.TryGetValue(loc, out result)) return result;

            // Detect circular references
            foreach (var ploc in DefinitionLocationStack)
            {
                if (ploc == loc)
                {
                    var mp = GetProperty(loc);
                    Log.Error(CreateTrace(mp, loc, DefinitionLocationStack), ErrorCode.E5003, "Circular reference to " + mp.Name.Quote() + " detected while processing " + Path.Quote());
                    return null;
                }
            }

            DefinitionLocationStack.Add(loc);

            foreach (var def in GetProperty(loc).Definitions)
            {
                if (IsDefinitionValid(loc, def))
                {
                    DefinitionLocationStack.RemoveLast();
                    Path.ReferencedMetaProperties.Add(loc, true);
                    ValidDefinitions.Add(loc, def);
                    return def;
                }
            }

            DefinitionLocationStack.RemoveLast();
            Path.ReferencedMetaProperties.Add(loc, false);
            ValidDefinitions.Add(loc, null);
            return null;
        }

        internal readonly List<MetaLocation> LocationStack = new List<MetaLocation>();
        internal readonly Dictionary<MetaLocation, StageValue> ProcessedMetaProperties = new Dictionary<MetaLocation, StageValue>();

        StageValue ProcessMetaProperty(MetaLocation loc, bool required = true)
        {
            StageValue result;
            if (ProcessedMetaProperties.TryGetValue(loc, out result))
                return new StageValue(result.Value.CopyExpression(new CopyState(DrawState.Path.DrawBlock.Method)), result.MinStage, result.MaxStage);

            var mp = GetProperty(loc);
            var def = GetValidDefinition(loc);

            if (def == null)
            {
                if (required)
                {
                    Log.Error(CreateTrace(mp, loc, null), ErrorCode.E5004, "No valid definition of " + mp.Name.Quote() + " was found in " + Path.Quote());
                    return new StageValue(Expression.Invalid, MetaStage.Const);
                }

                return new StageValue(null, MetaStage.Const);
            }

            // Detect circular references (should not happen)
            foreach (var ploc in LocationStack)
            {
                if (ploc == loc)
                {
                    Log.Error(CreateTrace(mp, loc, LocationStack), ErrorCode.E5005, "Circular reference to " + mp.Name.Quote() + " detected while processing " + Path.Quote());
                    return new StageValue(Expression.Invalid, MetaStage.Const);
                }
            }

            LocationStack.Add(loc);

            switch (def.Value.StatementType)
            {
                case StatementType.Expression:
                    result = ProcessValue(def.Value as Expression);
                    result.Value = new FunctionCompiler(Compiler, mp).CompileImplicitCast(def.Value.Source, mp.ReturnType, result.Value);
                    break;

                case StatementType.Scope:
                    result = ScopeProcessor.Process(this, def.Value as Scope);
                    break;

                case StatementType.FixedArrayDeclaration:
                    result = ProcessFixedArrayDeclaration(def.Value as FixedArrayDeclaration);
                    break;

                default:
                    // Should not happen
                    Log.Error(def.Value.Source, ErrorCode.I5006, "<" + def.Value.StatementType + "> is not supported by ShaderGenerator");
                    result = new StageValue(Expression.Invalid, MetaStage.Const);
                    ProcessedMetaProperties.Add(loc, result);
                    return result;
            }

            LocationStack.RemoveLast();

            if (!InlineOnStage(result.MinStage, result.Value))
            {
                MetaStage resultStage;
                Scope resultScope;

                switch (result.MinStage)
                {
                    case MetaStage.Pixel:
                        resultStage = MetaStage.Pixel;
                        resultScope = PixelScope;
                        break;

                    case MetaStage.Vertex:
                        resultStage = MetaStage.Vertex;
                        resultScope = VertexScope;
                        break;

                    case MetaStage.Volatile:
                        resultStage = MetaStage.Volatile;
                        resultScope = FrameScope;
                        break;

                    default:
                        resultStage = MetaStage.ReadOnly;
                        resultScope = InitScope;
                        break;
                }

                var val = new PlaceholderValue(mp, loc, result.Value, resultStage);
                result = new StageValue(new PlaceholderReference(val), val.Stage);
                resultScope.Statements.Add(new VariableDeclaration(mp.Source, null, CreateLocalName(mp, loc), val.ReturnType, VariableType.Default, val));
            }
            else if (!(result.Value is PlaceholderReference) && !(result.Value is PlaceholderValue) && !(result.Value is Constant))
            {
                result.Value = new PlaceholderValue(mp, loc, result.Value, MetaStage.Undefined);
            }

            ProcessedMetaProperties.Add(loc, result);
            return result;
        }
    }
}
