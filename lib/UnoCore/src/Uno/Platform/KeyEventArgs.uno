using Uno.Compiler.ExportTargetInterop;

namespace Uno.Platform
{
    public sealed class KeyEventArgs : EventArgs
    {
        public KeyEventArgs(Key key, EventModifiers modifiers, OSFrame origin=null)
        {
            Key = key;
            Origin = origin;
            Modifiers = modifiers;
        }

        public OSFrame Origin
        {
            get; private set;
        }

        public bool Handled
        {
            get; set;
        }

        public Key Key
        {
            get; private set;
        }

        internal EventModifiers Modifiers
        {
            get; private set;
        }

        public bool IsMetaKeyPressed
        {
            get { return Modifiers.HasFlag(EventModifiers.MetaKey); }
        }

        public bool IsControlKeyPressed
        {
            get { return Modifiers.HasFlag(EventModifiers.ControlKey); }
        }

        public bool IsShiftKeyPressed
        {
            get { return Modifiers.HasFlag(EventModifiers.ShiftKey); }
        }

        public bool IsAltKeyPressed
        {
            get { return Modifiers.HasFlag(EventModifiers.AltKey); }
        }
    }
}
