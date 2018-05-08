using Uno.Compiler.ExportTargetInterop;

namespace Uno.Runtime.Implementation
{
    [TargetSpecificType]
    [extern(DOTNET) DotNetType]
    [extern(CPLUSPLUS && !MOBILE) Set("TypeName", "::Xli::Window*")]
    [extern(CPLUSPLUS && MOBILE) Set("TypeName", "void*")]
    [extern(CPLUSPLUS) Set("ForwardDeclaration", "namespace Xli { class Window; }")]
    [extern(CPLUSPLUS) Require("Header.Include", "XliPlatform/Window.h")]
    public struct PlatformWindowHandle
    {
    }

    [extern(DOTNET) DotNetType]
    [extern(CPLUSPLUS) Require("Source.Include", "Uno/Support.h")]
    [extern(CPLUSPLUS) Require("Source.Include", "XliPlatform/Display.h")]
    [extern(CPLUSPLUS && !MOBILE) Require("Source.Declaration", "extern ::Xli::Window* _XliWindowPtr;")]
    public extern(!MOBILE) static class PlatformWindowImpl
    {
        private static extern(MOBILE) bool keyboardVisible;
        private static extern(MOBILE) Rect keyboardFrame;

        static PlatformWindowImpl()
        {
        }

        public static PlatformWindowHandle GetInstance()
        {
            if defined(CPLUSPLUS)
            @{
                return _XliWindowPtr;
            @}
            else if defined(CSHARP)
            @{
                return global::Uno.ApplicationContext.AppHost.GetPlatformWindow();
            @}
            else
                build_error;
        }

        public static void Close(PlatformWindowHandle handle)
        {
            if defined(CPLUSPLUS)
            @{
                $0->Close();
            @}
            else if defined(CSHARP)
            @{
                ($0).Close();
            @}
            else
                build_error;
        }

        public static string GetTitle(PlatformWindowHandle handle)
        {
            if defined(CPLUSPLUS)
            @{
                return uStringFromXliString($0->GetTitle());
            @}
            else if defined(CSHARP)
            @{
                return ($0).GetTitle();
            @}
            else
                build_error;
        }

        public static void SetTitle(PlatformWindowHandle handle, string title)
        {
            if defined(CPLUSPLUS)
            @{
                $0->SetTitle(uStringToXliString($1));
            @}
            else if defined(CSHARP)
            @{
                ($0).SetTitle($1);
            @}
            else
                build_error;
        }

        public static int2 GetClientSize(PlatformWindowHandle handle)
        {
            if defined(CPLUSPLUS)
            @{
                return uInt2FromXliVector2i($0->GetClientSize());
            @}
            else if defined(CSHARP)
            @{
                return ($0).GetClientSize();
            @}
            else
                build_error;
        }

        public static void SetClientSize(PlatformWindowHandle handle, int2 clientSize)
        {
            if defined(CPLUSPLUS)
            @{
                $0->SetClientSize(uInt2ToXliVector2i($1));
            @}
            else if defined(CSHARP)
            @{
                ($0).SetClientSize($1);
            @}
            else
                build_error;
        }

        public static bool GetFullscreen(PlatformWindowHandle handle)
        {
            if defined(CPLUSPLUS)
            @{
                return (@{bool})$0->IsFullscreen();
            @}
            else if defined(CSHARP)
            @{
                return ($0).GetFullscreen();
            @}
            else
                build_error;
        }

        public static void SetFullscreen(PlatformWindowHandle handle, bool fullscreen)
        {
            if defined(CPLUSPLUS)
            @{
                $0->SetFullscreen((bool)$1);
            @}
            else if defined(CSHARP)
            @{
                ($0).SetFullscreen($1);
            @}
            else
                build_error;
        }

        public static Platform.PointerCursor GetPointerCursor(PlatformWindowHandle handle)
        {
            if defined(CPLUSPLUS)
            @{
                return $0->GetSystemCursor();
            @}
            else if defined(CSHARP)
            @{
                return ($0).GetPointerCursor();
            @}
            else
                return Uno.Platform.PointerCursor.Default;
        }

        public static void SetPointerCursor(PlatformWindowHandle handle, Platform.PointerCursor cursor)
        {
            if defined(CPLUSPLUS)
            @{
                $0->SetSystemCursor((Xli::SystemCursor)$1);
            @}
            else if defined(CSHARP)
            @{
                ($0).SetPointerCursor($1);
            @}
        }

        public static bool GetMouseButtonState(PlatformWindowHandle handle, Platform.MouseButton button)
        {
            if defined(CPLUSPLUS)
            @{
                return $0->GetMouseButtonState((Xli::MouseButton)$1);
            @}
            else if defined(CSHARP)
            @{
                return ($0).GetMouseButtonState($1);
            @}
            else
                build_error;
        }

        public static bool GetKeyState(PlatformWindowHandle handle, Platform.Key key)
        {
            if defined(CPLUSPLUS)
            @{
                return $0->GetKeyState((Xli::Key)$1);
            @}
            else if defined(CSHARP)
            @{
                return ($0).GetKeyState($1);
            @}
            else
                build_error;
        }

        public static void BeginTextInput(PlatformWindowHandle handle, Platform.TextInputHint hint)
        {
            if defined(CPLUSPLUS)
            @{
                $0->BeginTextInput((Xli::TextInputHint)$1);
            @}
            else if defined(CSHARP)
            @{
                ($0).BeginTextInput($1);
            @}
        }

        public static void EndTextInput(PlatformWindowHandle handle)
        {
            if defined(CPLUSPLUS)
            @{
                $0->EndTextInput();
            @}
            else if defined(CSHARP)
            @{
                ($0).EndTextInput();
            @}
        }

        public static bool IsTextInputActive(PlatformWindowHandle handle)
        {
            if defined(CPLUSPLUS)
            @{
                return $0->IsTextInputActive();
            @}
            else if defined(CSHARP)
            @{
                return ($0).IsTextInputActive();
            @}
            else
                return false;
        }

        public static bool HasOnscreenKeyboardSupport(PlatformWindowHandle handle)
        {
            if defined(CPLUSPLUS)
            @{
                return $0->HasOnscreenKeyboardSupport();
            @}
            else if defined(CSHARP)
            @{
                return ($0).HasOnscreenKeyboardSupport();
            @}
            else
                return false;
        }

        public static bool IsOnscreenKeyboardVisible(PlatformWindowHandle handle)
        {
            if defined(CPLUSPLUS)
            @{
                return $0->IsOnscreenKeyboardVisible();
            @}
            else if defined(CSHARP)
            @{
                return ($0).IsOnscreenKeyboardVisible();
            @}
            else
                return false;
        }

        public static void SetOnscreenKeyboardPosition(PlatformWindowHandle handle, int2 position)
        {
            if defined(CPLUSPLUS)
            @{
                return $0->SetOnscreenKeyboardPosition(uInt2ToXliVector2i($1));
            @}
        }

        public static int2 GetOnscreenKeyboardPosition(PlatformWindowHandle handle)
        {
            if defined(CPLUSPLUS)
            @{
                return uInt2FromXliVector2i($0->GetOnscreenKeyboardPosition());
            @}
            else
                return int2(0, 0);
        }

        public static int2 GetOnscreenKeyboardSize(PlatformWindowHandle handle)
        {
            if defined(CPLUSPLUS)
            @{
                return uInt2FromXliVector2i($0->GetOnscreenKeyboardSize());
            @}
            else
                return int2(0, 0);
        }

        public static bool IsStatusBarVisible(PlatformWindowHandle handle)
        {
            if defined(CPLUSPLUS)
            @{
                return $0->IsStatusBarVisible();
            @}
            else
                return false;
        }

        public static int2 GetStatusBarPosition(PlatformWindowHandle handle)
        {
            if defined(CPLUSPLUS)
            @{
                return uInt2FromXliVector2i($0->GetStatusBarPosition());
            @}
            else
                return int2(0, 0);
        }

        public static int2 GetStatusBarSize(PlatformWindowHandle handle)
        {
            if defined(CPLUSPLUS)
            @{
                return uInt2FromXliVector2i($0->GetStatusBarSize());
            @}
            else
                return int2(0, 0);
        }

        public static float GetDensity(PlatformWindowHandle handle)
        {
            if defined(CPLUSPLUS)
            @{
                return ::Xli::Display::GetDensity($0->GetDisplayIndex());
            @}
            else if defined(CSHARP)
            @{
                return ($0).GetDensity();
            @}
            else
                return 1.0f;
        }
    }
}
