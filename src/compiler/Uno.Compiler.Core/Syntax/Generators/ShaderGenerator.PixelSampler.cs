using System.Collections.Generic;
using Uno.Compiler.API.Domain;
using Uno.Compiler.API.Domain.Graphics;
using Uno.Compiler.API.Domain.IL;
using Uno.Compiler.API.Domain.IL.Expressions;

namespace Uno.Compiler.Core.Syntax.Generators
{
    partial class ShaderGenerator
    {
        readonly Dictionary<string, int> PixelSamplers = new Dictionary<string, int>();

        DataType GetSamplerType(Source src, DataType textureType)
        {
            switch (textureType.BuiltinType)
            {
                case BuiltinType.Texture2D:
                    return Essentials.Sampler2D;
                case BuiltinType.TextureCube:
                    return Essentials.SamplerCube;
                case BuiltinType.VideoTexture:
                    return Essentials.VideoSampler;
            }

            if (!textureType.IsInvalid)
                Log.Error(src, ErrorCode.I0000, "Invalid texture type");

            return DataType.Invalid;
        }

        StageValue ProcessPixelSampler(NewPixelSampler s)
        {
            var texture = ProcessValuesToStage(MetaStage.Volatile, s.Texture);

            int index;
            if (!PixelSamplers.TryGetValue(texture.ToString(), out index))
            {
                var loc = LocationStack.Last();
                var mp = GetProperty(loc);

                index = DrawState.PixelSamplers.Count;
                DrawState.PixelSamplers.Add(new PixelSampler(GetSamplerType(s.Source, texture.ReturnType), CreateShaderName(mp, loc, texture), texture));
                PixelSamplers.Add(texture.ToString(), index);

                if (s.OptionalState != null)
                    DrawState.PixelSamplers[index].OptionalState = ProcessValuesToStage(MetaStage.Volatile, s.OptionalState);
            }
            else
            {
                var oldState = DrawState.PixelSamplers[index].OptionalState;
                var newState = s.OptionalState != null
                             ? ProcessValuesToStage(MetaStage.Volatile, s.OptionalState)
                             : null;

                if (oldState == null && newState != null ||
                    oldState != null && newState == null ||
                    oldState != null && !oldState.IsInvalid && newState != null && !newState.IsInvalid && oldState.ToString() != newState.ToString())
                    Log.Warning(s.Source, ErrorCode.W0000, "Ignoring sampler state. Texture is already sampled using a different sampler state in " + Path.Quote() + ". Reuse the existing sampler state to remove this warning.");
            }

            return new StageValue(new LoadPixelSampler(s.Source, DrawState, index), MetaStage.Pixel, MetaStage.Pixel);
        }
    }
}
