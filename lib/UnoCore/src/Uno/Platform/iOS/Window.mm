#include <uno.h>
#include <Uno-iOS/Uno-iOS.h>
#include <Window.h>
@{Uno.Platform.iOSDisplay:includeDirective}

@implementation uWindow
- (void)setFrame:(CGRect)frame
{
    [super setFrame:frame];

    if (self.uDisplay)
    {
        uAutoReleasePool pool;
        @{Uno.Platform.iOSDisplay:of(((@{Uno.Platform.iOSDisplay})self.uDisplay)).OnFrameChanged():call()};
    }
}
@end


@implementation UIWindow (uPlatform)
- (void)uSetDisplay:(void*)unoDisplay
{
}
@end
