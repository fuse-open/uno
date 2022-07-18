#pragma once
#ifdef __OBJC__
#include <UIKit/UIKit.h>
#if @(METAL:Defined)
#include <MetalANGLE/MGLKit.h>
#endif


@(AppDelegate.HeaderFile.Declaration:Join())


@interface uAppDelegate : UIViewController<@(AppDelegate.Implements:Join(', '))>
{
    uintptr_t primaryTouch;
}
#if @(METAL:Defined)
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
