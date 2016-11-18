// @(MSG_ORIGIN)
// @(MSG_EDIT_WARNING)

#pragma once
#include <Uno/_config.h>
struct uArray;
struct uArrayType;
struct uByRefType;
struct uClassType;
struct uDelegate;
struct uField;
struct uFieldInfo;
struct uFunction;
struct uInterfaceType;
struct uObject;
struct uStatic;
struct uString;
struct uType;
struct uTField;
struct uTRef;
struct uT;
struct uWeakObject;
struct uThreadData;
template<class T> struct uStrong;
template<class T> struct uSWeak;
template<class T> struct uWeak;

/**
    \addtogroup Memory
    @{
*/
struct uRuntime
{
    uRuntime();
    ~uRuntime();
private:
    uRuntime(const uRuntime&);
    uRuntime& operator =(const uRuntime&);
};

struct uStackFrame
{
    uStackFrame(const char* type, const char* function);
    ~uStackFrame();
private:
    uThreadData* _thread;
    uStackFrame(const uStackFrame&);
    uStackFrame& operator = (const uStackFrame&);
};

uString* uGetStackTrace();
uArray* uGetNativeStackTrace(int skipFrames);

struct uAutoReleasePool
{
    uAutoReleasePool();
    ~uAutoReleasePool();
private:
    uThreadData* _thread;
    uAutoReleasePool(const uAutoReleasePool&);
    uAutoReleasePool& operator = (const uAutoReleasePool&);
};

struct uForeignPool
{
    uForeignPool();
    ~uForeignPool();
private:
    uThreadData* _thread;
    bool _threadHasPool;
    uForeignPool(const uForeignPool&);
    uForeignPool& operator = (const uForeignPool&);
};

void uRetainStruct(uType* type, void* address);
void uReleaseStruct(uType* type, void* address);
void uAutoReleaseStruct(uType* type, void* address);

void uRetain(uObject* object);
void uRelease(uObject* object);
void uAutoRelease(uObject* object);

struct uWeakStateIntercept
{
    enum Event { OnRelease = 0, OnLoad = 1 };

    //  Callback is called on two occasions, both times while holding the
    //  internal uWeakObject's mutex:
    //
    //  1.  Called from uRelease, with Event::OnRelease, when the object's
    //      reference count reaches zero.
    //
    //      Returning true from the callback at this point allows deletion to
    //      proceed normally.
    //
    //      Returning false defers object deletion, at which point, the object
    //      is considered a Zombie. Weak references may bring a Zombie object
    //      back to life (see point 2, below).
    //
    //      NOTE: If the object's reference count reaches zero while it is
    //      considered a Zombie, object deletion proceeds without further
    //      notice.
    //
    //
    //  2.  Called from uLoadWeak, with Event::OnLoad, when attempting to
    //      obtain strong reference to a Zombie object (see point 1).
    //
    //      If the callback returns true, the object is brought back to life and
    //      no longer considered a Zombie. Namely, the next time the reference
    //      count reaches zero the callback will be invoked again, per point 1.
    //
    //      Returning false from the callback signals that object deletion is
    //      pending; uLoadWeak will return nullptr.
    //
    //
    //  For a given object, the callback will be invoked at least once with
    //  Event::OnRelease, before object deletion takes place. Any subsequent
    //  calls with Event::OnRelease will be preceded by exacly one call with
    //  Event::OnLoad as the first argument.

    typedef bool (*Callback)(Event, uObject *);

    static void SetCallback(uWeakObject* object, Callback callback);
};

void uStoreStrong(uObject** address, uObject* object);
void uStoreWeak(uWeakObject** address, uObject* object);
uObject* uLoadWeak(uWeakObject* ptr);

#ifdef DEBUG_DUMPS
void uDumpAllStrongRefs(const char* filename);
#endif

struct uObjectRefs
{
    size_t WeakCount;
    size_t StrongCount;
    size_t* Weak;
    size_t* Strong;
};

template<class T>
struct uStrongRef
{
    T* _address;

    uStrongRef(T* address)
        : _address(address) {
        U_ASSERT(_address);
        uAutoRelease(*_address);
    }
    ~uStrongRef() {
        U_ASSERT(_address);
        uRetain(*_address);
    }
    operator T*() {
        return _address;
    }
    operator uTRef&() {
        return (uTRef&)_address;
    }
    template<class U>
    explicit operator U() {
        return (U)_address;
    }
};

template<class T>
struct uWeakRef
{
    uWeakObject** _address;
    uObject* _value;

    uWeakRef(uWeakObject** address)
        : _address(address) {
        U_ASSERT(address);
        _value = uLoadWeak(*address);
    }
    ~uWeakRef() {
        uStoreWeak(_address, _value);
    }
    operator T*() {
        return (T*)&_value;
    }
    operator uTRef&() {
        return (uTRef&)&_value;
    }
    template<class U>
    explicit operator U() {
        return (U)&_value;
    }
};

template<class T>
struct uSStrong
{
    T _object;

    uSStrong() {
    }
    uSStrong(T object)
        : _object(object) {
        uRetain((uObject*)object);
    }
    uSStrong<T>& operator =(T object) {
        uStoreStrong((uObject**)&_object, (uObject*)object);
        return *this;
    }
    uSStrong<T>& operator =(const uSStrong<T>& copy) {
        return *this = copy._object;
    }
    uSStrong<T>& operator =(const uSWeak<T>& copy) {
        return *this = (T)uLoadWeak(copy._object);
    }
    bool operator ==(T object) const {
        return _object == object;
    }
    bool operator !=(T object) const {
        return _object != object;
    }
    bool operator !() const {
        return !_object;
    }
    uStrongRef<T> operator &() {
        return &_object;
    }
    operator T() const {
        return _object;
    }
    T operator ->() const {
        return _object;
    }
    template<class U>
    explicit operator U() const {
        return (U)_object;
    }
};

template<class T>
struct uSWeak
{
    uWeakObject* _object;

    uSWeak() {
    }
    uSWeak(T object)
        : _object(nullptr) {
        uStoreWeak(&_object, (uObject*)object);
    }
    uSWeak<T>& operator =(T object) {
        uStoreWeak(&_object, (uObject*)object);
        return *this;
    }
    uSWeak<T>& operator =(const uSStrong<T>& copy) {
        return *this = copy._object;
    }
    uSWeak<T>& operator =(const uSWeak<T>& copy) {
        _object = copy._object;
        return *this;
    }
    bool operator ==(T object) const {
        return (T)uLoadWeak(const_cast<uWeakObject*>(_object)) == object;
    }
    bool operator !=(T object) const {
        return (T)uLoadWeak(const_cast<uWeakObject*>(_object)) != object;
    }
    bool operator !() const {
        return !uLoadWeak(const_cast<uWeakObject*>(_object));
    }
    uWeakRef<T> operator &() {
        return &_object;
    }
    operator T() const {
        return (T)uLoadWeak(_object);
    }
    T operator ->() const {
        return (T)uLoadWeak(_object);
    }
    template<class U>
    explicit operator U() const {
        return (U)uLoadWeak(_object);
    }
};

template<class T>
struct uStrong : uSStrong<T>
{
    using uSStrong<T>::_object;
    using uSStrong<T>::operator =;

    uStrong() {
        _object = nullptr;
    }
    uStrong(T object)
        : uSStrong<T>(object) {
    }
    uStrong(const uStrong& copy)
        : uSStrong<T>(copy._object) {
    }
    ~uStrong() {
        uRelease((uObject*)_object);
    }
};

template<class T>
struct uWeak : uSWeak<T>
{
    using uSWeak<T>::_object;
    using uSWeak<T>::operator =;

    uWeak() {
        _object = nullptr;
    }
    uWeak(T object)
        : uSWeak<T>(object) {
    }
    ~uWeak() {
        uStoreWeak(&_object, nullptr);
    }
};
/** @} */
