#include <Uno-iOS/Context.h>
#include <Uno-iOS/Uno-iOS.h>
#include <uObjC.Foreign.h>
@{Uno.Platform.CoreApp:IncludeDirective}
@{Uno.Platform.iOS.Application:IncludeDirective}
@{Uno.Platform.EventSources.InterAppInvoke:IncludeDirective}
@(uContext.SourceFile.Declaration:Join())

@interface uContext()
{
@private
    uRuntime* _uno;
    EAGLContext* _glContext;
    UIWindow* (^_windowGetter)();
}
@end

@implementation uContext

@synthesize window = _window;
@synthesize glContext = _glContext;

struct NotificationSelector
{
    NSString* name;
    SEL selector;
};

const static NotificationSelector _notifications[] = {
    { UIApplicationDidFinishLaunchingNotification, @selector(applicationDidFinishLaunching:) },
    { UIApplicationWillEnterForegroundNotification, @selector(applicationWillEnterForeground:) },
    { UIApplicationDidBecomeActiveNotification, @selector(applicationDidBecomeActive:) },
    { UIApplicationWillResignActiveNotification, @selector(applicationWillResignActive:) },
    { UIApplicationDidEnterBackgroundNotification, @selector(applicationDidEnterBackground:) },
    { UIApplicationWillTerminateNotification, @selector(applicationWillTerminate:) },
    { UIApplicationDidReceiveMemoryWarningNotification, @selector(applicationDidReceiveMemoryWarning:) },
};

static uContext* instance = nil;
+ (instancetype)sharedContext
{
    if (instance == nil)
    {
        NSException* notInitializedException = [NSException
            exceptionWithName:@"Failed to get sharedContext"
            reason:@"uContext has not been initialized"
            userInfo: nil];
        @throw notInitializedException;
    }
    return instance;
}

+ (instancetype)initSharedContextWithWindow:(UIWindow*(^)())windowGetter
{
    if (instance == nil)
    {
        instance = [[uContext alloc] init];
        instance->_windowGetter = windowGetter;
    }
    return instance;
}

- (UIWindow*)window
{
    if (self->_windowGetter != nil)
    {
        return self->_windowGetter();
    }
    else
    {
        return nil;
    }
}

- (instancetype)init
{
    if (self = [super init])
    {
        _uno = new uRuntime();
        _glContext = [[EAGLContext alloc] initWithAPI:kEAGLRenderingAPIOpenGLES2];
        [EAGLContext setCurrentContext:_glContext];

        NSNotificationCenter* nc = [NSNotificationCenter defaultCenter];

        int num_notifications = sizeof(_notifications) / sizeof(_notifications[0]);

        for (int i = 0; i < num_notifications; ++i)
        {
            [nc addObserver:self
                selector:_notifications[i].selector
                name:_notifications[i].name
                object:nil];
        }
    }
    return self;
}

- (void)dealloc
{
    NSNotificationCenter* nc = [NSNotificationCenter defaultCenter];

    int num_notifications = sizeof(_notifications) / sizeof(_notifications[0]);

    for (int i = 0; i < num_notifications; ++i)
    {
        [nc removeObserver:self
            name:_notifications[i].name
            object:nil];
    }

    delete _uno;
}

- (BOOL)application:(UIApplication *)application willFinishLaunchingWithOptions:(NSDictionary *)launchOptions
{
    @{Uno.Platform.CoreApp.Start():Call()};
    @{Uno.Platform.iOS.Application.LaunchOptions:Set(launchOptions)};
    return YES;
}

- (BOOL)application:(UIApplication *)application openURL:(NSURL *)url sourceApplication:(NSString *)sourceApplication annotation:(id)annotation
{
    @{Uno.Platform.EventSources.InterAppInvoke.OnReceivedURI(string):Call([url absoluteString])};
    return YES;
}

- (BOOL)application:(UIApplication *)app openURL:(NSURL *)url options:(NSDictionary*)options
{
    @{Uno.Platform.EventSources.InterAppInvoke.OnReceivedURI(string):Call([url absoluteString])};
    return YES;
}

- (BOOL)application:(UIApplication *)application continueUserActivity:(NSURL *)url
{
    @{Uno.Platform.EventSources.InterAppInvoke.OnReceivedURI(string):Call([url absoluteString])};
    return YES;
}

- (void)applicationDidFinishLaunching:(NSNotification*)notification
{
    @{Uno.Platform.CoreApp.EnterForeground():Call()};
    @(uContext.SourceFile.DidFinishLaunching:Join('\n    '))
}

- (void)applicationWillEnterForeground:(NSNotification*)notification
{
    @{Uno.Platform.CoreApp.EnterForeground():Call()};
}

- (void)applicationDidBecomeActive:(NSNotification*)notification
{
    @{Uno.Platform.CoreApp.EnterInteractive():Call()};
}

- (void)applicationWillResignActive:(NSNotification*)notification
{
    @{Uno.Platform.CoreApp.ExitInteractive():Call()};
}

- (void)applicationDidEnterBackground:(NSNotification*)notification
{
    @{Uno.Platform.CoreApp.EnterBackground():Call()};
}

- (void)applicationWillTerminate:(NSNotification*)notification
{
    @{Uno.Platform.CoreApp.Terminate():Call()};
}

- (void)applicationDidReceiveMemoryWarning:(NSNotification*)notification
{
    @{Uno.Platform.CoreApp.OnReceivedLowMemoryWarning():Call()};
}

@end
