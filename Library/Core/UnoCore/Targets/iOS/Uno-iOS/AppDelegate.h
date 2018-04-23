#pragma once
#ifdef __OBJC__
#include <UIKit/UIKit.h>


@(AppDelegate.HeaderFile.Declaration:Join())


@interface uAppDelegate : UIViewController<@(AppDelegate.Implements:Join(', '))>
{
    uintptr_t primaryTouch;
}
@property (strong, nonatomic) EAGLContext *context;
@end


@interface uAppDelegate (TouchEvents)
@end


@interface uDisplayTickNotifier : NSObject
- (void)uOnDisplayTick:(CADisplayLink *)sender;
@end


#endif
