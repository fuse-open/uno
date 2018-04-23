#include <Uno/Uno.h>
#include <Uno-iOS/Uno-iOS.h>
#include <Window.h>
@{Uno.Platform.iOSDisplay:IncludeDirective}

@implementation uWindow
- (void)setFrame:(CGRect)frame
{
    [super setFrame:frame];

    if (self.uDisplay)
    {
        uAutoReleasePool pool;
        @{Uno.Platform.iOSDisplay:Of(((@{Uno.Platform.iOSDisplay})self.uDisplay)).OnFrameChanged():Call()};
    }
}
@end


@implementation UIWindow (uPlatform)
- (void)uSetDisplay:(void*)unoDisplay
{
}
@end
