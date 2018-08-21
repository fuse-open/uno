#pragma once

#include <Foundation/Foundation.h>

struct uObject;

@protocol UnoObject <NSObject>
@property (nonatomic) uObject* unoObject;
- (id)initWithUnoObject:(uObject*)object;
@end


/// Holds a strong reference to a Uno object, it's released on dealloc.
@interface StrongUnoObject : NSObject <UnoObject, NSCopying>
+ (instancetype)strongUnoObjectWithUnoObject:(uObject*)object;
- (id)initWithUnoObject:(uObject*)object NS_DESIGNATED_INITIALIZER;
- (id)init;
@end


/// Holds a (nullable) weak reference to a Uno object.
@interface WeakUnoObject : NSObject <UnoObject, NSCopying>
+ (instancetype)weakUnoObjectWithUnoObject:(uObject*)object;
- (id)initWithUnoObject:(uObject*)object NS_DESIGNATED_INITIALIZER;
- (id)init;
@end


/// Holds a naked pointer to a Uno object.
@interface UnsafeUnoObject : NSObject <UnoObject, NSCopying>
+ (instancetype)unsafeUnoObjectWithUnoObject:(uObject*)object;
- (id)initWithUnoObject:(uObject*)object NS_DESIGNATED_INITIALIZER;
- (id)init;
@end
