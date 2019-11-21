
#include <uBase/Time.h>
#include <uBase/BaseLib.h>
#include <XliPlatform/PlatformSpecific/Android.h>

#include <Uno/JNIHelper.h>
#include <Uno/Graphics/GLHelper.h>

@{Android.Base.JNI:IncludeDirective}
@{Uno.Platform.CoreApp:IncludeDirective}
@{Uno.Platform.TimerEventArgs:IncludeDirective}
@{Uno.Platform.EventSources.HardwareKeys:IncludeDirective}
@{Uno.Platform.EventSources.InterAppInvoke:IncludeDirective}
@{Uno.Compiler.ExportTargetInterop.Foreign.Android.ExternBlockHost:IncludeDirective}

//void uStartApp();

//--------------------------------------------------

extern "C"
{
    void JNICALL cppOnReceiveURI (JNIEnv* env , jobject obj, jstring data)
    {
        data = (jstring)@{Android.Base.JNI.NewGlobalRef(Android.Base.Primitives.ujobject):Call((jobject)data)};
        uAutoReleasePool pool;
        @{string} unoUri = JniHelper::JavaToUnoString(data);
        JniHelper jni;
        jni->DeleteGlobalRef(data);
        @{Uno.Platform.EventSources.InterAppInvoke.OnReceivedURI(string):Call(unoUri)};
    }
}

//--------------------------------------------------

extern "C"
{
    bool JNICALL cppOnKeyUp (JNIEnv* env, jobject obj, jint key)
    {
        uAutoReleasePool pool;
        return @{Uno.Platform.EventSources.HardwareKeys.OnKeyUp(Uno.Platform.Key,Uno.Platform.EventModifiers):Call(key,0)};
    }
}

//--------------------------------------------------

extern "C"
{
    bool JNICALL cppOnKeyDown (JNIEnv* env, jobject obj, jint key)
    {
        uAutoReleasePool pool;
        return @{Uno.Platform.EventSources.HardwareKeys.OnKeyDown(Uno.Platform.Key,Uno.Platform.EventModifiers):Call(key,0)};
    }
}

//--------------------------------------------------

extern "C"
{
    void JNICALL cppOnCreate(JNIEnv *env , jobject obj, jobject activity)
    {
        uAutoReleasePool pool;
        @{Android.Base.JNI.Init(Android.Base.Primitives.ujobject):Call(activity)};
        @{Uno.Compiler.ExportTargetInterop.Foreign.Android.ExternBlockHost.RegisterFunctions():Call()};
        @{Uno.Platform.CoreApp.Start():Call()};
    }
}

//--------------------------------------------------

extern "C"
{
    void JNICALL cppOnStartMainLoop (JNIEnv* env, jobject obj, jboolean _resurrected)
    {
        bool resurrected = (bool)_resurrected;
        if (resurrected) {
            GLHelper::SwapBackToBackgroundSurface();
            uAutoReleasePool pool;
        } else {
            GLHelper::InitGL();
        }
    }
}

//--------------------------------------------------

extern "C"
{
    void JNICALL cppOnRestart(JNIEnv *env , jobject obj)
    {
        uAutoReleasePool pool;
        GLHelper::SwapBackToBackgroundSurface();
    }
}

//--------------------------------------------------

extern "C"
{
    void JNICALL cppOnResume(JNIEnv *env , jobject obj)
    {
         uAutoReleasePool pool;
         @{Uno.Platform.CoreApp.EnterForeground():Call()};
    }
}

//--------------------------------------------------

extern "C"
{
    void JNICALL cppOnPause(JNIEnv *env , jobject obj)
    {
        uAutoReleasePool pool;
        @{Uno.Platform.CoreApp.EnterBackground():Call()};
    }
}

//--------------------------------------------------

extern "C"
{
    void JNICALL cppOnDestroy(JNIEnv *env , jobject obj)
    {
        uAutoReleasePool pool;
        @{Uno.Platform.CoreApp.Terminate():Call()};
        // {NOTE} We dont call GLHelper::DeInitGL() here as there is no reliable way to
        //        tell if it really is a destory or if we are going to get ressurected
        //        and we really want to survive that with gl intact if possible
    }
}

//--------------------------------------------------

extern "C"
{
    void JNICALL cppOnLowMemory(JNIEnv *env , jobject obj)
    {
        uAutoReleasePool pool;
        @{Uno.Platform.CoreApp.OnReceivedLowMemoryWarning():Call()};
    }
}

//--------------------------------------------------

extern "C"
{
    void JNICALL cppOnWindowFocusChanged(JNIEnv *env , jobject obj, jboolean hasFocus)
    {
        uAutoReleasePool pool;
        if ((bool)hasFocus)
        {
            @{Uno.Platform.CoreApp.EnterInteractive():Call()};
        } else {
            @{Uno.Platform.CoreApp.ExitInteractive():Call()};
        }
    }
}

//--------------------------------------------------

void AttachNativeCallbacks(JNIEnv* jni, jclass entryPointClass)
{
    static JNINativeMethod native_activity_funcs[] = {
        {(char* const)"cppOnCreate", (char* const)"(Ljava/lang/Object;)V", (void *)&cppOnCreate},
        {(char* const)"cppOnDestroy", (char* const)"()V", (void *)&cppOnDestroy},
        {(char* const)"cppOnPause", (char* const)"()V", (void *)&cppOnPause},
        {(char* const)"cppOnResume", (char* const)"()V", (void *)&cppOnResume},
        {(char* const)"cppOnRestart", (char* const)"()V", (void *)&cppOnRestart},
        {(char* const)"cppOnLowMemory", (char* const)"()V", (void *)&cppOnLowMemory},
        {(char* const)"cppOnWindowFocusChanged", (char* const)"(Z)V", (void *)&cppOnWindowFocusChanged},
        {(char* const)"cppOnStartMainLoop", (char* const)"(Z)V", (void *)&cppOnStartMainLoop},
        {(char* const)"cppOnReceiveURI", (char* const)"(Ljava/lang/String;)V", (void *)&cppOnReceiveURI},
        {(char* const)"cppOnKeyUp", (char* const)"(I)Z", (void *)&cppOnKeyUp},
        {(char* const)"cppOnKeyDown", (char* const)"(I)Z", (void *)&cppOnKeyDown},
    };
    // the last argument is the number of native functions
    int attached = (int)jni->RegisterNatives(entryPointClass, native_activity_funcs, 11);
    if (attached < 0)
        U_FATAL("COULD NOT REGISTER NATIVE ACTIVITY FUNCTIONS");
}

static void uInitRuntime()
{
    static uRuntime runtime;
}

// This is the first point we have in the app lifecycle when we are in control
// At this point Xli has been loaded by the Activity.java file and, as specified by
// the JNI spec, when a library is loaded, if there is a JNI_OnLoad method, it will
// be called.
// We use this point to grab the activity class and attach all the lifecycle callbacks
jint JNI_OnLoad(JavaVM* vm, void* reserved)
{
    // vm & activityClass
    JNIEnv* env;
    if (vm->GetEnv(reinterpret_cast<void**>(&env), JNI_VERSION_1_6) != JNI_OK) {
        U_LOG("&&&&&&& GetEnv failed &&&&&&");
        return -1;
    }
    jclass activityClass = env->FindClass("@(Activity.Package:Replace('.', '/'))/@(Activity.Name)");
    jclass entryPointsClass = env->FindClass("com/fuse/ActivityNativeEntryPoints");
    jclass nativeExternClass = env->FindClass("com/foreign/ExternedBlockHost");

    if (!activityClass)
        U_FATAL("COULD NOT FIND ACTIVITY CLASS");
    if (!entryPointsClass)
        U_FATAL("COULD NOT FIND ENTRYPOINTS CLASS");
    if (!nativeExternClass)
        U_FATAL("COULD NOT FIND NATIVEEXTERNCLASS CLASS");

    // systems
    JniHelper::Init(vm, env, activityClass, nativeExternClass);
    uBase::BaseLib::Init();
    Xli::PlatformSpecific::Android::PostInit();
    Xli::PlatformSpecific::Android::SetLogTag("@(Activity.Name)");


    // java callbacks
    AttachNativeCallbacks(env, entryPointsClass);

    // uno
    uInitRuntime();

    return JNI_VERSION_1_6;
}

//------------------------------------------------------------
