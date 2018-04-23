#pragma once

#include <UIKit/UIKit.h>

@interface uWindow : UIWindow
@property (nonatomic, setter=uSetDisplay:) void* uDisplay;
@end

@interface UIWindow (uPlatform)
- (void)uSetDisplay:(void*)unoDisplay;
@end
