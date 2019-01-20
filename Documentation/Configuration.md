# Configuration

To configure your Uno environment, edit the file `~/.unoconfig` (or `%USERPROFILE%\.unoconfig` on Windows) -
create it if necessary.

Type `uno config` to review your current configuration.

## Standard library

To work with [fuselibs](https://github.com/fusetools/fuselibs-public) source code, add this snippet
to your *.unoconfig*, after cloning the repo:
```
C:\> git clone https://github.com/fusetools/fuselibs-public.git
```
```javascript
if (DEV) {
    Packages.SourcePaths += "C:\\fuselibs-public\\Source"
}
```

(Replace `C:\fuselibs-public\Source` with your own location.)

The `if (DEV)` test makes sure we only use those packages when running `uno` built from source.
If omitted, the packages are also made available to any installed versions of Fuse Studio and Uno.

To build your standard library, type `uno doctor -e`.

## Android

To support building Android apps, we need to know where your [Android SDKs](https://developer.android.com/studio/index.html)
are installed. Running `npm install -g android-build-tools` will set this up automatically, or you can
specify other locations as demonstrated below.

### Windows

```javascript
Android.NDK.Directory: "%LOCALAPPDATA%\\Android\\sdk\\ndk-bundle"
Android.SDK.Directory: "%LOCALAPPDATA%\\Android\\sdk"
Java.JDK.Directory: "%PROGRAMFILES%\\Java\\jdk1.8.0_40"
```

### macOS

```javascript
Android.NDK.Directory: %HOME%/Library/Android/sdk/ndk-bundle
Android.SDK.Directory: %HOME%/Library/Android/sdk
```

## iOS

To support building iOS apps, we need macOS and Xcode.
- [Cocoapods](https://cocoapods.org/) is required by some Uno packages.

This is usually automatically detected, but configuring a signing identity can be useful.
```javascript
iOS.DeveloperTeam: ABCD012345
```

## Native

To support building native apps, we need [CMake](https://cmake.org/) and C++ compilers.
- **macOS:** Xcode with command line tools
- **Windows:** Visual Studio 2017

If `cmake` isn't in found your *PATH*, the location can be provided like this:
```javascript
Tools.CMake: `%PROGRAMFILES%\CMake\bin\cmake.exe`
```

## Node.js

We need [Node.js](https://nodejs.org/en/download/) to support transpiling FuseJS files to ES5.

If `node` isn't found in your *PATH*, the location can be provided like this:
```javascript
Tools.Node: `%PROGRAMFILES%\nodejs\node.exe`
```

## Package manager

These properties have meaningful defaults, but can be customized like this:
```javascript
Packages.Feeds += "https://www.packages.org/api/v2"
Packages.InstallDirectory: install/my/packages/here
Packages.SearchPaths += find/my/packages/here
Packages.SourcePaths += build/these/projects
```
