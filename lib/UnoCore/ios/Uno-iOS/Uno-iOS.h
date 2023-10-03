#pragma once

@{Uno.Byte:includeDirective}
@{Uno.Int2:includeDirective}
@{Uno.Float2:includeDirective}
@{Uno.Rect:includeDirective}
@{Uno.Recti:includeDirective}

#include <objc/objc.h>
#include <CoreGraphics/CoreGraphics.h>

#if __OBJC__
#define uOBJC_CLASS @class
#else
#define uOBJC_CLASS struct
#endif

uOBJC_CLASS CADisplayLink;
uOBJC_CLASS UIView;
uOBJC_CLASS UIWindow;

#if __OBJC__
#define U_FORWARD_DECLARE_OBJC_CLASS @class
#else
#define U_FORWARD_DECLARE_OBJC_CLASS struct
#endif

U_FORWARD_DECLARE_OBJC_CLASS NSString;
U_FORWARD_DECLARE_OBJC_CLASS UIImage;
U_FORWARD_DECLARE_OBJC_CLASS UIView;

namespace uPlatform { namespace iOS {

    void AssociateUnoObjectWeak(id nativeObject, uObject *unoObject);
    uObject *GetAssociatedUnoObject(id nativeObject);
    uString *ToUno(NSString *);

}}
