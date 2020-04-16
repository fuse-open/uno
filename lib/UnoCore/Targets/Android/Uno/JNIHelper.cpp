#include <stdarg.h>
#include <Uno/JNIHelper.h>
#include <XliPlatform/PlatformSpecific/Android.h>

JavaVM* JniHelper::VM;
jclass JniHelper::ActivityClass = 0;
jclass JniHelper::NativeExternClass = 0;
pthread_key_t JniHelper::JniThreadKey;

void JniHelper::JniDestroyThread(void* value)
{
    JNIEnv* env = (JNIEnv*)value;
    VM->DetachCurrentThread();
    pthread_setspecific(JniThreadKey, nullptr);
}

void JniHelper::Init(JavaVM* vm, JNIEnv* env, jclass activityClass, jclass nativeExternClass)
{
    VM = vm;
    if (pthread_key_create(&JniThreadKey, JniDestroyThread))
        U_LOG("JNI ERROR: Unable to create pthread key"); // Not fatal

    ActivityClass = reinterpret_cast<jclass>(env->NewGlobalRef(activityClass));
    NativeExternClass = reinterpret_cast<jclass>(env->NewGlobalRef(nativeExternClass));
    pthread_setspecific(JniThreadKey, (void*)env);

    Xli::PlatformSpecific::Android::PreInit(vm, env, ActivityClass);
}

JniHelper::JniHelper()
{
    int status_ = (int)VM->GetEnv(reinterpret_cast<void**>(&env), JNI_VERSION_1_6);
    if (status_ != JNI_OK)
    {
        status_ = (int)VM->AttachCurrentThread(&env, nullptr);
        if (status_ != JNI_OK)
            U_FATAL("JNI ERROR: Failed to attach current thread");
    }
    if (!pthread_getspecific(JniThreadKey))
    {
        pthread_setspecific(JniThreadKey, (void*)env);
    }
}

jclass JniHelper::GetActivityClass()
{
    return ActivityClass;
}

jclass JniHelper::GetNativeExternClass()
{
    return NativeExternClass;
}

JavaVM* JniHelper::GetVM()
{
    return VM;
}

JNIEnv* JniHelper::GetEnv()
{
    return env;
}

JNIEnv* JniHelper::operator->()
{
    // TODO: Check jni exceptions
    return env;
}

@{string} JniHelper::JavaToUnoString(jstring jstr)
{
    JniHelper jni;
    if (!jstr) { return nullptr; }
    jobject str = jni->NewLocalRef(jstr);
    const jchar* raw =  jni->GetStringChars((jstring)str, nullptr);
    int len = jni->GetStringLength((jstring)str);
    int size = len * sizeof(jchar);
    uString* result = uString::New(len);
    memcpy(result->_ptr, (void*)raw, size);
    jni->ReleaseStringChars((jstring)str, raw);
    jni->DeleteLocalRef(str);
    return result;
}

jstring JniHelper::UnoToJavaString(@{string} ustr)
{
    if (!ustr)
        return nullptr;

    JniHelper jni;
    return (jni->NewString((const jchar*) ustr->_ptr, ustr->_length));
}
