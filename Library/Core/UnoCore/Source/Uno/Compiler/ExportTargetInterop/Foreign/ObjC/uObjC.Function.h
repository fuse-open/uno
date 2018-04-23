#pragma once
#include <Uno.h>

namespace uObjC
{

template<class ReturnType, class... Params>
    using Function = ReturnType (^) (Params...);

template<class ReturnType, class... Params>
    using RawFunction = ReturnType (*) (Params...);

inline uDelegate* NewUnoDelegate(uType* type, const void* func, uObject* object)
{
    return object == nullptr ? nullptr : uDelegate::New(type, func, object);
}

}
