using System;
using System.Collections.Generic;
using Uno.Compiler.API.Domain.Graphics;
using Uno.Compiler.API.Domain.IL;

namespace Uno.Compiler.Backends.UnoDoc.Builders.Extensions
{
    public static class BlockExtensions
    {
        public static Namespace FindNamespace(this Block block)
        {
            if (block.ParentNamespace != null)
            {
                return block.ParentNamespace;
            }

            var parent = block.Parent;
            while (parent != null)
            {
                var ns = parent as Namespace;
                if (ns != null)
                {
                    return ns;
                }
                parent = parent.Parent;
            }

            throw new ArgumentException("Unable to find parent namespace for block " + block.FullName);
        }

        public static List<string> GetModifierNames(this Block block)
        {
            return block.Modifiers.GetModifierNames();
        }
    }
}