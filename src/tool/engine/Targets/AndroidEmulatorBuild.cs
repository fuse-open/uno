using Uno.Compiler.API;

namespace Uno.Build.Targets
{
    public class AndroidEmulatorBuild : AndroidBuild
    {
        public override string Identifier => "android-emu";
        public override string Description => "C++/JNI/GLES2 code and APK. Runs on emulator (x86_64).";

        public override void Initialize(IEnvironment env)
        {
            env.Define("ANDROID");
            env.Define("ANDROID_EMU");
        }
    }
}
