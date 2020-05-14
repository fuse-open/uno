using System.Collections.Generic;
using Uno.Compiler.API.Domain;
using Uno.Compiler.API.Domain.Graphics;
using Uno.Compiler.API.Domain.IL;
using Uno.Compiler.API.Domain.IL.Members;
using Uno.Compiler.API.Utilities;
using Uno.IO;
using Uno.Logging;

namespace Uno.Compiler.API.Backends
{
    public abstract class ShaderBackend : IKeywords
    {
        static ShaderBackend _dummy;
        public static ShaderBackend Dummy => _dummy ?? (_dummy = new DummyBackend());

        public abstract string Name { get; }

        public Log Log { get; private set; }
        protected internal Disk Disk { get; private set; }
        protected internal IBuildData Data { get; private set; }
        protected internal IEnvironment Environment { get; private set; }
        protected internal IEssentials Essentials { get; private set; }
        protected internal IILFactory ILFactory { get; private set; }
        protected internal IScheduler Scheduler { get; private set; }

        public readonly List<TerminalProperty> InputProperties = new List<TerminalProperty>
        {
            new TerminalProperty("IsFrontFacing", MetaStage.Pixel, "bool", "pixel extern<bool>\"gl_FrontFacing\""),
            new TerminalProperty("PixelCoord", MetaStage.Pixel, "float4", "pixel extern<float4>\"gl_FragCoord\""),
            new TerminalProperty("PointCoord", MetaStage.Pixel, "float2", "pixel extern<float2>\"gl_PointCoord\""),
        };

        public readonly List<TerminalProperty> OutputProperties = new List<TerminalProperty>
        {
            new TerminalProperty("CullDrawable", MetaStage.Volatile, "bool", null, false),

            new TerminalProperty("BaseVertex", MetaStage.Volatile, "int", "0"),
            new TerminalProperty("VertexCount", MetaStage.Volatile, "int", null, false),

            new TerminalProperty("ClipPosition", MetaStage.Vertex, "float4"),
            new TerminalProperty("DiscardPixel", MetaStage.Pixel, "bool", null, false),
            new TerminalProperty("PixelColor", MetaStage.Pixel, "float4"),

            new TerminalProperty("BlendEnabled", MetaStage.Volatile, "bool", "false"),
            new TerminalProperty("BlendSrc", MetaStage.Volatile, "Uno.Graphics.BlendOperand", "Uno.Graphics.BlendOperand.One"),
            new TerminalProperty("BlendDst", MetaStage.Volatile, "Uno.Graphics.BlendOperand", "Uno.Graphics.BlendOperand.One"),
            new TerminalProperty("BlendSrcRgb", MetaStage.Volatile, "Uno.Graphics.BlendOperand", "BlendSrc"),
            new TerminalProperty("BlendSrcAlpha", MetaStage.Volatile, "Uno.Graphics.BlendOperand", "BlendSrc"),
            new TerminalProperty("BlendDstRgb", MetaStage.Volatile, "Uno.Graphics.BlendOperand", "BlendDst"),
            new TerminalProperty("BlendDstAlpha", MetaStage.Volatile, "Uno.Graphics.BlendOperand", "BlendDst"),
            new TerminalProperty("BlendEquation", MetaStage.Volatile, "Uno.Graphics.BlendEquation", "Uno.Graphics.BlendEquation.Add"),
            new TerminalProperty("BlendEquationRgb", MetaStage.Volatile, "Uno.Graphics.BlendEquation", "BlendEquation"),
            new TerminalProperty("BlendEquationAlpha", MetaStage.Volatile, "Uno.Graphics.BlendEquation", "BlendEquation"),

            new TerminalProperty("DepthTestEnabled", MetaStage.Volatile, "bool", "true"),
            new TerminalProperty("DepthFunc", MetaStage.Volatile, "Uno.Graphics.CompareFunc", "Uno.Graphics.CompareFunc.LessOrEqual"),

            new TerminalProperty("WriteRed", MetaStage.Volatile, "bool", "true"),
            new TerminalProperty("WriteGreen", MetaStage.Volatile, "bool", "true"),
            new TerminalProperty("WriteBlue", MetaStage.Volatile, "bool", "true"),
            new TerminalProperty("WriteAlpha", MetaStage.Volatile, "bool", "true"),
            new TerminalProperty("WriteDepth", MetaStage.Volatile, "bool", "true"),

            new TerminalProperty("LineWidth", MetaStage.Volatile, "float", "1.0f"),
            new TerminalProperty("PointSize", MetaStage.Vertex, "float", null, false),

            new TerminalProperty("CullFace", MetaStage.Volatile, "Uno.Graphics.PolygonFace", "Uno.Graphics.PolygonFace.Back"),
            new TerminalProperty("PolygonWinding", MetaStage.Volatile, "Uno.Graphics.PolygonWinding", "Uno.Graphics.PolygonWinding.CounterClockwise"),
            new TerminalProperty("PrimitiveType", MetaStage.Volatile, "Uno.Graphics.PrimitiveType", "Uno.Graphics.PrimitiveType.Triangles"),
        };

        public virtual void Initialize(ICompiler compiler, IBundle bundle)
        {
            Log = compiler.Log;
            Disk = compiler.Disk;
            Data = compiler.Data;
            Environment = compiler.Environment;
            Essentials = compiler.ILFactory.Essentials;
            ILFactory = compiler.ILFactory;
            Scheduler = compiler.Scheduler;
        }

        public virtual bool IsIntrinsic(DataType dt)
        {
            return dt.BuiltinType != 0;
        }

        public virtual bool IsIntrinsic(Method f)
        {
            if (!f.HasBody || IsIntrinsic(f.DeclaringType))
                return true;

            switch (f.DeclaringType.QualifiedName)
            {
                case "Uno.Vector":
                case "Uno.Matrix":
                case "Uno.Math":
                    return true;
            }

            return false;
        }

        public virtual bool IsIntrinsic(Constructor f)
        {
            return !f.HasBody || IsIntrinsic(f.DeclaringType);
        }

        public virtual bool IsIntrinsic(Operator f)
        {
            return !f.HasBody || IsIntrinsic(f.DeclaringType);
        }

        public virtual bool IsIntrinsic(Cast f)
        {
            return !f.HasBody || IsIntrinsic(f.DeclaringType);
        }

        public virtual bool IsIntrinsic(Function f)
        {
            return
                f is Method && IsIntrinsic(f as Method) ||
                f is Cast && IsIntrinsic(f as Cast) ||
                f is Operator && IsIntrinsic(f as Operator) ||
                f is Constructor && IsIntrinsic(f as Constructor);
        }

        public virtual bool IsReserved(string id)
        {
            return false;
        }

        class DummyBackend : ShaderBackend
        {
            public override string Name => "DummyShader";
        }
    }
}
