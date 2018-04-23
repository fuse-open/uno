using Uno.Compiler.API.Backends;
using Uno.Compiler.API.Domain;
using Uno.Compiler.API.Domain.Graphics;
using Uno.Compiler.API.Domain.IL;
using Uno.Compiler.API.Domain.IL.Expressions;
using Uno.Compiler.API.Domain.IL.Statements;
using Uno.Compiler.Core.IL.Building.Functions;
using Uno.Compiler.Core.IL.Utilities;
using Uno.Compiler.Core.Syntax.Compilers;
using Uno.Compiler.Core.Syntax.Generators.Passes;
using Uno.Logging;

namespace Uno.Compiler.Core.Syntax.Generators
{
    partial class ShaderGenerator : LogObject
    {
        internal readonly Backend Backend;
        internal readonly Namespace IL;
        internal readonly BuildEnvironment Environment;
        internal readonly Essentials Essentials;
        internal readonly ILFactory ILFactory;
        internal readonly Compiler Compiler;
        internal readonly DrawState DrawState;
        internal readonly Drawable Path;
        internal readonly Scope InitScope;
        internal readonly Scope FreeScope;
        internal readonly Scope FrameScope;
        internal readonly Scope VertexScope;
        internal readonly Scope PixelScope;
        internal readonly int InitStart;
        internal readonly int FrameStart;

        int FieldNameCount;
        int ShaderNameCount;

        public ShaderGenerator(Compiler compiler, Drawable path, Scope initScope, Scope freeScope, Scope drawScope)
            : base(compiler)
        {
            Backend = compiler.Backend;
            IL = compiler.Data.IL;
            Environment = compiler.Environment;
            Essentials = compiler.Essentials;
            ILFactory = compiler.ILFactory;
            Compiler = compiler;
            DrawState = path.DrawState;
            Path = path;
            InitScope = initScope;
            FreeScope = freeScope;
            FrameScope = drawScope;
            VertexScope = DrawState.VertexShader.Entrypoint.Body;
            PixelScope = DrawState.PixelShader.Entrypoint.Body;
            InitStart = InitScope.Statements.Count;
            FrameStart = FrameScope.Statements.Count;
        }

        string GetBasename(MetaProperty mp, Expression value)
        {
            if (value is PlaceholderReference)
                return (value as PlaceholderReference).Value.Property.Name;

            return mp.Name;
        }

        internal string CreateShaderName(MetaProperty mp, MetaLocation loc, Expression value = null)
        {
            return GetBasename(mp, value) + "_" + loc.NodeIndex + "_" + loc.BlockIndex + "_" + ShaderNameCount++;
        }

        internal string CreateLocalName(MetaProperty mp, MetaLocation loc, Expression value = null)
        {
            return GetBasename(mp, value) + "_" + Path.Suffix + "_" + loc.NodeIndex + "_" + loc.BlockIndex + "_" + FieldNameCount++;
        }

        internal string CreateFieldName(MetaProperty mp, MetaLocation loc, Expression value = null)
        {
            return Path.DrawBlock.Method.UnoName + "_" + CreateLocalName(mp, loc, value);
        }

        public void Generate()
        {
            // Verify and add path to meta property cache
            for (int i = 0; i < Path.Nodes.Length; i++)
            {
                for (int j = Path.Nodes[i].Top; j <= Path.Nodes[i].Bottom; j++)
                {
                    var mp = (MetaProperty)Path.Nodes[i].Block.Members[j];
                    var loc = new MetaLocation(i, j);

                    MetaLocation ploc;
                    if (MetaProperties.TryGetValue(mp.Name, out ploc))
                    {
                        var pmp = GetProperty(ploc);


                        // TODO: This is not correct
                        if (!pmp.ReturnType.Equals(mp.ReturnType))
                            Log.Error(mp.Source, ErrorCode.E5000, mp.Name.Quote() + " does not have the same type as the previous declaration at " + pmp.Source + " when exposed from " + Path.Quote() + " at " + Path.Source);

                        PrevProperties.Add(loc, ploc);
                    }

                    MetaProperties[mp.Name] = loc;
                }
            }

            // Resolve terminal properties
            foreach (var tp in Backend.ShaderBackend.OutputProperties)
            {
                MetaLocation loc;
                if (!MetaProperties.TryGetValue(tp.Name, out loc))
                {
                    Log.Error(Path.Source, ErrorCode.I5001, "Terminal property " + tp.Name.Quote() + " was not found in " + Path.Quote());
                    continue;
                }

                var mp = GetProperty(loc);
                var dt = ILFactory.GetType(Path.Source, tp.TypeString);

                if (!mp.ReturnType.Equals(dt))
                {
                    Log.Error(Path.Source, ErrorCode.I5001, "Terminal property " + tp.Name.Quote() + " was found with type " + mp.ReturnType.Quote() + " when " + dt.Quote() + " was expected");
                    continue;
                }

                var sym = ProcessMetaProperty(loc, tp.Required);
                if (sym.Value == null) continue;

                LocationStack.Add(loc);

                switch (tp.Stage)
                {
                    case MetaStage.Vertex:
                        sym = ProcessStage(sym, MetaStage.Vertex, MetaStage.Vertex);
                        DrawState.VertexShader.Terminals[tp.Name] = sym.Value;
                        break;

                    case MetaStage.Pixel:
                        sym = ProcessStage(sym, MetaStage.Pixel, MetaStage.Pixel);
                        DrawState.PixelShader.Terminals[tp.Name] = sym.Value;
                        break;

                    default:
                        sym = ProcessStage(sym, MetaStage.Volatile, MetaStage.Volatile);
                        DrawState.Terminals[tp.Name] = sym.Value;
                        break;
                }

                LocationStack.RemoveLast();
            }

            if (!DrawState.Terminals.ContainsKey("VertexCount"))
            {
                var loc = MetaProperties["VertexCount"];
                var mp = GetProperty(loc);

                if (DetectedVertexCounts.Count == 1)
                {
                    LocationStack.Add(loc);

                    var fc = new FunctionCompiler(Compiler, IL);
                    DrawState.Terminals["VertexCount"] = fc.CompileImplicitCast(DetectedVertexCounts[0].Value.Source, mp.ReturnType, ProcessStage(DetectedVertexCounts[0], MetaStage.Volatile, MetaStage.Volatile).Value);

                    LocationStack.RemoveLast();
                }
                else
                {
                    Log.Error(CreateTrace(mp, loc, null), ErrorCode.E5002, "Unable to auto detect 'VertexCount' in " + Path.Quote());
                }
            }

            MetaPropertyEmitter.Emit(this);

            ShaderProcessor.ProcessShader(this, DrawState.VertexShader);
            ShaderProcessor.ProcessShader(this, DrawState.PixelShader);

            ProcessStructs();

            var p = new IndirectionTransform(Compiler.Pass);
            DrawState.VertexShader.Visit(p);
            DrawState.PixelShader.Visit(p);
        }
    }
}
