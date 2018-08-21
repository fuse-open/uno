using Uno;
using Uno.Collections;
using Uno.Compiler.ExportTargetInterop;
using Android.Base.Primitives;
using Android.Base.Wrappers;

namespace Android.Base
{
    extern(Android)
    public static class Versions
    {
        static void Initialize()
        {
            if (_api == -1)
            {
                var tmpCls = JNI.LoadClass("android/os/Build$VERSION");
                var sdk = JNI.GetStaticFieldID(tmpCls, "SDK_INT", "I");
                var release = JNI.GetStaticFieldID(tmpCls, "RELEASE", "Ljava/lang/String;");
                _api = extern<int>(JNI.GetEnvPtr(),tmpCls, sdk) "$0->GetStaticIntField($1,$2)";
                _release = Android.Base.Types.String.JavaToUno(extern<ujobject>(JNI.GetEnvPtr(),tmpCls, release) "$0->GetStaticObjectField($1,$2)");
            }
        }

        static int _api = -1;
        static string _release = "<unknown>";

        static public int ApiLevel
        {
            get
            {
                if (_api == -1)
                    Initialize();
                return _api;
            }
        }

        static public string Release
        {
            get
            {
                if (_api == -1)
                    Initialize();
                return _release;
            }
        }
    }
}
