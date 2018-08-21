#include <Uno/Uno.h>

#include <libkern/OSByteOrder.h>
#include <objc/runtime.h>

#include <Foundation/Foundation.h>
#include <CoreGraphics/CoreGraphics.h>
#include <UIKit/UIKit.h>

#include <Uno-iOS/AppDelegate.h>
#include <Uno-iOS/Uno-iOS.h>

#include <uObjC.UnoObject.h>

namespace {

    static uAppDelegate *_appDelegate()
    {
        return (uAppDelegate*) [UIApplication sharedApplication].delegate;
    }

} // <anonymous> namespace

namespace uPlatform { namespace iOS {

    static const char UnoObjectAssociationKey = 0;

    void AssociateUnoObjectWeak(id nativeObject, uObject *unoObject)
    {
        id value = [[WeakUnoObject alloc] initWithUnoObject:unoObject];

        objc_setAssociatedObject(
            nativeObject, &UnoObjectAssociationKey, value,
            OBJC_ASSOCIATION_RETAIN);
    }

    uObject *GetAssociatedUnoObject(id nativeObject)
    {
        uObject *result = NULL;

        id<UnoObject> value = (id<UnoObject>)
            objc_getAssociatedObject(nativeObject, &UnoObjectAssociationKey);

        if (value)
            result = value.unoObject;

        return result;
    }

#if __BIG_ENDIAN__
    enum { NativeUTF16Encoding = NSUTF16BigEndianStringEncoding };
#else
    enum { NativeUTF16Encoding = NSUTF16LittleEndianStringEncoding };
#endif


    uString *ToUno(NSString *str)
    {
        if (!str)
            return NULL;

        NSUInteger bytes = [str
            lengthOfBytesUsingEncoding:NativeUTF16Encoding];

        uString *result = uString::New(int(bytes / sizeof(char16_t)));

        NSUInteger usedBytes = 0;
        if ([str
                getBytes:result->_ptr
                maxLength:bytes
                usedLength:&usedBytes
                encoding:NativeUTF16Encoding
                options:0
                range:NSMakeRange(0, [str length])
                remainingRange:NULL])
        {
            if (usedBytes != bytes) {
                result->_length = int(usedBytes / sizeof(char16_t));
                result->_ptr[result->_length] = 0;
            }
            return result;
        }

        return NULL;
    }
}} // namespace uPlatform::iOS
