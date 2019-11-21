using Uno.Compiler.API;
using Uno.Compiler.API.Backends;
using Uno.Compiler.Backends.CPlusPlus;
using Uno.Compiler.Backends.OpenGL;
using Uno.Compiler.Extensions;
using Uno.IO;

namespace Uno.Build.Targets.Android
{
    public class AndroidBuild : BuildTarget
    {
        public override string Identifier => "Android";
        public override string Description => "C++/JNI/GLES2 code and APK. Runs on device.";

        public override Backend CreateBackend()
        {
            return new CppBackend(new GLBackend(), new CppExtension());
        }

        public override void Configure(ICompiler compiler)
        {
            new AndroidGenerator(
                    compiler.Environment,
                    compiler.Data.Extensions)
                .Configure();
        }

        public override void DeleteOutdated(Disk disk, IEnvironment env)
        {
            // Remove previously built APK to avoid Android caching issues
            var apk = env.GetString("Outputs.APK");
            if (apk.IsValidPath())
                disk.DeleteFile(env.Combine(apk.UnixToNative()));

            // Delete old Java files so Gradle won't try to build them
            disk.DeleteOutdatedFiles(env.GetOutputPath("Java.SourceDirectory"));
        }
    }
}
