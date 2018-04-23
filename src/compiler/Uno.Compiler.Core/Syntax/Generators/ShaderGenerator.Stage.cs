using System.Collections.Generic;
using Uno.Compiler.API.Domain;
using Uno.Compiler.API.Domain.Graphics;
using Uno.Compiler.API.Domain.IL;
using Uno.Compiler.API.Domain.IL.Expressions;
using Uno.Compiler.API.Domain.IL.Members;
using Uno.Compiler.API.Domain.IL.Types;

namespace Uno.Compiler.Core.Syntax.Generators
{
    partial class ShaderGenerator
    {
        internal StageValue ProcessStage(StageValue sym, MetaStage min, MetaStage max)
        {
            var loc = LocationStack.Last();
            var mp = GetProperty(loc);

            if (max < sym.MinStage)
            {
                Log.Error(sym.Value.Source, ErrorCode.E5015, sym.MinStage.ToLiteral().Quote() + " cannot be accessed from stage " + max.ToLiteral().Quote() + " while processing " + mp.Name.Quote() + " at " + mp.Source + " in " + Path.Quote() + " at " + Path.Source);
                return new StageValue(Expression.Invalid, MetaStage.Const);
            }

            // Handle stage change if necessary or default to lowest possible stage
            if (min > sym.MaxStage ||
                min > sym.MinStage && sym.Value != null && !InlineOnStage(min, sym.Value))
                return ProcessStageChange(sym, sym.MinStage, min, loc, mp);

            return sym;
        }

        readonly Dictionary<string, Field> Fields = new Dictionary<string, Field>();
        readonly Dictionary<string, int> Constants = new Dictionary<string, int>();
        readonly Dictionary<string, int> Uniforms = new Dictionary<string, int>();
        readonly Dictionary<string, int> Varyings = new Dictionary<string, int>();

        StageValue ProcessShaderConstant(StageValue s, MetaLocation loc, MetaProperty mp)
        {
            s = ProcessStage(s, MetaStage.Volatile, MetaStage.Volatile);

            var key = s.Value.ToString();

            int index;
            if (!Constants.TryGetValue(key, out index))
            {
                index = DrawState.RuntimeConstants.Count;
                DrawState.RuntimeConstants.Add(new ShaderVariable(s.Value.ReturnType, CreateShaderName(mp, loc, s.Value), s.Value));
                Constants.Add(key, index);
            }

            return new StageValue(new RuntimeConst(s.Value.Source, DrawState, index), MetaStage.Vertex, MetaStage.Pixel);
        }

        internal StageValue ProcessStageChange(StageValue s, MetaStage fromStage, MetaStage toStage, MetaLocation loc, MetaProperty mp)
        {
            if (s.Value == null)
                return new StageValue(null, toStage);

            if (s.MinStage > fromStage)
            {
                Log.Error(s.Value.Source, ErrorCode.I5016, "Stage error");
                return new StageValue(Expression.Invalid, toStage);
            }

            switch (toStage)
            {
                case MetaStage.Pixel:
                case MetaStage.Vertex:
                    if (fromStage == MetaStage.Vertex && toStage == MetaStage.Pixel)
                    {
                        // Vertex -> Pixel: Varying

                        var key = s.Value.ToString();

                        int index;
                        if (!Varyings.TryGetValue(key, out index))
                        {
                            index = DrawState.Varyings.Count;
                            DrawState.Varyings.Add(new ShaderVariable(s.Value.ReturnType, CreateShaderName(mp, loc, s.Value), s.Value));
                            Varyings.Add(key, index);
                        }

                        return new StageValue(new LoadVarying(s.Value.Source, DrawState, index), MetaStage.Pixel, MetaStage.Pixel);
                    }
                    else if (s.Value.ReturnType == Essentials.Bool || s.Value.ReturnType == Essentials.Int)
                    {
                        return ProcessShaderConstant(s, loc, mp);
                    }
                    else
                    {
                        // Init,Frame -> Vertex,Pixel: Uniform

                        s = ProcessStage(s, MetaStage.Volatile, MetaStage.Volatile);

                        var val = s.Value.ActualValue;
                        var dt = val.ReturnType;
                        var key = val.ToString();

                        int index;
                        if (!Uniforms.TryGetValue(key, out index))
                        {
                            index = DrawState.Uniforms.Count;

                            switch (dt.TypeType)
                            {
                                case TypeType.FixedArray:
                                    // Ugly workaround. ResolvedMetaPropertyValue could already be an address
                                    if (!(val is PlaceholderValue && (val as PlaceholderValue).Value is AddressOf))
                                        val = new AddressOf(val.ActualValue, AddressType.Const);

                                    break;

                                case TypeType.RefArray:
                                    var at = dt as RefArrayType;
                                    var et = at.ElementType;

                                    var size = ProcessShaderConstant(
                                        new StageValue(ILFactory.GetProperty(s.Value.Source, s.Value, "Length"), s.MinStage, s.MaxStage),
                                        loc, mp).Value;

                                    dt = new FixedArrayType(s.Value.Source, et, size, Essentials.Int);
                                    break;
                            }

                            DrawState.Uniforms.Add(new ShaderVariable(dt, CreateShaderName(mp, loc, val), val));
                            Uniforms.Add(key, index);
                        }

                        Expression u = new LoadUniform(s.Value.Source, DrawState, index);

                        if (u.ReturnType.IsFixedArray)
                            u = new AddressOf(u, AddressType.Const);

                        return new StageValue(u, MetaStage.Vertex, MetaStage.Pixel);
                    }

                case MetaStage.Volatile:
                    if (fromStage <= MetaStage.ReadOnly)
                    {
                        // Init -> Frame: Field

                        var src = s.Value.Source;
                        var dt = Path.DrawBlock.Method.DeclaringType;
                        var obj = new This(src, dt).Address;
                        var key = s.Value.ToString();

                        Field field;
                        if (!Fields.TryGetValue(key, out field))
                        {
                            for (int i = 0; i < InitScope.Statements.Count; i++)
                            {
                                if (InitScope.Statements[i] is StoreField)
                                {
                                    var sf = InitScope.Statements[i] as StoreField;
                                    if (sf.Field.ReturnType.Equals(s.Value.ReturnType) && sf.Value.ToString() == key)
                                        field = sf.Field;
                                }
                            }

                            if (field == null)
                            {
                                field = new Field(src, dt, CreateFieldName(mp, loc, s.Value), 
                                    null, Modifiers.Private | Modifiers.Generated, 0, s.Value.ReturnType);
                                dt.Fields.Add(field);
                                InitScope.Statements.Add(new StoreField(src, obj, field, s.Value));
                            }

                            Fields.Add(key, field);
                        }

                        return new StageValue(new LoadField(src, obj, field), MetaStage.ReadOnly, MetaStage.Volatile);
                    }
                    break;

                case MetaStage.ReadOnly:
                    return s;
            }

            Log.Error(s.Value.Source, ErrorCode.E5017, fromStage.ToLiteral().Quote() + " cannot be accessed from " + toStage.ToLiteral() + " stage while processing " + mp.Name.Quote() + " at " + mp.Source + " in " + Path.Quote() + " at " + Path.Source);
            return new StageValue(Expression.Invalid, MetaStage.Const);
        }
    }
}
