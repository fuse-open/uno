#include <Uno-iOS/SceneDelegate.h>

#if defined(__IPHONE_13_0) && __IPHONE_OS_VERSION_MAX_ALLOWED >= __IPHONE_13_0

#include <Uno-iOS/Context.h>

// Quick-action callbacks are injected into uAppDelegate by packages that
// handle shortcuts (see Fuse.Shortcut's appDelegate.sourceFile.implementationScope
// hooks). With the scene life cycle enabled UIKit no longer calls those app
// delegate methods directly, so the scene delegate forwards to them dynamically
// through this protocol, without requiring those packages at link time.
@protocol uSceneShortcutForwarding <NSObject>
- (void)application:(UIApplication *)application
    performActionForShortcutItem:(UIApplicationShortcutItem *)shortcutItem
                completionHandler:(void (^)(BOOL succeeded))completionHandler;
@end

@implementation uAppSceneDelegate

- (void)scene:(UIScene *)scene willConnectToSession:(UISceneSession *)session options:(UISceneConnectionOptions *)connectionOptions
{
    if (![scene isKindOfClass:[UIWindowScene class]])
        return;

    // The uno window is created by uAppDelegate before any scene exists; attach
    // it to the UIWindowScene UIKit created for us. This is required on iOS 13+
    // when UIApplicationSceneManifest is present, and mandatory for apps linked
    // against the iOS 26 SDK and newer.
    UIWindow* window = [[uContext sharedContext] window];
    if (window != nil)
    {
        if (@available(iOS 13.0, *))
        {
            window.windowScene = (UIWindowScene*)scene;
            [window makeKeyAndVisible];
        }
    }

    // With the scene life cycle enabled UIKit no longer delivers launch items
    // to the app delegate; forward them here instead.
    for (UIOpenURLContext* urlContext in connectionOptions.URLContexts)
        [[uContext sharedContext] openURI:urlContext.URL];

    for (NSUserActivity* activity in connectionOptions.userActivities)
    {
        if ([activity.activityType isEqualToString:NSUserActivityTypeBrowsingWeb] &&
            activity.webpageURL != nil)
            [[uContext sharedContext] openURI:activity.webpageURL];
    }

    // A quick action that launched the app; UIKit will not deliver it again
    // through windowScene:performActionForShortcutItem:completionHandler:.
    if (connectionOptions.shortcutItem != nil)
        [self handleShortcutItem:connectionOptions.shortcutItem
               completionHandler:^(BOOL succeeded) { }];
}

- (void)scene:(UIScene *)scene openURLContexts:(NSSet<UIOpenURLContext *> *)URLContexts
{
    for (UIOpenURLContext* urlContext in URLContexts)
        [[uContext sharedContext] openURI:urlContext.URL];
}

- (void)scene:(UIScene *)scene continueUserActivity:(NSUserActivity *)userActivity
{
    if ([userActivity.activityType isEqualToString:NSUserActivityTypeBrowsingWeb] &&
        userActivity.webpageURL != nil)
        [[uContext sharedContext] openURI:userActivity.webpageURL];
}

- (void)windowScene:(UIWindowScene *)windowScene performActionForShortcutItem:(UIApplicationShortcutItem *)shortcutItem completionHandler:(void (^)(BOOL succeeded))completionHandler
{
    [self handleShortcutItem:shortcutItem completionHandler:completionHandler];
}

- (void)handleShortcutItem:(UIApplicationShortcutItem *)shortcutItem
         completionHandler:(void (^)(BOOL succeeded))completionHandler
{
    id delegate = [UIApplication sharedApplication].delegate;
    if ([delegate respondsToSelector:@selector(application:performActionForShortcutItem:completionHandler:)])
    {
        id<uSceneShortcutForwarding> target = (id<uSceneShortcutForwarding>)delegate;
        [target application:[UIApplication sharedApplication]
            performActionForShortcutItem:shortcutItem
                       completionHandler:completionHandler];
    }
    else
        completionHandler(YES);
}

- (void)sceneDidDisconnect:(UIScene *)scene
{
}

- (void)sceneDidBecomeActive:(UIScene *)scene
{
}

- (void)sceneWillResignActive:(UIScene *)scene
{
}

- (void)sceneWillEnterForeground:(UIScene *)scene
{
}

- (void)sceneDidEnterBackground:(UIScene *)scene
{
}

@end

#endif