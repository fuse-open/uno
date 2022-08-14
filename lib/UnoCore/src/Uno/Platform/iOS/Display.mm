#include <UIKit/UIKit.h>

#include <Uno/Uno.h>
#include <Uno-iOS/AppDelegate.h>
#include <Uno-iOS/Uno-iOS.h>

#include <@{Uno.Platform.Displays:Include}>
#include <@{Uno.Platform.TimerEventArgs:Include}>
#include <@{Uno.Diagnostics.Clock:Include}>

@implementation uDisplayTickNotifier
- (void)uOnDisplayTick:(CADisplayLink *)sender
{
    @{double} lastTickTimestamp = sender.timestamp;
    @{double} tickDuration = sender.duration * sender.frameInterval;

    uAutoReleasePool pool;
    double currentTime = @{Uno.Diagnostics.Clock.GetSeconds():Call()};
    @{Uno.Platform.TimerEventArgs} args = @{Uno.Platform.TimerEventArgs(double,double,double):New(lastTickTimestamp,tickDuration, currentTime)};
    @{Uno.Platform.Displays.TickAll(Uno.Platform.TimerEventArgs):Call(args)};
}
@end
