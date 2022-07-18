#pragma once

#include <Uno/Uno.h>
#include <Foundation/Foundation.h>

@protocol UnoArray <NSObject>
@property (nonatomic) uArray* unoArray;
- (id)initWithUnoArray:(uArray*)array
    getAt:(id (^) (uArray*, int))getAt
    setAt:(void (^) (uArray*, int, id))setAt;
- (id)init;
- (id)objectAtIndexedSubscript:(int)idx;
- (void)setObject:(id)obj atIndexedSubscript:(int)idx;
- (NSUInteger)count;
- (NSArray*)copyArray;
@end

@interface StrongUnoArray : NSObject <UnoArray, NSCopying>
@property (nonatomic) uArray* unoArray;
@property(nonatomic, copy) id (^getAt)(uArray*, int);
@property(nonatomic, copy) void (^setAt)(uArray*, int, id);
+ (instancetype)strongUnoArrayWithUnoArray:(uArray*)array
    getAt:(id (^) (uArray*, int))getAt
    setAt:(void (^) (uArray*, int, id))setAt;
- (id)initWithUnoArray:(uArray*)array
    getAt:(id (^) (uArray*, int))getAt
    setAt:(void (^) (uArray*, int, id))setAt NS_DESIGNATED_INITIALIZER;
- (id)init;
- (id)objectAtIndexedSubscript:(int)idx;
- (void)setObject:(id)obj atIndexedSubscript:(int)idx;
- (NSUInteger)count;
- (NSArray*)copyArray;
@end
