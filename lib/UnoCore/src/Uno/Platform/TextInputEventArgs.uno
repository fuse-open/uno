using Uno.Compiler.ExportTargetInterop;

namespace Uno.Platform
{
    public sealed class TextInputEventArgs : EventArgs
    {
        public TextInputEventArgs(string text, EventModifiers modifiers, OSFrame origin)
        {
            Text = text;
            Origin = origin;
            Modifiers = modifiers;
        }

        public TextInputEventArgs(string text, EventModifiers modifiers)
        {
            Text = text;
            Origin = null;
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

        public string Text
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
