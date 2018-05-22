// This file was generated based on lib/UnoCore/Source/Uno/Platform/EventSources.uno.
// WARNING: Changes might be lost if you edit this file directly.

namespace Uno.Platform.EventSources
{
    [global::Uno.Compiler.ExportTargetInterop.DotNetTypeAttribute(null)]
    public static class HardwareKeys
    {
        public static bool OnKeyDown(global::Uno.Platform.Key key, global::Uno.Platform.EventModifiers modifiers)
        {
            global::Uno.Platform.KeyEventArgs args = new global::Uno.Platform.KeyEventArgs(key, modifiers, null);
            global::System.EventHandler<global::Uno.Platform.KeyEventArgs> handler = HardwareKeys.KeyDown;

            if (handler != null)
                handler(null, args);

            return args.Handled;
        }

        public static bool OnKeyUp(global::Uno.Platform.Key key, global::Uno.Platform.EventModifiers modifiers)
        {
            global::Uno.Platform.KeyEventArgs args = new global::Uno.Platform.KeyEventArgs(key, modifiers, null);
            global::System.EventHandler<global::Uno.Platform.KeyEventArgs> handler = HardwareKeys.KeyUp;

            if (handler != null)
                handler(null, args);

            return args.Handled;
        }

        public static event global::System.EventHandler<global::Uno.Platform.KeyEventArgs> KeyDown;
        public static event global::System.EventHandler<global::Uno.Platform.KeyEventArgs> KeyUp;
    }
}
