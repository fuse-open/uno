#pragma once
#ifdef __OBJC__
#include <UIKit/UIKit.h>
#if @(METAL:defined)
#include <MetalANGLE/MGLKit.h>
#endif


@(appDelegate.headerFile.declaration:join())


@interface uAppDelegate : UIViewController<@(appDelegate.implements:join(', '))>
{
    uintptr_t primaryTouch;
}
#if @(METAL:defined)
@property (strong, nonatomic) MGLContext *context;
#else
@property (strong, nonatomic) EAGLContext *context;
#endif
@end


@interface uAppDelegate (TouchEvents)
@end


@interface uDisplayTickNotifier : NSObject
- (void)uOnDisplayTick:(CADisplayLink *)sender;
@end


#endif
