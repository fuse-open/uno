using System.Collections.Generic;

namespace Uno.Compiler.API.Domain
{
    public static class TypeAliases
    {
        static readonly Dictionary<string, string> _typeAliases = new Dictionary<string, string>()
        {
            { "object",         "Uno.Object" },
            { "bool",           "Uno.Bool" },
            { "float",          "Uno.Float" },
            { "double",         "Uno.Double" },
            { "float2",         "Uno.Float2" },
            { "float2x2",       "Uno.Float2x2" },
            { "float3",         "Uno.Float3" },
            { "float4",         "Uno.Float4" },
            { "float3x3",       "Uno.Float3x3" },
            { "float4x4",       "Uno.Float4x4" },
            { "int2",           "Uno.Int2" },
            { "int3",           "Uno.Int3" },
            { "int4",           "Uno.Int4" },
            { "byte4",          "Uno.Byte4" },
            { "byte2",          "Uno.Byte2" },
            { "sbyte2",         "Uno.SByte2" },
            { "sbyte4",         "Uno.SByte4" },
            { "short2",         "Uno.Short2" },
            { "short4",         "Uno.Short4" },
            { "ushort2",        "Uno.UShort2" },
            { "ushort4",        "Uno.UShort4" },
            { "char",           "Uno.Char" },
            { "string",         "Uno.String" },
            { "byte",           "Uno.Byte" },
            { "ushort",         "Uno.UShort" },
            { "uint",           "Uno.UInt" },
            { "ulong",          "Uno.ULong" },
            { "sbyte",          "Uno.SByte" },
            { "short",          "Uno.Short" },
            { "int",            "Uno.Int" },
            { "long",           "Uno.Long" },
            { "texture2D",      "Uno.Graphics.Texture2D" },
            { "textureCube",    "Uno.Graphics.TextureCube" },
            { "sampler2D",      "Uno.Graphics.Sampler2D" },
            { "samplerCube",    "Uno.Graphics.SamplerCube" },
            { "framebuffer",    "Uno.Graphics.Framebuffer" }
        };

        public static IEnumerable<string> AllAliases => _typeAliases.Keys;

        public static IEnumerable<KeyValuePair<string, string>> AllPairs => _typeAliases;

        public static bool TryGetTypeFromAlias(string alias, out string type)
        {
            return _typeAliases.TryGetValue(alias, out type);
        }

        public static bool IsAlias(string alias)
        {
            return _typeAliases.ContainsKey(alias);
        }

        static Dictionary<string, string> _typeAliasesReverse;

        public static bool TryGetAliasFromType(string type, out string alias)
        {
            if (_typeAliasesReverse == null)
            {
                _typeAliasesReverse = new Dictionary<string, string>();
                foreach (var p in _typeAliases)
                    _typeAliasesReverse.Add(p.Value, p.Key);
            }

            return _typeAliasesReverse.TryGetValue(type, out alias);
        }

        public static bool HasAlias(string type)
        {
            string foo;
            return TryGetAliasFromType(type, out foo);
        }
    }
}
