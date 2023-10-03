#include <Uno-iOS/Context.h>
#include <Uno-iOS/Uno-iOS.h>
#include <uObjC.Foreign.h>
@{Uno.Platform.CoreApp:includeDirective}
@{Uno.Platform.iOS.Application:includeDirective}
@{Uno.Platform.EventSources.InterAppInvoke:includeDirective}
@(uContext.sourceFile.declaration:join())

@interface uContext()
{
@private
    uRuntime* _uno;
#if @(METAL:defined)
    MGLContext* _glContext;
#else
    EAGLContext* _glContext;
#endif
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
#if @(METAL:defined)
        _glContext = [[MGLContext alloc] initWithAPI:kMGLRenderingAPIOpenGLES2];
        [MGLContext setCurrentContext:_glContext];
#else
        _glContext = [[EAGLContext alloc] initWithAPI:kEAGLRenderingAPIOpenGLES2];
        [EAGLContext setCurrentContext:_glContext];
#endif

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
    @{Uno.Platform.CoreApp.Start():call()};
    @{Uno.Platform.iOS.Application.LaunchOptions:set(launchOptions)};
    return YES;
}

- (BOOL)application:(UIApplication *)application openURL:(NSURL *)url sourceApplication:(NSString *)sourceApplication annotation:(id)annotation
{
    @{Uno.Platform.EventSources.InterAppInvoke.OnReceivedURI(string):call([url absoluteString])};
    return YES;
}

- (BOOL)application:(UIApplication *)app openURL:(NSURL *)url options:(NSDictionary*)options
{
    @{Uno.Platform.EventSources.InterAppInvoke.OnReceivedURI(string):call([url absoluteString])};
    return YES;
}

- (BOOL)application:(UIApplication *)application continueUserActivity:(NSURL *)url
{
    @{Uno.Platform.EventSources.InterAppInvoke.OnReceivedURI(string):call([url absoluteString])};
    return YES;
}

- (void)applicationDidFinishLaunching:(NSNotification*)notification
{
    @{Uno.Platform.CoreApp.EnterForeground():call()};
    @(uContext.sourceFile.didFinishLaunching:join('\n    '))
}

- (void)applicationWillEnterForeground:(NSNotification*)notification
{
    @{Uno.Platform.CoreApp.EnterForeground():call()};
}

- (void)applicationDidBecomeActive:(NSNotification*)notification
{
    @{Uno.Platform.CoreApp.EnterInteractive():call()};
}

- (void)applicationWillResignActive:(NSNotification*)notification
{
    @{Uno.Platform.CoreApp.ExitInteractive():call()};
}

- (void)applicationDidEnterBackground:(NSNotification*)notification
{
    @{Uno.Platform.CoreApp.EnterBackground():call()};
}

- (void)applicationWillTerminate:(NSNotification*)notification
{
    @{Uno.Platform.CoreApp.Terminate():call()};
}

- (void)applicationDidReceiveMemoryWarning:(NSNotification*)notification
{
    @{Uno.Platform.CoreApp.OnReceivedLowMemoryWarning():call()};
}

@end
