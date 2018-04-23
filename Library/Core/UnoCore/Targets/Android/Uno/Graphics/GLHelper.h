#pragma once
#include <Uno/AndroidCommon.h>
#include <android/native_activity.h>
#include <android/native_window.h>
#include <android/native_window_jni.h>
#include <EGL/egl.h>
#include <GLES2/gl2.h>
#include <GLES2/gl2ext.h>
#include <uBase/Vector2.h>
#include <jni.h>

class GLHelper
{
private:
    static void _setEGLConfig(bool forPBuffer);
    static const EGLint _contextAttribs[];
    static EGLConfig _eglPBufferConfig;
    static EGLContext _eglPBufferContext;
    static EGLSurface _eglPBufferSurface;
    static EGLConfig _eglRenderConfig;
    static EGLContext _eglSurfaceContext;
    static EGLContext _eglWorkerThreadContext;
    static EGLDisplay _eglDisplay;

    static GLuint _dummyTexture;
    static jobject _dummyJavaSurface;
    static ANativeWindow* _dummyNativeWindow;
    static EGLSurface _eglDummySurface;
    static void SwapBackToPBufferSurface(EGLSurface surface);
    static void SwapBackToPBufferSurface();
    static void SwapToDummySurface();
    static void CreateDummySurface();
public:
    static void InitGL();
    static void DeInitGL();
    static void SwapBuffers(EGLSurface surface);
    static void MakeCurrent(EGLContext context, EGLSurface surface);
    static void CreatePBufferSurfaceAndMakeCurrent();
    static ANativeWindow* GetANativeWindowFromSurface(jobject surface);
    static void CreateNewSurfaceAndMakeCurrent(ANativeWindow* nativeWindow, EGLSurface& newSurface);
    static void CreateEGLSurfaceFromANativeWindow(ANativeWindow* nativeWindow, EGLConfig config, EGLSurface& newSurface);
    static void DestroyRenderSurface(EGLSurface surface);
    static void SwapBackToBackgroundSurface();
    static void SwapBackToBackgroundSurface(EGLSurface surface);
    static void MakeWorkerThreadContextCurrent();
    static EGLContext GetSurfaceContext();
    static void SetEGLConfigs() { _setEGLConfig(true); _setEGLConfig(false); }
    static void AssertValidContext();
};
