// This file was generated based on lib/UnoCore/Source/Uno/Platform/PointerEventArgs.uno.
// WARNING: Changes might be lost if you edit this file directly.

namespace Uno.Platform
{
    [global::Uno.Compiler.ExportTargetInterop.DotNetTypeAttribute(null)]
    public sealed class PointerEventArgs : global::System.EventArgs
    {
        public PointerEventArgs(PointerType type, EventModifiers modifiers, bool primary, global::Uno.Float2 position, int fingerId, MouseButton mouseButton, global::Uno.Float2 wheelDelta, WheelDeltaMode wheelDeltaMode)
        {
            this.PointerType = type;
            this.Modifiers = modifiers;
            this.IsPrimary = primary;
            this.Position = position;
            this.FingerId = fingerId;
            this.MouseButton = mouseButton;
            this.WheelDelta = wheelDelta;
            this.WheelDeltaMode = wheelDeltaMode;
        }

        public bool Handled
        {
            get;
            set;
        }

        public PointerType PointerType
        {
            get;
            set;
        }

        public bool IsPrimary
        {
            get;
            set;
        }

        public global::Uno.Float2 Position
        {
            get;
            set;
        }

        public int FingerId
        {
            get;
            set;
        }

        public MouseButton MouseButton
        {
            get;
            set;
        }

        public global::Uno.Float2 WheelDelta
        {
            get;
            set;
        }

        public WheelDeltaMode WheelDeltaMode
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

        public bool IsLeftButtonPressed
        {
            get { return this.Modifiers.HasFlag(EventModifiers.LeftButton); }
        }

        public bool IsMiddleButtonPressed
        {
            get { return this.Modifiers.HasFlag(EventModifiers.MiddleButton); }
        }

        public bool IsRightButtonPressed
        {
            get { return this.Modifiers.HasFlag(EventModifiers.RightButton); }
        }
    }
}
