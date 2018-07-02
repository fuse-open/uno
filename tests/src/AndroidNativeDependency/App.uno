using Uno;
using Uno.Compiler.ExportTargetInterop;

namespace AndroidNativeDependency
{
    public partial class App : Application
    {
    }

    [Require("Gradle.Dependency.NativeImplementation", "com.google.ar:core:1.3.0")]
    [Require("LinkLibrary", "arcore_sdk_c")]
    [Require("LinkLibrary", "arcore_sdk_jni")]
    extern(ANDROID) partial class App
    {
        // If this app builds, it means our feature is working!
    }
}
