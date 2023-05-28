#import <Foundation/NSObjCRuntime.h>
#import <Foundation/NSString.h>

void uLogApple(const char* prefix, const char* format_, va_list args)
{
    NSString* format = [NSString stringWithUTF8String:format_];
    NSLog(@"%@%@", [NSString stringWithUTF8String:prefix],
        [[NSString alloc] initWithFormat:format arguments:args]);
}
