#pragma once
#include <jni.h>
#include <pthread.h>
#include <uno.h>
#include <android/asset_manager.h>
#include <android/asset_manager_jni.h>

#define WITH_STATIC_JAVA_METHOD(HANDLEVAR,JCLASS,METHODNAMESTR,METHODSIGSTR) JniHelper __jni; static jmethodID HANDLEVAR; if (uEnterCriticalIfNull(&HANDLEVAR)) { HANDLEVAR = __jni->GetStaticMethodID(JCLASS, METHODNAMESTR, METHODSIGSTR); uExitCritical(); if (!HANDLEVAR) {U_FATAL("Cannot cache mid for " METHODNAMESTR);}}
#define WITH_STATIC_JAVA_METHOD_USEJNI(HANDLEVAR,JCLASS,METHODNAMESTR,METHODSIGSTR) static jmethodID HANDLEVAR; if (uEnterCriticalIfNull(&HANDLEVAR)) { HANDLEVAR = __jni->GetStaticMethodID(JCLASS, METHODNAMESTR, METHODSIGSTR); uExitCritical(); if (!HANDLEVAR) { U_FATAL("Cannot cache mid for " METHODNAMESTR); } }

#if @(Runtime.CatchCppExceptions)==2
#define JNI_TRY try{
#define JNI_CATCH } catch (const uThrowable& __t) { JniHelper jni; jni->ExceptionClear(); uString* message; if (uIs(__t.Exception, @{Uno.Exception:TypeOf})) { @{Uno.Exception} e = __t.Exception; message = e->Message(); } else { message = uString::Ansi("Unhandled C++ error in Main Loop"); }  @{Android.Base.JNI.ThrowNewException(string):Call(message)}; } catch (...) { JniHelper jni; jni->ExceptionClear(); uString* message; message = uString::Ansi("Unhandled C++ error in Main Loop"); @{Android.Base.JNI.ThrowNewException(string):Call(message)}; }
#elif @(Runtime.CatchCppExceptions)==1
#define JNI_TRY try{
#define JNI_CATCH } catch (const uThrowable& __t) { uAutoReleasePool pool; JniHelper jni; jni->ExceptionClear(); @{Uno.String} message; if (uIs(__t.Exception, @{Uno.Exception:TypeOf})) {  @{Uno.Exception} e = (@{Uno.Exception})__t.Exception; message = @{Uno.Exception:Of(e).ToString():Call()}; } else { message = uString::Ansi("Unhandled c++ error in Main Loop"); } @{Android.Base.JNI.ThrowNewException(string):Call(message)}; }
#else
#define JNI_TRY //
#define JNI_CATCH //
#endif

class JniHelper
{
    JNIEnv* env;
    static JavaVM* VM;
    static jclass ActivityClass;
    static jclass NativeExternClass;
    static pthread_key_t JniThreadKey;

    static void JniDestroyThread(void* value);

public:
    static void Init(JavaVM* vm, JNIEnv* env, jclass activityClass, jclass nativeExternClass);
    JniHelper();
    static JavaVM* GetVM();

    static jclass GetActivityClass();
    static jclass GetNativeExternClass();
    static @{string} JavaToUnoString(jstring jstr);
    static jstring UnoToJavaString(@{string} ustr);
    static float GetDensity();

    JNIEnv* GetEnv();
    JNIEnv* operator->();
};
