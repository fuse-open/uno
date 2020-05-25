// We include a precompiled monostub to make compilation of solution possible on non-mac platforms,
// so if this file is modified, it needs to be manually recompiled:
//     clang -m64 monostub.m -o monostub -framework AppKit -mmacosx-version-min=10.6
#import <Foundation/Foundation.h>
#include <dlfcn.h>

typedef int (* mono_main) (int argc, const char **argv);

int main(int argc, char **argv)
{
    NSAutoreleasePool *pool = [[NSAutoreleasePool alloc] init];

    // Find executable to be instantiated by mono
    NSString *appDir = [[NSBundle mainBundle] bundlePath];
    NSString *monoBundleDir = [appDir stringByAppendingPathComponent: @"Contents/MonoBundle"];
    NSString *currentExeName = [[NSString stringWithUTF8String: argv[0]] lastPathComponent];
    NSString *monoExecutable = [monoBundleDir stringByAppendingPathComponent: [NSString stringWithFormat: @"%@.exe", currentExeName]];

    // Default Mono location
    NSString *monoRoot = @"/Library/Frameworks/Mono.framework/Versions/Current";

    // Look for Mono installed by the dotnet-run package
    NSFileManager *fileManager = [NSFileManager defaultManager];
    NSString *monoLink = [NSHomeDirectory() stringByAppendingPathComponent: @".dotnet-run/.bin/mono"];

    if ([fileManager fileExistsAtPath:monoLink])
    {
        NSString *monoResolved = [[[NSURL fileURLWithPath:monoLink] URLByResolvingSymlinksInPath] path];

        if ([fileManager fileExistsAtPath:monoResolved])
            monoRoot = [[monoResolved stringByDeletingLastPathComponent] stringByDeletingLastPathComponent];
    }

    // Load mono
    NSString *libmonosgen = [monoRoot stringByAppendingPathComponent: @"lib/libmonosgen-2.0.dylib"];
    void *libMono = dlopen([libmonosgen UTF8String], RTLD_LAZY);
    if (libMono == nil)
    {
        NSLog(@"Failed to load '%@': %s.", libmonosgen, dlerror());
        return 1;
    }

    mono_main _mono_main = (mono_main) dlsym (libMono, "mono_main");
    if (!_mono_main)
    {
        NSLog(@"Could not load mono_main(): %s.", dlerror());
        return 1;
    }

    // Inject arguments
    int nargc = argc + 1;
    const char **newArgs = (const char**)malloc(sizeof(char)*nargc);
    int i = 0;

    newArgs[i++] = argv[0];
    newArgs[i++] = [monoExecutable UTF8String];
    for(int j = 1; j < argc; ++j)
        newArgs[i++] = argv[j];

    int result = _mono_main(nargc, newArgs);
    [pool drain];
    return result;
}