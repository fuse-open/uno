# Uno Core Platform
[![AppVeyor Build status](https://ci.appveyor.com/api/projects/status/github/fuse-open/uno?branch=master&svg=true)](https://ci.appveyor.com/project/fusetools/uno/branch/master)
[![Tracis CI Build Status](https://travis-ci.org/fuse-open/uno.svg?branch=master)](https://travis-ci.org/fuse-open/uno)
[![license: MIT](https://img.shields.io/github/license/fuse-open/uno.svg)](LICENSE.txt)
[![Slack](https://img.shields.io/badge/chat-on%20slack-blue.svg)](https://slackcommunity.fusetools.com/)

Welcome to Uno.

We're here to help [Fuse Open Source] development by building and maintaining several related pieces of core technology.
* Cross-platform tools for building and running applications
* Core libraries and platform abstraction
* Uno programming language and compiler
* Uno project format, build engine and package manager
* UX markup language and compiler
* Uno/UX test runner

Uno is used on macOS and Windows, and makes native apps for the following platforms:
* Android
* iOS
* macOS (native or Mono)
* Windows (native or .NET)

**Uno syntax**
```uno
class App : Uno.Application
{
    public App()
    {
        debug_log "Hello, world!";
    }
}
```

The Uno programming language is a fast, native dialect of [C#] that can cross-compile to *any native platform* (in theory),
by emitting portable [C++11] for mobile or desktop platforms, or [CIL bytecode] for desktop platforms (Mono/.NET) —
designed for developing high-performance UI-engines, platform abstractions or integrations, and other kinds of
software traditionally required written in native C/C++.

Access all APIs and features on the target platforms directly in Uno — add a snippet of *foreign code*, and
our compiler automatically generates the glue necessary to interoperate (two-way) with a foreign language.
The following foreign languages are supported:
* [C++11], [C99]
* [Java] (Android)
* [Objective-C] (iOS, macOS)
* [Swift] (iOS)

[Fuse Open Source]: https://fuseopen.com/
[C#]: https://en.wikipedia.org/wiki/C_Sharp_(programming_language)
[C++11]: https://en.wikipedia.org/wiki/C++11
[C99]: https://en.wikipedia.org/wiki/C99
[CIL bytecode]: https://en.wikipedia.org/wiki/Common_Intermediate_Language
[Java]: https://en.wikipedia.org/wiki/Java_(programming_language)
[Objective-C]: https://en.wikipedia.org/wiki/Objective-C
[Swift]: https://en.wikipedia.org/wiki/Swift_(programming_language)

**Run-time features**
* Memory in Uno is managed *semi-automatically* by [automatic reference counting], avoiding unpredictable GC stalls.
* *Real* [generics] – sharing the same compiled code in all generic type instantiations, without [boxing] values, and with
  *full run-time type system* support – avoiding exploding code-size and compile-times (while still being fast).
* *(Opt-in)* [reflection] on *all* platforms – to dynamically create objects and invoke methods based on type information
  *only known at run-time* – enabling high-level Fuse features such as *live-previewing UX documents*.

[automatic reference counting]: https://en.wikipedia.org/wiki/Automatic_Reference_Counting
[boxing]: https://en.wikipedia.org/wiki/Object_type_(object-oriented_programming)#Boxing
[generics]: https://en.wikipedia.org/wiki/Generic_programming
[reflection]: https://en.wikipedia.org/wiki/Reflection_(computer_programming)

See https://fuseopen.com/docs/ for more information about the Uno/UX (and JavaScript) stack.

## Building

Uno is built using the command-line on Linux, macOS or Windows – or [from inside an IDE](#building-from-ide).

### Prerequisites

- [Mono](http://www.mono-project.com/download/) / [Visual Studio](https://www.visualstudio.com/downloads/)<sup>[1]</sup>
- [Bash](http://www.msys2.org/) and [Make](http://gnuwin32.sourceforge.net/packages/make.htm)<sup>[2]</sup>
- [Node.js](https://nodejs.org/en/download/)
- [NuGet](https://www.nuget.org/downloads/)

| Build command         | Action                                                            |
|:----------------------|:------------------------------------------------------------------|
| `make`<sup>[3]</sup>  | Builds `uno` and core library. Works on all platforms.            |

 1. On Windows, we need [vswhere] to locate your Visual Studio 2017 installation. Please make sure we can find `vswhere` in
    `%PATH%` or at `%PROGRAMFILES(x86)%\Microsoft Visual Studio\Installer`.
 2. Our cross-platform build scripts are written in `bash`, and `make` is a convenient way to invoke the different build tasks.
 3. You also can run `bash scripts/build.sh` directly if you don't have `make`.

[vswhere]: https://github.com/Microsoft/vswhere

### Additional make targets

| Build command   | Action                                                                  |
|:----------------|:------------------------------------------------------------------------|
| `make install`  | Creates a symlink for `uno` in `/usr/local/bin`.                        |
| `make release`  | Prepares a release directory for distribution.                          |
| `make unocore`  | Generates C# code for Uno.Runtime.Core.dll, based on Uno code.          |
| `make clean`    | Removes build artifacts from the repository and `Packages.SourcePaths`. |
| `make check`    | Runs the local test suite.                                              |

### Building from IDE

Open `uno.sln` in a capable C# 6.0 IDE, such as Visual Studio 2015+ or JetBrains Rider. Dependencies will be automatically
installed when building, so you don't really need to use the command-line.

When debugging, make sure to use `main/Uno.CLI.Main` as startup project to launch `uno`'s entrypoint.

### Building the standard library

To build uno's standard library, run `<uno-root>/bin/uno doctor` – or pass `doctor` as parameter to the
`main/Uno.CLI.Main` project when running from an IDE.

## Running

To run `uno` in your terminal, type `<uno-root>/bin/uno`.

On Windows, some additional runtimes might be needed.
* VCRedist 2010: [x86](https://www.microsoft.com/en-us/download/details.aspx?id=5555), [x64](https://www.microsoft.com/en-US/Download/confirmation.aspx?id=14632)
* [VCRedist 2013](https://www.microsoft.com/en-gb/download/details.aspx?id=40784)

## Contributing

Please read [CONTRIBUTING.md](CONTRIBUTING.md) for details on our code of
conduct, and the process for submitting pull requests to us.

### Reporting issues

Please report issues [here](https://github.com/fuse-open/uno/issues).

## Configuration

Please read [the configuration reference documentation][doc1] for details on how to
set up uno's configuration files for your build-environment.

## Command Line Reference

Please read [the command-line reference documentation][doc2] for details on how to
use uno's command-line interface.

[doc1]: docs/configuration.md
[doc2]: docs/command-line-reference.md
