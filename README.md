# Uno/UX compiler

[![AppVeyor build status](https://img.shields.io/appveyor/ci/fusetools/uno/master.svg?logo=appveyor&logoColor=silver&style=flat-square)](https://ci.appveyor.com/project/fusetools/uno/branch/master)
[![Travis CI build status](https://img.shields.io/travis/fuse-open/uno/master.svg?style=flat-square)](https://travis-ci.org/fuse-open/uno)
[![NPM package](https://img.shields.io/npm/v/@fuse-open/uno.svg?style=flat-square)](https://www.npmjs.com/package/@fuse-open/uno)
[![NPM package](https://img.shields.io/npm/v/@fuse-open/uno/beta.svg?style=flat-square)](https://www.npmjs.com/package/@fuse-open/uno)
[![License: MIT](https://img.shields.io/github/license/fuse-open/uno.svg?style=flat-square)](LICENSE.txt)
[![Slack](https://img.shields.io/badge/chat-on%20slack-blue.svg?style=flat-square)](https://slackcommunity.fusetools.com/)
[![Financial Contributors on Open Collective](https://opencollective.com/fuse-open/all/badge.svg?label=financial+contributors&style=flat-square)](https://opencollective.com/fuse-open)

![Target platforms](https://img.shields.io/badge/target%20os-Android%20%7C%20iOS%20%7C%20Linux%20%7C%20macOS%20%7C%20Windows-7F5AB6?style=flat-square&logo=android&logoColor=silver)
![Host platforms](https://img.shields.io/badge/host%20os-Linux%20%7C%20macOS%20%7C%20Windows-7F5AB6?style=flat-square)

> Extremely fast, native C#-dialect and powerful tooling for mobile and desktop developers.

Welcome to Uno, the core component in [Fuse Open], a native app development tool suite.

## Install

```
$ npm install @fuse-open/uno
```

This will install the [`uno`][doc2] command and standard library.

### Related packages

* [android-build-tools](https://www.npmjs.com/package/android-build-tools)
* [fuse-sdk](https://www.npmjs.com/package/fuse-sdk)
* [fuselibs](https://www.npmjs.com/package/@fuse-open/fuselibs)

## Introduction

Uno is a collection of compiler and platform abstraction technologies.

* [Uno programming language](src/compiler)
* [UX markup language](src/ux)
* [Project tooling](src/tool)
* [Standard library](lib)
* [Test runner](src/test)

Uno is used on Linux, macOS and Windows, and makes native apps for the following platforms:

| Platform  | Build targets       |
|:----------|:--------------------|
| Android   | `android`           |
| iOS       | `ios`               |
| Linux     | `native`, `dotnet`  |
| macOS     | `native`, `dotnet`  |
| Windows   | `native`, `dotnet`  |

[Fuse Open]: https://fuseopen.com/

### Uno syntax

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
by emitting portable [C++17] for mobile or desktop platforms, or [CIL bytecode] for desktop platforms (Mono/.NET) —
designed for developing high-performance UI-engines, platform abstractions or integrations, and other kinds of
software traditionally required written in native C/C++.

Access all APIs and features on the target platforms directly in Uno — add a snippet of *foreign code*, and
our compiler automatically generates the glue necessary to interoperate (two-way) with a foreign language.
The following foreign languages are supported:

* [C++17], [C99]
* [Java] (Android)
* [Objective-C] (iOS, macOS)
* [Swift] (iOS)

[C#]: https://en.wikipedia.org/wiki/C_Sharp_(programming_language)
[C++17]: https://en.wikipedia.org/wiki/C++17
[C99]: https://en.wikipedia.org/wiki/C99
[CIL bytecode]: https://en.wikipedia.org/wiki/Common_Intermediate_Language
[Java]: https://en.wikipedia.org/wiki/Java_(programming_language)
[Objective-C]: https://en.wikipedia.org/wiki/Objective-C
[Swift]: https://en.wikipedia.org/wiki/Swift_(programming_language)

### Run-time features

* Memory in Uno is managed *semi-automatically* by [automatic reference counting], avoiding unpredictable GC stalls.
* *Real* [generics] – sharing the same compiled code in all generic type instantiations, without [boxing] values, and with
  *full run-time type system* support – avoiding exploding code-size and compile-times (while still being fast).
* *(Opt-in)* [reflection] on *all* platforms – to dynamically create objects and invoke methods based on type information
  *only known at run-time* – enabling high-level Fuse features such as *live-previewing UX documents*.

[automatic reference counting]: https://en.wikipedia.org/wiki/Automatic_Reference_Counting
[boxing]: https://en.wikipedia.org/wiki/Object_type_(object-oriented_programming)#Boxing
[generics]: https://en.wikipedia.org/wiki/Generic_programming
[reflection]: https://en.wikipedia.org/wiki/Reflection_(computer_programming)

> Please see [our documentation](https://fuseopen.com/docs/) for information about building apps.

## Build Instructions

Uno is built using the command-line on Linux, macOS or Windows – or [from inside an IDE](docs/build-instructions.md#building-from-an-ide).

```
make
make install
uno --version
```

> Please see [the build instructions](docs/build-instructions.md) for details
on how to build the source code.

## Configuration

> Please see [the configuration reference documentation][doc1] for details on how to
set up uno's configuration files for your build-environment.

## Command Line Reference

> Please see [the command-line reference documentation][doc2] for details on how to
use uno's command-line interface.

[doc1]: docs/configuration.md
[doc2]: docs/command-line-reference.md

## Contributing

> Please see [CONTRIBUTING](CONTRIBUTING.md) for details on our code of
conduct, and the process for submitting pull requests to us.

### Reporting issues

Please report issues [here](https://github.com/fuse-open/uno/issues).

## Contributors

### Code Contributors

This project exists thanks to all the people who contribute. [[Contribute](CONTRIBUTING.md)]
<a href="https://github.com/fuse-open/uno/graphs/contributors"><img src="https://opencollective.com/fuse-open/contributors.svg?width=890&button=false" /></a>

### Financial Contributors

 Become a financial contributor and help us sustain our community. [[Contribute](https://opencollective.com/fuse-open/contribute)]

#### Individuals

<a href="https://opencollective.com/fuse-open"><img src="https://opencollective.com/fuse-open/individuals.svg?width=890"></a>

#### Organizations

Support this project with your organization. Your logo will show up here with a link to your website. [[Contribute](https://opencollective.com/fuse-open/contribute)]

<a href="https://opencollective.com/fuse-open/organization/0/website"><img src="https://opencollective.com/fuse-open/organization/0/avatar.svg"></a>
<a href="https://opencollective.com/fuse-open/organization/1/website"><img src="https://opencollective.com/fuse-open/organization/1/avatar.svg"></a>
<a href="https://opencollective.com/fuse-open/organization/2/website"><img src="https://opencollective.com/fuse-open/organization/2/avatar.svg"></a>
<a href="https://opencollective.com/fuse-open/organization/3/website"><img src="https://opencollective.com/fuse-open/organization/3/avatar.svg"></a>
<a href="https://opencollective.com/fuse-open/organization/4/website"><img src="https://opencollective.com/fuse-open/organization/4/avatar.svg"></a>
<a href="https://opencollective.com/fuse-open/organization/5/website"><img src="https://opencollective.com/fuse-open/organization/5/avatar.svg"></a>
<a href="https://opencollective.com/fuse-open/organization/6/website"><img src="https://opencollective.com/fuse-open/organization/6/avatar.svg"></a>
<a href="https://opencollective.com/fuse-open/organization/7/website"><img src="https://opencollective.com/fuse-open/organization/7/avatar.svg"></a>
<a href="https://opencollective.com/fuse-open/organization/8/website"><img src="https://opencollective.com/fuse-open/organization/8/avatar.svg"></a>
<a href="https://opencollective.com/fuse-open/organization/9/website"><img src="https://opencollective.com/fuse-open/organization/9/avatar.svg"></a>
