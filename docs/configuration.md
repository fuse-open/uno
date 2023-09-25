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
searchPaths.sources += `C:\fuselibs\Source`
```

(Replace `C:\fuselibs\Source` with your own location.)

Run `uno doctor` to build your standard library.

### Additional libraries

Use the following `.unoconfig` properties to add additional Uno libraries (or projects) to your search paths.

```javascript
searchPaths += find/my/packages/here
searchPaths.sources += build/these/projects
```

### Conditional inclusion

```javascript
if (DEV) {
    searchPaths.sources += `C:\fuselibs\Source`
}
```

The `if (DEV)` test makes sure we only use those packages when running `uno` built from source.
If omitted, the packages are also made available to any installed versions of Fuse Studio and Uno,
which might have unintended side-effects.

## Dependencies

### Android

To support building Android apps, we need to know where your [Android SDK](https://developer.android.com/studio/index.html)
is installed. Running `npm install android-build-tools@2.x -g` will set this up automatically, or you can
specify other locations as demonstrated below.

### Windows

```javascript
android.sdk: `%LOCALAPPDATA%\Android\sdk`
java.jdk: `%PROGRAMFILES%\Java\jdk17`
```

### macOS

```javascript
android.sdk: ~/Library/Android/sdk
```

### iOS

To support building iOS apps, we need macOS and Xcode.

- [Cocoapods](https://cocoapods.org/) is required by some Uno packages.

This is usually automatically detected, but configuring a signing identity can be useful.

```javascript
ios.developerTeam: ABCD012345
```

### Native

To support building native apps, we need [CMake](https://cmake.org/) and C++ compilers.

- **macOS:** Xcode with command line tools
- **Windows:** Visual Studio 2019
