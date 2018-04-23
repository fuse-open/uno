using System.Collections.Generic;
using System.Linq;
using Uno.Compiler.API.Domain;
using Uno.Compiler.API.Domain.IL;
using Uno.Compiler.API.Domain.IL.Members;
using Uno.Compiler.API.Domain.IL.Statements;
using Uno.Compiler.API.Domain.IL.Types;

namespace Uno.Compiler.Core.Syntax.Generators
{
    partial class ShaderGenerator
    {
        internal readonly Dictionary<StructType, int> ShaderStructMap = new Dictionary<StructType, int>();
        internal readonly List<StructType> ShaderStructQueue = new List<StructType>();
        internal int ShaderGlobalCounter;

        class StructProvider : CopyProvider
        {
            public readonly Dictionary<SourceObject, SourceObject> Map = new Dictionary<SourceObject, SourceObject>();

            public override DataType TryGetType(DataType dt)
            {
                SourceObject result;
                if (Map.TryGetValue(dt, out result))
                    return result as DataType;

                return null;
            }

            public override Member TryGetMember(Member member)
            {
                SourceObject result;
                if (Map.TryGetValue(member, out result))
                    return result as Member;

                return null;
            }
        }

        void ProcessStructs()
        {
            if (ShaderStructQueue.Count == 0)
                return;

            var provider = new StructProvider();
            var state = new CopyState(null, provider);
            var owner = Path.DrawBlock.Method.DeclaringType;

            foreach (var st in ShaderStructQueue)
            {
                var newst = new StructType(st.Source, IL, null, Modifiers.Public | Modifiers.Generated, st.UnoName + "_" + ShaderGlobalCounter++);
                provider.Map.Add(st, newst);
                DrawState.Structs.Add(newst);
            }

            foreach (var st in ShaderStructQueue)
            {
                var newst = state.GetType(st);

                foreach (var f in st.Fields)
                {
                    if (f.IsStatic)
                        continue;

                    var newf = new Field(f.Source, newst, f.UnoName, null, Modifiers.Public | Modifiers.Generated, 0, state.GetType(f.ReturnType));

                    newst.Fields.Add(newf);
                    provider.Map.Add(f, newf);
                }
            }


            state.SetFunction(DrawState.VertexShader.Entrypoint);
            DrawState.VertexShader.Entrypoint.SetBody(DrawState.VertexShader.Entrypoint.Body.CopyNullable(state));

            for (int i = 0; i < DrawState.Varyings.Count; i++)
            {
                var v = DrawState.Varyings[i];
                v.Value = v.Value.CopyExpression(state);
                DrawState.Varyings[i] = v;
            }

            foreach (var k in DrawState.VertexShader.Terminals.Keys.ToArray())
                DrawState.VertexShader.Terminals[k] = DrawState.VertexShader.Terminals[k].CopyExpression(state);

            foreach (var f in DrawState.VertexShader.Functions)
            {
                state.SetFunction(f);
                f.ReturnType = state.GetType(f.ReturnType);
                f.SetBody(f.Body.CopyNullable(state));

                foreach (var p in f.Parameters)
                    p.Type = state.GetType(p.Type);
            }


            state.SetFunction(DrawState.PixelShader.Entrypoint);
            DrawState.PixelShader.Entrypoint.SetBody(DrawState.PixelShader.Entrypoint.Body.CopyNullable(state));

            foreach (var k in DrawState.PixelShader.Terminals.Keys.ToArray())
                DrawState.PixelShader.Terminals[k] = DrawState.PixelShader.Terminals[k].CopyExpression(state);

            foreach (var f in DrawState.PixelShader.Functions)
            {
                state.SetFunction(f);
                f.ReturnType = state.GetType(f.ReturnType);
                f.SetBody(f.Body.CopyNullable(state));

                foreach (var p in f.Parameters)
                    p.Type = state.GetType(p.Type);
            }
        }
    }
}
