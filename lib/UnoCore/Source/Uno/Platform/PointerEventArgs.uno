using Uno.Compiler.ExportTargetInterop;

namespace Uno.Platform
{
    public enum WheelDeltaMode
    {
        DeltaPixel = 0,
        DeltaLine = 1,
        DeltaPage = 2
    }

    public sealed class PointerEventArgs : EventArgs
    {
        public PointerEventArgs(PointerType type, EventModifiers modifiers, bool primary, float2 position, int fingerId, MouseButton mouseButton, float2 wheelDelta, WheelDeltaMode wheelDeltaMode)
        {
            PointerType = type;
            Modifiers = modifiers;
            IsPrimary = primary;
            Position = position;
            FingerId = fingerId;
            MouseButton = mouseButton;
            WheelDelta = wheelDelta;
            WheelDeltaMode = wheelDeltaMode;
        }

        public bool Handled
        {
            get; set;
        }

        public PointerType PointerType
        {
            get; private set;
        }

        // Note: Returns whether this pointer event is fired from the primary pointer device. I.e. first finger on mobile, left mouse button down on desktop
        public bool IsPrimary
        {
            get; private set;
        }

        public float2 Position
        {
            get; private set;
        }

        public int FingerId
        {
            get; private set;
        }

        public MouseButton MouseButton
        {
            get; private set;
        }

        public float2 WheelDelta
        {
            get; private set;
        }

        public WheelDeltaMode WheelDeltaMode
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

        public bool IsLeftButtonPressed
        {
            get { return Modifiers.HasFlag(EventModifiers.LeftButton); }
        }

        public bool IsMiddleButtonPressed
        {
            get { return Modifiers.HasFlag(EventModifiers.MiddleButton); }
        }

        public bool IsRightButtonPressed
        {
            get { return Modifiers.HasFlag(EventModifiers.RightButton); }
        }
    }
}
