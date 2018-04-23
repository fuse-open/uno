using Uno;
using Uno.Graphics;
using Uno.Platform;
using Uno.Collections;
using Uno.Compiler.ExportTargetInterop;

namespace UnoTest.Parser
{
    public class Purple : Uno.Application
    {
        public Purple()
        {
            debug_log "well hi there";
        }

        public override void Draw()
        {
            debug_log "hello to you, dear sir!";
        }
    }
}
//GIDSignIn.sharedInstance().signOut()
