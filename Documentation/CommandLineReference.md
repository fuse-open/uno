# Command Line Reference

## $ uno
```
Usage: uno COMMAND [args ...]
  or   uno COMMAND --help
  or   uno --version

Global options
  -h, -?, --help        Get help
  -v, -vv, -vvv         Increment verbosity level
  -x, --experimental    Enable experimental features
      --(no-)anim       Enable or disable terminal animations
      --trace           Print exception stack traces where available

Available commands
  create         Create a new project file
  update         Update packages and project(s)
  build          Build a project for given target
  clean          Delete generated build and cache directories in project(s)
  test           Run unit test project(s) and print results
  doctor         Repair/rebuild packages found in search paths
  config         Print information about your Uno environment
  ls             Print project items found to STDOUT

Experimental commands
  no-build       Invoke generated build steps without triggering a build
  test-gen       Generate compilation tests
  install        Install Uno package(s) to the local cache
  uninstall      Uninstall Uno package(s) from the local cache
  feed           Maintain Uno package feeds
  pack           Build project(s) and make Uno package(s)
  push           Upload Uno package(s) to remote server
  stuff          Built-in simple package manager
  lint           Parses uno source files and output syntax errors
  adb            Use Android Debug Bridge (adb)
  android        Open Deprecated Android SDK Manager
  sdkmanager     Interact with Android's CLI SDK Manager
  launch-apk     Deploy and start APK on a connected device
  open           Open file(s) in external application

Environment variables
  LOG_LEVEL=<0..3>  Set verbosity level (default = 0)
  LOG_TRACE=1       Print exception stack traces where available
  DEBUG_GL=1        Enable the OpenGL debug layer in .NET builds
```

## $ uno create
```
Usage: uno create [options] [file|directory]

Create a new project file.

Available options
  -c, --class=NAME      Initialize project with an empty class with this name [optional]
  -n, --name=NAME       Specify project file name [optional]
  -d, --defaults        Add default settings
  -e, --empty           Create empty project without packages or items
  -f, --force           Overwrite any existing project without warning
      --flatten         Flatten items to explicit list of files
```

## $ uno update
```
Usage: uno update [options] [project-path ...]

Update packages and project(s).

Example
  uno update -pr --files    Update projects recursively, adding new files

Common options
      (default)           Install/update packages
  -p, --project           Update project file(s)
  -r, --recursive         Look for project files recursively

Project options
  -d, --defaults          Add default values
  -s, --strip             Remove properties with default values
  -c, --clear             Remove all include items
  -e, --exclude           Remove include items that are exluded
  -f, --files             Scan project directory for new files to add/remove
  -g, --glob=PATTERN      Scan project directory for matching files to add/remove
      --flatten           Flatten items to explicit list of files
      --dry-run           Don't save updated project(s)
```

## $ uno build
```
Usage: uno build [target] [options] [project-path]

Build a project for given target.

Examples
  uno build android           Build Android app, in current directory
  uno build ios --run         Build & run iOS app, in current directory
  uno build native --debug    Build & open Visual C++ or Xcode, if available

Common options
  -c, --configuration=STRING  Build configuration [Debug|Release]
  -t, --target=STRING         Build target (see: Available build targets)
  -d, --debug                 Open IDE for debugging after successful build
  -r, --run                   Start the program after successful build
  -z, --clean                 Clean the output directory before building
  -v, -vv, -vvv               Increment verbosity level

Additional options
  -n, --native-args=STRING    Arguments to native build command
  -a, --run-args=STRING       Arguments to run command
  -m, --main=STRING           Override application entrypoint
  -s, --set:NAME=STRING       Override build system property
  -o, --out-dir=PATH          Override output directory
  -f, --force                 Build even if output is up-to-date
  -l, --libs                  Rebuild package library if necessary
  -p, --print-internals       Print a list of build system properties
  -N, --no-native             Disable native build step (faster)
  -P, --no-parallel           Disable multi-threading (slower)
  -S, --no-strip              Disable removal of unused code (slower)

Compiler options
  -D, --define=STRING         Add define, to enable a feature
  -U, --undefine=STRING       Remove define, to disable a feature
  -E, --max-errors=NUMBER     Set max error count (0 = disable)
  -W<0..3>                    Set warning level (0 = disable)

C++ options
  -DREFLECTION                Enable run-time type reflection
  -DSTACKTRACE                Enable stack traces on Exception
  -DDEBUG_UNSAFE              Enable C++ asserts in unsafe code
  -DDEBUG_NATIVE              Disable C++ optimizations when debugging
  -DDEBUG_ARC<0..4>           Log events from ARC/memory management
  -DDEBUG_DUMPS               Dump GraphViz files to help identify cycles in memory

GLSL options
  -DDUMP_SHADERS              Dump shaders to build directory for inspection

Testing options (-x)
  --test                      Generate testable code
  --test-server-url=URL       URL to test server
  --test-list-file=PATH       File to write list of discovered tests

Available build targets
  * android            C++/JNI/GLES2 code and APK. Runs on device.
  * native             C++/GL code, CMake project and native executable.
  * ios        (-x)    (Objective-)C++/GLES2 code and Xcode project. (macOS only)
  * dotnet             .NET/GL bytecode and executable. (default)
  * corelib    (-x)    C# implementation of Uno corelib.
  * docs       (-x)    Uno documentation files.
  * pinvoke    (-x)    PInvoke libraries.
  * package    (-x)    Uno package files.
```

## $ uno no-build
```
Usage: uno no-build [target] [options] [project-path]

Invoke generated build steps without triggering a build.

Common options
  -c, --configuration=STRING  Build configuration [Debug|Release|Preview]
  -o, --out-dir=PATH          Specify output directory [optional]
  -b, --build                 Execute native build command
  -d, --debug                 Open IDE for debugging
  -r, --run                   Start the program
  -v, -vv, ...                Increment verbosity level

Additional options
  -n, --native-args=ARGS      Arguments to native build command
  -a, --run-args=ARGS         Arguments to run command
  -t, --target=STRING         Build target (see: uno build --help)
```

## $ uno clean
```
Usage: uno clean [options] [project-path ...]

Delete generated build and cache directories in project(s).

Available options
  -r, --recursive       Look for project files recursively
```

## $ uno test
```
Usage: uno test [options] [paths-to-search]

[paths-to-search] is a list of paths to unoprojs to run tests from, and/or
directories in which to search for test projects.
When a directory is given, uno test searches recursively in that directory
for projects named '*Test.unoproj'

Available options:
  -h, -?, --help             Show help
  -r, --reporter=VALUE       Reporter type. teamcity|console
  -l, --logfile=VALUE        Write output to this file instead of stdout
  -t, --target=VALUE         Build target. Currently supports DotNet|WebGL|
                               Android|CMake
      --webgl-browser=VALUE  Location of browser (only used for webgl targets,
                               defaults to OS default browser)
  -v, --verbose              Verbose, always prints output from compiler and
                               debug_log
  -q, --quiet                Quiet, only prints output from compiler and debug_
                               log in case of errors.
  -f, --filter=VALUE         Only run tests matching this string
  -e, --regex-filter=VALUE   Only run tests matching this regular expression
  -o, --timeout=VALUE        Timeout for individual tests (in seconds)
      --startup-timeout=VALUE
                             Timeout for connection from uno process (in
                               seconds)
      --trace                Print trace information from unotest
      --allow-debugger       Don't run compiled program, allow user to start it
                               from a debugger.
  -d, --debug                Open IDE for debugging tests.
      --run-local            Run the test directly, without using HTTP
      --no-uninstall         Don't uninstall tests after running on device
  -D, --define=VALUE         Add define, to enable a feature
  -U, --undefine=VALUE       Remove define, to disable a feature
      --output-dir=VALUE     Override output directory
      --libs                 Rebuild package library if necessary

Examples:
  uno test
  uno test Path\Projects
  uno test Path\Projects\FooTest.unoproj Path\Projects\BarTest.unoproj
  uno test Path\Projects Path\OtherProjects\FooTest.unoproj
  uno test -t=dotnet -r=teamcity -v Path\Projects
```

## $ uno test-gen
```
Usage: uno test-gen <path to packages> <path for temporary project> [--exclude=name]
```

## $ uno doctor
```
Usage: uno doctor [options] [package ...]

Repair/rebuild packages found in search paths.

Available options
  -a, --all                    Build all projects, regardless of modification time
  -f, --force                  Update all package caches, regardless of modification time
  -e, --express                Express mode. Don't rebuild packages depending on a modified package
  -z, --clean                  Clean projects before building them
  -c, --configuration=NAME     Set build configuration (Debug|Release) [optional]
  -b, --build-number=VERSION   Override version for all packages built [optional]
  -C, --no-cache               Disable in-memory AST & IL caches
  -P, --no-parallel            Disable multi-threading
  -s, --silent                 Very quiet build log
```

## $ uno install
```
Usage: uno install <package ...> [version] [source]
  or   uno install <project-file|directory>
  or   uno install <package-list-file>
  or   uno install <upk-file ...>

Install Uno package(s) to the local cache.

Install options
  -d, --directory=PATH  Specify a custom install directory [optional]
  -n, --version=STRING  Install a specific version of <package>
  -s, --source=URL      Install <package> from a specific source
  -f, --force           Install a package even if already installed
  -P, --no-parallel     Disable multi-threading
```

## $ uno uninstall
```
Usage: uno uninstall <package|wildcard ...> [version]

Uninstall Uno package(s) from the local cache.

Uninstall options
  -n, --version=STRING  Uninstall a specific version of <package>
  -f, --force           Uninstall all matching packages without warning
```

## $ uno feed
```
Usage: uno feed [package ...] [version] [source]
  or   uno feed [package ...] [version] --installed

Maintain Uno package feeds.

Example
  uno feed UnoCore 1.0.0        Look up 'UnoCore' version 1.0.0

Filter options
      (default)             Show packages from all available feeds
  -i, --installed           Show installed or built packages
  -s, --source=URL          Show packages from a remote source
  -n, --version=STRING      Show only packages of this version
```

## $ uno pack
```
Usage: uno pack [options] [project-path ...]

Build project(s) and make Uno package(s).

Available options
  -o, --out-dir=PATH        Specify output directory [optional]
  -b, --build-dir=PATH      Specify build directory [optional]
  -n, --version=STRING      Specify package version [optional]
  -s, --suffix=STRING       Specify package suffix [optional]
  -r, --recursive           Look for project files recursively
```

## $ uno push
```
Usage: uno push <options> <upk-file ...>

Upload Uno package(s) to remote server.

Upload options
  -k, --key=STRING      Token to validate against a remote API
  -s, --server=URL      URL to a remote Uno package server
  -t, --timeout=NUMBER  Time out after [600] seconds [optional]
  -o, --out=FILE        Save a list of packages to FILE [optional]
  -U, --no-upload       Don't push anything to server
```

## $ uno stuff
```
Usage: uno stuff COMMAND [args]
  or   uno stuff --version

Available commands
  install     Downloads and installs stuff in .STUFF file(s)
  clean       Removes stuff installed by .STUFF file(s)
  pack        Builds .ZIP and .STUFF-UPLOAD file(s)
  push        Uploads .STUFF-UPLOAD file(s) to a remote server
  gc          Deletes files older than specified number of days from the download cache
  sln         Generates C# .SLN file(s)
  symlink     Finds duplicate files in a directory tree and replaces them with symlinks

Type "uno stuff COMMAND --help" to get instructions for a command
```

## $ uno config
```
Usage: uno config [property ...]
  or   uno config [options]

Print information about your Uno environment.

Available options
  -a, --asm             Print .NET assemblies
  -s, --system          Print system settings
  -v                    Print everything
```

## $ uno ls
```
Usage: uno ls [options] [project-path ...]

Print project items found to STDOUT.

Available options
  -r, --recursive       Look for project files recursively
  -p, --projects        List project references instead of files
  -F, --no-files        Don't list files included in project(s)
```

## $ uno lint
```
Usage: uno lint [source-path ...]

Parses uno source files and output syntax errors.
```

## $ uno adb
```
Usage: uno adb [arguments ...]

Use Android Debug Bridge (adb).
This commands forwards given arguments to 'adb', which is a tool included in the Android SDK.

Type 'uno adb' to see what's available.
```

## $ uno android
```
Usage: uno android

Open Deprecated Android SDK Manager.
```

## $ uno sdkmanager
```
Usage: uno sdkmanager [arguments ...]

Interact with Android's CLI SDK Manager.
```

## $ uno launch-apk
```
Usage: uno launch-apk [options] <filename>

Deploy and start APK on a connected device.

Available options
  -a, --activity=NAME   Android activity name
  -p, --package=NAME    Java package name
  -s, --sym-dir=PATH    Symbol directory, for stack traces [optional]
  -i, --install         Install only, then exit
  -C, --no-clear        Don't clear logcat logs before launch
  -L, --no-log          Don't run logcat, just launch
```

## $ uno open
```
Usage: uno open [options] <filename ...>

Open file(s) in external application.

Available options
  -a, --app=NAME        The name of the application to open
  -a, --exe=PATH        The path to the executable to open [optional]
  -t, --title=NAME      Look for an existing window with this title [optional]
  -n, --new             Create a new process
```
