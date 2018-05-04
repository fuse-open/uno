# Uno AppLoader for macOS

This project is used for loading uno apps targeting dotnet on macOS.

## Mono stub

To make it possible to compile this project on other platforms than macOS we
have included a `monostub` executable, prebuilt from `monostub.m`.

```sh
clang -m64 monostub.m -o monostub -framework AppKit -mmacosx-version-min=10.6
```

When making changes to `monostub.m` be sure to also recompile it, as this won't
happen automatically. Consequently, any commit modifying `monostub.m` should also
contain an updated `monostub` binary.
