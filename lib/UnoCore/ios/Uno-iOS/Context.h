#pragma once
#ifdef __OBJC__
#include <UIKit/UIKit.h>
#if @(METAL:defined)
#include <MetalANGLE/MGLKit.h>
#else
#include <OpenGLES/EAGL.h>
#endif

@interface uContext : NSObject

+ (instancetype)sharedContext;
+ (instancetype)initSharedContextWithWindow:(UIWindow*(^)())windowGetter;

- (BOOL)application:(UIApplication *)application willFinishLaunchingWithOptions:(NSDictionary *)launchOptions;
- (BOOL)application:(UIApplication *)application openURL:(NSURL *)url sourceApplication:(NSString *)sourceApplication annotation:(id)annotation;
- (BOOL)application:(UIApplication *)app openURL:(NSURL *)url options:(NSDictionary*)options;
- (BOOL)application:(UIApplication *)application continueUserActivity:(NSURL *)url;

// Delivers an incoming URI to the uno app, raising InterAppInvoke.OnReceivedURI.
// Used by the app delegate's openURL/continueUserActivity callbacks and by the
// scene delegate, since with the UIScene life cycle enabled UIKit no longer
// routes URL opens and universal links to the app delegate
// (see scene:openURLContexts: and scene:continueUserActivity:).
- (void)openURI:(NSURL *)url;

@property (readonly) UIWindow* window;
#if @(METAL:defined)
@property (readonly) MGLContext *glContext;
#else
@property (readonly) EAGLContext *glContext;
#endif

@end

#endif
