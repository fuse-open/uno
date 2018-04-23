using Uno.Compiler.API.Domain;
using Uno.Compiler.API.Domain.Graphics;
using Uno.Compiler.API.Domain.IL;
using Uno.Compiler.API.Domain.IL.Members;
using Uno.Compiler.API.Domain.IL.Statements;

namespace Uno.Compiler.Core.IL.Validation.ControlFlow
{
    /// <summary>
    /// Verifies that all control flow is valid:
    /// - All code is reachable
    /// - All return paths return a value (if non-void)
    /// - All break/continue have valid destinations (this is reported by the bytecode compiler)
    /// - All variables are initialized before read or passed by ref
    /// - All out-parameters are written to before returning
    /// - All values passed as ref-parameters are written to before passed
    /// - All struct fields are written to before constructors return
    /// </summary>
    class ControlFlowVerifier : CompilerPass
    {
        public ControlFlowVerifier(CompilerPass parent)
            : base(parent)
        {
        }

        public override bool Begin(DataType dt)
        {
            if (Backend.IsDefault && (
                    dt.Package.IsCached ||
                    dt.Package.IsVerified))
                return false;
            return true;
        }

        public override bool Begin(MetaProperty mp)
        {
            foreach (var def in mp.Definitions)
                if (def.Value is Scope)
                    new ControlFlowValidator(
                        new Method(mp.Source,
                            mp.Parent.TryFindTypeParent() ?? Essentials.Object, 
                            null, Modifiers.Static,
                            ".meta_property", mp.ReturnType, 
                            new Parameter[0], (Scope) def.Value))
                        .Validate(Log);
            return false;
        }

        public override bool Begin(Function f)
        {
            if (f.HasBody)
                new ControlFlowValidator(f).Validate(Log);
            return false;
        }
    }
}
