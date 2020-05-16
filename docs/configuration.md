# Configuration

You can create or edit your local user config file using a text editor to configure your Uno environment. This file is located at `~/.unoconfig` (`%USERPROFILE%\.unoconfig` on Windows).

Run `uno config` to review your current configuration.

## Standard library

To work with [fuselibs](https://github.com/fuse-open/fuselibs) source code, add this snippet
to your *.unoconfig*, after cloning the repo:

```
C:\> git clone https://github.com/fuse-open/fuselibs.git
```

```javascript
if (DEV) {
    Packages.SourcePaths += `C:\fuselibs\Source`
}
```

(Replace `C:\fuselibs\Source` with your own location.)

The `if (DEV)` test makes sure we only use those packages when running `uno` built from source.
If omitted, the packages are also made available to any installed versions of Fuse Studio and Uno,
which might have unintended side-effects.

Run `uno doctor` to build your standard library.

## Android

To support building Android apps, we need to know where your [Android SDKs](https://developer.android.com/studio/index.html)
are installed. Running `npm install -g android-build-tools` will set this up automatically, or you can
specify other locations as demonstrated below.

### Windows

```javascript
Android.NDK: `%LOCALAPPDATA%\Android\sdk\ndk-bundle`
Android.SDK: `%LOCALAPPDATA%\Android\sdk`
Java.JDK: `%PROGRAMFILES%\Java\jdk1.8.0_40`
```

### macOS

```javascript
Android.NDK: ~/Library/Android/sdk/ndk-bundle
Android.SDK: ~/Library/Android/sdk
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
- **Windows:** Visual Studio 2019

## Node.js

We need [Node.js](https://nodejs.org/en/download/) to support transpiling FuseJS files to ES5.

## Package manager

```javascript
Packages.SearchPaths += find/my/packages/here
Packages.SourcePaths += build/these/projects
```
