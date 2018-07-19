using System.Collections.Generic;
using Uno.Compiler.API.Domain;
using Uno.Compiler.API.Domain.Graphics;
using Uno.Compiler.API.Domain.IL;
using Uno.Compiler.API.Domain.IL.Expressions;
using Uno.Compiler.API.Domain.IL.Members;

namespace Uno.Compiler.Core.Syntax.Generators
{
    partial class ShaderGenerator
    {
        string IndexBuffer, IndexType;

        readonly Dictionary<string, Expression> VertexBuffers = new Dictionary<string, Expression>();
        readonly List<StageValue> DetectedVertexCounts = new List<StageValue>();

        void AddDetectedVertexCount(StageValue c)
        {
            var key = c.Value.ToString();

            foreach (var d in DetectedVertexCounts)
                if (d.Value.ToString() == key)
                    return;

            DetectedVertexCounts.Add(c);
        }

        void ProcessIndexBuffer(Source vaSrc, StageValue indexBuffer, StageValue indexType)
        {
            if (IndexBuffer == null && indexBuffer.Value == null && IndexType == null && indexType.Value == null ||
                indexBuffer.Value != null && IndexBuffer == indexBuffer.Value.ToString() && indexType.Value != null && IndexType == indexType.Value.ToString())
            {
                // OK
                return;
            }

            if (IndexBuffer != null || indexBuffer.Value == null || IndexType != null || indexType.Value == null)
            {
                Log.Error(vaSrc, ErrorCode.E5023, "Index buffer argument must be consistent for all <vertex_attrib>s in " + Path.Quote());
                return;
            }

            var src = indexBuffer.Value.Source;
            var type = ILFactory.GetType(src, "Uno.Graphics.IndexBuffer");

            IndexBuffer = indexBuffer.Value.ToString();
            IndexType = indexType.Value.ToString();

            if (indexBuffer.Value.ReturnType.Equals(type))
            {
                DrawState.OptionalIndices = new IndexBinding(
                    ProcessStage(indexType, MetaStage.Volatile, MetaStage.Volatile).Value,
                    ProcessStage(indexBuffer, MetaStage.Volatile, MetaStage.Volatile).Value);

                return;
            }

            var loc = LocationStack.Last();
            var mp = GetProperty(loc);

            var name = CreateFieldName(mp, loc);
            var owner = Path.DrawBlock.Method.DeclaringType;

            var field = new Field(src, owner, name, 
                null, Modifiers.Private | Modifiers.Generated, 0, type);
            owner.Fields.Add(field);

            DrawState.OptionalIndices = new IndexBinding(
                ProcessStage(indexType, MetaStage.Volatile, MetaStage.Volatile).Value,
                new LoadField(src, new This(src, owner), field));

            if (indexBuffer.MinStage > MetaStage.Volatile)
            {
                Log.Error(src, ErrorCode.E5024, "Index buffer cannot be accessed from " + indexBuffer.MinStage + " stage");
                return;
            }

            if (indexBuffer.MinStage == MetaStage.Volatile)
            {
                InitScope.Statements.Add(
                    new StoreField(src, new This(src, owner), field,
                        ILFactory.NewObject(src, "Uno.Graphics.IndexBuffer",
                            ILFactory.GetExpression(src, "Uno.Graphics.BufferUsage.Dynamic"))));

                FrameScope.Statements.Add(
                    ILFactory.CallMethod(src, new LoadField(src, new This(src, owner), field), "Update",
                        indexBuffer.Value));
            }
            else
            {
                InitScope.Statements.Add(
                    new StoreField(src, new This(src, owner), field, ILFactory.NewObject(src, "Uno.Graphics.IndexBuffer",
                        indexBuffer.Value,
                        ILFactory.GetExpression(src, "Uno.Graphics.BufferUsage.Immutable"))));
            }

            FreeScope.Statements.Add(
                ILFactory.CallMethod(src, new LoadField(src, new This(src, owner), field), "Dispose"));
        }

        Expression ProcessVertexBuffer(StageValue vertexBuffer)
        {
            Expression result;
            if (!VertexBuffers.TryGetValue(vertexBuffer.Value.ToString(), out result))
            {
                var src = vertexBuffer.Value.Source;
                var type = ILFactory.GetType(src, "Uno.Graphics.VertexBuffer");

                if (vertexBuffer.Value.ReturnType.Equals(type))
                {
                    result = ProcessStage(vertexBuffer, MetaStage.Volatile, MetaStage.Volatile).Value;
                    VertexBuffers.Add(vertexBuffer.Value.ToString(), result);
                    return result;
                }

                var loc = LocationStack.Last();
                var mp = GetProperty(loc);

                var name = CreateFieldName(mp, loc);
                var owner = Path.DrawBlock.Method.DeclaringType;

                var field = new Field(src, owner, name, null, Modifiers.Private | Modifiers.Generated, 0, type);
                owner.Fields.Add(field);

                result = new LoadField(src, new This(src, owner), field);
                VertexBuffers.Add(vertexBuffer.Value.ToString(), result);

                if (vertexBuffer.MinStage > MetaStage.Volatile)
                {
                    Log.Error(src, ErrorCode.E5025, "Vertex buffer cannot be accessed from " + vertexBuffer.MinStage + " stage");
                    return result;
                }
                else if (vertexBuffer.MinStage == MetaStage.Volatile)
                {
                    InitScope.Statements.Add(
                        new StoreField(src, new This(src, owner), field,
                            ILFactory.NewObject(src, "Uno.Graphics.VertexBuffer",
                                ILFactory.GetExpression(src, "Uno.Graphics.BufferUsage.Dynamic"))));

                    FrameScope.Statements.Add(
                        ILFactory.CallMethod(src, new LoadField(src, new This(src, owner), field), "Update",
                            vertexBuffer.Value));
                }
                else
                {
                    InitScope.Statements.Add(
                        new StoreField(src, new This(src, owner), field,
                            ILFactory.NewObject(src, "Uno.Graphics.VertexBuffer",
                                vertexBuffer.Value,
                                ILFactory.GetExpression(src, "Uno.Graphics.BufferUsage.Immutable"))));
                }

                FreeScope.Statements.Add(
                    ILFactory.CallMethod(src, new LoadField(src, new This(src, owner), field), "Dispose"));
            }

            return result;
        }

        void DetectVertexCount(StageValue buffer)
        {
            foreach (var p in buffer.Value.ReturnType.Properties)
            {
                if (p.UnoName == "Length")
                {
                    AddDetectedVertexCount(new StageValue(new GetProperty(buffer.Value.Source, buffer.Value, p), buffer.MinStage, buffer.MaxStage));
                    return;
                }
            }
        }

        StageValue ProcessVertexAttrib(NewVertexAttrib s)
        {
            var vertexAttributeType = ProcessValue(s.VertexAttributeType);
            var vertexBuffer = ProcessValue(s.VertexBuffer);
            var vertexOffset = ProcessValue(s.VertexBufferOffset);
            var vertexStride = ProcessValue(s.VertexBufferStride);

            var indexType = ProcessValue(s.OptionalIndexType);
            var indexBuffer = ProcessValue(s.OptionalIndexBuffer);

            var loc = LocationStack.Last();
            var mp = GetProperty(loc);
            var name = CreateShaderName(mp, loc);

            int attrIndex = DrawState.VertexAttributes.Count;

            DetectVertexCount(indexBuffer.Value != null 
                    ? indexBuffer 
                    : vertexBuffer);
            ProcessIndexBuffer(s.Source, indexBuffer, indexType);

            DrawState.VertexAttributes.Add(new VertexAttribute(s.ReturnType, name,
                ProcessStage(vertexAttributeType, MetaStage.Volatile, MetaStage.Volatile).Value,
                ProcessVertexBuffer(vertexBuffer),
                ProcessStage(vertexOffset, MetaStage.Volatile, MetaStage.Volatile).Value,
                ProcessStage(vertexStride, MetaStage.Volatile, MetaStage.Volatile).Value));

            return new StageValue(new LoadVertexAttrib(s.Source, DrawState, attrIndex), MetaStage.Vertex, MetaStage.Vertex);
        }
    }
}
