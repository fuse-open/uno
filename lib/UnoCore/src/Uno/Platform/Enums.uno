using Uno.Compiler.ExportTargetInterop;

namespace Uno.Platform
{
    public enum Key
    {
        Backspace = 8,
        Tab = 9,
        Enter = 13,
        ShiftKey = 16,
        ControlKey = 17,
        AltKey = 18,
        Break = 19,
        CapsLock = 20,
        Escape = 27,
        Space = 32,
        PageUp = 33,
        PageDown = 34,
        End = 35,
        Home = 36,
        Left = 37,
        Up = 38,
        Right = 39,
        Down = 40,
        Insert = 45,
        Delete = 46,
        D0 = 48,
        D1 = 49,
        D2 = 50,
        D3 = 51,
        D4 = 52,
        D5 = 53,
        D6 = 54,
        D7 = 55,
        D8 = 56,
        D9 = 57,
        A = 65,
        B = 66,
        C = 67,
        D = 68,
        E = 69,
        F = 70,
        G = 71,
        H = 72,
        I = 73,
        J = 74,
        K = 75,
        L = 76,
        M = 77,
        N = 78,
        O = 79,
        P = 80,
        Q = 81,
        R = 82,
        S = 83,
        T = 84,
        U = 85,
        V = 86,
        W = 87,
        X = 88,
        Y = 89,
        Z = 90,
        NumPad0 = 96,
        NumPad1 = 97,
        NumPad2 = 98,
        NumPad3 = 99,
        NumPad4 = 100,
        NumPad5 = 101,
        NumPad6 = 102,
        NumPad7 = 103,
        NumPad8 = 104,
        NumPad9 = 105,
        F1 = 112,
        F2 = 113,
        F3 = 114,
        F4 = 115,
        F5 = 116,
        F6 = 117,
        F7 = 118,
        F8 = 119,
        F9 = 120,
        F10 = 121,
        F11 = 122,
        F12 = 123,


        // TODO: Review these later

        MenuButton = 200,
        BackButton = 201,

        OSKey = 202,
        MetaKey = 203,
    }

    public enum MouseButton
    {
        Left = 1,
        Middle = 2,
        Right = 3,
        X1 = 4,
        X2 = 5,
    }

    public enum PointerCursor
    {
        None = 0,
        Default = 1,
        Crosshair = 2,
        Pointer = 3,
        Help = 4,
        Move = 5,
        Wait = 6,
        Progress = 7,
        ResizeNorth = 8,
        ResizeEast = 9,
        ResizeSouth = 10,
        ResizeWest = 11,
        ResizeNorthEast = 12,
        ResizeNorthWest = 13,
        ResizeSouthEast = 14,
        ResizeSouthWest = 15,
        IBeam = 16,
    }

    public enum PointerType
    {
        Mouse = 1,
        Touch = 2,
    }

    public enum TextInputHint
    {
        Default = 0,
        Email = 1,
        URL = 2,
        Phone = 3,
        Number = 4
    }

    [Flags]
    public enum EventModifiers
    {
        MetaKey = 1 << 0,
        ControlKey = 1 << 1,
        ShiftKey = 1 << 2,
        AltKey = 1 << 3,
        LeftButton = 1 << 4,
        MiddleButton = 1 << 5,
        RightButton = 1 << 6,
        X1Button = 1 << 7,
        X2Button = 1 << 8,
    }
}
