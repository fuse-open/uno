// @(MSG_ORIGIN)
// @(MSG_EDIT_WARNING)

#include <uBase/Memory.h>
#include <XliPlatform/Display.h>
#include <XliPlatform/GL.h>
#include <XliPlatform/GLContext.h>
#include <XliPlatform/MessageBox.h>
#include <XliPlatform/PlatformLib.h>
#include <XliPlatform/Window.h>
#include <cstdlib>
#include <sstream>
#include <string>
#include <thread>
@{Uno.Diagnostics.Clock:includeDirective}
@{Uno.Exception:includeDirective}
@{Uno.Platform.CoreApp:includeDirective}
@{Uno.Platform.WindowBackend:includeDirective}
@{Uno.Platform.Internal.Bootstrapper:includeDirective}
@{Uno.Application:includeDirective}
@{string:includeDirective}

#if WIN32
#include <uno/WinAPI.h>
#elif LINUX || OSX
#include <XliPlatform/PlatformSpecific/SDL2.h>
#endif

#ifdef DEBUG_DUMPS
#include <stdio.h> // needed for sprintf
#endif

// Used by Uno.Environment.GetCommandLineArgs()
int uArgc = 0;
char** uArgv = nullptr;

/**
    \addtogroup Main
    @{
*/
Xli::Window* _XliWindowPtr;
Xli::GLContext* _XliGLContextPtr;

struct uMainLoop : Xli::WindowEventHandler
{
    uMainLoop()
    {
        Xli::PlatformLib::Init();

        Xli::DisplaySettings settings;
        int maxFps = Xli::Display::GetCount() > 0 &&
                        Xli::Display::GetCurrentSettings(0, settings) &&
                        settings.RefreshRate > 0
                    ? settings.RefreshRate
                    : 60;

        uBase::Auto<Xli::Window> wnd = Xli::Window::Create(uBase::Vector2i(375, 667), "@(project.title)", Xli::WindowFlagsResizeable);

#if WIN32
        HWND hWnd = (HWND)wnd->GetNativeHandle();
        HICON hIcon = LoadIcon(GetModuleHandle(0), MAKEINTRESOURCE(101));
        SendMessage(hWnd, WM_SETICON, ICON_BIG, (LPARAM)hIcon);
        SendMessage(hWnd, WM_SETICON, ICON_SMALL, (LPARAM)hIcon);
#endif

        const char* hidden = getenv("UNO_WINDOW_HIDDEN");
        if (hidden && !strcmp(hidden, "1"))
        {
#if WIN32
            ShowWindow((HWND)wnd->GetNativeHandle(), SW_HIDE);
#elif LINUX || OSX
            SDL_HideWindow(Xli::PlatformSpecific::SDL2::GetWindowHandle(wnd));
#endif
        }

        uBase::Auto<Xli::GLContext> gl = Xli::GLContext::Create(wnd, Xli::GLContextAttributes::Default());

        // Store global references to wnd and gl
        _XliWindowPtr = wnd;
        _XliGLContextPtr = gl;

#ifdef U_GL_DESKTOP
        glEnable(GL_VERTEX_PROGRAM_POINT_SIZE);
#endif
        glClearColor(0, 0, 0, 0);
        glClear(GL_COLOR_BUFFER_BIT);

        gl->SetSwapInterval(0);
        gl->SwapBuffers();

        wnd->SetEventHandler(this);
        Xli::Window::ProcessMessages();
        @{Uno.Platform.CoreApp.Start():call()};

        while (!wnd->IsClosed())
        {
            double startTime = @{Uno.Diagnostics.Clock.GetSeconds():call()};

            Xli::Window::ProcessMessages();
            OnUpdate(wnd);

            if (wnd->IsVisible())
                OnDraw(wnd);

            if (maxFps > 0)
            {
                double targetTime = 1.0 / (double)maxFps;
                double renderTime = @{Uno.Diagnostics.Clock.GetSeconds():call()} - startTime;
                int msTimeout = (int)((targetTime - renderTime) * 1000.0 + 0.5);

                if (msTimeout > 0)
                    std::this_thread::sleep_for(std::chrono::milliseconds(msTimeout));
            }
        }
    }

    virtual void OnUpdate(Xli::Window* wnd)
    {
        uAutoReleasePool pool;
        @{Uno.Platform.Internal.Bootstrapper.OnUpdate():call()};
    }

    virtual void OnDraw(Xli::Window* wnd)
    {
        {
            uAutoReleasePool pool;

            if (@{Uno.Application.Current:get()} && @{Uno.Application.Current:get().NeedsRedraw:get()})
            {
                @{Uno.Platform.Internal.Bootstrapper.OnDraw():call()};
                _XliGLContextPtr->SwapBuffers();
            }
        }

#ifdef DEBUG_DUMPS
        static int frame = 0;
        char path[32];
        sprintf(path, "draw%d.dot", frame++);
        uDumpAllStrongRefs(path);
#endif
    }

    virtual bool OnKeyDown(Xli::Window* wnd, Xli::Key key)
    {
        uAutoReleasePool pool;

        if (@{Uno.Platform.Internal.Bootstrapper.OnKeyDown(Uno.Platform.WindowBackend,Uno.Platform.Key):call(@{Uno.Platform.WindowBackend.Instance}, key)})
            return true;

#ifdef WIN32
        if (key == Xli::KeyF11)
        {
            wnd->SetFullscreen(!wnd->IsFullscreen());
            return true;
        }
#endif

        return false;
    }

    virtual bool OnKeyUp(Xli::Window* wnd, Xli::Key key)
    {
        uAutoReleasePool pool;

        if (@{Uno.Platform.Internal.Bootstrapper.OnKeyUp(Uno.Platform.WindowBackend,Uno.Platform.Key):call(@{Uno.Platform.WindowBackend.Instance}, key)})
            return true;

        return false;
    }

    virtual bool OnTextInput(Xli::Window* wnd, const uBase::String& text)
    {
        uAutoReleasePool pool;

        if (@{Uno.Platform.Internal.Bootstrapper.OnTextInput(Uno.Platform.WindowBackend,string):call(@{Uno.Platform.WindowBackend.Instance}, uString::Utf8(text.Ptr()))})
            return true;

        return false;
    }

    virtual bool OnMouseDown(Xli::Window* wnd, uBase::Vector2i pos, Xli::MouseButton button)
    {
        uAutoReleasePool pool;

        if (@{Uno.Platform.Internal.Bootstrapper.OnMouseDown(Uno.Platform.WindowBackend,int,int,Uno.Platform.MouseButton):call(@{Uno.Platform.WindowBackend.Instance}, pos.X, pos.Y, button)})
            return true;

        return false;
    }

    virtual bool OnMouseUp(Xli::Window* wnd, uBase::Vector2i pos, Xli::MouseButton button)
    {
        uAutoReleasePool pool;

        if (@{Uno.Platform.Internal.Bootstrapper.OnMouseUp(Uno.Platform.WindowBackend,int,int,Uno.Platform.MouseButton):call(@{Uno.Platform.WindowBackend.Instance}, pos.X, pos.Y, button)})
            return true;

        return false;
    }

    virtual bool OnMouseMove(Xli::Window* wnd, uBase::Vector2i pos)
    {
        uAutoReleasePool pool;

        if (@{Uno.Platform.Internal.Bootstrapper.OnMouseMove(Uno.Platform.WindowBackend,int,int):call(@{Uno.Platform.WindowBackend.Instance}, pos.X, pos.Y)})
            return true;

        return false;
    }

    virtual bool OnMouseWheel(Xli::Window* wnd, uBase::Vector2i delta)
    {
        uAutoReleasePool pool;

        if (@{Uno.Platform.Internal.Bootstrapper.OnMouseWheel(Uno.Platform.WindowBackend,float,float,int):call(@{Uno.Platform.WindowBackend.Instance}, (float)delta.X, (float)delta.Y, 1)})
            return true;

        return false;
    }

    virtual bool OnTouchDown(Xli::Window* wnd, uBase::Vector2 pos, int id)
    {
        uAutoReleasePool pool;

        if (@{Uno.Platform.Internal.Bootstrapper.OnTouchDown(Uno.Platform.WindowBackend,float,float,int):call(@{Uno.Platform.WindowBackend.Instance}, pos.X, pos.Y, id)})
            return true;

        return false;
    }

    virtual bool OnTouchMove(Xli::Window* wnd, uBase::Vector2 pos, int id)
    {
        uAutoReleasePool pool;

        if (@{Uno.Platform.Internal.Bootstrapper.OnTouchMove(Uno.Platform.WindowBackend,float,float,int):call(@{Uno.Platform.WindowBackend.Instance}, pos.X, pos.Y, id)})
            return true;

        return false;
    }

    virtual bool OnTouchUp(Xli::Window* wnd, uBase::Vector2 pos, int id)
    {
        uAutoReleasePool pool;

        if (@{Uno.Platform.Internal.Bootstrapper.OnTouchUp(Uno.Platform.WindowBackend,float,float,int):call(@{Uno.Platform.WindowBackend.Instance}, pos.X, pos.Y, id)})
            return true;

        return false;
    }

    virtual void OnNativeHandleChanged(Xli::Window* wnd)
    {
        _XliGLContextPtr->MakeCurrent(wnd);
    }

    virtual void OnSizeChanged(Xli::Window* wnd)
    {
        uAutoReleasePool pool;
        @{Uno.Platform.Internal.Bootstrapper.OnWindowSizeChanged(Uno.Platform.WindowBackend):call(@{Uno.Platform.WindowBackend.Instance})};
#if WIN32 || OSX
        if (wnd->GetMouseButtonState(Xli::MouseButtonLeft))
        {
            OnUpdate(wnd);
            OnDraw(wnd);
        }
#endif
    }

    virtual bool OnClosing(Xli::Window* wnd)
    {
        uAutoReleasePool pool;
        @{Uno.Platform.CoreApp.EnterBackground():call()};
        return false;
    }

    virtual void OnClosed(Xli::Window* wnd)
    {
        uAutoReleasePool pool;
        @{Uno.Platform.CoreApp.Terminate():call()};
    }

    virtual void OnAppLowMemory(Xli::Window* wnd)
    {
        uAutoReleasePool pool;
        @{Uno.Platform.CoreApp.OnReceivedLowMemoryWarning():call()};
    }

    virtual void OnAppTerminating(Xli::Window* wnd)
    {
        uAutoReleasePool pool;
        @{Uno.Platform.CoreApp.Terminate():call()};
    }

    virtual void OnAppDidEnterForeground(Xli::Window* wnd)
    {
        uAutoReleasePool pool;
        @{Uno.Platform.CoreApp.EnterInteractive():call()};
    }

    virtual void OnAppDidEnterBackground(Xli::Window* wnd)
    {
        uAutoReleasePool pool;
        @{Uno.Platform.CoreApp.EnterBackground():call()};
    }
};

static void uUnhandledException(const char* msg, const char* function = nullptr, int line = 0)
{
    std::stringstream ss;
    ss << msg;
    if (function)
        ss << "\n\nFunction: " << function << "\nLine: " << line;

    std::string str = ss.str();
    uLog(uLogLevelFatal, "Unhandled Exception: %s", str.c_str());
    Xli::MessageBox::Show(nullptr, str.c_str(), "Unhandled Exception", Xli::DialogButtonsOK, Xli::DialogHintsError);
}

int main(int argc, char** argv)
{
    uArgc = argc;
    uArgv = argv;

    uRuntime uno;
    uAutoReleasePool pool;

    try
    {
        uMainLoop();
    }
    catch (const uThrowable& t)
    {
        uCString cstr(t.Exception->ToString());
        uUnhandledException(cstr.Ptr, t.Function, t.Line);
        return 1;
    }
    catch (const uBase::Exception& e)
    {
        uUnhandledException(e.GetMessage().Ptr(), e._func, e._line);
        return 1;
    }
    catch (const std::system_error& e)
    {
        std::stringstream ss;
        ss << "std::system_error " << e.code();
        uUnhandledException(ss.str().c_str());
        return 1;
    }
    catch (const std::exception& e)
    {
        uUnhandledException(e.what());
        return 1;
    }

    return 0;
}

#if WIN32
int WINAPI WinMain(HINSTANCE hInstance, HINSTANCE hPrevInstance, LPSTR lpCmdLine, int nCmdShow)
{
    return main(__argc, __argv);
}
#endif

@(typeObjects.declaration:joinSorted())
void uInitTypes(uType*(*factories[])());

void uInitTypes()
{
    static uType*(*factories[])() =
    {
        @(typeObjects.functionPointer:joinSorted('\n        ', '', ','))
        nullptr
    };

    uInitTypes(factories);
}

@(main.include:joinSorted('\n', '#include <', '>'))

void uStartApp()
{
    @(main.body:indent(' ', 4))
}

/** @} */
