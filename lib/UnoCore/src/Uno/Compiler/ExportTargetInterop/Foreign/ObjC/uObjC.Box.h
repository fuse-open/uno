#pragma once

#include <Foundation/Foundation.h>
#include "uObjC.Function.h"
#include "uObjC.UnoArray.h"
#include "uObjC.UnoObject.h"

namespace uObjC
{

template <class T> id Box(T x);
template <> inline id Box<bool>(bool x) { return [NSNumber numberWithInteger: x]; }
template <> inline id Box<char>(char x) { return [NSNumber numberWithChar: x]; }
template <> inline id Box<double>(double x) { return [NSNumber numberWithDouble: x]; }
template <> inline id Box<float>(float x) { return [NSNumber numberWithFloat: x]; }
template <> inline id Box<int>(int x) { return [NSNumber numberWithInt: x]; }
template <> inline id Box<long>(long x) { return [NSNumber numberWithLong: x]; }
template <> inline id Box<long long>(long long x) { return [NSNumber numberWithLongLong: x]; }
template <> inline id Box<short>(short x) { return [NSNumber numberWithShort: x]; }
template <> inline id Box<unsigned char>(unsigned char x) { return [NSNumber numberWithUnsignedChar: x]; }
template <> inline id Box<unsigned int>(unsigned int x) { return [NSNumber numberWithUnsignedInt: x]; }
template <> inline id Box<unsigned long>(unsigned long x) { return [NSNumber numberWithUnsignedLong: x]; }
template <> inline id Box<unsigned long long>(unsigned long long x) { return [NSNumber numberWithUnsignedLongLong: x]; }
template <> inline id Box<unsigned short>(unsigned short x) { return [NSNumber numberWithUnsignedShort: x]; }

template <class T> T Unbox(id x);
template <> inline bool Unbox<bool>(id x) { return [x boolValue]; }
template <> inline char Unbox<char>(id x) { return [x charValue]; }
template <> inline double Unbox<double>(id x) { return [x doubleValue]; }
template <> inline float Unbox<float>(id x) { return [x floatValue]; }
template <> inline int Unbox<int>(id x) { return [x intValue]; }
template <> inline long Unbox<long>(id x) { return [x longValue]; }
template <> inline long long Unbox<long long>(id x) { return [x longLongValue]; }
template <> inline short Unbox<short>(id x) { return [x shortValue]; }
template <> inline unsigned char Unbox<unsigned char>(id x) { return [x unsignedCharValue]; }
template <> inline unsigned int Unbox<unsigned int>(id x) { return [x unsignedIntValue]; }
template <> inline unsigned long Unbox<unsigned long>(id x) { return [x unsignedLongValue]; }
template <> inline unsigned long long Unbox<unsigned long long>(id x) { return [x unsignedLongLongValue]; }
template <> inline unsigned short Unbox<unsigned short>(id x) { return [x unsignedShortValue]; }

}
