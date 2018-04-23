using Uno.Compiler.API.Domain;
using Uno.Compiler.API.Domain.Graphics;

namespace Uno.Compiler.Core.IL.Validation
{
    public partial class ILVerifier
    {
        public override bool Begin(BlockBase b)
        {
            var bp = b.ParentBlock;
            if (bp != null && bp.BlockType == BlockType.MetaBlock)
            {
                if (b.BlockType != BlockType.MetaBlock ||
                    ((MetaBlock)bp).MetaBlockType != MetaBlockType.Scope)
                    Log.Error(b.Source, ErrorCode.E0000, "Block " + b.Quote() + " is not allowed in meta block");
            }

            return !Environment.IsGeneratingCode;
        }

        public override void OnApply(Apply apply)
        {
            switch (apply.Modifier)
            {
                case ApplyModifier.Sealed:
                case ApplyModifier.Virtual:
                    if (apply.Object == null) // TODO: Also verify that object is a variable
                        Log.Error(apply.Source, ErrorCode.E0000, "Can only 'apply " + apply.Modifier.ToString().ToLower() + "' a variable");
                    else if (apply.Modifier == ApplyModifier.Virtual)
                    {
                        var dt = apply.Object.ReturnType;

                        if (!dt.IsClass && !dt.IsInvalid)
                            Log.Error(apply.Source, ErrorCode.E0000, "Cannot 'apply virtual' an instance of " + dt.TypeType.ToString().ToLower() + " " + dt.Quote());
                        else if (dt.IsSealed)
                            Log.Error(apply.Source, ErrorCode.E0000, "Cannot 'apply virtual' an instance of sealed class " + dt.Quote());

                        if (!(Block is DrawBlock))
                            Log.Error(apply.Source, ErrorCode.E0000, "'apply virtual' is only allowed as part of a draw statement");
                    }

                    break;
            }
        }
    }
}
