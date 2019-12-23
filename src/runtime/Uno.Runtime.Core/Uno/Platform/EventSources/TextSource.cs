// This file was generated based on lib/UnoCore/Source/Uno/Platform/EventSources.uno.
// WARNING: Changes might be lost if you edit this file directly.

namespace Uno.Platform.EventSources
{
    [global::Uno.Compiler.ExportTargetInterop.DotNetTypeAttribute(null)]
    public static class TextSource
    {
        public static void BeginTextInput(global::Uno.Platform.TextInputHint hint)
        {
            global::Uno.Application.Current.Window.Backend.BeginTextInput(hint);
        }

        public static void EndTextInput()
        {
            global::Uno.Application.Current.Window.Backend.EndTextInput();
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
                return global::Uno.Application.Current.Window.Backend.IsTextInputActive();
            }
        }

        public static event global::System.EventHandler<global::Uno.Platform.TextInputEventArgs> TextInput;
    }
}
