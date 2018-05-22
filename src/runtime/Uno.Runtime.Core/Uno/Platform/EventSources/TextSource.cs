// This file was generated based on lib/UnoCore/Source/Uno/Platform/EventSources.uno.
// WARNING: Changes might be lost if you edit this file directly.

namespace Uno.Platform.EventSources
{
    [global::Uno.Compiler.ExportTargetInterop.DotNetTypeAttribute(null)]
    public static class TextSource
    {
        public static void BeginTextInput(global::Uno.Platform.TextInputHint hint)
        {
            global::Uno.Runtime.Implementation.PlatformWindowHandle _handle = global::Uno.Application.Current.Window._handle;
            global::Uno.Runtime.Implementation.PlatformWindowImpl.BeginTextInput(_handle, hint);
        }

        public static void EndTextInput()
        {
            global::Uno.Runtime.Implementation.PlatformWindowHandle _handle = global::Uno.Application.Current.Window._handle;
            global::Uno.Runtime.Implementation.PlatformWindowImpl.EndTextInput(_handle);
        }

        public static bool OnTextInput(string text, global::Uno.Platform.EventModifiers modifiers)
        {
            global::Uno.Platform.TextInputEventArgs args = new global::Uno.Platform.TextInputEventArgs(text, modifiers);
            global::System.EventHandler<global::Uno.Platform.TextInputEventArgs> handler = TextSource.TextInput;

            if (handler != null)
                handler(null, args);

            return args.Handled;
        }

        public static bool IsTextInputActive
        {
            get
            {
                global::Uno.Runtime.Implementation.PlatformWindowHandle _handle = global::Uno.Application.Current.Window._handle;
                return global::Uno.Runtime.Implementation.PlatformWindowImpl.IsTextInputActive(_handle);
            }
        }

        public static event global::System.EventHandler<global::Uno.Platform.TextInputEventArgs> TextInput;
    }
}
