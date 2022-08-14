using Uno.Compiler.ExportTargetInterop;

namespace Uno.Platform.Xli
{
    [TargetSpecificType]
    [Set("TypeName", "::Xli::Window*")]
    [Set("ForwardDeclaration", "namespace Xli { class Window; }")]
    [Require("Header.Include", "XliPlatform/Window.h")]
    extern(CPLUSPLUS && !MOBILE) struct XliWindowPtr
    {
    }

    [Require("Source.Include", "XliPlatform/Display.h")]
    [Require("Source.Declaration", "extern ::Xli::Window* _XliWindowPtr;")]
    extern(CPLUSPLUS && !MOBILE) class XliWindow : WindowBackend
    {
        readonly XliWindowPtr _ptr;

        public XliWindow()
        @{
            @{$$._ptr} = _XliWindowPtr;
        @}

        public override void Close()
        @{
            @{$$._ptr}->Close();
        @}

        public override string GetTitle()
        @{
            const auto temp = @{$$._ptr}->GetTitle();
            return uString::Utf8(temp.Ptr(), temp.Length());
        @}

        public override void SetTitle(string title)
        @{
            uCString temp($0);
            @{$$._ptr}->SetTitle(uBase::String(temp.Ptr, (int)temp.Length));
        @}

        public override int2 GetClientSize()
        @{
            const auto temp = @{$$._ptr}->GetClientSize();
            return *(@{int2}*)&temp;
        @}

        public override void SetClientSize(int2 size)
        @{
            @{$$._ptr}->SetClientSize(*(uBase::Vector2i*)&$0);
        @}

        public override bool GetFullscreen()
        @{
            return (@{bool})@{$$._ptr}->IsFullscreen();
        @}
        
        public override void SetFullscreen(bool fullscreen)
        @{
            @{$$._ptr}->SetFullscreen((bool)$0);
        @}

        public override void SetPointerCursor(PointerCursor p)
        @{
            @{$$._ptr}->SetSystemCursor((::Xli::SystemCursor)$0);
        @}

        public override PointerCursor GetPointerCursor()
        @{
            return @{$$._ptr}->GetSystemCursor();
        @}

        public override bool GetMouseButtonState(MouseButton button)
        @{
            return @{$$._ptr}->GetMouseButtonState((::Xli::MouseButton)$0);
        @}

        public override bool GetKeyState(Key key)
        @{
            return @{$$._ptr}->GetKeyState((::Xli::Key)$0);
        @}

        public override void BeginTextInput(TextInputHint hint)
        @{
            @{$$._ptr}->BeginTextInput((::Xli::TextInputHint)$0);
        @}

        public override void EndTextInput()
        @{
            @{$$._ptr}->EndTextInput();
        @}

        public override bool IsTextInputActive()
        @{
            return @{$$._ptr}->IsTextInputActive();
        @}
        
        public override bool HasOnscreenKeyboardSupport()
        @{
            return @{$$._ptr}->HasOnscreenKeyboardSupport();
        @}

        public override bool IsOnscreenKeyboardVisible()
        @{
            return @{$$._ptr}->IsOnscreenKeyboardVisible();
        @}

        public override float GetDensity()
        @{
            return ::Xli::Display::GetDensity(@{$$._ptr}->GetDisplayIndex());
        @}

        public override void SetOnscreenKeyboardPosition(int2 position)
        @{
            return @{$$._ptr}->SetOnscreenKeyboardPosition(*(uBase::Vector2i*)&$0);
        @}

        public override int2 GetOnscreenKeyboardPosition()
        @{
            const auto temp = @{$$._ptr}->GetOnscreenKeyboardPosition();
            return *(@{int2}*)&temp;
        @}

        public override int2 GetOnscreenKeyboardSize()
        @{
            const auto temp = @{$$._ptr}->GetOnscreenKeyboardSize();
            return *(@{int2}*)&temp;
        @}

        public override bool IsStatusBarVisible()
        @{
            return @{$$._ptr}->IsStatusBarVisible();
        @}

        public override int2 GetStatusBarPosition()
        @{
            const auto temp = @{$$._ptr}->GetStatusBarPosition();
            return *(@{int2}*)&temp;
        @}

        public override int2 GetStatusBarSize()
        @{
            const auto temp = @{$$._ptr}->GetStatusBarSize();
            return *(@{int2}*)&temp;
        @}
    }
}
