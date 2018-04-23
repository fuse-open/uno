// This file was generated based on Library/Core/UnoCore/Source/Uno/Platform/KeyEventArgs.uno.
// WARNING: Changes might be lost if you edit this file directly.

namespace Uno.Platform
{
    [global::Uno.Compiler.ExportTargetInterop.DotNetTypeAttribute(null)]
    public sealed class KeyEventArgs : global::System.EventArgs
    {
        public KeyEventArgs(Key key, EventModifiers modifiers, OSFrame origin)
        {
            this.Key = key;
            this.Origin = origin;
            this.Modifiers = modifiers;
        }

        public OSFrame Origin
        {
            get;
            set;
        }

        public bool Handled
        {
            get;
            set;
        }

        public Key Key
        {
            get;
            set;
        }

        public EventModifiers Modifiers
        {
            get;
            set;
        }

        public bool IsMetaKeyPressed
        {
            get { return this.Modifiers.HasFlag(EventModifiers.MetaKey); }
        }

        public bool IsControlKeyPressed
        {
            get { return this.Modifiers.HasFlag(EventModifiers.ControlKey); }
        }

        public bool IsShiftKeyPressed
        {
            get { return this.Modifiers.HasFlag(EventModifiers.ShiftKey); }
        }

        public bool IsAltKeyPressed
        {
            get { return this.Modifiers.HasFlag(EventModifiers.AltKey); }
        }
    }
}
