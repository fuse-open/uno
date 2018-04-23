#pragma once
#ifdef __OBJC__
#include <UIKit/UIKit.h>
#include <OpenGLES/EAGL.h>

@interface uContext : NSObject

+ (instancetype)sharedContext;
+ (instancetype)initSharedContextWithWindow:(UIWindow*(^)())windowGetter;

- (BOOL)application:(UIApplication *)application willFinishLaunchingWithOptions:(NSDictionary *)launchOptions;
- (BOOL)application:(UIApplication *)application openURL:(NSURL *)url sourceApplication:(NSString *)sourceApplication annotation:(id)annotation;
- (BOOL)application:(UIApplication *)app openURL:(NSURL *)url options:(NSDictionary*)options;

@property (readonly) UIWindow* window;
@property (readonly) EAGLContext *glContext;

@end

#endif
