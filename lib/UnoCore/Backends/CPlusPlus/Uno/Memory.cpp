// @(MSG_ORIGIN)
// @(MSG_EDIT_WARNING)

#include <Uno/_internal.h>
#include <Uno/ObjectMonitor.h>
#include <Uno/Support.h>
#include <string>
#include <sstream>
@{Uno.Type:IncludeDirective}
@{string:IncludeDirective}

static bool _Initialized;
static std::mutex _Mutex;
static uThreadLocal* _Storage;

#ifdef DEBUG_DUMPS
static std::unordered_map<uObject*, bool>* _HeapObjects;
#endif

static bool uTryClearWeak(uObject*);

#if @(REFLECTION:Defined)
void uInitReflection();
void uFreeReflection();
void uRegisterIntrinsics();
#endif
void uInitObjectModel();
void uFreeObjectModel();
void uInitSupport();
void uFreeSupport();

static uThreadData* uGetThreadData()
{
    void* value = uGetThreadLocal(_Storage);

    if (!value)
    {
        value = new uThreadData();
        uSetThreadLocal(_Storage, value);
    }

    return (uThreadData*)value;
}

static void uFreeThreadData(void* value)
{
    delete (uThreadData*)value;
}

uRuntime::uRuntime()
{
    if (_Initialized)
        U_FATAL("There is only room for one Uno Runtime object in this process.");

    _Initialized = true;
    _Storage = uCreateThreadLocal(uFreeThreadData);
#ifdef DEBUG_DUMPS
    _HeapObjects = new std::unordered_map<uObject*, bool>();
#endif

    uAutoReleasePool pool;
#if @(REFLECTION:Defined)
    uInitReflection();
#endif
    uInitObjectModel();
#if @(REFLECTION:Defined)
    uRegisterIntrinsics();
#endif
}

uRuntime::~uRuntime()
{
    uFreeObjectModel();
#if @(REFLECTION:Defined)
    uFreeReflection();
#endif
#ifdef DEBUG_DUMPS
    delete _HeapObjects;
#endif
    uDeleteThreadLocal(_Storage);
}

uStackFrame::uStackFrame(const char* type, const char* function)
    : _thread(uGetThreadData())
{
    uCallStackFrame* frame = ++_thread->CallStackPtr;
    U_ASSERT(frame < _thread->CallStackEnd &&
             !frame->Type && !frame->Function);
    frame->Type = type;
    frame->Function = function;
}

uStackFrame::~uStackFrame()
{
#ifdef DEBUG_UNSAFE
    uCallStackFrame* frame =
#endif
    _thread->CallStackPtr--;
#ifdef DEBUG_UNSAFE
    frame->Type = nullptr;
    frame->Function = nullptr;
#endif
}

uString* uGetStackTrace()
{
    std::stringstream sb;
    uThreadData* thread = uGetThreadData();

    for (uCallStackFrame* frame = thread->CallStackPtr;
         frame >= thread->CallStack;
         frame--)
    {
        if (sb.tellp() > 0)
            sb << '\n';

        sb << "   at ";
        sb << frame->Type;
        sb << '.';
        sb << frame->Function;
    }

    std::string str = sb.str();
    return uString::Utf8(str.c_str(), str.size());
}

#ifdef DEBUG_ARC
static std::string uGetCaller()
{
    uThreadData* thread = uGetThreadData();

    if (thread->CallStackPtr < thread->CallStack)
        return "";

    std::stringstream sb;
    uCallStackFrame* frame = thread->CallStackPtr;
    sb << " -- at ";
    sb << frame->Type;
    sb << '.';
    sb << frame->Function;
    return sb.str();
}
#endif

static void uPushAutoReleasePool(uThreadData* thread)
{
    uAutoReleaseFrame* frame = ++thread->AutoReleasePtr;
    U_ASSERT(frame < thread->AutoReleaseEnd);
    frame->StartIndex = thread->AutoReleaseList.size();

#ifdef DEBUG_ARC
    frame->AllocCount = 0;
    frame->AllocSize = 0;
    frame->FreeCount = 0;
    frame->FreeSize = 0;
#endif
}

static void uPopAutoReleasePool(uThreadData* thread)
{
    uAutoReleaseFrame* frame = thread->AutoReleasePtr;
    U_ASSERT(thread->AutoReleasePtr >= thread->AutoReleaseStack);

    for (size_t i = frame->StartIndex; i < thread->AutoReleaseList.size(); i++)
    {
        uObject* object = thread->AutoReleaseList[i];
#ifdef DEBUG_ARC
        frame->AllocCount++;
        frame->AllocSize += object->__size;
#endif
        uRelease(object);
    }

#if DEBUG_ARC >= 1
    U_LOG("--- Alloc'd %d objects (%d bytes), Free'd %d objects (%d bytes) ---",
          frame->AllocCount, frame->AllocSize, frame->FreeCount, frame->FreeSize);
#endif
    thread->AutoReleaseList.resize(frame->StartIndex);
    thread->AutoReleasePtr--;
}

uAutoReleasePool::uAutoReleasePool()
    : _thread(uGetThreadData())
{
    uPushAutoReleasePool(_thread);
}

uAutoReleasePool::~uAutoReleasePool()
{
    U_ASSERT(_thread == uGetThreadData());
    uPopAutoReleasePool(_thread);
}

uForeignPool::uForeignPool()
    : _thread(uGetThreadData())
    , _threadHasPool(_thread->AutoReleasePtr >= _thread->AutoReleaseStack)
{
    if (!_threadHasPool)
        uPushAutoReleasePool(_thread);
}

uForeignPool::~uForeignPool()
{
    U_ASSERT(_thread == uGetThreadData());
    if (!_threadHasPool)
        uPopAutoReleasePool(_thread);
}

void uStoreStrong(uObject** address, uObject* object)
{
    uAutoRelease(*address);
    uRetain(*address = object);
}

void uAutoRelease(uObject* object)
{
    if (object)
    {
        uThreadData* thread = uGetThreadData();
        thread->AutoReleaseList.push_back(object);
        U_ASSERT(thread->AutoReleasePtr >= thread->AutoReleaseStack);
#ifdef DEBUG_ARC
        int releaseCount = 0;
        for (size_t i = 0; i < thread->AutoReleaseList.Length(); i++)
            if (thread->AutoReleaseList[i] == object)
                releaseCount++;

        int retainCount = object->__retains - releaseCount;
        if (retainCount < 0)
        {
            U_ERROR("*** BAD AUTORELEASE: %s #%d (%d bytes, %d retains) ***%s",
                    object->__type->FullName, object->__id, object->__size, retainCount, uGetCaller().c_str());
            U_FATAL("Attempted to auto release invalid object");
        }
#endif
#if DEBUG_ARC >= 4
        U_LOG("autorelease %s #%d (%d bytes, %d retains)%s",
              object->__type->FullName, object->__id, object->__size, object->__retains, uGetCaller().c_str());
#endif
    }
}

void uRetainStruct(uType* type, void* address)
{
#if DEBUG_ARC >= 4
    U_LOG("retain %s [struct] (%d bytes)%s", type->FullName, type->ValueSize, uGetCaller().c_str());
#endif
    for (size_t i = 0; i < type->Refs.StrongCount; i++)
        uRetain(*(uObject**)((uint8_t*)address + type->Refs.Strong[i]));
}

void uReleaseStruct(uType* type, void* address)
{
#if DEBUG_ARC >= 4
    U_LOG("release %s [struct] (%d bytes)%s", type->FullName, type->ValueSize, uGetCaller().c_str());
#endif
    for (size_t i = 0; i < type->Refs.StrongCount; i++)
    {
        uObject*& ptr = *(uObject**)((uint8_t*)address + type->Refs.Strong[i]);
        uRelease(ptr);
        ptr = nullptr;
    }

    for (size_t i = 0; i < type->Refs.WeakCount; i++)
        uStoreWeak((uWeakObject**)((uint8_t*)address + type->Refs.Weak[i]), nullptr);
}

void uAutoReleaseStruct(uType* type, void* address)
{
#if DEBUG_ARC >= 4
    U_LOG("autorelease %s [struct] (%d bytes)%s", type->FullName, type->ValueSize, uGetCaller().c_str());
#endif
    for (size_t i = 0; i < type->Refs.StrongCount; i++)
        uAutoRelease(*(uObject**)((uint8_t*)address + type->Refs.Strong[i]));
}

void uRetain(uObject* object)
{
    if (object)
    {
        ++object->__retains;
#if DEBUG_ARC >= 3
        U_LOG("retain %s #%d (%d bytes, %d retains)%s",
              object->__type->FullName, object->__id, object->__size, object->__retains, uGetCaller().c_str());
#endif
    }
}

void uRelease(uObject* object)
{
    if (object)
    {
        if (--object->__retains == 0)
        {
            if (!uTryClearWeak(object))
                return;
#ifdef DEBUG_ARC
            uThreadData* thread = uGetThreadData();

            if (thread->AutoReleasePtr >= thread->AutoReleaseStack)
            {
                uAutoReleaseFrame* frame = thread->AutoReleasePtr;
                if (frame->AllocCount > 0)
                {
                    frame->FreeCount++;
                    frame->FreeSize += object->__size;
                }
            }
#endif
            uType* type = object->__type;

            switch (type->Type)
            {
            case uTypeTypeClass:
            {
                uType* baseType = type;
                do
                {
                    if (baseType->fp_Finalize)
                    {
                        try
                        {
                            (*baseType->fp_Finalize)(object);
                        }
                        catch (const std::exception& e)
                        {
                            U_ERROR("Runtime Error: Unhandled exception in finalizer for %s: %s", baseType->FullName, e.what());
                        }
                    }
                } while ((baseType = baseType->Base));
                uReleaseStruct(type, object);
                break;
            }

            case uTypeTypeStruct:
                // This must be a boxed value, so append size of object header
                if (type->Flags & uTypeFlagsRetainStruct)
                    uReleaseStruct(type, (uint8_t*)object + sizeof(uObject));
                break;

            case uTypeTypeDelegate:
                uRelease(((uDelegate*)object)->_object);
                uRelease(((uDelegate*)object)->_prev);
                break;

            case uTypeTypeArray:
            {
                uArray* array = (uArray*)object;
                uArrayType* arrayType = (uArrayType*)type;
                uType* elmType = arrayType->ElementType;

                switch (elmType->Type)
                {
                case uTypeTypeClass:
                case uTypeTypeInterface:
                case uTypeTypeDelegate:
                case uTypeTypeArray:
                    for (uObject** objAddr = (uObject**)array->_ptr;
                         array->_length--;
                         objAddr++)
                        uRelease(*objAddr);
                    break;

                case uTypeTypeStruct:
                    if (elmType->Flags & uTypeFlagsRetainStruct)
                        for (uint8_t* address = (uint8_t*)array->_ptr;
                             array->_length--;
                             address += elmType->ValueSize)
                            uReleaseStruct(elmType, address);
                    break;
                }
                break;
            }
            }

            delete object->__monitor;

#if DEBUG_ARC >= 2
            U_LOG("free %s #%d (%d bytes)%s",
                  object->__type->FullName, object->__id, object->__size, uGetCaller().c_str());
#endif
#ifdef DEBUG_DUMPS
            _Mutex.lock();
            _HeapObjects->erase(object);
            _Mutex.unlock();
#endif
            U_ASSERT(object->__type != @{Uno.Type:TypeOf});
            free(object);
            return;
        }

        if (object->__retains < 0)
        {
#if DEBUG_ARC >= 4
            U_ERROR("*** BAD OBJECT: %s #%d (%d retains) ***%s",
                    object->__type->FullName, object->__id, object->__retains, uGetCaller().c_str());
#else
            U_ERROR("*** BAD OBJECT: 0x%p ***", object);
#endif
            U_FATAL("Attempted to free invalid object");
        }
        else
        {
#if DEBUG_ARC >= 3
            U_LOG("release %s #%d (%d bytes, %d retains)%s",
                  object->__type->FullName, object->__id, object->__size, object->__retains, uGetCaller().c_str());
#endif
        }
    }
}

static void uAlignField(size_t& offset, size_t align)
{
    U_ASSERT(align);
    size_t rem = offset % align;

    if (rem > 0)
        offset += align - rem;
}

void uBuildMemory(uType* type)
{
    U_ASSERT(type);
    if (!type->IsClosed())
        return;

    size_t strongCount = 0,
           weakCount = 0,
           objOffset = U_IS_OBJECT(type)
               ? sizeof(uObject)
               : 0,
           typeOffset = 0,
           align = 0;

    if (type->Base)
        type->Base->Build();

    for (size_t i = 0; i < type->FieldCount; i++)
    {
        uFieldInfo& f = type->Fields[i];
        U_ASSERT(f.Type);

        if (f.Type != type && !U_IS_OBJECT(f.Type))
            f.Type->Build();

        if ((f.Flags & uFieldFlagsStatic) == 0)
        {
            if (f.Type->Alignment > align)
                align = f.Type->Alignment;

            if ((f.Flags & uFieldFlagsConstrained) == 0)
                objOffset = f.Offset + f.Type->ValueSize;

            if (U_IS_VALUE(f.Type))
            {
                strongCount += f.Type->Refs.StrongCount;
                weakCount += f.Type->Refs.WeakCount;
            }
            else if ((f.Flags & uFieldFlagsWeak) != 0)
                weakCount++;
            else
                strongCount++;
        }
        else if (type->GenericCount)
        {
            uAlignField(typeOffset, f.Type->Alignment);
            f.Offset = typeOffset;
            typeOffset += f.Type->ValueSize;
        }
    }

    size_t size = typeOffset + (strongCount + weakCount) * sizeof(size_t);
    uint8_t* ptr = (uint8_t*)malloc(size); // Leak
    memset(ptr, 0, size);

    type->Refs.Strong = (size_t*)ptr;
    ptr += strongCount * sizeof(size_t);

    type->Refs.Weak = (size_t*)ptr;
    ptr += weakCount * sizeof(size_t);

    for (size_t i = 0; i < type->FieldCount; i++)
    {
        uFieldInfo& f = type->Fields[i];

        if ((f.Flags & uFieldFlagsStatic) == 0)
        {
            if ((f.Flags & uFieldFlagsConstrained) != 0)
            {
                uAlignField(objOffset, f.Type->Alignment);
                f.Flags &= ~uFieldFlagsConstrained;
                f.Offset = objOffset;
                objOffset += f.Type->ValueSize;
            }

            if (U_IS_VALUE(f.Type))
            {
                f.Flags &= ~uFieldFlagsWeak;

                for (size_t j = 0; j < f.Type->Refs.StrongCount; j++)
                    type->Refs.Strong[type->Refs.StrongCount++] = f.Type->Refs.Strong[j] + f.Offset;
                for (size_t j = 0; j < f.Type->Refs.WeakCount; j++)
                    type->Refs.Weak[type->Refs.WeakCount++] = f.Type->Refs.Weak[j] + f.Offset;
            }
            else if ((f.Flags & uFieldFlagsWeak) != 0)
                type->Refs.Weak[type->Refs.WeakCount++] = f.Offset;
            else
                type->Refs.Strong[type->Refs.StrongCount++] = f.Offset;
        }
        else
        {
            if ((f.Flags & uFieldFlagsConstrained) != 0)
                f.Flags &= ~uFieldFlagsConstrained;
            if (type->GenericCount)
                f.Offset += (uintptr_t)ptr;
        }
    }

    if (U_IS_VALUE(type))
    {
        if (align != 0)
        {
            U_ASSERT(type->Alignment == 0 || type->Alignment == align);
            type->Alignment = align;
        }

        if (objOffset != 0)
        {
            uAlignField(objOffset, type->Alignment);
            U_ASSERT(type->ValueSize == objOffset || type->ValueSize == 0);
            type->ValueSize = objOffset;
        }

        type->ObjectSize = sizeof(uObject) + type->ValueSize;
    }
    else
    {
        if (type->Base && type->Base->ObjectSize > objOffset)
            objOffset = type->Base->ObjectSize;

        if (objOffset > type->ObjectSize)
            type->ObjectSize = objOffset;
    }

#ifdef DEBUG_UNSAFE
    uint8_t* layout = (uint8_t*)alloca(type->ObjectSize);
    memset(layout, 0, type->ObjectSize);

    for (size_t i = 0; i < type->FieldCount; i++)
    {
        uFieldInfo& f = type->Fields[i];
        if ((f.Flags & uFieldFlagsStatic) == 0)
        {
            for (size_t j = 0; j < f.Type->ValueSize; j++)
            {
                U_ASSERT(f.Offset + j < type->ObjectSize);
                layout[f.Offset + j]++;
            }
        }
    }

    // Verify that no fields are overlapping
    for (size_t i = 0; i < type->ObjectSize; i++)
        U_ASSERT(layout[i] < 2);
#endif
}

void uWeakStateIntercept::SetCallback(uWeakObject* weak, uWeakStateIntercept::Callback cb)
{
    if (!weak || !cb || weak->ZombieState != uWeakObject::Healthy)
        U_FATAL();

    weak->ZombieState = uWeakObject::Infected;
    weak->ZombieStateIntercept = cb;
}

template<class RT, class T0>
static RT uCallWithWeakRefLock(RT(*func)(T0), T0 t0)
{
    std::lock_guard<std::mutex> lock(_Mutex);
    return func(t0);
}

static bool uTryClearWeak_inner(uObject* object)
{
    if (object->__retains != 0)
        return false;

    if (object->__weakptr->ZombieState == uWeakObject::Infected)
    {
        object->__weakptr->ZombieState = uWeakObject::Zombie;
        if (!object->__weakptr->ZombieStateIntercept(uWeakStateIntercept::OnRelease, object))
            return false;
    }

    object->__weakptr->ZombieState = uWeakObject::Dead;
    object->__weakptr->Object = nullptr;
    return true;
}

static bool uTryClearWeak(uObject* object)
{
    if (!object->__weakptr)
        return true;

    if (!uCallWithWeakRefLock(&uTryClearWeak_inner, object))
        return false;

    if (object->__weakptr)
    {
        if (--object->__weakptr->RefCount == 0)
            free(object->__weakptr);

        object->__weakptr = nullptr;
    }

    return true;
}

static void uNewWeak(uObject* object)
{
    if (object->__weakptr)
        return;

    uWeakObject* weak = (uWeakObject*)calloc(1, sizeof(uWeakObject));
    weak->Object = object;
    weak->RefCount = 1;
    weak->ZombieState = uWeakObject::Healthy;
    weak->ZombieStateIntercept = 0;
    object->__weakptr = weak;
}

void uStoreWeak(uWeakObject** address, uObject* object)
{
    if (*address && --(*address)->RefCount == 0)
        free(*address);

    if (!object)
    {
        *address = nullptr;
        return;
    }

    if (!object->__weakptr)
        uCallWithWeakRefLock(&uNewWeak, object);

    ++object->__weakptr->RefCount;
    *address = object->__weakptr;
}

static uObject* uLoadWeak_inner(uWeakObject* weak)
{
    if (weak->ZombieState == uWeakObject::Zombie)
    {
        if (!weak->ZombieStateIntercept(uWeakStateIntercept::OnLoad, weak->Object))
        {
            weak->ZombieState = uWeakObject::Dead;
            weak->Object = nullptr;
            return nullptr;
        }

        weak->ZombieState = uWeakObject::Infected;
    }

    uRetain(weak->Object);
    return weak->Object;
}

uObject* uLoadWeak(uWeakObject* weak)
{
    if (!weak)
        return nullptr;

    uObject* object = uCallWithWeakRefLock(&uLoadWeak_inner, weak);
    uAutoRelease(object);
    return object;
}

static uObject* uInitObject(uType* type, void* ptr, size_t size)
{
    U_ASSERT(type &&
        type->ObjectSize && type->ValueSize &&
        type->IsClosed() && !U_IS_ABSTRACT(type));

    if (type->State < uTypeStateInitializing &&
        strcmp(type->FullName, "Uno.String") != 0 &&
        strcmp(type->FullName, "Uno.Type") != 0)
        type->Init();

    uObject* object = (uObject*)ptr;
    object->__type = type;
    object->__retains = 1;

#ifdef DEBUG_ARC
    object->__size = size;
    object->__id = type->Definition->ObjectCount++;
#endif

#if DEBUG_ARC >= 2
    U_LOG("alloc %s #%d (%d bytes)%s", type->FullName, object->__id, size, uGetCaller().c_str());
#endif

#ifdef DEBUG_DUMPS
    _Mutex.lock();
    (*_HeapObjects)[object] = true;
    _Mutex.unlock();
#endif
    uAutoRelease(object);
    return object;
}

uObject* uNew(uType* type)
{
    U_ASSERT(type);
    size_t size = type->ObjectSize;
    U_ASSERT(size);
    return uInitObject(type, calloc(1, size), size);
}

uObject* uNew(uType* type, size_t size)
{
    U_ASSERT(type && size);
    return uInitObject(type, calloc(1, size), size);
}

static uString* uInitString(size_t length)
{
    size_t size = sizeof(uString) + sizeof(char16_t) * length + sizeof(char16_t);
    uString* string = (uString*)uInitObject(@{string:TypeOf}, calloc(1, size), size);
    string->_ptr = (char16_t*)((uint8_t*)string + sizeof(uString));
    string->_length = length;
    return string;
}

uString* uString::New(size_t length)
{
    if (length == 0)
    {
        static uStrong<uString*> empty = uInitString(0);
        return empty;
    }

    return uInitString(length);
}

#ifdef DEBUG_DUMPS

static void uDumpObject(FILE* fp, uObject* object, const char* label)
{
    fprintf(fp, "\tobject_at_%p [label=\"%s refcount: %d\"]\n",
            object, label, object->__retains);
}

static void uDumpGlobalRef(FILE* fp, uObject** object, const char* label)
{
    fprintf(fp, "\tglobal_ref_at_%p [label=\"%s\" color=\"blue\"]\n",
            object, label);
    if (*object)
        fprintf(fp, "\tglobal_ref_at_%p -> object_at_%p\n", object, *object);
}

static void uDumpStrongRef(FILE* fp, uObject* object, const char *label, uObject* target)
{
    if (target)
        fprintf(fp, "\tobject_at_%p -> object_at_%p [label=\"%s\"]\n", object, target, label);
}

static void uDumpAllStrongRefs(FILE* fp, uObject* object, void* base, uType* type, const char *labelPrefix = "")
{
    do
    {
        const uReflection& reflection = type->Reflection;
        for (size_t i = 0; i < reflection.FieldCount; i++)
        {
            uField* field = reflection.Fields[i];
            const uFieldInfo& fieldInfo = field->Info();

            if ((fieldInfo.Flags & (uFieldFlagsWeak | uFieldFlagsStatic)) != 0)
                continue;

            if (U_IS_OBJECT(fieldInfo.Type))
            {
                uObject* target = *(uObject**)((char *)base + fieldInfo.Offset);
                if (target)
                {
                    uCString fieldName(field->Name);
                    char *label = new char[strlen(labelPrefix) + fieldName.Length + 1];
                    sprintf(label, "%s%s", labelPrefix, fieldName.Ptr);

                    uDumpStrongRef(fp, object, label, target);

                    delete[] label;
                }
            }
            else if (U_IS_VALUE(fieldInfo.Type))
            {
                uCString fieldName(field->Name);
                char *newLabelPrefix = new char[strlen(labelPrefix) + fieldName.Length + 2];
                sprintf(newLabelPrefix, "%s%s.", labelPrefix, fieldName.Ptr);

                void* target = (void*)((uint8_t*)base + fieldInfo.Offset);
                uDumpAllStrongRefs(fp, object, target, fieldInfo.Type, newLabelPrefix);

                delete[] newLabelPrefix;
            }
        }

        type = type->Base;
    } while (type);
}

static void uDumpObjectAndStrongRefs(FILE* fp, uObject* object)
{
    uType* type = object->GetType();

    // type-info is not "real" types that we care about in this respect
    if (type == @{Uno.Type:TypeOf})
        return;

    uDumpObject(fp, object, type->FullName);

    switch (type->Type)
    {
    case uTypeTypeClass:
        uDumpAllStrongRefs(fp, object, object, type);
        break;

    case uTypeTypeEnum:
        break;

    case uTypeTypeStruct:
    {
        uint8_t* address = (uint8_t*)object + sizeof(uObject);
        uDumpAllStrongRefs(fp, object, address, type);
        break;
    }
    case uTypeTypeDelegate:
    {
        uDelegate* delegate = (uDelegate*)object;
        uDumpStrongRef(fp, object, "_object", delegate->_object);
        uDumpStrongRef(fp, object, "_prev", delegate->_prev);
        break;
    }
    case uTypeTypeArray:
    {
        uArray* array = (uArray*)object;
        uArrayType* arrayType = (uArrayType*)type;
        uType* elmType = arrayType->ElementType;

        switch (elmType->Type)
        {
        case uTypeTypeClass:
        case uTypeTypeInterface:
        case uTypeTypeDelegate:
        case uTypeTypeArray:
            for (int32_t i = 0; i < array->Length(); ++i)
            {
                uObject* target = ((uObject**)array->Ptr())[i];
                char label[20];
                sprintf(label, "[%d]", i);
                uDumpStrongRef(fp, object, label, target);
            }
            break;

        case uTypeTypeEnum:
            break;

        case uTypeTypeStruct:
            for (int32_t i = 0; i < array->Length(); ++i)
            {
                uint8_t* address = (uint8_t*)array->Ptr() + i * elmType->ValueSize;
                char labelPrefix[20];
                sprintf(labelPrefix, "[%d].", i);
                uDumpAllStrongRefs(fp, object, address, elmType, labelPrefix);
            }
            break;

        default:
            U_FATAL();
        }
        break;
    }

    default:
        U_FATAL();
    }
}

static void uDumpStaticStrongRefs(FILE* fp, uType* type)
{
    if (type == @{Uno.Type:TypeOf})
        return;

    const uReflection& reflection = type->Reflection;
    const size_t fullNameLength = strlen(type->FullName);
    for (size_t i = 0; i < reflection.FieldCount; i++)
    {
        uField* field = reflection.Fields[i];
        const uFieldInfo& fieldInfo = field->Info();
        if (U_IS_OBJECT(fieldInfo.Type) && ((fieldInfo.Flags & uFieldFlagsWeak) == 0) &&
            ((fieldInfo.Flags & uFieldFlagsStatic) != 0))
        {
            uObject* target = field->GetValue(nullptr);
            if (target)
            {
                uCString fieldName(field->Name);
                char *label = new char[fullNameLength + fieldName.Length + 2];
                sprintf(label, "%s.%s", type->FullName, fieldName.Ptr);
                uDumpGlobalRef(fp, (uObject**)fieldInfo.Address, label);
                delete[] label;
            }
        }
    }
}

void uDumpAllStrongRefs(const char* path)
{
    FILE* fp = fopen(path, "w");
    if (!fp)
        return;

    fprintf(fp, "digraph object_dump {\n");
    _Mutex.lock();

    for (auto it = _HeapObjects->begin();
         it != _HeapObjects->end(); ++it)
        uDumpObjectAndStrongRefs(fp, it->first);

    uArray* allTypes = uReflection::GetTypes();
    for (int32_t i = 0; i < allTypes->_length; ++i)
        uDumpStaticStrongRefs(fp, allTypes->Unsafe<uType*>(i));

    _Mutex.unlock();
    fprintf(fp, "}\n");
    fclose(fp);
}

#endif // DEBUG_DUMPS
