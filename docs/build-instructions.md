# Build instructions

Uno is built using the command-line on Linux, macOS or Windows – or [from inside an IDE](#building-from-ide).

## Prerequisites

- [Mono](http://www.mono-project.com/download/) / [Visual Studio](https://www.visualstudio.com/downloads/)
- [Bash](http://www.msys2.org/) and [Make](http://gnuwin32.sourceforge.net/packages/make.htm)
- [Node.js](https://nodejs.org/en/download/)
- [NuGet](https://www.nuget.org/downloads/)

Our cross-platform build scripts are written in `bash`, and `make` is a convenient way to invoke the different build tasks.
Bash is included in [Git for Windows](https://git-scm.com/downloads).

On Windows, we need [vswhere] to locate your Visual Studio 2017 installation. Please make sure we can find `vswhere` in
`%PATH%` or at `%PROGRAMFILES(x86)%\Microsoft Visual Studio\Installer`.

[vswhere]: https://github.com/Microsoft/vswhere

## Building from command-line

```
make
```

This builds `uno` and core library, and works on all platforms. You also can run `bash scripts/build.sh` directly if you don't have `make`.

After building, you can use `<uno-root>/bin/uno` to run your local `uno`.

### Additional make targets

| Build command   | Action                                                                  |
|:----------------|:------------------------------------------------------------------------|
| `make lib`      | Builds Uno libraries in `lib/` only, instead of full build.             |
| `make install`  | Creates a symlink for `uno` in `/usr/local/bin`.                        |
| `make release`  | Prepares a release directory for distribution.                          |
| `make unocore`  | Generates C# code for Uno.Runtime.Core.dll, based on Uno code.          |
| `make clean`    | Removes build artifacts from the repository and `Packages.SourcePaths`. |
| `make check`    | Runs the local test suite.                                              |

## Building from IDE

Open `uno.sln` in a capable C# 6.0 IDE, such as Visual Studio 2015+ or JetBrains Rider. Dependencies will be automatically
installed when building, so you don't really need to use the command-line.

When debugging, make sure to use `main/Uno.CLI.Main` as startup project to launch `uno`'s entrypoint.

## Building the standard library

To build uno's standard library, run `<uno-root>/bin/uno doctor` – or pass `doctor` as parameter to the
`main/Uno.CLI.Main` project when running from an IDE.
