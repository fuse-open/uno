#include <Uno-iOS/AppDelegate.h>
#include <Uno-iOS/Context.h>
#include <Window.h>
@(appDelegate.sourceFile.declaration:join())

@interface uAppDelegate()
{
@private
    uContext* _unoContext;
}
@end

@implementation uAppDelegate
- (id)init
{
    if ((self = [super init]))
    {
        CGRect screenBounds = [UIScreen mainScreen].bounds;
        uWindow* window = [[uWindow alloc] initWithFrame:screenBounds];
        _unoContext = [uContext initSharedContextWithWindow:^UIWindow* () { return window; }];
        _context = [_unoContext glContext];
    }
    return self;
}

- (UIWindow*)window {
    return [[uContext sharedContext] window];
}

- (void)setView:(UIView *)view
{
    [super setView:view];
}

- (BOOL)application:(UIApplication *)application willFinishLaunchingWithOptions:(NSDictionary *)launchOptions
{
#if @(METAL:defined)
    [MGLContext setCurrentContext:self.context];
#else
    [EAGLContext setCurrentContext:self.context];
#endif
    return [_unoContext
        application:application
        willFinishLaunchingWithOptions:launchOptions];
}

-(BOOL)application:(UIApplication *)application openURL:(NSURL *)url sourceApplication:(NSString *)sourceApplication annotation:(id)annotation
{
    BOOL result = [_unoContext
        application:application
        openURL:url
        sourceApplication:sourceApplication
        annotation:annotation];
    @(appDelegate.sourceFile.openURL:join('\n    '));
    return result;
}

- (BOOL)application:(UIApplication *)app openURL:(NSURL *)url options:(NSDictionary*)options
{
    BOOL result = [_unoContext
        application:app
        openURL:url
        options:options];
    @(appDelegate.sourceFile.openURLLegacy:join('\n    '));
    return result;
}

- (BOOL)application:(UIApplication *)application didFinishLaunchingWithOptions:(NSDictionary *)launchOptions
{
    uAutoReleasePool pool;
    @(appDelegate.sourceFile.didFinishLaunchingWithOptions:join('\n    '))
    return YES;
}

#if defined(__IPHONE_13_0) && __IPHONE_OS_VERSION_MAX_ALLOWED >= __IPHONE_13_0
-(BOOL)application:(UIApplication *)application continueUserActivity:(NSUserActivity *)userActivity restorationHandler:(void (^)(NSArray<id<UIUserActivityRestoring>> * __nullable restorableObjects))restorationHandler
#else
-(BOOL)application:(UIApplication *)application continueUserActivity:(NSUserActivity *)userActivity restorationHandler:(void (^)(NSArray * _Nullable))restorationHandler
#endif
{

    if ([userActivity.activityType isEqualToString: NSUserActivityTypeBrowsingWeb]) {
        NSURL *url = userActivity.webpageURL;

#if @(METAL:defined)
        [MGLContext setCurrentContext:self.context];
#else
        [EAGLContext setCurrentContext:self.context];
#endif
        return [_unoContext
                application:application
                continueUserActivity:url];
    }
    return YES;
}


#if @(APPDELEGATE.PUSHNOTIFICATIONMETHODS:defined)
- (void)application:(UIApplication *)application didRegisterForRemoteNotificationsWithDeviceToken:(NSData *)deviceToken
{
    id cxt = _unoContext;
    if ([cxt respondsToSelector:@selector(application:didRegisterForRemoteNotificationsWithDeviceToken:)])
        [cxt application:application didRegisterForRemoteNotificationsWithDeviceToken:deviceToken];
}

- (void)application:(UIApplication *)application didFailToRegisterForRemoteNotificationsWithError:(NSError *)error
{
    id cxt = _unoContext;
    if ([cxt respondsToSelector:@selector(application:didFailToRegisterForRemoteNotificationsWithError:)])
        [cxt application:application didFailToRegisterForRemoteNotificationsWithError:error];
}

- (void)application:(UIApplication *)application didReceiveRemoteNotification:(NSDictionary *)userInfo
{
    id cxt = _unoContext;
    if ([cxt respondsToSelector:@selector(application:didReceiveRemoteNotification:)])
        [cxt application:application didReceiveRemoteNotification:userInfo];
}

- (void)application:(UIApplication *)application dispatchPushNotification:(NSDictionary *)userInfo fromBar:(BOOL)fromBar
{
    id cxt = _unoContext;
    if ([cxt respondsToSelector:@selector(application:dispatchPushNotification:fromBar:)])
        [cxt application:application dispatchPushNotification:userInfo fromBar:fromBar];
}
#endif

#if @(APPDELEGATE.LOCALNOTIFICATIONMETHODS:defined)
- (void)application:(UIApplication *)application didReceiveLocalNotification:(UILocalNotification *)notification
{
    id cxt = _unoContext;
    if ([cxt respondsToSelector:@selector(application:didReceiveLocalNotification:)])
        [cxt application:application didReceiveLocalNotification:notification];
}
#endif

@(appDelegate.sourceFile.implementationScope:join())

@end
