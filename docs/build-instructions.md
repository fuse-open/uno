# Build instructions

Uno is built using the command-line on Linux, macOS or Windows – or [from Visual Studio 2022](#building-from-an-ide).

## Building from command-line

### Prerequisites

- [.NET 6.0 SDK](https://dotnet.microsoft.com/download/dotnet-core/6.0) / [Visual Studio 2022](https://www.visualstudio.com/downloads/)
- [Bash](http://www.msys2.org/) and [Make](http://gnuwin32.sourceforge.net/packages/make.htm)
- [Node.js](https://nodejs.org/en/download/)

> Our cross-platform build scripts are written in `bash`, and `make` is a convenient way to invoke build tasks. Bash is included in [Git for Windows](https://git-scm.com/downloads).

### Build

```
make
```

This will build `uno` and standard library.

> You can also try `npm run build` or `bash scripts/build.sh` for similar results.

### Install and run

```
make install
uno --version
```

This will add your local-built `uno` to PATH and print version information.

> You can also try `npm link` or `<uno-root>/bin/uno` for similar results.

### Additional make commands

| Command         | Description                                                 |
|:----------------|:------------------------------------------------------------|
| `make lib`      | Build projects in `lib/` only (instead of a full build).    |
| `make release`  | Build in release configuration.                             |
| `make clean`    | Remove build artifacts.                                     |
| `make check`    | Run test suite.                                             |

### Additional npm commands

| Command         | Description                 |
|:----------------|:----------------------------|
| `npm pack`      | Build package tarball.      |
| `npm test`      | Run test suite.             |
| `npm version`   | Increment version number.   |

## Building from an IDE

Open `uno.sln` in Visual Studio 2022.

Make sure to set `tool/uno` as startup project when launching `uno`.

> See also `disasm.sln` (Windows only) and `runtime.sln` (multi-platform).

## Building the standard library

Try one of the following methods.

> Pass `doctor` as parameter to `tool/uno` when launching from your IDE.

> Run `<uno-root>/bin/uno doctor`.

> Run `npm run uno doctor`.

> Run `make lib`.
