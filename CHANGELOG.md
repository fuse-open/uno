Uno Changelog
=============

Unreleased
----------

1.14
----

### Android
- Added dynamic delivery options (#252).
- Added Android Go support (#255).
- Switched to Android X libraries (#254).
- Build Android AAR libraries (#258).
- Merged namespace `com.Bindings` into `com.foreign` (#264).
- Dropped legacy Java code (#263).
- Added generated Proguard file for optimizations (#266).

### iOS
- Added `arm64e` dependencies (#270).
- Fixed compile-time warnings (#271).
- Upgraded Xcode project format (#271).

### .NET
- Build .NET library without AppLoader when `-DLIBRARY` (#277).

### Compiler
- Fixed bug with `abstract extern` members (#273).
- Dropped legacy importer system (#272). 
- Auto-detect main-class when building a library.

### Standard library
- Fixed bug with UTF8 BOM in `Uno.IO.BundleFile` (#251).
- Dropped legacy code (#259).
    - `GL.BufferData(GLBufferTarget target, int sizeInBytes, GLBufferUsage usage)`
    - `GL.BufferData(GLBufferTarget target, byte[] data, GLBufferUsage usage)`
    - `GL.BufferData(GLBufferTarget target, Buffer data, GLBufferUsage usage)`
    - `GL.BufferSubData(GLBufferTarget target, int offset, byte[] data)`
    - `GL.BufferSubData(GLBufferTarget target, int offset, Buffer data)`
    - `GL.TexImage2D(GLTextureTarget target, int level, GLPixelFormat internalFormat, int width, int height, int border, GLPixelFormat format, GLPixelType type, byte[] data)`
    - `GL.TexImage2D(GLTextureTarget target, int level, GLPixelFormat internalFormat, int width, int height, int border, GLPixelFormat format, GLPixelType type, Buffer data)`
    - `GL.TexSubImage2D(GLTextureTarget target, int level, int xoffset, int yoffset, int width, int height, GLPixelFormat format, GLPixelType type, byte[] data)`
    - `Uno.Compiler.Ast.Block`
    - `Uno.Compiler.ExportTargetInterop.DontCopyStruct`
    - `Uno.Compiler.ExportTargetInterop.ExportConditionAttribute`
    - `Uno.Compiler.ExportTargetInterop.ExportNameAttribute`
    - `Uno.Compiler.ExportTargetInterop.Foreign.ForeignDataView.ForeignDataView(Uno.Buffer buffer)`
    - `Uno.Compiler.ExportTargetInterop.Foreign.ForeignDataView.Create(Uno.Buffer unoBuffer)`
    - `Uno.Compiler.ImportServices.BlockFactory`
    - `Uno.Compiler.ImportServices.BlockFactoryContext`
    - `Uno.Compiler.ImportServices.FilenameAttribute`
    - `Uno.Compiler.ImportServices.Importer`
    - `Uno.Compiler.ImportServices.ImporterContext`
    - `Uno.Diagnostics.AllocateEvent`
    - `Uno.Diagnostics.Debug.Undefined`
    - `Uno.Diagnostics.Debug.Alert(string message, string caption, DebugMessageType type)`
    - `Uno.Diagnostics.Debug.Alert(string message)`
    - `Uno.Diagnostics.Debug.Confirm(string message, string caption, DebugMessageType type)`
    - `Uno.Diagnostics.Debug.Confirm(string message)`
    - `Uno.Diagnostics.EnterEvent`
    - `Uno.Diagnostics.EventType`
    - `Uno.Diagnostics.ExitEvent`
    - `Uno.Diagnostics.FreeEvent`
    - `Uno.Diagnostics.Profile`
    - `Uno.Diagnostics.ProfileData`
    - `Uno.Diagnostics.ProfileEvent`
    - `Uno.Diagnostics.ProfileSerializer`
    - `Uno.FakeTime`
    - `Uno.Float.ZeroTolerance`
    - `Uno.Graphics.DeviceBuffer.Update(Buffer data)`
    - `Uno.Graphics.Framebuffer.SupportsMipmap`
    - `Uno.Graphics.IndexBuffer.IndexBuffer(Buffer data, BufferUsage usage)`
    - `Uno.Graphics.Texture2D.Load(BundleFile file)`
    - `Uno.Graphics.Texture2D.Load(string filename)`
    - `Uno.Graphics.Texture2D.Load(string filename, byte[] bytes)`
    - `Uno.Graphics.Texture2D.Update(Buffer mip0)`
    - `Uno.Graphics.Texture2D.Update(int firstMip, params Buffer[] mips)`
    - `Uno.Graphics.Texture2D.SupportsMipmap`
    - `Uno.Graphics.TextureCube.Load(BundleFile file)`
    - `Uno.Graphics.TextureCube.Load(string filename)`
    - `Uno.Graphics.TextureCube.Load(string filename, byte[] bytes)`
    - `Uno.Graphics.TextureCube.Update(Buffer mip0)`
    - `Uno.Graphics.TextureCube.Update(int firstMip, params Buffer[] mips)`
    - `Uno.Graphics.TextureCube.SupportsMipmap`
    - `Uno.Graphics.VertexBuffer.VertexBuffer(Buffer data, BufferUsage usage)`
    - `Uno.Graphics.VideoTexture.SupportsMipmap`
    - `Uno.IO.BundleFileImporter`
    - `Uno.IO.TextWriter.InitialNewLine`
    - `Uno.Runtime.Implementation.BufferImpl`
    - `Uno.Runtime.Implementation.Internal.BufferConverters`
    - `Uno.Runtime.Implementation.Internal.BufferReader`
- Extracted `Uno.Graphics.Utils` library from `UnoCore` (#269).
- Clean-ups in `Uno.Platform` (#274).

### Other improvements
- Upgraded to C++14 standard (#250).
- Upgraded NPM dependencies (#262).
- Added more options for `uno clean` (#267).
- Print path to resulting app or library (#276).

[`v1.13.2...v1.14.0`](https://github.com/fuse-open/uno/compare/v1.13.2...v1.14.0)

1.13
----

### Android
- Added support for CMake 3.10.2.4988404 (#219).
- Added support for Dark Theme (#236).
- Added support for Splash screen (#240).
- Added support for generating [Android App Bundle](https://developer.android.com/platform/technology/app-bundle) on Release (#220).
- Fixed crash with `windowIsTranslucent` together with `screenOrientation=portrait` on Android 8 Oreo (#211).
- Upgraded to Gradle 5.6, and Gradle-plugin 3.4.2 (#220).
- Upgraded build tools to version 28.0.3 (#220).
- Upgraded support libraries to version 28.0.0 (#220).
- Upgraded SDK compile and target versions to 28 (#220).
- Added the following build properties (#220).
    * `Bundle`
    * `Bundle.BuildName`
    * `Bundle.Gradle.Task`
- Renamed the following build properties (#220).
    * `APK.Configuration -> Build.Configuration`
- Set default versionCode to positive Integer (1) so `gradlew` command doesn't produce error when building using latest Gradle (#220).

### iOS
- Support for iPhone XR Launch Image has been added. This can be customized using the `iOS.LaunchImages.iPhone_Portrait_iPhoneXr_2x` and `iOS.LaunchImages.iPhone_Landscape_iPhoneXr_2x` project-setting (#225).
- Support for iPhone XS Max Launch Image has been added. This can be customized using the `iOS.LaunchImages.iPhone_Portrait_iPhoneXsMax_3x` and `iOS.LaunchImages.iPhone_Landscape_iPhoneXsMax_3x` project-setting (#225).
- Switched to user-installed `ios-deploy`, for iOS 13 support (#226).
- Added Sign-in with Apple capability (#233).

### C++ backend
- Optimized `char` and `string` classes, which gives less run-time overhead and faster code (#232).
- Fixed a bug with `uMainLoop::OnClosing` related to overload resolving, and warnings (#202).

### UX markup
- Added `ux:Simulate` attribute, to disable/enable the Fuse Studio simulator per UX class (#200).

### Uno command
- Added `--build-only` switch for `uno build` (#245).
- Added `--libs` switch for `uno config` (#244).
- Added support for passing project files to `uno doctor` (#237).
- Dropped legacy package manager commands (#148).
    - `uno install`
    - `uno uninstall`
    - `uno feed`
    - `uno pack`
    - `uno push`
    - `uno stuff`

### Other improvements
- Automatically install Mono on macOS (#228).
- Detect required software and provide friendly hints on how to install missing software (#227, #243).
- Always run `cmake` and `node` from PATH, dropping `Tools.CMake` and `Tools.Node` properties in `.unoconfig` (#243).
- Fixed security warnings in JavaScript dependencies by upgrading to newest versions (#203, #204).
- Upgraded .NET dependencies to newest versions (#229).
- Various logging improvements and tweaks (#198, #242).
- Cleaned up config files (#230).

[`v1.12.2...v1.13.0`](https://github.com/fuse-open/uno/compare/v1.12.2...v1.13.0)

1.12
----

### FuseJS
- Added support for [TypeScript](https://devblogs.microsoft.com/typescript/typescript-and-babel-7/), and new JavaScript feature sets ES2016, ES2017 and ES2018.

### Project Format
- Added the following properties:
    * `Android.Architectures.Debug` (string array)
    * `Android.Architectures.Release` (string array)
    * `Android.AssociatedDomains` (string array)
    * `Android.Defines` (string array)
    * `iOS.Defines` (string array)
    * `DotNet.Defines` (string array)
    * `Native.Defines` (string array)
    * `Mac.Icon` (path to .ICNS)
    * `Windows.Icon` (path to .ICO)
- Removed the following properties:
    * `HTML.Title`
    * `HTML.Favicon`

### Android
- Added support for [App Links](https://developer.android.com/training/app-links).
- Added support for targeting the following architectures (ABIs):
    * `armeabi-v7a`
    * `arm64-v8a`
    * `x86`
    * `x86_64`
- Added support for multi-architecture (fat) builds, by default building:
    * `armeabi-v7a` on Debug
    * `armeabi-v7a` + `arm64-v8a` on Release
- Added support for long filenames on Windows, and building of larger projects.
- Fixed launching Android Studio on Windows, when passing `--debug`.
- Don't try to launch apps on network devices, when passing `--run`.
- Suggest installing `android-build-tools` using NPM when SDK/NDK can't be found.
- Deprecated `@(ABI)` macro. Please use `${ANDROID_ABI}` instead.

### iOS
- Added support for [Universal Links](https://developer.apple.com/ios/universal-links).

### Linux
- Added support for building on Linux (x86_64). Tested on Ubuntu 18.04 and 16.04.

### macOS
- Switched to generating Xcode projects when using the Native target. This gives slightly faster builds.

### UnoCore
- Performance improvements in `String` and `StringBuilder` classes.
- Added the following new methods:
    * `Uno.Graphics.DeviceBuffer.Update(Array data, int elementSize, int index, int count)`
    * `Uno.String.Join(string separator, params object[] value)`
    * `Uno.Text.Ascii.GetString(byte[] value, int index, int count)`
    * `Uno.Text.StringBuilder(string value)`
    * `Uno.Text.Utf8.GetString(byte[] value, int index, int count)`

### Other improvements
- Now slightly faster subsequental builds when using the C++ backend.
- Don't show app window when running tests using `uno test`.
- Added `--only-build` option for `uno test`.
- Search for projects directly under `Packages.SourcePaths` in `uno doctor`.
- Fixed a problem when exporting documentation files.
- Fixed newlines in output when exporting documentation from Windows.
- Fixed several warnings.
- Removed the `uno stuff` command.
- Removed the `-DHEADLESS` flag.

See also [v1.11.1...v1.12.0](https://github.com/fuse-open/uno/compare/v1.11.1...v1.12.0).

1.11
----

### Distribution
- Switched to NPM for distribution and standalone installation of `uno`. Previously we had to install the complete Fuse Studio to get this component.
- Automatically load `.unoconfig` files found in `node_modules`, so we can now use NPM to conveniently install any Uno or Fuse component.
- Added the [android-build-tools](https://www.npmjs.com/package/android-build-tools) package on NPM that automatically sets up Android SDK and NDK components, similar to the `fuse install android` functionality found in Fuse Studio.

### Android build target
- Added support for both landscape modes when setting Landscape in the project file.
- Fixed a problem that happens sometimes when downloading packages during Gradle build.

### Uno compiler
- Added a `metadata` build target that can produce metadata for code-completion plugins.
- Fixed a bug where the visibility of generic argument types were not validated correctly.

See also [the full list of changes](https://github.com/fuse-open/uno/compare/release-1.10...release-1.11).

1.10
----

### Command-line tool
- `uno disasm` has been removed, as it's not very useful to end-users, and it complicates project maintainence.
- The JavaScript backend including WebGL and FirefoxOS build targets has been removed after being broken and unmaintained for a long time.
- Fixed a bug causing `uno launch-apk` to hang if an Android app crashes early.
- `dotnet` is now the default build target for `uno build` and `uno test` on all platforms.
- Removed the `-O` flag on `uno build`, as using it has no effect.
- Removed the `uno android` sub-command, as it has not been maintained and no longer works on recent Android SDKs.
- Removed the `uno sdkmanager` sub-command, as it has not been maintained and no longer works on recent Android SDKs.

### Android build target
- Upgraded to Gradle 4.4, and Gradle-plugin 3.1.3.
- Upgraded build tools to version 27.0.3.
- Upgraded support libraries to version 27.1.1.
- Upgraded NDK platform version to 16.
- Upgraded SDK compile and target versions to 26.
- Fixed issues when building against the new NDK r18.
- Added the following build property for linking native libraries from downloaded AAR packages. This makes it possible to integrate for example the ARCore SDK.
    * `Gradle.Dependency.NativeImplementation`
- Added the following UXL file type to more conveniently include java files in generated projects.
    * `JavaFile`
- Renamed the following build properties. Using the old names will now produce deprecation warnings.
    * `Gradle.Dependency.{Compile -> Implementation}`
    * `JNI.SharedLibrary -> SharedLibrary`
    * `JNI.StaticLibrary -> StaticLibrary`
    * `JNI.SystemLibrary -> LoadLibrary`
- Marked the following build property as obsolete.
    * `Gradle.Model.Repository`

### iOS build target
- Upgraded to latest Xcode project format.
- Fixed build errors when linking against `GStreamer.framework`.
- Fixed build warnings.

### Native build target
- Upgraded to VS2017 C++ compilers when building on Windows.
- Added the following build properties
    * `LinkLibrary.Debug`
    * `LinkLibrary.Release`

### C++ API
- Added optional argument on the `U_FATAL(message)` macro.
- Added `U_ERROR(format, ...)` macro to more conveniently report errors.
- The `@{Method()}` macro now expands to the qualified name of the generated method, and can be used to pass callbacks to C APIs.

### Uno syntax
- The `extern(CONDITION)` modifier is now accepted on `partial` classes.

### UnoCore
- Moved the following class from the `Uno.Threading` package.
    * `Uno.Threading.Thread`
- Added the following classes.
    * `Uno.ByteArrayExtensions`
    * `Uno.ValueType`
- Added the following property.
    * `Uno.Array.Length`
- Added the following methods.
    * `OpenGL.GL.BufferData(GLBufferTarget target, int sizeInBytes, IntPtr data, GLBufferUsage usage)`
    * `OpenGL.GL.BufferSubData(GLBufferTarget target, int offset, int sizeInBytes, IntPtr data)`
    * `Uno.Graphics.DeviceBuffer.Update(Array data, int elementSize)`
    * `Uno.Graphics.IndexBuffer.Update(ushort[] data)`
    * `Uno.Graphics.VertexBuffer.Update(float[] data)`
    * `Uno.Graphics.VertexBuffer.Update(float2[] data)`
    * `Uno.Graphics.VertexBuffer.Update(float3[] data)`
    * `Uno.Graphics.VertexBuffer.Update(float4[] data)`
- Added the following constructors.
    * `Uno.Graphics.IndexBuffer(ushort[] data, BufferUsage usage)`
    * `Uno.Graphics.VertexBuffer(float[] data, BufferUsage usage)`
    * `Uno.Graphics.VertexBuffer(float2[] data, BufferUsage usage)`
    * `Uno.Graphics.VertexBuffer(float3[] data, BufferUsage usage)`
    * `Uno.Graphics.VertexBuffer(float4[] data, BufferUsage usage)`
- Implemented the following methods on C++.
    * `Uno.Buffer.PinPtr()`
    * `Uno.Runtime.InteropServices.GCHandle.AddrOfPinnedObject()`
- Marked the following methods as obsolete.
    * `OpenGL.GL.BufferData(GLBufferTarget target, int sizeInBytes, GLBufferUsage usage)`
    * `OpenGL.GL.BufferData(GLBufferTarget target, byte[] data, GLBufferUsage usage)`
    * `OpenGL.GL.BufferSubData(GLBufferTarget target, int offset, byte[] data)`
    * `OpenGL.GL.TexImage2D(GLTextureTarget target, int level, GLPixelFormat internalFormat, int width, int height, int border, GLPixelFormat format, GLPixelType type, byte[] data)`
    * `OpenGL.GL.TexSubImage2D(GLTextureTarget target, int level, int xoffset, int yoffset, int width, int height, GLPixelFormat format, GLPixelType type, byte[] data)`
    * `Uno.Graphics.TextureCube.Load(BundleFile)`
    * `Uno.Graphics.TextureCube.Load(string)`
    * `Uno.Graphics.TextureCube.Load(string, byte[])`
- Marked the following classes as obsolete.
    * `Uno.Buffer`
    * `Uno.Compiler.ExportTargetInterop.DontCopyStructAttribute`
    * `Uno.Compiler.ExportTargetInterop.ExportNameAttribute`
    * `Uno.Content.Images.Bitmap`
    * `Uno.Diagnostics.EnterEvent`
    * `Uno.Diagnostics.ExitEvent`
    * `Uno.Diagnostics.FreeEvent`
    * `Uno.Runtime.Implementation.BufferImpl`
    * `Uno.Runtime.Implementation.Internal.BufferConverters`
    * `Uno.Threading.MainThreadAttribute`

1.9
---

### UnoCore
- Marked the following methods as obsolete. To get the old behavior, use `Uno.Math.Floor(x + 0.5f)` instead.
    * `Uno.Math.Round(float)`
    * `Uno.Math.Round(float2)`
    * `Uno.Math.Round(float3)`
    * `Uno.Math.Round(float4)`
- Marked the following methods as obsolete. To get the old behavior, use `Uno.Math.Round(double, int)` instead.
    * `Uno.Math.Round(float, int)`
    * `Uno.Math.Round(float2, int)`
    * `Uno.Math.Round(float3, int)`
    * `Uno.Math.Round(float4, int)`
- `Math.Round(double, int)` now throws ArgumentOutOfRangeException if digits are less than zero or larger than 15. This matches .NET behavior.
- Fixed a bug where `Math.Round(double, int)` with large, finite inputs would produce infinities.
- `Math.Abs(int.MinValue)` now throws OverflowException. This matches .NET behavior.
- Added overloads of `Math.Abs`, matching .NET:
    * `Math.Abs(sbyte)`
    * `Math.Abs(short)`
    * `Math.Abs(long)`
- Removed long obsolete importers:
    * ArrayImporter
    * BufferImporter
    * FontFaceImporter
    * SpriteFontImporter
    * IndexBufferImporter
    * Texture2DFileImporter
    * Texture2DImporter
    * TextureCubeFileImporter
    * TextureCubeImporter
    * VertexBufferImporter
- Marked the following methods as obsolete. Those pulled in a big dependency that is currently unused.
    * `Uno.Graphics.Texture2D.Load(BundleFile)`
    * `Uno.Graphics.Texture2D.Load(string)`
    * `Uno.Graphics.Texture2D.Load(string, byte[])`

### .NET
- Fixed a code-generator bug where unsigned modulo got treated like signed modulo, causing negative values in the result.
- Fixed a code-generator bug where ulong values larger than long.MaxValue didn't get sign-extended correctly.
- Fixed a code-generator bug where signed division was used instead of unsigned division, leading to incorrect results for big dividends.

### Uno Compiler
- The following unsafe implicit casts has been turned to explicit casts. This matches C# behavior, and should avoid unexpected overflows:
    * sbyte -> ushort
    * sbyte -> uint
    * sbyte -> ulong
    * short -> uint
    * short -> ulong
    * int -> ulong
- We no longer sign the .NET assemblies in Uno.
- `uno disasm` has been removed on macOS, as it's has been broken for a while and there's essentially no users.

1.8
---

### Uno compiler
- Exposing internal types through protected members is now a compilation error, to match C#'s behavior. Some code may need to be updated, for instance by making the exposed type `public` or the member `internal`.
- Fixed a bug that resulted in a crash at startup during the construction of Uno's runtime type information when using certain (rare) combinations of generic types.

### UX Compiler
- Fixed a codegen bug where property bindings to properties with generic type arguments (such as LetFloat) would generate invalid Uno code.

### Uno.Net.Http
- Made the following types internal. They were never meant to be exposed in the first place, and shouldn't be used:
    * `Uno.Net.Http.AbsolutePathParser`
    * `Uno.Net.Http.HashParser`
    * `Uno.Net.Http.HostInfo`
    * `Uno.Net.Http.HostInfoParser`
    * `Uno.Net.Http.QueryParser`
    * `Uno.Net.Http.SchemeParserResult`
    * `Uno.Net.Http.SchemeParserResult`
    * `Uno.Net.Http.UserInfoParser`
    * `Uno.Net.Http.UriScheme`
    * `Uno.Net.Http.UriSchemeType`
- Introduced `Uri.Fragment`.
- Marked `Uri.Hash` as obsolete. Use `Uri.Fragment` instead.
- Marked `Uri.GetQueryParameters()` as obsolete. Use `Query.Substring(1).Split('&')` instead.
- Marked `Uri.Combine(string, string)` as obsolete. Use `String.Format("{0}/{1}, baseUri.TrimEnd(new char[] { '/' }, uri.TrimStart(new char[] { '/' }))"` instead.
- Marked `Uri.Combine(string, string)` as obsolete. Use `Uri(Uri, string).OriginalString` instead.
- Marked `Uri.Encode` as obsolete. Use `Uri.EscapeDataString(string)` instead.
- Marked `Uri.Decode` as obsolete. Use `Uri.UnescapeDataString(string)` instead.

### Uno.Threading
- Marked public versions of `ConcurrentDictionary.Add(TKey, TValue)` and `ConcurrentDictionary.Remove(TKey)` as obsolete. Use IDictionary-versions instead.
- Marked public versions of `ConcurrentDictionary.GetEnumerator()` as obsolete. Use IEnumerable-versions instead.
- Marked public versions of `ConcurrentDictionary.Keys` and `ConcurrentDictionary.Values` as obsolete. Use IDictionary-versions instead.

### UnoCore
- Removed obosolete methods from `Uno.Color`. These has been obsolete since Uno 0.46:
    * `Uno.Color.ToRgb24(float3)`
    * `Uno.Color.ToRgba32(float4)`
    * `Uno.Color.FromRgb24(int3)`
    * `Uno.Color.FromRgb24(uint)`
    * `Uno.Color.FromRgba32(int4)`
    * `Uno.Color.FromRgba32(uint)`
    * `Uno.Color.ToHex(int3)`
    * `Uno.Color.ToHex(int4)`

### .NET
- Fixed a code-generator bug where long-literals that were larger than int.MaxValue, but smaller than uint.MaxValue accidentally got sign-extended.
- Fixed a code-generator bug where shifts on unsigned values accidentally got sign-extended.

1.7.1
-----

### UnoCore
- Fixed a bug in IntPtr.GetHashCode where the returned hash-code was more or less random. This lead especially to big problems when using IntPtr as a Dictionary key.

1.7
---

### UnoCore
- Removed obsolete method `BitmapFont.CreateFromPbf()`.
- Removed obsolete classes `SdfFontImporter`, `SdfFontShaderBlock` and `SdfFontShader`.
- Made `String.IndexOf()` and `String.LastIndexOf()` throw `ArgumentOutOfRangeException` on negative values on all build-targets.
- Implemented `String.IndexOf(char, int, int)`, `String.LastIndexOf(char, int, int)`, `String.IndexOfAny()` and `String.LastIndexOfAny`.
- Implemented `Array.IndexOf<T>()` and `Array.LastIndexOf<T>()`.
- Removed everything in Uno.Content.Splines and Uno.Content.Models. This was already marked as obsolete a while ago, and all known users of the API has been removed.
- `Random.NextInt(int)` now throws `ArgumentOutOfRangeException` when passed negative values, and returns zero when passed zero.
- Random API has been more aligned with the .NET API:
    * The `SetSeed`-method has been marked as obsolete. Construct a new Random object instead to change the seed.
    * The `NextInt`-methods has been renamed to `Next`. The old methods are still available, but will produce obsolete warnings when used.
    * A `NextDouble` has been added, that returns a number between 0.0 and 1.0.
    * The `NextFloat`-methods has been marked as obsolete. Use the `NextDouble` method instead.
    * The `NextFloat2`, `NextFloat3` and `NextFloat4`-methods has been marked as obsolete. Use multiple calls to `NextDouble` instead.

1.6.1
-----

### UX Compiler
- Fixed a bug introduced in 1.6 that caused `E0000: The given key was not present in the dictionary.` errors to be generated for some UX document roots.

1.6
---

### Long path? Long code.
We can now extract zip files containing long filenames (255+ characters) on Windows. This enables things like providing Node.js on-demand, requiring long filenames to work.

### Environment variables
The following environment variables can be used to configure Uno globally, potentially useful in CI environments:
- `LOG_LEVEL=<0..3>` - Set verbosity level (default = 0)
- `LOG_TRACE=1` - Print exception stack traces where available
- `DEBUG_GL=1` - Enable the OpenGL debug layer in .NET builds

### Android build target
- Added the following build properties for requiring gradle repositories via UXL:
    * `Gradle.BuildScript.Repository`
    * `Gradle.AllProjects.Repository`

### Project files
- Support for iPhone 20pt 2x and 3x icons has been added. This can be customized using the `iOS.Icons.iPhone_20_2x` and `iOS.Icons.iPhone_20_3x` project-setting, respectively.
- Support for iPad 20pt 1x and 2x icons has been added. This can be customized using the `iOS.Icons.iPad_20_1x` and `iOS.Icons.iPad_20_2x` project-setting, respectively.
- Support for iPad Pro icons has been added. This can be customized using the `iOS.Icons.iPad_83.5_2x` project-setting.
- Support for iOS Marketing icons has been added. This can be customized using the `iOS.Icons.iOS-Marketing_1024_1x` project-setting.
- Support for iPhoneX Launch Image has been added. This can be customized using the `iOS.LaunchImages.iPhone_Portrait_iPhoneX_3x` and `iOS.LaunchImages.iPhone_Landscape_iPhoneX_3x` project-setting.

### Command-line interface
- The following old commands were removed from the release: `cp`, `disasm`, `mkdir`.

### .NET backend
- We now build 64-bit code by default when the OS is 64-bit on all platforms. You can pass `-DX86` or `-DX64` to `uno build` to override the architecture.
- UXL: `<Require UnmanagedLibrary[.{x86,x64}]>` was added to handle native DLL dependencies for P/Invoke. We can now make sure the DLL is copied to the right directory without making assumptions about where it should go.
- Set `DEBUG_GL=1` in your environment to enable the OpenGL debug layer at run-time. This is available in .NET builds on macOS and Windows.

### MSVC build target deprecated
- The 'native' target now serves the same purpose, and works on both macOS and Windows.
- We now build 64-bit code by default when the OS is 64-bit on all platforms. You can pass `-DX86` or `-DX64` to `uno build` to override the architecture.
- UXL: `<Require SharedLibrary[.{x86,x64}]>` was added to handle DLL dependencies on Windows. We can now make sure the DLL is copied to the right directory without making assumptions about where it should go.

### Environment.NewLine
- We've introduced `Uno.Environment.NewLine` as an alias for `"\r\n"` on Windows and `"\n"` on macOS, iOS, and Android.
- `Uno.IO.TextWriter` and `Uno.Text.StringBuilder` has been updated to respect `Uno.Environment.NewLine`.

### C++ backend
- (Breaking) We're now using `char16_t` as the underlying type for `Uno.Char`. Previously we were using `wchar_t` on WIN32 and `int16_t` on other platforms, so call-sites marshaling strings between Uno and native APIs need to make sure they're using the correct pointer types for the native API.
- We're now using `int32_t` as the underlying type for `Uno.Int`.

### UnoCore
- Added `Parse(string)` and `TryParse(string, out float4)` to `Color`.
- Marked `Color.FromHex(string)` as obsolete. Use `Color.Parse(string)` instead.
- `Uno.Buffer` does no longer support "read-only" buffers.
- Deprecated methods taking `Uno.Buffer` in `OpenGL` and `Uno.Graphics` namespaces - use the `byte[]` overloads instead.
- Now using type-safe GL handle types on .NET, previously we were passing `int`s.
- The following types in the `Uno.Native.Textures` namespace (.NET utils for native font & texture loading) were moved from fuselibs:
    * `Uno.Native.Fonts.FontFace `
    * `Uno.Native.Fonts.RenderedGlyph`
    * `Uno.Native.Fonts.FontRenderMode`
    * `Uno.Native.Textures.TextureFileFormat`
    * `Uno.Native.Textures.TextureType`
    * `Uno.Native.Textures.PixelFormat`
    * `Uno.Native.Textures.Texture`
- The following importer APIs still exist, but will no longer work (deprecated since before 1.0):
    * `Uno.Content.Fonts.SdfFontImporter`
    * `Uno.Content.Fonts.SpriteFontImporter`
    * `Uno.Content.Models.ModelBlockFactory`
    * `Uno.Content.Models.ModelImporter`
    * `Uno.Content.Models.ModelFileImporter`

### Node.js support

In this release we give Uno projects two new features:
- NPM support: When a `package.json` is found in the same directory as the `.unoproj`, we automatically run `npm install`.
- FuseJS (ES2015) support: JavaScript files added as `filename.js:FuseJS` in a `.unoproj` are automatically transpiled to ES5 before bundled into the app.

To support this we need to find `node` and `npm`. If the commands aren't in your PATH, their locations can be provided like this in `~/.unoconfig` (`%USERPROFILE%\.unoconfig`):
```javascript
Tools.Node: "%PROGRAMFILES%\\nodejs\\node.exe"
Tools.NPM: "%PROGRAMFILES%\\nodejs\\npm.cmd"
```

1.4
---

### C++ backend
- The following unused entities were removed from `Uno/Memory.h`:
    * `uGarbageCollect()`
- The following unused entities were removed from `Uno/ObjectModel.h`:
    * `uArray::Copy()`

### UnoCore
- Added `NewGuid()` & `ToByteArray()` to `Guid`.

### Gradle
- Added support for customizing `settings.gradle` by using `[Require Gradle.Settings="include ':something'"]`.

### Uno compiler
- We now check that finally clauses don't contain illegal control flow. `break`, `continue` and `return` are not allowed in finally clauses.

### AppDelegate
- Fixed regression on iOS where, because we had implemented the methods for pushnotifications apps were rejected from the app store for not requiring notifications even if they didnt use the notification's feature

1.3
---

### Project format

- Fixed a bug where the `Name` property in `.unoproj` wasn't honored. By default, `Name` is derived from the project filename if not set explicitly, and by accident we were using the default value instead of the user-provided value, leading to inconsistencies. If you have provided `Name` explicitly, we will now use that name when producing output files, i.e. `UserProvidedName.apk` (Android) and `UserProvidedName.exe` (.NET) instead of `DefaultName.apk` and `DefaultName.exe`.

### C++ backend

- Don't lazy initialize types without a user-defined static constructor. This removes a bunch of `Foo_typeof()->Init()` calls from generated code, for instance when accessing a static field, to reduce run-time overhead.

### Deprecated importers
- `import BundleFile(FILENAME)` and remaining parts of the importer system in `Uno.Compiler.ImportServices` has been deprecated.
  We can now pass the filename directly to the operator instead: `import(FILENAME)`. The import operator returns a `BundleFile`
  object. The classes `texture2D` and `textureCube` contains `Load()` methods accepting `BundleFile` which can be used to import
  texture files, replacing the deprecated texture importers.

  | Old syntax                     | New syntax                           |
  |--------------------------------|--------------------------------------|
  | `import BundleFile(FILENAME)`  | `import(FILENAME)`                   |
  | `import Texture2D(FILENAME)`   | `texture2D.Load(import(FILENAME))`   |
  | `import TextureCube(FILENAME)` | `textureCube.Load(import(FILENAME))` |

### Removed UXL variable 'Mono'
- The UXL variable 'Mono' was removed, since the need of it was removed.

### Improvements to Android builds
- Strip SO files in release builds. This can massively reduce the size of the shipping app.
- Use gradle to generate required `.iml` files. This should make us more flexible both to changes from Fuse & to changes from Google in Android Studio. There is now only 1 `.iml` generated and that is in the `app` sub-directory of the project and only for builds with the `-d` flag.

### Resource Crunching
- Disabled android's resource crunching. Android will no longer convert pngs to WebP or otherwise modify your images to save space. See the following for details https://developer.android.com/studio/build/optimize-your-build.html#disable_crunching

### .NET backend
- Validation of method signatures and data types when linking external assemblies has been improved. This revealed a few minor problems in UnoCore that has been fixed:
  * `Uno.Net.EndPoint`: Constructor is now internal because a compatible constructor doesn't exist in .NET.
  * `Uno.Threading.Monitor.TryEnter()`: Return type is now `bool` to match .NET.
  * `Uno.Text.StringBuilder`: This class now maps to `System.Text.StringBuilder` on .NET.
  * `Uno.Tuple<...>`: The `AppendItems(StringBuilder sb)` methods are no longer public because compatible methods don't exist in .NET.
- Introduced `Uno.Compiler.ExportTargetInterop.DotNetOverrideAttribute`. This attribute is used to override implementations of static methods in types that are mapped to .NET, so that the Uno implementation is used instead of the existing .NET implementation. We use this on `string.Format()` because Uno only implements a subset of the functionality.

### Uno.DateTime changes (breaking!)
- `Uno.DateTime` was changed from being a `class` to a `struct`, to make it .NET-compatible
- The `UtcNow` property has changed types from being `Uno.Time.ZonedDateTime` to being `Uno.DateTime`, to make it .NET compatible
- The `Now` property has been made internal until better .NET-compatible support can be provided

### Uno compiler
- The compiler has become stricter about accessing entities from other packages. Previously it was in some cases possible to access internal entities from a different package without getting a build error.
- The compiler now accepts empty structs. It's no longer necessary to add a dummy field to work around build errors when forward declaring .NET structs for instance.

1.2
---

### Broken tools removed
- `uno perf-test` and `uno perf-cmp` were unused and broken, and have been removed.

### Deprecations
- The unused class Uno.FakeTime has been marked as obsolete.
- The `ExportCondition` attribute in `Uno.Compiler.ExportTargetInterop` has been marked as obsolete.
- The legacy `Alert` and `Confirm` methods in `Uno.Diagnostics.Debug` has been marked as obsolete.
- The redundant `SupportsMipmap` fields in `Uno.Graphics` has been marked as obsolete.
- The constant `float.ZeroTolerance` has been marked as obsolete.

1.1
---

### Uno.Data.Json
- Fixed a bug where a JSON object with the same key repeated would cause a crash.

### UX Compiler/Fuse Preview Optimizations
- Many optimizations to the UX compiler to reduce compile times and editing speed in Fuse Preview.

### Android
- Android's `allowBackup` option is now configurable via the optional `Android.AllowBackup` value in the unoproj file

### Uno.Testing
- A bunch of internal details that were never intended to be public has been marked as internal:
  * `AbstractRunner`
  * `DebugLogMessageDispatcher`
  * `AssertionFailedException`
  * `HttpMessageDispatcher`
  * `NamedTestMethod`
  * `Registry.Add()`
  * `Registry.Count`
  * `NamedTestMethod Registry[int index]`
- `Registry.FindTest()` has been removed.
- `TestSetup.Runner` has been marked as readonly.
- `Assert.Ignore()` has been added to allow ignoring tests at run-time.
- A bug in `Assert.AreNotEqual(float, float)` and `Assert.AreNotEqual(double, double)` has been fixed, to make them format the result strings similar to their `Assert.AreEqual` counter-parts.
- `TestAttribute` and `IgnoreAttribute` have gotten the apropriate `AttributeUsage`s to match their behaviour. This means that using them outside of methods will now give compile-errors instead of doing nothing.

1.0
---

### `ux:Ref` fix for MaterialDesign community package
- Fixed a bug where nodes marked `ux:Ref` would not generate correct code. This fixes https://github.com/Duckers/Fuse.MaterialDesign/issues/11.

### Add `ux:Test` support in UX compiler
- The UX compiler now supports `ux:Test`, which is similar to `ux:Class` but generates a bootstrapped test class which can be run with `uno test`. See docs for details.

### Add 'uno sdkmanager'
- This lets you use Android's new CLI sdkmanager. The old `uno android` approach is deprecated.

### New UX Markup features
- Introduced `<ux:Resources>` tag which can be used as the root tag in UX files that contain only `ux:Class`'es and `ux:Global`'s. This avoids having to use a "dummy panel" to encapsulate such resources.

### iOS Screen Rotation
- Fixed a bug that broke layout when rotating the screen on iOS

0.47
----

### iOS Backend
- Fixed a problem that resulted in "FATAL ERROR: value cannot be null" when building for iOS without having set a development team manually.

### .NET backend

- Worked around an issue that resulted in errors like `ERROR: Failed to compile .NET type 'XXX': The invoked member is not supported before the type is created.` when using certain class attributes when compiling for .NET (e.g. local preview)

### Uno.Net.Sockets

The following methods that were marked as obsolete in Fuse 0.33 have now been removed:
- `Socket.Send(byte[], int, int)`
- `Socket.Receive(byte[], int, int)`

### All Android builds now use Gradle & CMake

Back in 2013 Google announce they would be changing their supported build system to Gradle and that Ant would be deprecated. Since then Gradle and android have grown to the point where Gradle is now 'the way' to build and manage your dependencies for Android projects. The road for native development has been much rockier especially if, like us, you interact with Android from an external tool.

However we are finally in the position where we are happy enough with stability and compatibility that we are going to remove the deprecated Ant build system and switch to Gradle for all builds. We have been offering Gradle builds from Fuse using a build flag for a long time and the input from users has been invaluable.

What this means for you is as follows:

- You no longer have to use the `-DGradle` flag when building for Gradle support
- You will be able to use a simple Uno attribute to install any of the mountain of libraries available for the android platform via Maven & JCenter
- The projects we generate should be compatible with Android Studio[0]. Build with `-d` to open the project in AS.

This does however have an effect on what themes are available for Native widgets. The reason for this is that more and more 3rd party android libraries (such as Facebook's login UI) are using UI elements (or other features) not available on older android versions. To keep to our high standard of device & version compatibility this required us to switch from using `com.android.Activity` to `android.support.v7.app.AppCompatActivity`. This provided compatibility versions of the APIs needed but means that the supported Android native themes must inherit from `AppCompat.Theme`

That means that the `Holo` will no longer work with Fuse apps although `Theme.AppCompat`, `Theme.AppCompat.Light`, etc will.

As always please reach out to us if you hit any issues.

[0] We aren't providing support for this workflow as changes to Android Studio are out of our control.

### Android Studio Details
- We no longer generate the .iml files & instead let Android Studio do this.

### iOS Launch Options
- You can now access the latest launch options in iOS apps as a `ObjC.Object` from the `Uno.Platform.iOS.Application.LaunchOptions` property.

0.46
----

### Foreign Objective-C
- Uno has now enabled automatic reference-counting (ARC) in Foreign Objective-C code. If you have foreign code and Xcode complains about methods like `dealloc`, `retain`, `release`, or `autorelease` being unavailable, this can be fixed by removing the calls to those methods.

### Uno.Color
- `Color.ToRgb24`, `Color.ToRgba32`, `Color.FromRgb24`, `Color.FromRgb24`, `Color.FromRgba32` and `Color.Rgba32FromHex`, `Color.ToHex(int3)` and `Color.ToHex(int4)` have been marked as obsolete. See the obosletion-warnings for replacements.
- `Color.FromHex` no longer accepts sign-characters in the hex-string. This wasn't intended to work in the first place, and only worked in some bizarre corner-cases.
- `Color.FromArgb` and `Color.ToArgb` has been added.

### Switch from gradle-experimental to standard Gradle
- Using the `-DGRADLE` flag will now use the standard Gradle and CMake to build your projects.
- Gradle will become the default build system in the next release and Ant support will be removed.

0.45
----

### Android.Theme

The native Android-theme used by an app has been made configurable, by adjusting the `Android.Theme`-property in the project-file. At the same time, we've changed the default theme from "Holo" to "Material".

To get the old behavior, add this snippet to your project file:

"Android": {
	"Theme" : "Holo"
}

0.44
----

### Xcode project generation
- Added `$(inherited)` to the framework search paths, which is sometimes necessary when using CocoaPods.

### Uno.Threading

As mentioned in the changelog for Fuse 0.32, the following obsolete methods have been removed:

- `AutoResetEvent.Create()`
- `ManualResetEvent.Create()`
- `Mutex.Create()`
- `Mutex.Lock()`
- `Mutex.TryLock()`
- `Mutex.Unlock()`
- `Semaphore.Create()`
- `Thread.Create()`
- `Thread.Join(int)`

If you still haven't updated your code to to match, you'll now get build-errors. See the changelog for Fuse 0.32 for upgrade instructions.

In addition, `EventWaitHandle` has been added as a base-class for `AutoResetEvent` and `ManualResetEvent`, and ConcurrentCueue<T> no longer implements IDisposable, both similar to what .NET does.

### Uno.Net.Sockets
- `Socket.Send(byte[], int, int)` has been marked as obsolete. It doesn't exist (nor work) on .NET-targets. Use `Socket.Send(byte[], int, int, SocketFlags)` instead.
- `Socket.Receive(byte[], int, int)` has been marked as obsolete. It doesn't exist (nor work) on .NET-targets. Use `Socket.Receive(byte[], int, int, SocketFlags)` instead.

### Uno Refactor

A large amount of refactoring has been undertaken to solve two issue:

- Enabling testing without GL
- Moving the root-view code to fuselibs

This series of changes are seen as steps to enable where we want to go, we understand that things can feel a bit in-cohesive at this stage. But with these changes landed we will be able to iterate on the code bases more easily and fix some long standing issues (especially in areas such as system ui & fullscreen)

- The new `CoreApp` class has been created. This is the new root application class instead of `Uno.Application`. It contains the lifecycle, `Current` & `Load`.

- All platforms now feed lifecycle events through `Platform.CoreApp`

- `Uno.Application` inherits from this and on desktop adds the `GraphicsController` and `Window`. Much of the Application class is deprecated for mobile targets. Window is still present for mobile but is deprecated for imminent removal.

- `GraphicsController` is a class which holds much of the code that used to belong to `GraphicsContext`. It is only used on desktop. The reason for the change is that for mobile, everything in managed in fuselibs. `GraphicsContext` now exists as a handle.

- A class called `Displays` had been added which contains a list of `Display`s. For now the most used field is `MainDisplay`, which for all current platforms is the only `Display`

- A `Display` class now exists which holds the `Density` property & `Tick` event. There are subclasses of this iOS, Android & Desktop that handle the plumbing.

- A class called `OSFrame` has been added which is the new base class of `Window`. As mentioned above, `Window` continues to be used on desktop & is deprecated on mobile.

- Window contained far too many of the events. The `EventSources` namespace the new home for these migrated events. There are separate static classes for InterApp, HardwareKeys, Text & Mouse events. This separation will allow us to re-evaluate them separately and work on their future place in the API.

- The mobile root-view bootstrapping is removed from Uno and moved to Fuselibs

- SystemUI and it's bootstrapping is removed from Uno and moved to Fuselibs

- Key events now have an `Origin` field, which is the `OSFrame` they originated from. This is for platforms where these events originate from `Window`s as opposed to UI elements

- All core libraries that touched Platform2 events have been updated to use Platform rather than Platform2

- Events which on some platforms are forbidden (like `quit`) now live in an class called `Unsafe`

0.43
----

### Xcode project generation
- `Xcode.ShellScript`s are now escaped as string literals.

### Android builds
- Added a `JNI.SystemLibrary` UXL element for linking in libraries like `liblog`.
- Gradle builds now depend on v23.4.0 of support-v4, appcompat & design. Rather than using wildcard for the version.

### Foreign Code Improvements

* You can now make Uno properties where the bodies of the getter and setter are foreign code.
    For example:

        [Foreign(Language.Java)]
        public int Foo
        {
            get
            @{
                return getSomeSubsystem().refreshRate;
            @}
            set
            @{
                getSomeSubsystem().refreshRate = value;
            @}
        }

    On foreign Java methods you can also provide annotations. 

        [ForeignAnnotation(Language.Java, "TargetApi(19)")]
        public void Foo()
        @{
            ...
        @}

    Note that you do not specify the leading `@` symbol. This is added by Uno.

* Fixed a bug where the argument names to foreign methods would be renamed
    because of other conflicting names in the same scope.

* @{TypeName} macros now expand correctly in foreign Java & ObjC code.

* The `Uno.Compiler.ExportTargetInterop.Foreign.ObjC.Object` class has now been removed. Use `ObjC.Object` instead.

### Uno.Threading

To align better with the .NET APIs and to reduce some performance issues with our low-level threading API, we have revamped our API a bit. The following changes are noteworthy:

- `Thread.Join(int)` has been marked as obsolete. It's not implemented on most platforms, so it's not very useful in its current form.
- `Thread.Create()`, `Mutex.Create()`, `Semaphore.Create()`, `AutoResetEvent.Create()` and `ManualResetEvent.Create()` has been marked as obsolete. Use the constructors for the respective classes instead.
- `Mutex.Lock()` and `Mutex.TryLock()` has been marked as obsolete. Use `Mutex.WaitOne()` instead.
- `Mutex.Unlock()` has been marked as obsolete. Use `Mutex.ReleaseMutex()` instead.
- `Thread`, `Mutex`, `Semaphore`, `AutoResetEvent` and `ManualResetEvent` are now sealed, and cannot be inherited from.

In order to get a smooth transition, the deprecated APIs are still present in the current release (but should produce compiler-warnings when used). They will however be removed soon.

0.42
----

### Legacy Android Bindings Removed

As planned Legacy bindings are removed this release. Foreign Code is now the only official way of interfacing with native code.

### Various changes

- Makes `uno build native` work on Windows (eventually to replace `uno build msvc`)
- Removes outdated Android solution for VS2015
- Some other commits that should be harmless
- Android NDK has renamed `ndk-stack.exe` to `ndk-stack.cmd`, we can now use either of those

0.41
----

### Deprecation of build targets

The following build targets will generate a warning if used:
```
uno build webgl
uno build ffos
```

We're deprecating build targets based on JavaScript because our JavaScript backend is no longer
maintained.

### Deprecation of importers

The following will now generate a warning if compiled:
```
import Uno.Array<T>()
import Uno.Buffer()
import Uno.Content.Fonts.FontFace()
import Uno.Content.Fonts.SfdFont()
import Uno.Content.Fonts.SpriteFont()
import Uno.Content.Models.Model()
apply Uno.Content.Models.Model()
import Uno.Graphics.IndexBuffer()
import Uno.Graphics.Texture2D()
import Uno.Graphics.TextureCube()
import Uno.Graphics.VertexBuffer()
```

These APIs are scheduled for removal in an upcoming release, because we no longer want to maintain
dependencies to the proprietary FBX SDK from Autodesk in our core product.


### Syntax sugar

Three new literal types are added in Uno: Points, pixels & hex color codes.

```
#000         float3(0,0,0)
#FFFF        float4(1,1,1,1)
#FF0000      float3(1,0,0)
#FF0000FF    float4(1,0,0,1)
24.0px       new Uno.UX.Size(24.0f, Uno.UX.Unit.Pixels)
32.0pt       new Uno.UX.Size(32.0f, Uno.UX.Unit.Points)
```


0.40
----

### Preview

- Android preview and export now works for users with spaces in their paths, as long `-DGRADLE` is used. Note that for this to work with preview, you have to specify the `.unoproj` explicitly: `fuse preview -tandroid my.unoproj -DGRADLE`.
- Added `Uno.Net.Sockets.TcpListener` helper-class for hosting TCP servers. It's a subset of .NETs System.Net.Sockets.TcpListener class, and resolves to that implementation for .NET-targets.

### Support for adding Swift files to `unoproj`s
- Added a `Swift` file type to `unoproj`s. Swift files currently do not get the foreign macro expansion that `ObjCSource` files get.
- The version of Swift to that is used can be configured with the `iOS.SwiftVersion` project property:

    ```
    "iOS": {
      "SwiftVersion": 3.0,
    },
    ```

    It currently defaults to `3.0`.

The following example shows how to use this feature:

`Hello.swift:`
```
import Foundation

public class HelloSwiftWorld: NSObject {
    public func hello() {
        print("Hello world!");
    }
}
```

`unoproj`:
```
{
    "Includes": [
        "Hello.swift:Swift:iOS",
        ...
    ]

}
```

Since Swift can be used from Objective-C, we can call into the Swift code by using Foreign Objective-C, for instance as follows:

```
[ForeignInclude(Language.ObjC, "@(Project.Name)-Swift.h")]
public class Example
{
    [Foreign(Language.ObjC)]
    public static void DoIt()
    @{
        HelloSwiftWorld* x = [[HelloSwiftWorld alloc] init];
        [x hello];
    @}
}
```

### Gradle

Fix app signing for gradle build on Windows

### Misc

Fix the `source value 1.5 is obsolete and will be removed in a future release` warning.

0.39
----

### Added more control of PList in unoproj
We have added option to specify the following iOS plist entries from the `unoproj` file:

- NSCalendarsUsageDescription
- NSPhotoLibraryUsageDescription
- NSBluetoothPeripheralUsageDescription
- NSCameraUsageDescription
- NSMicrophoneUsageDescription
- NSAppleMusicUsageDescription
- NSContactsUsageDescription
- NSHomeKitUsageDescription
- NSLocationAlwaysUsageDescription
- NSLocationUsageDescription
- NSLocationWhenInUseUsageDescription
- NSMotionUsageDescription

They can be used as follows:

    "iOS": {
        "PList": {
            "NSCameraUsageDescription": "ReasonA",
            "NSMicrophoneUsageDescription": "ReasonB",
            "NSPhotoLibraryUsageDescription": "ReasonC",
        }
    }


### Foreign Code

Foreign Java is alittle more strict and does a little more type checking during the build.

1. `Action<object>`s passed to Java will now have the type `Action_UnoObject` rather than `Action_Object`
   `Action<Java.Object>`s passed to Java will still have the type `Action_Object`
2. If you make a foreign method with an argument with type `object`. You must pass an uno object, not a java object.

Here is an example for point `2` that will no longer compile:

    [Foreign(Language.Java)]
    static string Foo()
    @{
       Object jobj = MakeSomeJavaObject();
       @{Bar(object):Call(jobj)};
    @}

    [Foreign(Language.Java)]
    static string Bar(object x)
    @{
       ...
    @}

The fix for the above would be to change `Bar(object x)` to `Bar(Java.Object x)` and `@{Bar(object):Call(jobj)}` to `@{Bar(Java.Object):Call(jobj)}`

### Gradle Support
- We can now build signed APKs with Gradle.
- For users who run into platform specific filepath length limitations whilst build for Android with Gradle, you can now use the `--set:AltBuildPath="/tmp"` argument to specify the root of your android builds.

### Xcode project generation and signing
- Fix crash occuring when running multiple test fixtures on iOS devices

### UX Compiler improvements
- Binding syntax is now supported in shorthands such as `<Change foo.bar="{binding}" />` and similar (bugfix).
- Fixed bug where `Font` and other types that have required UX attributes (constructor arguments) could not be used as `ux:Property`
- Added basic reactive expression framework to Uno.UX: `AddOperator`, `SubtractOperator` etc.

0.38
----

### Xcode project generation and signing
- Added a property to set the Xcode Development Team ID, used for code-signing, in `unoproj`s:

    ```
    {
      "iOS": {
        "DevelopmentTeam": "YOURTEAMID"
      },
      ...
    }
    ```

    This property can also be set using the `--set:Project.iOS.DevelopmentTeam="YOURTEAMID"` flag to `uno build`, e.g.

    ```
    uno build iOS --set:Project.iOS.DevelopmentTeam="YOURTEAMID"

    ```

    The Team ID can be found at https://developer.apple.com/account/#/membership/.
- The Development Team ID can also be set user-wide by adding the following to ~/.unoconfig:

    ```
    iOS.DevelopmentTeam: "YOURTEAMID"
    ```

    The file can be created if it doesn't already exist.

- Uno now attempts to automatically find a Development Team ID if it hasn't already been set using either of the above methods. It does so by querying the machine's code-signing identities and selecting the first valid one it finds.

### Foreign Java Bugfix
- Fix bug causing build error when returning structs from foreign Java Code

### Expand on using Uno arrays from Foreign Java

- Can now pass Uno unsigned byte arrays to foreign Java
- Add ability to make boxed Uno arrays from Java
  You can now create Uno arrays in Java. To do this you make a new instance of the boxed array types passing in either:
  - the desired length of the Uno array
  - the Java array you want to convert

```
LongArray ularr = new LongArray(100);
IntArray uiarr = new IntArray(new int[]{1,2,3});
```
The length constructor works on all the following types. The Java array conversion works on all except the types marked with a `*`
> BoolArray
> ByteArray
> CharArray *
> DoubleArray
> FloatArray
> IntArray
> LongArray
> ObjectArray *
> ShortArray
> StringArray *

0.37
----

### Java foreign code

- Fix crash occuring after wrapping a Java delegate 256 times, due to a leak causing the JNI local reference table to fill up.

### Xcode project generation

- We now add the directory of `Require`d `Xcode.Framework`s and `Xcode.EmbeddedFramework`s to `Xcode.FrameworkDirectory`, meaning that `Xcode.FrameworkDirectory` will rarely be necessary to use.

### .unoconfig package management (internal)

The following properties are introduced.
```
Packages.Feeds
Packages.InstallDirectory
Packages.LockFiles
```

The following properties were renamed for consistency. `PackageSourcePaths` still works for
compatibility.
```
PackageSearchPaths -> Packages.SearchPaths
PackageSourcePaths -> Packages.SourcePaths
DefaultPackages -> Packages.Default
```

0.35
----

- Various internal Uno bugfixes
- In the current release the default minumum sdk version for android builds has been set to 16. This now matches what we have been saying is our minimum supported sdk version. However for those who wish to continue building for older versions of android you and simply add the following to your `unoproj` file

    "Android": {
       "SDK": {
             "MinVersion": "10"
        }
    }

or, if you prefer, you can use the following command line argument:

    --set:SDK.MinVersion=10


0.34
----

### Foreign code

- Fixed a bug that meant fields accessed using macros that didn't use `:Get()` in foreign Objective-C, such as `@{MyClass:Of(_this)._someField}`, did not have their types converted from Uno to their Objective-C equivalents
- Fixed a bug that resulted in foreign code in static constructors not working

### Uno parser reimplementation

Faster & harder parser that comes with some minor breaking changes to the syntax.

Previously, some invalid variable declarations were accepted:

    var a = 0, b = 0;           // Invalid 'var' with multiple declarators
    var c = fixed int[] {0};    // Invalid 'var' with fixed array

Correct syntax is:

    int a = 0, b = 0;           // OK
    fixed int c[] = {0};        // OK

### Xcode project generation

- Added a new property to set the `UIBackgroundModes` property of the generated Xcode project. You can use it by adding the following to your .unoproj:
    ```
    "iOS": {
      "PList": {
        "UIBackgroundModes": [
          "location",
          "voip"
        ]
      }
    }
    ```

### Improved incremental build

- `uno build`: Don't trigger a build if the previous build is still up-to-date
- Automatically remove outdated files and APK from Android builds

### Bugfixes

- `unoproj`: Allow `-` in version strings
- Fix crash in `ProcessExtensions` on Windows

0.33
----

### Preliminary Cocoapod support

Coacoapods is the defacto unofficial standard for iOS package management, until an official package manager support Objective-C (and a bunch of other stuff) cocoapods is a great choice.

#### Install steps
- `brew install ruby`
- `sudo gem install cocoapods`

The cocoapods folks say you should be able to use the ruby that comes with OSX however gem tries to install `activesupport v5` as a dependency and this requires at leave Ruby `v2.2.2`

#### Using Pods in Uno
build with `-DCOCOAPODS` to enable cocoapod support.

You can then use the `require` macro or attribute as follows:
```
[Require("Cocoapods.Podfile.Target", "pod 'Firebase'")]
```

0.32
----

### Running on iOS devices
- When building with `uno build --target=ios --run`, the `uno` command now stays open, printing any output from the app to stdout. This makes building for Android and iOS behave the same way. (Previously, `uno` would exit as soon as the app was deployed to the iOS device.)

### Compiler bug fixes

- "Using Generic interfaces on nested structs generates faulty C++"
- "Protected member not allowed to be of protected class type"

### MSVC target

- Debug using Visual Studio 2015 by default. Will fallback to Visual Studio 2013 when 2015 is unavailable. VS2013 is still required for building the generated code.


0.31
----

### Xcode project generation

- The default setting for frameworks included with the `Xcode.EmbeddedFramework` element is now to code sign on copy which is also Xcode's default.

- Added a new element for adding shellscripts to the generated Xcode project

    For example, the code
    ```
    [Require("Xcode.ShellScript", "someScript")]
    class ...
    ```

    will add `someScript` in a `PBXProjShellScriptBuildPhase` in the generated `.pbxproj` file.

### Foreign code

- Allow passing enums to foreign Objective-C and Java.
  Enums are passed as their underlying primitive type (which is `int` by
  default).

- Fix bug where you couldnt box structs when passed to Foreign Java code

### Gradle projects

- To build using experimental gradle support use the `-DGRADLE` flag

- The `Gradle.Dependencies.Compile` element has been renamed to
    `Gradle.Dependency.Compile`. It's used to specify dependencies in Gradle builds
    (targeting Android) of the form `compile 'XXX'`.

    ```
    [Require("Gradle.Dependency.Compile", "myDependency")]
    ```
    results in
    ```
    compile 'myDependency'
    ```
    under `dependencies` in the generated Gradle build file.

- Added the `Gradle.Dependency` element for specifying "free-form" dependencies:

    ```
    [Require("Gradle.Dependency", "compile('myDependency') { transitive = true }")]
    ```
    results in
    ```
    compile('myDependency') { transitive = true }
    ```
    under `dependencies` in the generated Gradle build file.

0.30
----

### Project format

- Support globs in project references
    ```
    "Projects": [
      "projects/**/*.unoproj"
    ]
    ```

- New default DeploymentTarget on iOS

    The new iOS target is `8.0`. You can edit the default in your `.unoproj`.
    ```
    "iOS.DeploymentTarget": "8.0"
    ```

### Other changes (internal)

- Add new `pack`, `push` (usable now) & `install`, `feed` (not yet) commands.
- Add Uno.Compiler.Extensions who consists of Bindings, Foreign & Plugins.
- Rename some root directories to lower case.
    - Later, we'll also want to rename the remaining directories
        - `Library/Core` -> `lib`
        - `Source` -> `src`

0.29
----

### Project format

- Transitive references.

    ```
    Given the projects App, A & B:
    * App refences A, which has a transitive reference to B.
    * This means that App has a implicit reference to B through A.
    ```

    You can use transitive references in your project file like this:
    ```
    "IsTransitive": true
    ```

    This will make all references in your project transitive, effectively
    making your project a transitive package.

- New default package names (Android) and bundle identifiers (iOS)

    - By default we use the string `com.apps.@(Project.Name)` converted to lower case.
    - This was done to solve collisions in the file system causing builds to fail.
    - On iOS, `_` is replaced by `-` because `_` are not allowed in bundle identifiers.

    If you want to override the defaults, you can edit your `.unoproj`.
    ```
    "Android.Package": "com.domain.my_app"
    ```
    ```
    "iOS.BundleIdentifier": "com.domain.my-app"
    ```
    Users that have apps in app stores probably want something
    that fits their app or organization, rather than the defaults.

0.28
----
## Bugfixes
- Fixed an erratic crash while performing HTTP-requests in MSVC and CMake targets.

### CLI improvements

- Add `no-build` command.

    This allows deferring `--build`, `--debug` & `--run` without triggering a full `uno build`.
    Options are simplified but similar to `uno build`, please see `uno no-build --help`.

    One example:
    ```
    cd <project-dir>
    uno build ios --no-native
    uno no-build ios --build --run
    ```

    Thanks to @bolav for suggesting this feature.

### Other goodies

- C++/MSVC: Add *.natvis files for Visual Studio. This makes debugger better able to visualize Uno objects.
- ~~build: Use <project>.lock during build (or clean) for exclusive access to project.~~

### Architecture (internal)

- Clean up Uno.Common.dll and Uno.Compiler.API.dll
- Factor out Uno.Compiler.Foreign.dll (work in progress)
- Factor out Uno.Compiler.Plugins.dll (work in progress)
- Add diagrams, see https://github.com/fusetools/Uno/tree/master/Source
- This breaks Fuse, but a migration branch exists at https://github.com/fusetools/Fuse/tree/feature-uno

### NuGet & UPM packages (internal)

- In TeamCity, .NUPKGs for upload to UPM are now produced from core Uno packages and included in `upload.zip`.
- A X-platform NuGet package for C# including Uno.Compiler.API.dll and Uno.Common.dll is also produced on Windows agents.

0.27
----

- Add `Uno.Permissions`. Currently this is android only but will be expanded to
  iOS in due course. It supports both the old Android static permissions and
  the newer request based permissions.

- All Uno projects now include the android 'support-v4' library. This means
  there is no longer a dedicated uno package for 'support-v4'`

0.26
----

### Foreign Code

- Fixed a bug which resulted in comments not working in foreign Objective-C.
- Added support for writing constructors using foreign code.
- Foreign Objective-C code is now automatically wrapped in an Objective-C `@autoreleasepool`.

### out/ref

- Added support for using interfaces as parameters to foreign Objective-C functions.

- Added support for `out` and `ref` parameters in foreign Objective-C functions.
The Objective-C type for such a parameter is a pointer to the Objective-C type of
the parameter according to the old rules.

The following two examples show how it works:

```uno
[Foreign(Language.ObjC)]
extern(iOS) void PrimitiveOutParam(ref int m, out int n)
@{
    // m and n are of type `int*` here.
    *m = 222;
    *n = 123;
@}

[Foreign(Language.ObjC)]
extern(iOS) void StringOutParam(ref string m, out string n)
@{
    // m and n are of type `NSString**` here.
    *m = @"Out1";
    *n = @"Out2";
@}

```

As Java doesnt have out/ref parameters, it is very unlikely that we will support this for Java.

### ForeignInclude

You can now use the `ForeignInclude` attribute to:

- Add header files in ObjC
- Add `import`s in Java

It can only be used on classes. The includes affect all Foreign methods in the uno class.
```uno
[ForeignInclude(Language.Java, "java.lang.Runnable", "java.lang.Boolean", "android.app.Activity")]
[ForeignInclude(Language.ObjC, "Example.hh")]
public class SomeUnoClass : Uno.Application
{
    ...
}
```

## UX Compiler improvements

- `this` is now the implicit name of the current `ux:Class` or `ux:InnerClass` root node.

0.25
----

### API changes

- Added `Uno.IO.Bundle` and `Uno.IO.BundleFile`
- Removed old `Uno.BundleFile`

### C++ changes

- Changes in base libraries (uno-base)

    - `Xli/Foo.h` -> `uBase/Foo.h`
    - `Xli::Foo` -> `uBase::Foo`
    - ...

    Ask us on slack/#uno if you get problems.

### UXL
- Required `Xcode.Framework`s that have file extensions now do not get an
  additional `.framework` extension.

### ux:Global support for value types

You can now create `ux:Global` objects of value types, such as `float4`. This is convenient e.g. for creating global color constants.

### ux:InnerClass

- Added support for `ux:InnerClass`, which is a class that has access to named UX objects in its scope.
  Specifically, the differences are:

	ux:Class

		Declares a global class that can be used anywhere.
		Such classes have no access to ux:Names the scope in which they are declared.

	ux:InnerClass

		Declares a class that can only be used in the scope where it is declared.
		In return, it has full access to all ux:Names in the scope it is declared.

		Inner classes can be extracted into separate .ux files, and then included
		using <ux:Include File="Foo.ux"> in the scope you want to use it.

		Inner classes can not specify namespaces, they will implicitly be in the
		name scope of their containing class.

		If root nodes are marked ux:InnerClass, they are ignored by the UX compiler
		unless ux:Included somewhere.

### Foreign code

- Added support for processed foreign files

  An include file in an Uno project with type `ObjCSource` or `ObjCHeader` will have any UXL macros pre-processed in the same way that such macros are expanded in Foreign Code blocks. The macro expansion requires the file to be an Objective-C++ file (it uses features from both C++ and Objective-C to interoperate with Uno code), and that the file `uObjC.Foreign.h` has been included.

- Added support for passing delegates to and from foreign Objective-C functions

    Delegates get converted to an Objective-C block of the corresponding type. As an example, an argument of the type `Func<string, int, object>` becomes a block of type `^ id<UnoObject>(NSString*, int)`. The argument and return type of the block use the same type conversions as arguments to foreign functions normally do.

- Added support for passing arrays to foreign Objective-C functions

    Arrays are converted to an object of type `id<UnoArray>` which is a wrapper around the Uno array. It can be indexed and updated with the familiar `arr[i]` syntax and has a `count` method (called with `[arr count]`) that returns an `NSUInteger`. Note that updates to it are reflected in the original Uno array --- it's a wrapper, not a copy.  It's also possible to copy the `id<UnoArray>` to an `NSArray*` by calling `[xs copyArray]`.

    Since Objective-C lacks generics, indexing into the `id<UnoArray>` object to get an element returns `id` regardless of the element type of the array on the Uno side. This `id` is a _boxed_ representation of the element type according to the following table:

    | Uno                         | Objective-C           | Boxed array element |
    |-----------------------------|-----------------------|---------------------|
    | `int`, `bool`, `char`, etc. | `int`, `bool`, `char` | `NSNumber*`         |
    | `string`                    | `NSString*`           | `NSString*`         |
    | `ObjC.Object`               | `id`                  | `id`                |
    | `object`                    | `id<UnoObject>`       | `id<UnoObject>`     |
    | `Func<string, int>` etc.    | `^ int(NSString*)`    | `^ int(NSString*)`  |
    | `SomeType[]`                | `id<UnoArray>`        | `id<UnoArray>`      |

    Most types are already boxed, but note that primitive types like `int`, `bool`, and `char` are boxed as `NSNumber*` when accessed in a wrapped array. This means that to update an Uno array argument `int[] x` on the Objective-C side, we have to write e.g. `x[index] = @42;`. When copying an array, the resulting `NSArray*`'s elements are also boxed following the same rules.

    It's possible to circumvent the boxing behaviour by using good old UXL macros. The following examples contrast the two ways to use arrays in foreign Objective-C code:

    ```uno
    [Foreign(Language.ObjC)]
    public static extern(iOS) void ForeignIntArray(int[] xs)
    @{
        @{int[]:Of(xs).Set(3, 123)};
        for (int i = 0; i < @{int[]:Of(xs).Length:Get()}; ++i)
        {
            NSLog(@"array[%d]=%d", i, @{int[]:Of(xs).Get(i)});
        }
    @}
    ```

    ```uno
    [Foreign(Language.ObjC)]
    public static extern(iOS) void ForeignIntArray(int[] xs)
    @{
        xs[3] = @123;
        for (int i = 0; i < [xs count]; ++i)
        {
            NSLog(@"array[%d]=%@", i, xs[i]);
        }
    @}
    ```

- Added support for passing uno arrays to foreign Java functions

Just like with ObjC we can now pass arrays. And like ObjC there are some language specific details.

First off we try to avoid copying massive ammounts of data by default, so we pass up a boxed uno array. You can then call `.copyArray()` on that to get the native array (at which point the data is copied)

The conversions look like this (Note: Java does not allow generics of primitives types)

    | Uno Type              | Boxed Java Type      | Unboxed Java Type   |
    |-----------------------|----------------------|---------------------|
    | bool[]                | com.uno.BoolArray    | bool[]              |
    | sbyte[]               | com.uno.ByteArray    | byte[]              |
    | char[]                | com.uno.CharArray    | char[]              |
    | short[]               | com.uno.ShortArray   | short[]             |
    | int[]                 | com.uno.IntArray     | int[]               |
    | long[]                | com.uno.LongArray    | long[]              |
    | float[]               | com.uno.FloatArray   | float[]             |
    | double[]              | com.uno.DoubleArray  | double[]            |
    | string[]              | com.uno.StringArray  | String[]            |
    | anyOtherType[]        | com.uno.ObjectArray  | com.uno.UnoObject[] |

Java also doesn't allow operator overloading so to get the first int from an IntArray called `x` use `x.get(0)`. To set first value in the `IntArray` `x` to `10` use `x.set(0, 10)`


- Added support for passing uno delegate to foreign Java functions

You can now use uno delegate types in your foreign java functions.

So if you define a delegate like this

    ```uno
    namespace Foo
    {
       public delegate void Bar(float x, float y, float z);
    }
    ```
Then you can do this:

    ```uno
    [Foreign(Language.Java)]
    public static extern(Android) void ForeignDelegate(Bar x)
    @{
        x.run(1.0f, 2.0f, 3.0f);
    @}
    ```
Now 'Java < 8' doesn't have lambdas, and runnable and callables don't take arguments, so behind the scenes the compiler makes a java class called `com.foreign.Foo.Bar` with a `public void run(float x, float y, float z) { ... }` method.

The usual type conversion for primitive, string, objects & arrays still apply to arguments of delegates.

You can also pass uno `Action`s to Java. Again because Java delegates don't support primitives we have to generate a class for this.

The type conversions follow this pattern:

    ```uno
    Action -> com.foreign.Uno.Action
    Action<int> -> com.foreign.Uno.Action_int
    Action<int[],int> -> com.foreign.Uno.Action_IntArray_int
    ```


0.24
----

### C++ backend

- Fixed a bug where the `finally` block in a `try-finally` statement was not
  run if the `try` block returned.

### Internal changes

- Renamed Dependencies/ -> Prebuilt/
- Renamed `uno browse` -> `uno disasm`
- Removed outdated `uno run`
- No longer need Xamarin Studio installed to build on OS X
- Solutions removed, Uno.sln is generated when running `build` (Windows) or `make`
- New .unoconfig syntax
    - Append operator: `SearchPaths += [ foo bar ]`
    - Includes: `include <filename>`
        - Works when file doesn't exist
        - Supports wildcard: `include *.unoconfig`

0.23
------

### Uno language

- Added `@(Macro)` and `@keyword`

    ```uno
    string projectName = @(Project.Name);
    string @enum = "this strings name is a keyword";
    ```

    `@(Macro)` expressions allows you to access settings from your Uno project, UXL or the
    command-line. The return value is always string.

    Uno supports built-in macros which will return different values based on context.

    | Built-in macro        | Returns                                       |
    |-----------------------|-----------------------------------------------|
    | `@(FILE)`             | Path to the current source file.              |
    | `@(LINE)`             | Number of the current line.                   |
    | `@(DIRNAME)`          | Path to the parent directory of `@(FILE)`.    |
    | `@(PACKAGE)`          | Name of the package containing `@(FILE)`.     |
    | `@(PACKAGE_DIR)`      | Root directory of `@(PACKAGE)`.               |
    | `@(PACKAGE_VERSION)`  | Version of `@(PACKAGE)`.                      |

- Added `[Attribute]` on enum literals

    ```uno
    enum Foo
    {
        [Bar]
        Baz = 1
    }
    ```

### Internal changes

- Package format changed

    This means your package library must be updated.

    - From source: `make clean && make`
    - From stuff: Make sure you got the newest stuff.

- `uno doctor` optimization

    `uno doctor -a` runs about 20-30 seconds faster/roughly twice as fast because of caching.

- Root scripts moved to Scripts/ and Tests/

    UNIX users should use `make` instead of running the scripts directly:

    - `make` -> Run Scripts/build.sh
    - `make clean` -> Run Scripts/clean.sh
    - `make release` -> Run Scripts/pack.sh
    - `make test` -> Run Tests/test.sh

    `build.bat` and `clean.bat` kept in root for now because of missing make on Windows.

0.21
------

### UX markup

- Bugfixes for ux:Property

    - This release fixes many issues related to the ux:Property feature.

- Changes to ux:Property

    A node marked with `ux:Property` no longer represents the default value, but just a definition of the property.
    The default value has to be specified either on the containing class node, for example:

        <App Theme="Basic">
            <Panel ux:Class="MyPanel" ux:Name="self" Paint="#f00">
                <Brush ux:Property="Paint"/>

                <Rectangle Fill="{Property self.Paint}"/>
            </Panel>
            <Panel>
                <MyPanel Paint="#00f" />
            </Panel>
        </App>

    Note that for `{Property self.Paint}` to work, the declared property type has to exactly match the type where it is used (`Brush`).

    You can also use `ux:Binding` to set a complex object as default value:

            <Panel ux:Class="MyPanel" ux:Name="self" >
                <Brush ux:Property="Paint"/>

                <LinearGradient ux:Binding="Paint">
                    <GradientStop Offset="0" Color="#f00" />
                    <GradientStop Offset="1" Color="#0f0" />
                </LinearGradient>

                <Rectangle Fill="{Property self.Paint}"/>
            </Panel>

- New feature : ux:Include

    - BREAKING CHANGE: UX Markup files with no `ux:Class` on the root node no longer produce a class based on the file name. You have to explicitly set a class name on the file's root node (`ux:Class="ClassName"`).
      The UX compiler will politely tell you how to fix this in a compile-time error message.

    - `<ux:Include File="Foo.ux" />` will copy/paste include another UX file at the given location. This will make it easier to split large UX components into separate files.

### Uno language

- Foreign code blocks

    Uno methods marked with the `[Foreign(Language.LANGAGE)]` attribute, where
    `LANGUAGE` is `ObjC` or `Java` can be written in the foreign language provided
    the function body is written inside "foreign braces": `@{ ... @}`.

    Foreign functions automatically convert the arguments and return value from
    their Uno representation to a corresponding foreign language
    representation.

    Primitive (int, char, float, etc) types are converted to the corresponding
    primitive type in the foreign language.

    Other types are automatically converted in both directions according to the
    following tables:

    | Uno           | Java            |
    |---------------|-----------------|
    | `string`      | `String`        |
    | `Java.Object` | `Object`        |
    | `object`      | `UnoObject`     |

    | Uno           | Objective-C     |
    |---------------|-----------------|
    | `string`      | `NSString*`     |
    | `ObjC.Object` | `id`            |
    | `object`      | `id<UnoObject>` |

    We can see that we have `UnoObject` to wrap Uno objects on the foreign
    side, and `{Java,ObjC}.Object` to wrap foreign objects on the Uno side.

    UXL macros such as `Call`
    (`@{Type:Of(thing).Method(argTypes):Call(args)}`), `Set`, and `Get`  can be
    used to call back to Uno code from the foreign code block with reversed
    type conversions.

    If the foreign method is an instance method, the foreign code block can
    additionally access the wrapped `this` object in the automatically added
    `_this` parameter.

- New Uno project file types `CSource`, `CHeader`, and `Java`

    These can be used instead of having to add the file as a `File` in the Uno
    project and then using `<ProcessFile {Header,Source}File="filename" />` in
    an extensions file.

- Conditional file inclusion in Uno projects

    Using

    ```uno
    "Includes": [
      "file.ext:FileType:CONDITION"
    ]
    ```

    in a project file includes the file only in builds fulfilling CONDITION
    (e.g. `Android`, `iOS`).

- `Require` attribute

    `[Require("Key", "Value")]` can now be used on Uno classes and methods in
    place of specifying `<Require Key="Value" />` separately in an extensions file.

- `extern` statements (`@{...@}`)

    You can pass arguments, similar to `extern` expressions.

    ```uno
    int arg = 10;
    ...
    extern(arg)
    @{
        printf("%d", $0);
    @}
    ```

    When only access to method arguments is needed, the `extern` header can be omitted.

    ```uno
    void foo(int arg)
    ...
    @{
        printf("%d", $0);
    @}
    ```

### Build system

- New iOS build settings

    - Added new project properties

        ```json
        "iOS": {
            "BundleIdentifier": "com.uno.$(Name)",
            "BundleName": "$(Title)",
            "DeploymentTarget": "7"
        },
        ```

        These `plist` options are supported by Uno projects:

        - UIRequiredDeviceCapabilities
        - MKDirectionsApplicationSupportedModes
        - NSHealthShareUsageDescription
        - NSHealthUpdateUsageDescription
        - UIApplicationExitsOnSuspend
        - UIFileSharingEnabled
        - UINewsstandApp
        - UIPrerenderedIcon
        - UIRequiresPersistentWiFi
        - UISupportedExternalAccessoryProtocols
        - UIViewControllerBasedStatusBarAppearance
        - UIViewEdgeAntialiasing

        This is an example plist section of an unoproj using this feature:

        ```json
        "iOS": {
            "PList": {
                "MKDirectionsApplicationSupportedModes": [
                    "MKDirectionsModeCar",
                    "MKDirectionsModeBus",
                ],
                "UIRequiresPersistentWiFi": true,
                "UIRequiredDeviceCapabilities": [
                    "camera-flash"
                ]
            },
        },
        ```

    - Added a new UXL element called `Xcode.EmbeddedFramework` for adding embedded frameworks to the
      generated Xcode project.

    - Absolute paths to frameworks in `Xcode.Framework` elements are now allowed. This means that
      the following ways to include frameworks are accepted:

        ```xml
        <Require Xcode.Framework="some.framework" /> <-- relative to SDKROOT/System/Library/Frameworks/ as before
        <Require Xcode.Framework="/path/to/some.framework" /> <-- absolute path
        <Require Xcode.Framework="@('some.framework':Path)" /> <-- relative to UXL file
        ```

        The `.framework` extension is optional and will be added automatically if omitted.

- Improved Android support

    - Android Studio support (experimental)

        Uno projects can now be opened in Android Studio, making it easier to debug native code on
        device. Type `uno build android --debug` to launch Android Studio directly from the
        command line. Tested using Android Studio version 1.5 which can be downloaded from here:
        http://developer.android.com/tools/studio/index.html

    - New project structure

        In order to support Android Studio, we had to adapt to the new "Gradle" project structure,
        involving moving files around in the generated project. `AndroidManifest.xml` for example,
        can be found under `@(Project.Name)/app/src/main` instead of in the root output directory.
        Java source files are written to `@(Project.Name)/app/src/main/java` instead of `src`. Use
        the `@(Java.SourceDirectory)` macro to refer to the java source directory.

    - New APK launcher

        Some Windows users has been getting stuck on "Uninstalling APK" when trying to deploy APKs.
        This is usually because "Developer Mode" isn't enabled on the device, missing USB drivers,
        or the device is not properly connected. The new APK launcher is now able to print a better
        error message and terminate, instead of getting stuck.

        The new launcher will also print `logcat` output from the device while app is running.

        See `uno launch-apk -h` for more.
        You can pass launch-apk options through `uno build android -a"OPTIONS"`.

    - Added ADB wrapper

        You can use `uno adb [args]` if you need to run `adb` from the Android SDK without adding
        "platform-tools" to PATH.

    - Added Android SDK Manager wrapper

        You can use `uno android` if you need to open the Android SDK Manager.

    - Improved "space support", so Android users can enjoy...

        - ...projects containing spaces in the name
        - ...projects containing spaces in the path
        - ...Android SDK and NDK installed in a location containing spaces in the path

    - New Uno project settings for SDK configuration

        If you have specific requirements regarding SDK, you can now control those with the
        following properties:

        ```json
        "Android": {
          "NDK": {
            "PlatformVersion": null     // default: 9
          },
          "SDK": {
            "BuildToolsVersion": null,  // default: 23.0.0
            "CompileVersion": null,     // default: 19
            "MinVersion": null,         // default: 10
            "TargetVersion": null       // default: 19
          }
        },
        ```

        We do however recommend to keep `null` for automatic detection of settings based on the
        current SDK installed on the build machine (`fuse install android`).

    - Fixed a problem causing `mkdir` to sometimes fail on Windows
    - Updated native compilers to GCC 4.9

- New project build directory defaults

    ```json
    "BuildDirectory": "build",
    "CacheDirectory": ".uno",
    "OutputDirectory": "$(BuildDirectory)/@(Target)/@(Configuration)"
    ```

    The reason for this is that the old build directory starting with a `.` gets hidden by default
    in OS X, causing some confusion. E.g. when you want to open the build output in another program
    that doesn't support showing hidden files, the non-dotted directory is now much easier to find.

    If this cause trouble, you can pass `--out-dir` to `uno build` to override, or configure your
    project using these old settings:

    ```json
    "BuildDirectory": ".build",
    "CacheDirectory": ".cache",
    "OutputDirectory": "$(BuildDirectory)/@(Target)-@(Configuration)"
    ```

    Remember to update your `.gitignore`s.

- `uno build` improvements

    - The following targets where renamed, but you can still use the old name:

        | New name  | Old name   |
        |-----------|------------|
        | MSVC      | MSVC12     |
        | DotNet    | DotNetExe  |

    - The `--debug` argument is supported by the following targets:

        | Build target  | Action                                   |
        |---------------|------------------------------------------|
        | Android       | Opens Android Studio                     |
        | CMake         | Opens Xcode (OS X only)                  |
        | iOS           | Opens Xcode (OS X only)                  |
        | MSVC          | Opens Visual Studio 2013 (Windows only)  |

        If the project is already open in an IDE/debugger, we won't open any additional windows. You can
        use `--debug` as often as you want without being worried about getting spammed by new windows.

        `--debug` will disable the native compile step in Uno, and enable native debugging, so that you
        can build and debug from the IDE. Otherwise `--debug` is short hand for `--run-args=debug`,
        and will pass `debug` as an argument to the run script. This may fail without error if used on
        an unsupported build target.

    - Specify target without `--target`

        For example, to build for Android:

        ```
        uno build android
        ```

    - Added new shorthand arguments:

        - `-N` -> `--no-native` (Disable native compile step)
        - `-S` -> `--no-strip` (Disable removal of unused code)
        - `-d` -> `--debug` (Open IDE for debugging)

    - C++ debugging flags can be passed on the command line:

        - `-DDEBUG_UNSAFE` (Enable asserts in unsafe code)
        - `-DDEBUG_ARC1..4` (Print debug info from memory manager)
        - `-DDEBUG_DUMPS` (Dumps object graphs to disk for analysis)

- UXL changes

    - New macro operators

        | Macro                 | Input      | Output       |
        |-----------------------|------------|--------------|
        | `@(FOO:EscapeSpace)`  | `foo bar`  | `foo\ bar`   |
        | `@(FOO:QuoteSpace)`   | `foo bar`  | `"foo bar"`  |

    - Tolerant XML syntax

        The UXL parser is able to handle attributes and inner text containing non-encoded XML entities.
        Quotes are optional on simple attribute values, such as numbers and booleans.
        For example, this is now accepted by the parser:

        ```xml
        <Define UNIX />
        <Set Foo=1 Condition="FOO && BAR" />
        <Expression>a < b</Expression>
        <Expression>a < b</Expression>
        ```

        Instead of, compared to strict XML:

        ```xml
        <Define UNIX="UNIX" />
        <Set Foo="1" Condition="FOO &amp;&amp; BAR" />
        <Expression><![DATA[a < b]]></Expression>
        <Expression>a &lt; b</Expression>
        ```

### Internal changes

- `uno doctor` rewrite to handle standard library development more efficiently

    It's now better at detecting what projects needs to be built or not. This improvement fixes bugs
    and saves time.

    Rebuild specific package(s) `Foo` and `Bar` from the command line:

    ```
    uno doctor Foo Bar
    ```

    See `uno doctor -h` for more.

- No more NuGet or git submodules

    NuGet and git submodules are completely removed from this repository forever.
    This improves build times (10-30s) and just makes the repository easier to deal with.

    C# dependencies are managed by Stuff™, and the source is available on G-hub:
    https://ghub.com/fusetools/uno-dependencies

- Updated to Stuff™ version 1.0.5

    Fixes file lock bugs and adds some new syntax:

    - `!`, `||`, `()` are now supported in conditional expressions
    - `else if` is now supported, to complement `if..else` statements

- `.unoconfig`: `PackageBuildPaths` was renamed to `PackageSourcePaths` (deprecated)


0.19
------

### Uno.Http fixes

- Change Uri.Encode and Uri.Decode to treat space as %20, like RFC 2396 and the rest of the world agrees on.
- Change Uri.Encode to not percent-encode tilde and single quotes, in accordance with RFC 2396.
- Make Uri.Encode and Uri.Decode grok UTF-8.


0.18
------

### General changes

- Smaller installer footprint

    Platform specific binaries (Xli, V8, FMOD, ...) used by Uno packages are no longer included in the installer,
    but downloaded on demand if needed during build. This enables us to remove some large and not-always-needed binary files
    from the installer, making the download ~30MB smaller on each platform. This also introduces `uno stuff`, a simple
    built-in package manager.

- Integrated **uno doctor**, **test**, **perf-test** and **perf-cmp** commands

    `uno doctor` replaces StdLibBuilder. Use whenever the package library needs updating. In addition,
    the `--libs` switch can be used on `uno build` to update on demand. The redundant `unotest` and
    `performancetest` commands were removed -- use `uno test` and `uno perf-test` instead.
    See https://github.com/fusetools/Uno#command-reference.

- Append to path arrays in **.unoconfig**

    Personal/project specific *.unoconfig* files can now append additional paths using
    `$(LastPathArray)`. For example:
```json
    PackageSearchPaths: [
        $(LastPathArray)
        "MyPackages/.build"
    ]
    PackageBuildPaths: [
        $(LastPathArray)
        "MyPackages"
    ]
```

- Less verbose build log

    `uno build` will produce minimal output by default. Add `-v`, `-vv` or `-vvv` if you need more output.

- Better **--version**

    `uno --version` will now present the correct version.

- Various performance and stability fixes


### iOS fixes

- Add **iOS.BundleVersion** project property to set plist property `CFBundleVersion` on iOS.
- Fix launching Xcode simulator on iPhone 6. Previously only older generations worked.


### API changes

- Generics and partial reflection support in **Uno.Type**

    Almost 30 new members are introduced in **Uno.Type**, modelled after **System.Type** in .NET.
    This enables runtime generic type support. See https://github.com/fusetools/Uno/commit/b49635699dd64b08c2673a1cf3844ee13ab37101
    for details.

- Use correct return type for bitwise operators

    This affects the following types: **byte, sbyte, short** and **ushort**.


### UXL changes

- Array macros

    - Create arrays using `@{ELEMENT_TYPE:Array([ELEMENTS, ...])}` or `@{ELEMENT_TYPE[]:New(SIZE)}`
    - Access elements using `@{ARRAY:Get(INDEX)}` and `@{ARRAY:Set(INDEX, VALUE)}`

- Renamed C++ specific elements and properties

    The old names will generate a warning when used. Just replace with the new name for silence.

    | New name                        | Old name                      |
    |---------------------------------|-------------------------------|
    | TypeName                        | InstanceTypeName              |
    | FileExtension                   | Source.FileExtension          |
    | SourceFile                      | Build.SourceFile              |
    | HeaderFile                      | Build.HeaderFile              |
    | PreprocessorDefinition          | Build.PreprocessorDefinition  |
    | LinkLibrary                     | Build.LinkLibrary             |
    | LinkDirectory                   | Build.LinkDirectory           |
    | IncludeDirectory                | Build.IncludeDirectory        |
    | Xcode.Framework                 | iOS.Build.Framework           |
    | Xcode.PrefixHeader.Declaration  | iOS.PrefixHeader.Declaration  |

- Renamed CIL specific properties

    The old names will generate a warning when used. Just replace with the new name for silence.

    | New name                        | Old name                      |
    |---------------------------------|-------------------------------|
    | Assembly                        | AssemblyReference             |

- Preprocessor improvement

    `#if..#endif` directives will expand to comments containing the disabled code instead of blank
    lines, when false.


### C++ improvements

- C++ API changes

    As the C++ code generator is more or less rewritten, the underlying C++ APIs are cleaned up too.
    This doesn't affect regular Uno code, but may affect handwritten code using the C++ APIs directly.

    * Most notably, the following methods were changed in *Uno/ObjectModel.h*:

        | New method                             | Old method                             |
        |----------------------------------------|----------------------------------------|
        | uArray::New(arrayType, length, [data]) | uNewArray(elementType, length, [data]) |
        | uString::Ansi(cstr, [length])          | uNewStringAnsi(cstr, [length])         |
        | uString::Utf8(cstr, [length])          | uNewStringUtf8(cstr, [length])         |
        | uString::Const(cstr)                   | uGetStringConst(cstr)                  |
        | uAllocCStr(string)                     | uStringToCStr(string)                  |

      Note that `uArray::New()` now expects `arrayType` directly rather than `elementType`,
      for better cache usage.

    * And these methods in *Uno/Memory.h*:

        | New method                             | Old method                             |
        |----------------------------------------|----------------------------------------|
        | uRetain(object)                        | uRetainObject(object)                  |
        | uRelease(object)                       | uReleaseObject(object)                 |
        | uAutoRelease(object)                   | uAutoReleaseObject(object)             |

    * Some fields were given better names. For example, `uArray::_len` -> `_length`.

    * *Xli.h*, the all including header, was removed. Include a more specific header instead. For
      example *Xli/Mutex.h*.

    Just ask in #uno on our slack community if you need help.

- Runtime parameterization and reflection of generic classes and methods

    * 100% code sharing between generic parameterizations, reducing the amount of C++ code generated
      by 30-40% (default Fuse project, `--no-strip`, MSVC12)

    * Creation of new type parameterizations at run time using reflection, which for example helps
      the Fuse UX simulator in terms of robustness

    The new C++ APIs powering this are found in the *Uno/ObjectModel.h* header.

- General dynamic invoke

    Any Uno method can be invoked by function pointer using a single C++ function: `void uInvoke(const void* func, void** args, size_t count)`.
    To make this possible we've changed the calling convention when using function pointers. All function pointers must return void while any parameters and
    return value is passed by pointer. This allowed us to remove a lot of C++ template clutter, enabling faster compilation and better code.

- Less call stack clutter

    Previously nearly all method calls resulted in two entries in the call stack because of wrapping.
    Now there's just one entry for common method calls, when not being invoked using a delegate or interface.
    This should help make the call stacks less deep and easier to navigate. Many generated methods
    also get cleaner C++ names (`Object::New()` vs. `Object__New()`).

- Keyword awareness

    Uno is aware of keywords and other reserved words, and will rename as necessary to not produce any conflicts.
    A list of words for C++ is found in UXL: https://github.com/fusetools/Uno/blob/master/Library/Core/UnoCore/Targets/CPlusPlus/Config.cpp.uxl#L68

- Most runtime hash look ups eliminated

    - String constants are cached and loaded from static fields
    - Type objects are cached and loaded from static fields (in most cases)
    - `import` will generate static fields instead of using a Dictionary based runtime system

    This fixes serious performance problems in some cases, for example in code where string constants are used heavily.

- Faster interface look ups

    *O(log n)* virtual method table look up, instead of *O(n)*. `is` and `as` operators are also faster.

- Array initializers

    Emit array initializers, e.g. `uArray::Init<int>(Int__typeof(), 2, 0, 1)`, instead of
    unrolling the expression and setting all the elements individually. This makes the generated C++
    code smaller and easier to read.

- Type initializers

    Implement runtime support for type initializers, instead of generating code using a compiler transform.
    This enables support for static generic fields, and generates somewhat cleaner and better behaving code.
    Static constructors are invoked lazily and are called before accessing a static member on the declaring type,
    or instantiation an object of the type.

- Release build by default

    Use release settings when building the generated C++ code. Debug is still the default for Uno code.

    When common case is to *not* run the code using a C++ debugger, optimized settings should be the default as
    they make the code run significantly faster. Specify `-O0` for unoptimized build when debugging.

- iOS: **--run** on device, or **-adebug** in Xcode

    `uno build -tios --run` will build an iOS app and launch it on a connected device, instead of opening Xcode.
    Pass `-adebug` when you want to open Xcode.

- MSVC12: Added **-adebug** flag

    `uno build -tmsvc12 -adebug` will open the Visual Studio project ready for debugging,
    instead of running the executable.

- Various code generator bug fixes

    Fixed a bug where passing a reference field through `ref` or `out` didn't work, and some
    other cases of valid code that didn't compile.

- Less Xcode warnings produced when building for iOS and OS X

### Test runner improvements

- Added timers

    Print how many microseconds (μs) spent running each test, in addition to total time spent
    building and testing.

- Less verbose output

    Only warnings (if any) and STDERR are printed from the build log. Use `-v` if you need more output.

- Tests are now run in alphabetical order


### Internal changes

- Makefile

    You can now build Uno using `make` on all platforms. The following targets are available: `all`,
    `clean`, `install` and `min`. <small>(The MakeRunner package for Atom is nice.)</small>

- Renamed **Library** and **Dependencies** folders

    Instead of *StdLib* and *Stuff*. This removes frustration when attempting to type `St<TAB>` in a
    terminal, and it looks more presentable when we go open source eventually.

- Renamed **Scripts/bin** folder and added **uno-dev** script

    `uno-dev` was added to avoid collisions with installed `uno`. The other wrappers formerly located
    in *Scripts/Commands* were removed -- use `uno test` / `uno-dev test` and similar commands instead.

- Code base clean up

    Major naming/structure clean up, and merging of compiler assemblies. The core compiler
    logic is split between the following assemblies: *Uno.Compiler.API* (BackendFramework, IL,
    Bytecode), *Uno.Compiler.Frontend* (Parser, AST, UXL), *Uno.Compiler.Core*, and backend specific
    assemblies. Otherwise the C# code base is organized in two root directories: *Source* and *Tools*.
    See https://github.com/fusetools/Fuse/commit/135a9b65526efacf20906df7d1ddbdf8f57b2bf7 for details.

- Migration branches

    The following migration branches are currently active:
    - https://github.com/fusetools/fuselibs/tree/feature-uno
    - https://github.com/fusetools/unolibs/tree/feature-uno
    - https://github.com/fusetools/Android/tree/feature-uno
    - https://github.com/fusetools/iOS/tree/feature-uno
    - https://github.com/fusetools/Fuse/tree/feature-uno

- Improved Fuse/Uno link script

    The new `link-uno` script on Fuse works on both Windows (with MSYS) and OS X/Linux, and will create
    symlinks to the Uno binaries instead of copying them. This means that you won't need to rerun
    the script when making changes to Uno. Just make sure to rebuild Fuse, from Visual Studio if you
    prefer. The new `$(LastPathArray)` feature in Unoconfig also makes the experience more robust.
    The `build-uno` variants are hopefully redundant now. See https://github.com/fusetools/Fuse/commit/90d88c42ebccc05f2473ef517622ae0d9d3e0604.

- Added **-DSIMULATOR** to replace **--preview-mode**

    This is fixed on Fuse/feature-uno in https://github.com/fusetools/Fuse/commit/6952f989148fc58b7ee62d2bd7bc3510b4f9ae3b.


2015/10/20
----------

- Add Xcode.FrameworkDirectory UXL element

2015/09/08
----------

- Better support for multitouch on android

2015/09/02
----------

- Speed improvements to android http for small requests
- Fix for http threading bugs

2015/08/20
----------

- Remove the following from Platform.Window
    AppLowMemory
    AppTerminating
    AppEnteringBackground
    AppEnteredBackground
    AppEnteringForeground
    AppEnteredForeground
  We should be using Platform2.Application for lifecycle.
- Split Platform2/EventArgs into Platform/TimerEventArgs & Platform/FrameChangedEventArgs
- Moved ViewNativeHandle from Platform2 to Platform
- Moved SystemUI from Platform2 to Platform
- Moved iOS.uno from Platform2 to Platform

- Added ToString methods for float2x2 float3x3 float4x4

2015/08/13
----------

- Renamed Experimental.Threading to Uno.Threading
- Optionally send in dispatcher to HttpMessageHandler.CreateRequest


2015/08/05
----------

- Fixed bug in UX JavaScript tag parsing

2015/08/4
----------

** Http changes**

- Renamed Uri.EncodeUri to Uri.Encode and Uri.DecodeUri to Uri.Decode


2015/07/22
----------

**New features**

- Add support for marking projects and packages "Internal" (https://github.com/fusetools/Uno/issues/158)


2015/07/17
----------

**New features**

- Added Uri.Parse method (https://github.com/fusetools/unolibs/issues/72)
- Android and IOS now use the 'Mobile.Orientations' project setting
- Added OSX and APPLE UXL conditions
- OSX now uses same http backend as ios and so no longer requires xli

2015/07/17
----------

**Internal changes**

- Increased Ticks accuracy (https://github.com/fusetools/Uno/issues/150)


2015/07/17
----------

**Internal changes**

- Add milliseconds support for OffsetDateTimePattern (https://github.com/fusetools/Uno/issues/103)


2015/07/08
----------

**New features**

- Added `string Uno.Exception.StackTrace` property. This is implemented in .NET and C++ targets, and
  will give you a string similar to `System.Exception.StackTrace` in .NET.
- `Uno.Exception.ToString()` in C++ will now include the exception type name and stack trace in the
  returned string (similar to .NET behaviour).


2015/06/29
----------

**New features**

- Projects: Glob support in `Includes` and `Excludes` items (see: Glob notes).
- `uno build`: Removed undocumented options `--debug` and `--no-debug`. Added shortcut options. (See
  `uno build --help`.)
- `uno create`: Updated options. (See `uno create --help`.)
- `uno update`: Support updating multiple projects recursively. Project items are no longer updated
  automatically, unless `--glob=<pattern> (-g)` is specified. Updated options. (See `uno update --help`.)
- `uno ls`: New command that lists resolved project items, for debugging purposes. (See `uno ls --help`.)
- Fixed compiler warnings in core packages.

**Internal changes**

- Minor changes in `Project` class (https://github.com/fusetools/Uno/commit/e97febe18fdd1f47c58a1edb143ec56b65442b83)

**Glob notes**

These glob features are supported:

- Brace Expansion
- Extended glob matching
- "Globstar" `**` matching

Patterns without `/` in `.unoproj` or `.unoignore` files are matched recursively.

```
*             # Matches all files
*.foo         # Mathces all files named *.foo
*.bar         # Matches all files named *.bar
*.+(bar|foo)  # Matches all files named *.bar or *.foo
foobar        # Matches all files named foobar
```

Prefixing with `/` or `./` disables recursion.

```
/*.png        # Not recursive, matches all PNG files found directly in project directory
```

Use globstar (`**`) for explicit recursion.

```
Foo/**/*.uno  # Mathces all files named *.uno in Foo directory recursively
```


2015/06/26
----------

**New features**

- Compact logging. Compiler will produce alot less output by default. Use `uno build -v` to enable
  verbose output.
- Conditional compilation. Platform-specific code that won't be needed is completely culled, which
  improves performance. This introduces new syntax that replaces `[ExportCondition]` (see: Porting Notes).
- Multithreaded parser. Parsing is now performed on several threads in parallel to improve performance.
- New define: `MOBILE` (replaces `ANDROID || IOS`).
- New define: `DOTNET` (replaces `CIL || CSHARP`).

**Porting notes**

Previously platform-specific types and members were decorated with `[ExportCondition]` attributes for
conditional usage by different build targets. Because attribues requires compilation before the
information can be properly resolved, we've introduced a new `extern(EXPRESSION)` modifier in order to
cull these entities before anything is compiled. This reduces the work load for the compiler
significantly.

In addition, `if defined(EXPRESSION)` syntax is introduced for conditional compilation. If `EXPRESSION`
evaluates to false, code inside the block won't be compiled because entities referred inside the block
might have been culled (producing compile error). `defined(EXPRESSION) ?:` expressions implements the
same behaviour.

In order to stay backwards compatible in most cases, `[ExportCondition("EXPRESSION")]` is automatically
rewritten to `extern(EXPRESSION)` by the parser. Code like `if (defined(ANDROID) || defined(IOS))` is
rewritten to `if defined(ANDROID || IOS)` when possible. More advanced cases must be broken up by hand.

Any compile errors introduced by upgrading to this version can usually be fixed by adding
`if defined(...)` tests or `extern(...)` modifiers. Otherwise help is available on slack.

*Before*:
```uno
[ExportCondition("ANDROID")]
class AndroidClass
{
}
```
*After*:
```uno
extern(ANDROID) class AndroidClass
{
}
```

This change in behaviour also happens to make things like this now possible:
```uno
class Foo
{
    extern(ANDROID) void Bar()
    {
        // Android specific code
    }

    extern(IOS) void Bar()
    {
        // iOS specific code
    }

    void Method()
    {
        if defined(ANDROID || IOS)
            Bar();
    }
}
```

**Internal changes**

- C++ reflection API (`Uno.Reflection.CppReflection`). Build with `uno build -DREFLECTION` to enable.
- C++ backend: CppClassWriter is split into CppType and CppGenerator.
- Several other optimizations/refactorings.


2015/06/23
----------

**New features**

- Added a more compact JSON project format (old XML will still be loaded without problems).
- ~~JSON project format supports implicit files when omitting the `Includes` property, the project
  directory will then be scanned for files automatically.~~ An `Excludes` property is also provided.
- CIL backend: UXL `<Require Assembly="System.Core" />` now supports linking assemblies
  from the Global Assembly Cache (GAC) in addition to plain DLL filenames.

**Internal changes**

- `Project.Files` is now known as `Project.IncludeItems`, `Project.ExludeItems`, and
  `Project.FlattenedItems` (managed automatically)
- `Project.PackageReferences`, `Project.ProjectReferences` and similar properties are changed to
  `IReadOnlyList<T>`. When editing the project use the new properties `Project.MutablePackageReferences`,
  `Project.MutableProjectReferences` and so on. Using the mutable variants will invalidate the
  project and cause it to regenerate `Project.FlattenedItems`.
- `Project.ResolveBuildDirectory(Logger log)` and similar are now replaced by properties
  `Project.BuildDirectory` and so on.


Next release
--------------------

- HTTP: Renamed Experimental.Net.Http to Uno.Net.Http
- HTTP: Enabled support for caching by default
- HTTP: Removed SetContentType, use SetHeader("Content-type", "text/javascript") instead
- HTTP: New Android implementation
- HTTP: New iOS implementation
- HTTP: Removed outdated HttpClient, it's recommended to create your own wrapper around HttpMessageHandler instead.

0.1.542 (2015/05/22)
--------------------
**Internal changes**
- Packages are now shipped with version numbers. If you have projects referring to version 0.1.0, a warning will be printed, and the latest package version will be used instead. To remove the warning, simply delete 'Version="0.1.0" from your .unoproj.

0.1.466 (2015/05/12)
--------------------

**New features**

- CLI: Adding `-D<define>` and `-U<define>` switches for specifying defines
- CLI: Improved `uno update`. It will now scan the project directory and add/remove files automatically. Use `.unoignore` to specify excludes.
- C++ backend: Put all generated code in a global namespace `app::`. This reduces the chance of Uno entities colliding with native C++ entities.
- UXL (Android): Renamed properties `Makefile.SharedLibrary` and `Makefile.StaticLibrary` to `JNI.SharedLibrary` and `JNI.StaticLibrary`

**Internal changes**

- Renamed properties in Project class.
- Renamed some namespaces in Uno.Compiler.Core.

A branch called Fuse/feature-NewUno contains fixed to get Fuse compiling with the new changes.

0.1.341 (2015/04/03)
--------------------

**Uno Changes**

* Feature: Cleaning a project now cleans referenced projects recursively

0.1.225 (2015/04/16)
--------------------

**UnoTest Changes**

* Bugfix: When testing several projects, the statistics at the end always said 0 tests were run and 0 failed. These numbers are now correct
* Feature: `--allow-debugger` flag will allow you to debug your unit tests. Please see `unotest --help`, or the testing tutorial for details.
* Feature: Unotest now returns non-zero when a test has failed.
