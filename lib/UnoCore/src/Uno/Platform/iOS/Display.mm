#include <UIKit/UIKit.h>

#include <uno.h>
#include <Uno-iOS/AppDelegate.h>
#include <Uno-iOS/Uno-iOS.h>

#include <@{Uno.Platform.Displays:include}>
#include <@{Uno.Platform.TimerEventArgs:include}>
#include <@{Uno.Diagnostics.Clock:include}>

@implementation uDisplayTickNotifier
- (void)uOnDisplayTick:(CADisplayLink *)sender
{
    @{double} lastTickTimestamp = sender.timestamp;
    @{double} tickDuration = sender.duration * sender.frameInterval;

    uAutoReleasePool pool;
    double currentTime = @{Uno.Diagnostics.Clock.GetSeconds():call()};
    @{Uno.Platform.TimerEventArgs} args = @{Uno.Platform.TimerEventArgs(double,double,double):new(lastTickTimestamp,tickDuration, currentTime)};
    @{Uno.Platform.Displays.TickAll(Uno.Platform.TimerEventArgs):call(args)};
}
@end
