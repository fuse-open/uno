using System.Collections.Generic;
using Uno.Compiler.API.Domain.IL;
using Uno.Compiler.API.Domain.IL.Statements;

namespace Uno.Compiler.Core.IL.Validation
{
    class FinallyVerifier : Pass
    {
        int _inLoop;
        int _inSwitch;

        public FinallyVerifier(CompilerPass parent)
            : base(parent)
        {
        }

        public override void Begin(ref Statement e)
        {
            switch (e.StatementType)
            {
                default:
                    return;
                case StatementType.For:
                    _inLoop++;
                    return;
                case StatementType.While:
                    _inLoop++;
                    return;
                case StatementType.Switch:
                    _inSwitch++;
                    return;

                case StatementType.Break:
                    // Allow 'break' inside 'switch', 'while' and 'for'
                    if (_inLoop > 0 || _inSwitch > 0)
                        return;
                    break;

                case StatementType.Continue:
                    // Allow 'continue' inside 'while' and 'for'
                    if (_inLoop > 0)
                        return;
                    break;

                case StatementType.Return:
                    break;
            }

            Log.Error(e.Source, ErrorCode.E0000, "Control cannot leave the body of a finally clause");
        }

        public override void End(ref Statement e)
        {
            switch (e.StatementType)
            {
                case StatementType.For:
                    _inLoop--;
                    break;
                case StatementType.While:
                    _inLoop--;
                    break;
                case StatementType.Switch:
                    _inSwitch--;
                    break;
            }
        }
    }
}
