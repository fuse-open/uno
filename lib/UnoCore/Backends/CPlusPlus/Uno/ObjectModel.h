// @(MSG_ORIGIN)
// @(MSG_EDIT_WARNING)

#pragma once
#include <atomic>
#include <cstdarg>
#include <cstdlib>
#include <exception>
#if @(REFLECTION:Defined)
#include <Uno/Reflection.h>
#else
#include <Uno/Memory.h>
#endif
@{Uno.Exception:ForwardDeclaration}
struct uObjectMonitor;

/**
    \addtogroup ObjectModel
    @{
*/
struct uObject
{
    uType* __type;
    uWeakObject* __weakptr;
    uObjectMonitor* __monitor;
#ifdef DEBUG_ARC
    size_t __size;
    size_t __id;
#endif
    std::atomic_int __retains;

    uType* GetType() const;
    int32_t GetHashCode();
    bool Equals(uObject* object);
    uString* ToString();

protected:
    uObject() {}
private:
    uObject& operator =(const uObject&) = delete;
    uObject(const uObject&) = delete;
};

enum uTypeType
{
    uTypeTypeVoid,
    uTypeTypeEnum,
    uTypeTypeStruct,
    uTypeTypeGeneric,
    uTypeTypeByRef,
    uTypeTypeClass,
    uTypeTypeDelegate,
    uTypeTypeInterface,
    uTypeTypeArray,
};

enum uTypeState
{
    uTypeStateUninitialized,
    uTypeStateBuilding,
    uTypeStateBuilt,
    uTypeStateInitializing,
    uTypeStateInitialized
};

enum uTypeFlags
{
    uTypeFlagsAbstract = 1 << 0,
    uTypeFlagsRetainStruct = 1 << 1,
    uTypeFlagsClosedKnown = 1 << 2,
    uTypeFlagsClosed = 1 << 3,
};

enum uFieldFlags
{
    uFieldFlagsConstrained = 1 << 0,
    uFieldFlagsInherited = 1 << 1,
    uFieldFlagsStatic = 1 << 2,
    uFieldFlagsWeak = 1 << 3
};

struct uFieldInfo
{
    union {
        void* Address;
        uintptr_t Offset;
    };
    uType* Type;
    size_t Flags;
};

struct uInterfaceInfo
{
    uType* Type;
    size_t Offset;
};

struct uOperatorTable
{
    void(*fp_StorePtr)(uType*, const void*, void*);
    void(*fp_StoreValue)(uType*, const void*, void*);
    void(*fp_StoreStrong)(uType*, const void*, void*);
    void(*fp_StoreStrongValue)(uType*, const void*, void*);
};

struct uType : uObject
{
    const char* FullName;
    uint32_t Type;
    uint16_t Flags;
    uint16_t State;
    size_t TypeSize;
    size_t ValueSize;
    size_t ObjectSize;
    size_t Alignment;
#ifdef DEBUG_ARC
    size_t ObjectCount;
#endif

    // Initialization
    void Build();
    void Init();

    size_t DependencyCount;
    uType** DependencyTypes;
    void SetDependencies(uType* first, ...);

    // Inheritance
    uType* Base;
    void SetBase(uType* base);

    // Interfaces
    size_t InterfaceCount;
    uInterfaceInfo* Interfaces;
    void SetInterfaces(uType* type, size_t offset, ...);

    const void* InterfacePtr(const uObject* object);

    // Type checking
    bool Is(const uType* type) const;

    // Fields
    size_t FieldCount;
    uFieldInfo* Fields;
    void SetFields(size_t inherited);
    void SetFields(size_t inherited, uType* type, uintptr_t offset, int flags, ...);

    uTField Field(uTRef object, size_t index);
    uTField Field(size_t index);

    // Generics
    uType* Definition;
    size_t GenericCount;
    uType** Generics;
    size_t MethodTypeCount;
    uType** MethodTypes;
    size_t PrecalcCount;
    uType** PrecalcTypes;
    void SetPrecalc(uType* first, ...);

    bool IsClosed();
    uType* NewMethodType(size_t genericCount, size_t precalcCount = 0, size_t dependencyCount = 0, bool isDefinition = true);
    uType* NewMethodType(uType* definition);
    uType* GetBase(uType* definition);
    uType* GetVirtual(size_t index, uType* method);
    uType* MakeGeneric(size_t count, uType** args);
    uType* MakeMethod(size_t index, uType* first, ...);
    uType* MakeType(uType* first, ...);
    uType* Precalced(size_t index);
    uType* T(size_t index);
    uType* U(size_t index);
    uObject* New();

    // Arrays
    uArrayType* _array;
    uArrayType* Array();

    // Pass-by-ref
    uByRefType* _byRef;
    uByRefType* ByRef();

    // Operators
    uOperatorTable* Operators;

    // ARC
    uObjectRefs Refs;

    // Reflection
#if @(REFLECTION:Defined)
    uReflection Reflection;
#endif

    // V-table
    void(*fp_build_)(uType*);
    const void* fp_ctor_;
    void(*fp_cctor_)(uType*);
    void(*fp_Finalize)(uObject*);
    void(*fp_GetHashCode)(uObject*, int32_t*);
    void(*fp_Equals)(uObject*, uObject*, bool*);
    void(*fp_ToString)(uObject*, uString**);
};

struct uTypeOptions
{
    uType* BaseDefinition;
    size_t FieldCount;
    size_t GenericCount;
    size_t MethodTypeCount;
    size_t InterfaceCount;
    size_t DependencyCount;
    size_t PrecalcCount;
    size_t ObjectSize;
    size_t TypeSize;
    size_t ValueSize;
    size_t Alignment;

    uTypeOptions() {
        memset(this, 0, sizeof(uTypeOptions));
    }
};

inline uType* uObject::GetType() const {
    return __type;
}
inline int32_t uObject::GetHashCode() {
    int32_t result;
    return (*__type->fp_GetHashCode)(this, &result), result;
}
inline bool uObject::Equals(uObject* object) {
    bool result;
    return (*__type->fp_Equals)(this, object, &result), result;
}
inline uString* uObject::ToString() {
    uString* result;
    return (*__type->fp_ToString)(this, &result), result;
}

#define U_IS_VOID(type) ((type)->Type == uTypeTypeVoid)
#define U_IS_BY_REF(type) ((type)->Type >= uTypeTypeByRef)
#define U_IS_OBJECT(type) ((type)->Type >= uTypeTypeClass)
#define U_IS_VALUE(type) ((type)->Type < uTypeTypeByRef)
#define U_IS_ABSTRACT(type) ((type)->Flags & uTypeFlagsAbstract)

uType* uObject_typeof();
uType* uVoid_typeof();
/** @} */

/**
    \addtogroup Exception
    @{
*/
struct uThrowable : public std::exception
{
    uStrong<@{Uno.Exception}> Exception;
    const char* Function;
    int Line;

    uThrowable(@{Uno.Exception} exception, const char* func, int line)
        : Exception(exception)
        , Function(func)
        , Line(line) {}
    uThrowable(const uThrowable&) = default;
    virtual const char* what() const throw();

    static U_NORETURN void ThrowIndexOutOfRange(const char* func, int line);
    static U_NORETURN void ThrowInvalidCast(const char* func, int line);
    static U_NORETURN void ThrowInvalidCast(const uType* from, const uType* to, const char* func, int line);
    static U_NORETURN void ThrowInvalidOperation(const char* message, const char* func, int line);
    static U_NORETURN void ThrowNullReference(const char* func, int line);

private:
    uThrowable& operator =(const uThrowable&) = delete;
    uThrowable() = delete;
};

#define U_THROW(exception) throw uThrowable((exception), U_FUNCTION, __LINE__)
#define U_THROW_ICE() uThrowable::ThrowInvalidCast(U_FUNCTION, __LINE__)
#define U_THROW_ICE2(from, to) uThrowable::ThrowInvalidCast(from, to, U_FUNCTION, __LINE__)
#define U_THROW_IOE(message) uThrowable::ThrowInvalidOperation(message, U_FUNCTION, __LINE__)
#define U_THROW_IOORE() uThrowable::ThrowIndexOutOfRange(U_FUNCTION, __LINE__)
#define U_THROW_NRE() uThrowable::ThrowNullReference(U_FUNCTION, __LINE__)

/** @} */

/**
    \addtogroup Class
    @{
*/
struct uClassType : uType
{
    static uClassType* New(const char* name, uTypeOptions& options);
};

uObject* uNew(uType* type);
uObject* uNew(uType* type, size_t size);

inline bool uIs(const uObject* object, const uType* type) {
    return object && object->__type->Is(type);
}
template<class T>
T uAs(uObject* object, const uType* type) {
    U_ASSERT(sizeof(T) == type->ValueSize);
    return object && object->__type->Is(type) ? (T)object : nullptr;
}
template<class T>
T uCast(uObject* object, const uType* type) {
    U_ASSERT(sizeof(T) == type->ValueSize);
    if (object && !object->__type->Is(type))
        U_THROW_ICE2(object->__type, type);
    return (T)object;
}
template<class T>
T uPtr(const T& ptr) {
    if (!ptr) U_THROW_NRE();
    return ptr;
}
template<class T>
T uPtr(const uSStrong<T>& ref) {
    return uPtr(ref._object);
}
template<class T>
T uPtr(const uStrong<T>& ref) {
    return uPtr(ref._object);
}
template<class T>
T uPtr(const uSWeak<T>& ref) {
    return uPtr((T)uLoadWeak(ref._object));
}
template<class T>
T uPtr(const uWeak<T>& ref) {
    return uPtr((T)uLoadWeak(ref._object));
}
/** @} */

/**
    \addtogroup Interface
    @{
*/
struct uInterfaceType : uType
{
    static uInterfaceType* New(const char* name, size_t genericCount = 0, size_t methodCount = 0);
};

struct uInterface
{
    uObject* _object;
    const void* _vtable;

    uInterface(uObject* object, uType* interfaceType) {
        U_ASSERT(interfaceType);
        _object = object;
        _vtable = interfaceType->InterfacePtr(object);
    }
    template<class T>
    const T* VTable() const {
        U_ASSERT(_object && _vtable);
        return (const T*)_vtable;
    }
    uObject* operator ->() const {
        return _object;
    }
    operator uObject*() const {
        return _object;
    }
};

/** @} */

/**
    \addtogroup Delegate
    @{
*/
struct uDelegateType : uType
{
    uType* ReturnType;
    size_t ParameterCount;
    uType** ParameterTypes;

    void SetSignature(uType* returnType, ...);
    static uDelegateType* New(const char* name, size_t parameterCount = 0, size_t genericCount = 0);
};

struct uDelegate : uObject
{
    void* _this;
    const void* _func;
    uType* _generic;
    uStrong<uObject*> _object;
    uStrong<uDelegate*> _prev;

    void InvokeVoid();
    void InvokeVoid(void* arg);
    void Invoke(uTRef retval, void** args, size_t count);
    void Invoke(uTRef retval, size_t count = 0, ...);
    uObject* Invoke(uArray* args = nullptr);
    uObject* Invoke(size_t count, ...);
    uDelegate* Copy();

    static uDelegate* New(uType* type, const void* func, uObject* object = nullptr, uType* generic = nullptr);
    static uDelegate* New(uType* type, uObject* object, size_t offset, uType* generic = nullptr);
    static uDelegate* New(uType* type, const uInterface& iface, size_t offset, uType* generic = nullptr);
};
/** @} */

/**
    \addtogroup Struct
    @{
*/
struct uStructType : uType
{
    static uStructType* New(const char* name, uTypeOptions& options);

    // Value adapters (usable w/o boxing)
    void(*fp_GetHashCode_struct)(void*, uType*, int32_t*);
    void(*fp_Equals_struct)(void*, uType*, uObject*, bool*);
    void(*fp_ToString_struct)(void*, uType*, uString**);
};

uObject* uBoxPtr(uType* type, const void* src, void* stack = nullptr, bool ref = false);
void uUnboxPtr(uType* type, uObject* object, void* dst);

template<class T>
uObject* uBox(uType* type, const T& value, void* stack = nullptr) {
    U_ASSERT(type && type->ValueSize == sizeof(T));
    return uBoxPtr(type, &value, stack, true);
}
template<class T>
T uDefault() {
    T t;
    return memset(&t, 0, sizeof(T)), t;
}
template<class T>
T uUnbox(uType* type, uObject* object) {
    T t;
    U_ASSERT(type && type->ValueSize == sizeof(T));
    return uUnboxPtr(type, object, &t), t;
}
template<class T>
T uUnbox(uObject* object) {
    U_ASSERT(object && object->__type->ValueSize == sizeof(T));
    return *(const T*)((const uint8_t*)object + sizeof(uObject));
}
/** @} */

/**
    \addtogroup Enum
    @{
*/
struct uEnumLiteral
{
    uString* Name;
    int64_t Value;
};

struct uEnumType : uType
{
    size_t LiteralCount;
    uEnumLiteral* Literals;
    void SetLiterals(const char* name, int64_t value, ...);
    static uEnumType* New(const char* name, uType* base, size_t literalCount = 0);
};

struct uEnum
{
    static bool TryParse(uType* type, uString* value, bool ignoreCase, uTRef result);
    static uString* GetString(uType* type, void* value);
};
/** @} */

/**
    \addtogroup Array
    @{
*/
struct uArrayType : uType
{
    uType* ElementType;
};

struct uArray : uObject
{
    void* _ptr;
    size_t _length;

    int32_t Length() const { return (int32_t) _length; }
    const void* Ptr() const { return _ptr; }
    void* Ptr() { return _ptr; }

    void MarshalPtr(size_t index, const void* value, size_t size);
    uTField TItem(size_t index);
    uTField TUnsafe(size_t index);

    template<class T>
    T& Item(size_t index) {
        U_ASSERT(sizeof(T) == ((uArrayType*)__type)->ElementType->ValueSize);
        if (index >= _length)
            U_THROW_IOORE();
        return ((T*)_ptr)[index];
    }
    template<class T>
    uStrong<T>& Strong(size_t index) {
        U_ASSERT(sizeof(T) == ((uArrayType*)__type)->ElementType->ValueSize);
        if (index >= _length)
            U_THROW_IOORE();
        return ((uStrong<T>*)_ptr)[index];
    }
    template<class T>
    T& Unsafe(size_t index) {
        U_ASSERT(sizeof(T) == ((uArrayType*)__type)->ElementType->ValueSize &&
                 index >= 0 && index < _length);
        return ((T*)_ptr)[index];
    }
    template<class T>
    uStrong<T>& UnsafeStrong(size_t index) {
        U_ASSERT(sizeof(T) == ((uArrayType*)__type)->ElementType->ValueSize &&
                 index >= 0 && index < _length);
        return ((uStrong<T>*)_ptr)[index];
    }

    static uArray* New(uType* type, size_t length, const void* optionalData = nullptr);
    static uArray* InitT(uType* type, size_t length, ...);

    template<class T>
    static uArray* Init(uType* type, size_t length, ...) {
        va_list ap;
        va_start(ap, length);
        uArray* array = New(type, length);
        for (size_t i = 0; i < length; i++) {
            T item = va_arg(ap, T);
            array->MarshalPtr(i, &item, sizeof(T));
        }
        va_end(ap);
        return array;
    }
};
/** @} */

/**
    \addtogroup String
    @{
*/
struct uString : uObject
{
    char16_t* _ptr;
    size_t _length;

    int32_t Length() const { return (int32_t) _length; }
    const char16_t* Ptr() const { return _ptr; }

    const char16_t& Item(size_t index) const {
        if (index >= _length)
            U_THROW_IOORE();
        return _ptr[index];
    }
    const char16_t& Unsafe(size_t index) const {
        return _ptr[index];
    }

    static uString* New(size_t length);
    static uString* Ansi(const char* cstr);
    static uString* Ansi(const char* cstr, size_t length);
    static uString* Utf8(const char* mutf8);
    static uString* Utf8(const char* mutf8, size_t length);
    static uString* Utf16(const char16_t* nullTerminatedUtf16);
    static uString* Utf16(const char16_t* utf16, size_t length);
    static uString* Const(const char* mutf8);
    static uString* CharArray(const uArray* chars);
    static uString* CharArrayRange(const uArray* chars, size_t startIndex, size_t length);
    static bool Equals(const uString* left, const uString* right, bool ignoreCase = false);
};

// Leak warning: The returned string must be deleted using free()
char* uAllocCStr(const uString* string, size_t* length = nullptr);
// Deprecated: Use free() instead - the parameter type shouldn't be const
void DEPRECATED("Use free() instead") uFreeCStr(const char* cstr);

struct uCString
{
    char* Ptr;
    size_t Length;

    uCString(const uString* string) {
        Ptr = uAllocCStr(string, &Length);
    }
    uCString(const uSStrong<uString*>& string){
        Ptr = uAllocCStr(string._object, &Length);
    }
    ~uCString() {
        free(Ptr);
    }
};
/** @} */

/**
    \addtogroup Generic
    @{
*/
struct uGenericType : uType
{
    size_t GenericIndex;
};

struct uTBase
{
    uType* _type;
    void* _address;

    uTBase(uType* type, void* address)
        : _type(type)
        , _address(address) {
        U_ASSERT(type && address);
    }
    uObject* ObjectPtr() {
        return *(uObject**)_address;
    }
    void* ValuePtr() {
        return _address;
    }
    operator void*() {
        return U_IS_OBJECT(_type)
            ? *(void**)_address
            : _address;
    }
    bool operator !() const {
        return U_IS_OBJECT(_type) && !*(uObject**)_address;
    }

private:
    uTBase& operator =(const uTBase&) { U_FATAL(); }
    uTBase() {}
};

struct uTRef
{
    void* _address;

    uTRef(void* address)
        : _address(address) {
    }
    template<class T>
    T& Value(uType* type) {
        U_ASSERT(_address);
        return *(T*)_address;
    }
    uTRef& Default(uType* type) {
        U_ASSERT(_address && type);
        memset(_address, 0, type->ValueSize);
        return *this;
    }
    uTBase Load(uType* type) {
        return uTBase(type, _address);
    }
    uTRef& Store(uType* type, const void* value) {
        (*type->Operators->fp_StoreValue)(type, value, _address);
        return *this;
    }
    uTRef& Store(const uTBase& value) {
        (*value._type->Operators->fp_StorePtr)(value._type, value._address, _address);
        return *this;
    }

private:
    uTRef& operator =(const uTRef&) = delete;
    uTRef() = delete;
};

struct uTStrongRef : uTBase
{
    uTStrongRef(uType* type, void* address)
        : uTBase(type, address) {
        if (U_IS_OBJECT(_type))
            uAutoRelease(*(uObject**)_address);
    }
    uTStrongRef(const uTStrongRef& copy)
        : uTBase(copy) {
        if (U_IS_OBJECT(_type))
            uAutoRelease(*(uObject**)_address);
    }
    ~uTStrongRef() {
        if (U_IS_OBJECT(_type))
            uRetain(*(uObject**)_address);
    }
    operator uTRef&() {
        return (uTRef&)_address;
    }
};

struct uTField : uTBase
{
    uTField(uType* type, void* address)
        : uTBase(type, address) {
        U_ASSERT(!U_IS_OBJECT(type) || !*(uObject**)address || uIs(*(uObject**)address, type));
    }
    uTField& Default() {
        if (U_IS_OBJECT(_type))
            uAutoRelease(*(uObject**)_address);
        else if (_type->Flags & uTypeFlagsRetainStruct)
            uAutoReleaseStruct(_type, _address);
        memset(_address, 0, _type->ValueSize);
        return *this;
    }
    template<class T>
    uStrong<T>& Strong() {
        U_ASSERT(sizeof(T) == _type->ValueSize);
        return *(uStrong<T>*)_address;
    }
    template<class T>
    T& Value() {
        U_ASSERT(sizeof(T) == _type->ValueSize);
        return *(T*)_address;
    }
    uTField& operator =(const uTField& value) {
        (*_type->Operators->fp_StoreStrong)(_type, value._address, _address);
        return *this;
    }
    uTField& operator =(const uTBase& value) {
        (*_type->Operators->fp_StoreStrong)(_type, value._address, _address);
        return *this;
    }
    uTField& operator =(const void* value) {
        (*_type->Operators->fp_StoreStrongValue)(_type, value, _address);
        return *this;
    }
    uTField operator [](size_t index) {
        return _type->Field(_address, index);
    }
    uTStrongRef operator &() {
        return uTStrongRef(_type, _address);
    }
    bool operator !() const {
        return U_IS_OBJECT(_type) && !*(uObject**)_address;
    }
};

struct uT : uTBase
{
    uT(uType* type, void* stack)
        : uTBase(type, stack) {
        memset(stack, 0, type->ValueSize);
    }
    uT(const uT& copy)
        : uTBase(copy) {
        if (_type->Flags & uTypeFlagsRetainStruct)
            uRetainStruct(_type, _address);
    }
    ~uT() {
        if (_type->Flags & uTypeFlagsRetainStruct)
            uAutoReleaseStruct(_type, _address);
    }
    uT& Default() {
        memset(_address, 0, _type->ValueSize);
        return *this;
    }
    uT& operator =(const uT& value) {
        (*_type->Operators->fp_StorePtr)(_type, value._address, _address);
        return *this;
    }
    uT& operator =(const uTBase& value) {
        (*_type->Operators->fp_StorePtr)(_type, value._address, _address);
        return *this;
    }
    uT& operator =(const void* value) {
        (*_type->Operators->fp_StoreValue)(_type, value, _address);
        return *this;
    }
    uTField operator [](size_t index) {
        return _type->Field(_address, index);
    }
    uTRef& operator &() {
        return (uTRef&)_address;
    }
    bool operator !() const {
        return U_IS_OBJECT(_type) && !*(uObject**)_address;
    }
};

struct uTPtr
{
    void* _ptr;

    uTPtr(void* ptr)
        : _ptr(ptr) {
    }
    uTRef& operator &() {
        return *(uTRef*)this;
    }
    operator void*() const {
        return _ptr;
    }

private:
    uTPtr& operator =(const uTPtr&) = delete;
    uTPtr() = delete;
};

template<class T>
bool uIs(const uType* type, const T& value, const uType* test) {
    U_ASSERT(type);
    return U_IS_OBJECT(type)
        ? uIs(*(const uObject**)&value, test)
        : type->Is(test);
}
template<class T>
uTPtr uConstrain(const uType* type, const T& value) {
    U_ASSERT(type && type->ValueSize == sizeof(T));
    return U_IS_VALUE(type)
        ? const_cast<T*>(&value)
        : *(void**)&value;
}
inline uTPtr uUnboxAny(const uType* type, uObject* object) {
    U_ASSERT(type);
    return U_IS_VALUE(type)
        ? (uint8_t*)uPtr(object) + sizeof(uObject)
        : (void*)object;
}
inline uTField uArray::TItem(size_t index) {
    if (index >= _length) U_THROW_IOORE();
    uType* type = ((uArrayType*)__type)->ElementType;
    return uTField(type, (uint8_t*)_ptr + type->ValueSize * index);
}
inline uTField uArray::TUnsafe(size_t index) {
    uType* type = ((uArrayType*)__type)->ElementType;
    return uTField(type, (uint8_t*)_ptr + type->ValueSize * index);
}
inline uTField uType::Field(size_t index) {
    U_ASSERT(index < FieldCount);
    uFieldInfo& field = Fields[index];
    U_ASSERT(field.Flags == uFieldFlagsStatic);
    return uTField(field.Type, field.Address);
}
inline uTField uType::Field(uTRef object, size_t index) {
    U_ASSERT(index < FieldCount);
    uFieldInfo& field = Fields[index];
    U_ASSERT(field.Flags == 0 && object._address);
    return uTField(field.Type, (uint8_t*)object._address + field.Offset);
}
inline uType* uType::Precalced(size_t index) {
    U_ASSERT(index < PrecalcCount);
    return U_ASSERT_PTR(PrecalcTypes[index]);
}
inline uType* uType::T(size_t index) {
    U_ASSERT(index < GenericCount);
    return U_ASSERT_PTR(Generics[index]);
}
inline uType* uType::U(size_t index) {
    uGenericType* generic = (uGenericType*)this;
    U_ASSERT(Type == uTypeTypeGeneric &&
             generic->GenericIndex + index < GenericCount);
    return U_ASSERT_PTR(Generics[generic->GenericIndex + index]);
}
/** @} */

/**
    \addtogroup ByRef
    @{
*/
struct uByRefType : uType
{
    uType* ValueType;
};

template<class T>
T* uCRef(const T& value) {
    return const_cast<T*>(&value);
}
/** @} */
