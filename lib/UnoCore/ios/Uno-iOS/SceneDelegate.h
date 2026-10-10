#pragma once
#ifdef __OBJC__
#include <UIKit/UIKit.h>

#if defined(__IPHONE_13_0) && __IPHONE_OS_VERSION_MAX_ALLOWED >= __IPHONE_13_0

// Attaches the uno window (created by uAppDelegate, before scenes exist) to
// the UIWindowScene UIKit created for us. This is required on iOS 13+ when
// UIApplicationSceneManifest is present, and mandatory for apps linked
// against the iOS 26 SDK and newer.
@interface uAppSceneDelegate : UIResponder <UIWindowSceneDelegate>
@end

#endif
#endif
