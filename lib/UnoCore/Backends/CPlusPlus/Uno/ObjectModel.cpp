// @(MSG_ORIGIN)
// @(MSG_EDIT_WARNING)

#include <Uno/_internal.h>
#include <ConvertUTF.h>
#include <algorithm>
#include <cstdio>
#include <mutex>
#include <string>
@{bool:IncludeDirective}
@{byte:IncludeDirective}
@{char:IncludeDirective}
@{float:IncludeDirective}
@{int:IncludeDirective}
@{object:IncludeDirective}
@{sbyte:IncludeDirective}
@{short:IncludeDirective}
@{string:IncludeDirective}
@{ushort:IncludeDirective}
@{Uno.Array:IncludeDirective}
@{Uno.ArgumentNullException:IncludeDirective}
@{Uno.ArgumentOutOfRangeException:IncludeDirective}
@{Uno.Delegate:IncludeDirective}
@{Uno.Enum:IncludeDirective}
@{Uno.IndexOutOfRangeException:IncludeDirective}
@{Uno.InvalidCastException:IncludeDirective}
@{Uno.InvalidOperationException:IncludeDirective}
@{Uno.NullReferenceException:IncludeDirective}
@{Uno.TypeInitializationException:IncludeDirective}
@{Uno.Type:IncludeDirective}
@{Uno.ValueType:IncludeDirective}

#ifdef DEBUG_GENERICS
#include <sstream>
static std::atomic_int _IndentCount;
#endif

static std::recursive_mutex _Mutex;
static std::unordered_map<std::string, uString*>* _StringConsts;
static std::unordered_map<uTypeKey, uType*, uTypeKeyHash>* _TypeMap;
static std::vector<uGenericType*>* _GenericTypes;
static std::vector<uType*>* _RuntimeTypes;
static uType* _ObjectTypePtr;
static uType* _VoidTypePtr;
static uType* _ByteTypePtr;
static uType* _SByteTypePtr;
static uType* _BoolTypePtr;
static uType* _ShortTypePtr;
static uType* _UShortTypePtr;
static uType* _CharTypePtr;
static uType* _FloatTypePtr;
static size_t _BuildIndex;
static bool _IsBuilding;

static void uInitOperators();
static void uBuildOperators(uType* type);
static uType* uGetGenericType(size_t index);
uType* uSwapThreadType(uType* type);

void uInvoke(const void* func, void** args = nullptr, size_t count = 0);
void uBuildMemory(uType* type);
#if @(REFLECTION:Defined)
void uBuildReflection(uType* type);
void uRegisterType(uType* type);
#endif

void uInitObjectModel()
{
    uInitOperators();

    _GenericTypes = new std::vector<uGenericType*>();
    _RuntimeTypes = new std::vector<uType*>();
    _StringConsts = new std::unordered_map<std::string, uString*>();
    _TypeMap = new std::unordered_map<uTypeKey, uType*, uTypeKeyHash>();

    // Create Uno.Object and Uno.Void type objects
    _ObjectTypePtr = (uType*)calloc(1, sizeof(uType));
    _VoidTypePtr = (uType*)calloc(1, sizeof(uType));

    memset(_ObjectTypePtr, 0, sizeof(uType));
    _ObjectTypePtr->Definition = _ObjectTypePtr;
    _ObjectTypePtr->Type = uTypeTypeClass;
    _ObjectTypePtr->FullName = "Uno.Object";
    _ObjectTypePtr->TypeSize = sizeof(uType);
    _ObjectTypePtr->fp_Equals = @{object.Equals(object):Function};
    _ObjectTypePtr->fp_GetHashCode = @{object.GetHashCode():Function};
    _ObjectTypePtr->fp_ToString = @{object.ToString():Function};
    _ObjectTypePtr->__retains = 1;

    memcpy(_VoidTypePtr, _ObjectTypePtr, sizeof(uType));
    _VoidTypePtr->Definition = _VoidTypePtr;
    _VoidTypePtr->FullName = "Uno.Void";
    _VoidTypePtr->Type = uTypeTypeVoid;

    _ObjectTypePtr->ObjectSize = sizeof(uObject);
    _ObjectTypePtr->ValueSize = sizeof(uObject*);
    _ObjectTypePtr->Alignment = alignof(uObject*);
    _ObjectTypePtr->__type = @{Uno.Type:TypeOf};
    _VoidTypePtr->__type = @{Uno.Type:TypeOf};
    _ByteTypePtr = @{byte:TypeOf};
    _SByteTypePtr = @{sbyte:TypeOf};
    _BoolTypePtr = @{bool:TypeOf};
    _ShortTypePtr = @{short:TypeOf};
    _UShortTypePtr = @{ushort:TypeOf};
    _CharTypePtr = @{char:TypeOf};
    _FloatTypePtr = @{float:TypeOf};
}

void uFreeObjectModel()
{
    delete _GenericTypes;
    delete _RuntimeTypes;
    delete _StringConsts;
    delete _TypeMap;
    free(_ObjectTypePtr);
    free(_VoidTypePtr);
}

uType* uObject_typeof()
{
    return _ObjectTypePtr;
}

uType* uVoid_typeof()
{
    return _VoidTypePtr;
}

static uType* uNewType(uint32_t type, const char* name, const uTypeOptions& options, bool isDefinition = true)
{
    bool isType = strcmp(name, "Uno.Type") == 0;
    uType* result =
        (uType*)uNew(
            isType
                ? _ObjectTypePtr
                : @{Uno.Type:TypeOf},
            options.TypeSize
                + options.GenericCount * sizeof(uType*)
                + options.MethodTypeCount * sizeof(uType*)
                + options.PrecalcCount * sizeof(uType*)
                + options.DependencyCount * sizeof(uType*)
                + options.FieldCount * sizeof(uFieldInfo)
                + options.InterfaceCount * sizeof(uInterfaceInfo));
    uint8_t* ptr = (uint8_t*)result + options.TypeSize;

    if (isType)
        result->__type = result;

    result->Type = type;
    result->FullName = name;
    result->Definition = result;
    result->ObjectSize = options.ObjectSize;
    result->ValueSize = options.ValueSize;
    result->TypeSize = options.TypeSize;
    result->Alignment = options.Alignment;

    result->InterfaceCount = options.InterfaceCount;
    result->Interfaces = (uInterfaceInfo*)ptr;
    ptr += options.InterfaceCount * sizeof(uInterfaceInfo);

    result->FieldCount = options.FieldCount;
    result->Fields = (uFieldInfo*)ptr;
    ptr += options.FieldCount * sizeof(uFieldInfo);

    result->GenericCount = options.GenericCount;
    result->Generics = (uType**)ptr;
    ptr += options.GenericCount * sizeof(uType*);

    result->MethodTypeCount = options.MethodTypeCount;
    result->MethodTypes = (uType**)ptr;
    ptr += options.MethodTypeCount * sizeof(uType*);

    result->PrecalcCount = options.PrecalcCount;
    result->PrecalcTypes = (uType**)ptr;
    ptr += options.PrecalcCount * sizeof(uType*);

    result->DependencyCount = options.DependencyCount;
    result->DependencyTypes = (uType**)ptr;
    ptr += options.DependencyCount * sizeof(uType*);

    if (isDefinition && result->GenericCount)
    {
        uTypeKey key(result);
        for (size_t i = 0; i < result->GenericCount; i++)
            key.Arguments[i] =
                result->Generics[i] =
                    uGetGenericType(i);
        (*_TypeMap)[key] = result;
        uRetain(result);
    }

    if (options.BaseDefinition)
    {
        uType* base = options.BaseDefinition;
        const size_t offset = offsetof(uType, fp_GetHashCode);
        memcpy((uint8_t*)result + offset, (uint8_t*)base + offset, base->TypeSize - offset);
        result->Base = base;
    }

    _RuntimeTypes->push_back(result);
    return result;
}

static uType* uGetGenericType(size_t index)
{
    for (size_t i = _GenericTypes->size(); i <= index; i++)
    {
        char* name = (char*)malloc(8); // Leak
        snprintf(name, 8, "T%u", (unsigned int) index);

        uTypeOptions options;
        options.TypeSize = sizeof(uGenericType);
        uGenericType* type = (uGenericType*)uNewType(uTypeTypeGeneric, name, options, false);
        type->GenericIndex = index;
        _GenericTypes->push_back(type);
        uRetain(type);
    }

    return (*_GenericTypes)[index];
}

uType* uType::NewMethodType(size_t genericCount, size_t precalcCount, size_t dependencyCount, bool isDefinition)
{
    size_t len = strlen(FullName) + 8;
    char* name = (char*)malloc(len); // Leak
    snprintf(name, len, "%s``%u", FullName, (unsigned int) genericCount);

    uTypeOptions options;
    options.GenericCount = GenericCount + genericCount;
    options.DependencyCount = dependencyCount;
    options.PrecalcCount = precalcCount;
    options.ObjectSize = 0;
    options.TypeSize = sizeof(uGenericType);
    uGenericType* type = (uGenericType*)uNewType(uTypeTypeGeneric, name, options, isDefinition);
    type->Base = this;
    type->GenericIndex = GenericCount;
    uRetain(type);
    return type;
}

uType* uType::NewMethodType(uType* definition)
{
    U_ASSERT(definition && definition->Type == uTypeTypeGeneric);
    uGenericType* gtype = (uGenericType*)definition;
    uType* result = NewMethodType(definition->GenericCount - gtype->GenericIndex, definition->PrecalcCount, definition->DependencyCount);
    result->Definition = definition;
    return result;
}

void uType::SetBase(uType* base)
{
    U_ASSERT(base && base->Definition == Base);
    Base = base;
}

void uType::SetInterfaces(uType* type, size_t offset, ...)
{
    U_ASSERT(InterfaceCount);
    Interfaces[0].Type = U_ASSERT_PTR(type);
    Interfaces[0].Offset = offset;

    va_list ap;
    va_start(ap, offset);

    for (size_t i = 1; i < InterfaceCount; i++)
    {
        Interfaces[i].Type = U_ASSERT_PTR(va_arg(ap, uType*));
        Interfaces[i].Offset = va_arg(ap, size_t);
    }

    va_end(ap);
}

void uType::SetFields(size_t inherited)
{
    U_ASSERT(inherited <= FieldCount);
    for (size_t i = 0; i < inherited; i++)
        Fields[i].Flags = uFieldFlagsInherited;
}

void uType::SetFields(size_t inherited, uType* type, uintptr_t offset, int flags, ...)
{
    SetFields(inherited);
    Fields[inherited].Type = U_ASSERT_PTR(type);
    Fields[inherited].Offset = offset;
    Fields[inherited].Flags = (uint8_t)flags;

    va_list ap;
    va_start(ap, flags);

    for (size_t i = inherited + 1; i < FieldCount; i++)
    {
        Fields[i].Type = U_ASSERT_PTR(va_arg(ap, uType*));
        Fields[i].Offset = va_arg(ap, uintptr_t);
        Fields[i].Flags = (uint8_t)va_arg(ap, int);
    }

    va_end(ap);
}

void uType::SetPrecalc(uType* first, ...)
{
    U_ASSERT(PrecalcCount);
    PrecalcTypes[0] = U_ASSERT_PTR(first);

    va_list ap;
    va_start(ap, first);

    for (size_t i = 1; i < PrecalcCount; i++)
        PrecalcTypes[i] = U_ASSERT_PTR(va_arg(ap, uType*));

    va_end(ap);
}

void uType::SetDependencies(uType* first, ...)
{
    U_ASSERT(DependencyCount);
    DependencyTypes[0] = U_ASSERT_PTR(first);

    va_list ap;
    va_start(ap, first);

    for (size_t i = 1; i < DependencyCount; i++)
        DependencyTypes[i] = U_ASSERT_PTR(va_arg(ap, uType*));

    va_end(ap);
}

uArrayType* uType::Array()
{
    if (_array)
        return _array;

    size_t len = strlen(FullName) + 3;
    char* name = (char*)malloc(len); // Leak
    snprintf(name, len, "%s[]", FullName);

    std::lock_guard<std::recursive_mutex> lock(_Mutex);
    uTypeOptions options;
    options.TypeSize = sizeof(uArrayType);
    options.ObjectSize = sizeof(uArray);
    options.ValueSize = sizeof(uArray*);
    options.Alignment = alignof(uArray*);
    options.BaseDefinition = @{Uno.Array:TypeOf};
    uArrayType* type = (uArrayType*)uNewType(uTypeTypeArray, name, options);
    type->Definition = type;
    type->ElementType = this;

    uRetain(type);
    return _array = type;
}

uByRefType* uType::ByRef()
{
    if (_byRef)
        return _byRef;

    size_t len = strlen(FullName) + 2;
    char* name = (char*)malloc(len); // Leak
    snprintf(name, len, "%s&", FullName);

    std::lock_guard<std::recursive_mutex> lock(_Mutex);
    uTypeOptions options;
    options.TypeSize = sizeof(uByRefType);
    options.ObjectSize = sizeof(uObject);
    options.ValueSize = sizeof(void*);
    options.Alignment = alignof(void*);
    uByRefType* type = (uByRefType*)uNewType(uTypeTypeByRef, name, options);
    type->Definition = _ObjectTypePtr;
    type->ValueType = this;

    uRetain(type);
    return _byRef = type;
}

static void uBuildTypes()
{
    if (_IsBuilding)
        return;

    _IsBuilding = true;

    while (_BuildIndex < _RuntimeTypes->size())
        (*_RuntimeTypes)[_BuildIndex++]->Build();

    _IsBuilding = false;
}

#ifdef DEBUG_GENERICS
static std::string uTypeString(uType* type, bool includeAddress = true)
{
    if (!type)
        return "(null)";

    std::stringstream sb;
    sb << type->FullName;

    if (type->GenericCount)
    {
        sb << "<";

        for (size_t i = 0; i < type->GenericCount; i++)
        {
            if (i > 0)
                sb << ", ";
            sb << uTypeString(type->Generics[i], false);
        }

        sb << ">";
    }

    if (includeAddress)
        sb << " 0x" << type;
    
    return sb.str();
}
#endif

static uType* uGetParameterization(const uTypeKey& key)
{
    std::lock_guard<std::recursive_mutex> lock(_Mutex);

    uType* result;
    auto it = _TypeMap->find(key);
    if (it != _TypeMap->end())
        result = it->second;
    else
    {
        uTypeOptions options;
        uType* def = key.Definition;
        options.FieldCount = def->FieldCount;
        options.GenericCount = def->GenericCount;
        options.MethodTypeCount = def->MethodTypeCount;
        options.InterfaceCount = def->InterfaceCount;
        options.DependencyCount = def->DependencyCount;
        options.PrecalcCount = def->PrecalcCount;
        options.ObjectSize = def->ObjectSize;
        options.TypeSize = def->TypeSize;
        options.ValueSize = def->ValueSize;
        options.Alignment = def->Alignment;
        result = uNewType(def->Type, def->FullName, options, false);
        result->Definition = def;

        if (result->Type == uTypeTypeGeneric)
        {
            result->Base = def->Base;
            ((uGenericType*)result)->GenericIndex = ((uGenericType*)def)->GenericIndex;
        }

        for (size_t i = 0; i < result->GenericCount; i++)
            result->Generics[i] = key.Arguments[i];
        for (size_t i = 0; i < result->MethodTypeCount; i++)
            result->MethodTypes[i] = result->NewMethodType(def->MethodTypes[i]);

        (*_TypeMap)[key] = result;
        uRetain(result);

        if (def->State)
            uBuildTypes();
    }

    return result;
}

uType* uGetParameterized(uType* type, uType* root)
{
    U_ASSERT(type && root);
    if (type == root || type->IsClosed())
        return type;

#ifdef DEBUG_GENERICS
    std::string indent(2 * _IndentCount++, ' ');
    U_LOG("%suGetParameterized(\n    %s%s,\n    %s%s)", indent.c_str(),
          indent.c_str(), uTypeString(type).c_str(),
          indent.c_str(), uTypeString(root).c_str());
#endif
    uType* result;
    if (type->GenericCount)
    {
        uTypeKey key(type->Definition);
        for (size_t i = 0; i < type->GenericCount; i++)
            key.Arguments[i] = uGetParameterized(type->Generics[i], root);

        result = uGetParameterization(key);
        U_ASSERT(result && result->Definition == type->Definition);
    }
    else switch (type->Type)
    {
    case uTypeTypeGeneric:
        result = root->T(((uGenericType*)type)->GenericIndex);
        break;
    case uTypeTypeArray:
        result = uGetParameterized(((uArrayType*)type)->ElementType, root)->Array();
        break;
    case uTypeTypeByRef:
        result = uGetParameterized(((uByRefType*)type)->ValueType, root)->ByRef();
        break;
    default:
        result = type;
        break;
    }

#ifdef DEBUG_GENERICS
    U_LOG("%s => %s", indent.c_str(), uTypeString(result).c_str());
    --_IndentCount;
#endif
    return result;
}

static size_t uCopyBaseFields(uType* type, uType* base, size_t& parentCount)
{
    if (!base)
        return 0;

    uType* def = base->Definition;
    U_ASSERT(def);
    def->Build();

    for (size_t i = uCopyBaseFields(type, base->Base, parentCount);
         i < def->FieldCount;
         i++)
    {
        uFieldInfo& b = def->Fields[i];
        if (b.Flags & uFieldFlagsStatic)
            return i;

        U_ASSERT(parentCount < type->FieldCount);
        uFieldInfo& f = type->Fields[parentCount++];
        U_ASSERT(f.Flags == uFieldFlagsInherited);
        f = b;

        if (base != def)
            f.Type = uGetParameterized(f.Type, base);
    }

    return def->FieldCount;
}

static void uBuildParameterization(uType* type)
{
    uType* def = type->Definition;
    U_ASSERT(def);

    if (type->Type == uTypeTypeGeneric)
    {
        // Precalc generic method types
        if (!type->GenericCount || def == type)
            return;

        def->Build();
        U_ASSERT(type->DependencyCount == def->DependencyCount &&
                 type->PrecalcCount == def->PrecalcCount &&
                 type->Base && def->Base &&
                 type->Base->Definition == def->Base->Definition);

        for (size_t i = 0; i < type->DependencyCount; i++)
            type->DependencyTypes[i] = uGetParameterized(def->DependencyTypes[i], type);
        for (size_t i = 0; i < type->PrecalcCount; i++)
            type->PrecalcTypes[i] = uGetParameterized(def->PrecalcTypes[i], type);
    }
    else if (def == type)
    {
        // Copy instance fields from base class
        size_t parentCount = 0;
        uCopyBaseFields(type, type->Base, parentCount);
    }
    else
    {
        def->Build();

        // Copy v-table from definition
        const size_t offset = offsetof(uType, fp_ctor_);
        memcpy((uint8_t*)type + offset, (uint8_t*)def + offset, def->TypeSize - offset);
        U_ASSERT(type->DependencyCount == def->DependencyCount &&
                 type->PrecalcCount == def->PrecalcCount &&
                 type->InterfaceCount == def->InterfaceCount &&
                 type->FieldCount == def->FieldCount);

        if (def->Base)
            type->Base = uGetParameterized(def->Base, type);
        for (size_t i = 0; i < type->DependencyCount; i++)
            type->DependencyTypes[i] = uGetParameterized(def->DependencyTypes[i], type);
        for (size_t i = 0; i < type->PrecalcCount; i++)
            type->PrecalcTypes[i] = uGetParameterized(def->PrecalcTypes[i], type);

        for (size_t i = 0; i < type->InterfaceCount; i++)
        {
            type->Interfaces[i].Type = uGetParameterized(def->Interfaces[i].Type, type);
            type->Interfaces[i].Offset = def->Interfaces[i].Offset;
        }
        for (size_t i = 0; i < type->FieldCount; i++)
        {
            type->Fields[i].Type = uGetParameterized(def->Fields[i].Type, type);
            type->Fields[i].Offset = def->Fields[i].Offset;
            type->Fields[i].Flags = def->Fields[i].Flags;
        }

        if (type->Type == uTypeTypeDelegate)
        {
            uDelegateType* typeDelegate = (uDelegateType*)type;
            uDelegateType* defDelegate = (uDelegateType*)def;
            typeDelegate->ParameterCount = defDelegate->ParameterCount;
            typeDelegate->ParameterTypes = (uType**)((uint8_t*)type + sizeof(uDelegateType));
            typeDelegate->ReturnType = uGetParameterized(defDelegate->ReturnType, type);

            for (size_t i = 0; i < typeDelegate->ParameterCount; i++)
                typeDelegate->ParameterTypes[i] = uGetParameterized(defDelegate->ParameterTypes[i], type);
        }
    }
}

static void uVerifyBuild(uType* type)
{
    // Sort interfaces by address for O(log n) look up.
    if (type->InterfaceCount)
    {
        struct {
            bool operator ()(const uInterfaceInfo& a, const uInterfaceInfo& b) 
            {
                return a.Type < b.Type;
            }
        } interfaceComparer;
        std::sort(type->Interfaces, type->Interfaces + type->InterfaceCount, interfaceComparer);

#ifdef DEBUG_UNSAFE
        for (size_t i = 1; i < type->InterfaceCount; i++)
            U_ASSERT(type->Interfaces[i].Type >= type->Interfaces[i - 1].Type);
#endif
    }
        

// TODO: Gives false positives on empty interfaces
/*
#ifdef DEBUG_UNSAFE
    void* vtable = type->Type == uTypeTypeClass
            ? (uint8_t*)type + sizeof(uClassType) :
        type->Type == uTypeTypeStruct
            ? (uint8_t*)type + sizeof(uStructType)
            : nullptr;

    // Verify that v-table is fully assigned (non-abstract type)
    if (vtable)
        for (void** ptr = (void**)vtable;
             (void*)ptr < (uint8_t*)type + type->TypeSize;
             ptr++)
            if (!*ptr)
                type->Flags |= uTypeFlagsAbstract;
#endif
*/

    // Check that operators are set
    U_ASSERT(type->Operators);
    U_ASSERT(type->Operators->fp_StorePtr);
    U_ASSERT(type->Operators->fp_StoreValue);
    U_ASSERT(type->Operators->fp_StoreStrong);
    U_ASSERT(type->Operators->fp_StoreValue);
}

uType* uType::GetBase(uType* def)
{
    for (uType* type = this; type; type = type->Base)
        if (type->Definition == def)
            return type;
    U_FATAL();
}

uType* uType::GetVirtual(size_t index, uType* method)
{
    U_ASSERT(index < MethodTypeCount && MethodTypes[index] &&
             method && method->Type == uTypeTypeGeneric);

    if (method->Base == this)
        return method;

    uGenericType* gmethod = (uGenericType*)method;
    size_t count = gmethod->GenericCount - gmethod->GenericIndex;
    U_ASSERT(count);

    uTypeKey key(MethodTypes[index]);
    memcpy(key.Arguments, Generics, GenericCount * sizeof(uType*));
    memcpy(key.Arguments + GenericCount, gmethod->Generics + gmethod->GenericIndex, count * sizeof(uType*));
    return uGetParameterization(key);
}

uType* uType::MakeGeneric(size_t count, uType** args)
{
    if (count != GenericCount)
        U_THROW_IOORE();

    uPtr(args);
    for (size_t i = 0; i < count; i++)
        uPtr(args[i]);

    return uGetParameterization(uTypeKey(this, args));
}

uType* uType::MakeMethod(size_t index, uType* first, ...)
{
    U_ASSERT(index < MethodTypeCount && MethodTypes[index]);

    uTypeKey key(MethodTypes[index]);
    size_t count = key.Definition->GenericCount - GenericCount;
    U_ASSERT(count > 0);

    memcpy(key.Arguments, Generics, GenericCount * sizeof(uType*));
    key.Arguments[GenericCount] = U_ASSERT_PTR(first);

    va_list ap;
    va_start(ap, first);

    for (size_t i = 1; i < count; i++)
        key.Arguments[GenericCount + i] = U_ASSERT_PTR(va_arg(ap, uType*));

    U_ASSERT(va_arg(ap, uType*) == nullptr);
    va_end(ap);
    return uGetParameterization(key);
}

uType* uType::MakeType(uType* first, ...)
{
    uTypeKey key(this);
    key.Arguments[0] = U_ASSERT_PTR(first);

    va_list ap;
    va_start(ap, first);

    for (size_t i = 1; i < GenericCount; i++)
        key.Arguments[i] = U_ASSERT_PTR(va_arg(ap, uType*));

    U_ASSERT(va_arg(ap, uType*) == nullptr);
    va_end(ap);
    return uGetParameterization(key);
}

uObject* uType::New()
{
    if (!U_IS_OBJECT(this) ||
            !fp_ctor_)
        return uNew(this);

    uObject* result;
    GenericCount > 0
        ? ((void(*)(uType*, uObject**))fp_ctor_)(this, &result)
        : ((void(*)(uObject**))fp_ctor_)(&result);
    return result;
}

void uType::Build()
{
    switch (State)
    {
    default:
        return;
    case uTypeStateUninitialized:
        State = uTypeStateBuilding;

        if (fp_build_)
            (*fp_build_)(this);

        uBuildParameterization(this);
        uBuildMemory(this);
        uBuildOperators(this);
#if @(REFLECTION:Defined)
        uBuildReflection(this);
#endif
        uVerifyBuild(this);
        State = uTypeStateBuilt;
        break;
    }
}

void uType::Init()
{
    if (State == uTypeStateInitialized)
        return;

    std::lock_guard<std::recursive_mutex> lock(_Mutex);

    Build();

    // Already initialized by another thread, or this thread is in the middle
    // of initializing the type
    if (State >= uTypeStateInitializing)
        return;

    auto prevState = State;
    State = uTypeStateInitializing;

    try
    {
        if (Base)
            Base->Init();

        // Note: If Base is the same as this, it won't start initializing again due
        // to the above early return, so we don't need to check the state again here

        for (size_t i = 0; i < PrecalcCount; i++)
            PrecalcTypes[i]->Init();

        try
        {
            if (fp_cctor_)
                (*fp_cctor_)(this);
        }
        catch (const uThrowable& t)
        {
            U_THROW(@{Uno.TypeInitializationException(string, Uno.Exception):New(uString::Utf8(FullName), t.Exception)});
        }

        for (size_t i = 0; i < DependencyCount; i++)
            DependencyTypes[i]->Init();
    }
    catch (...)
    {
        State = prevState;
        throw;
    }

    State = uTypeStateInitialized;
}

void uInitRtti(uType*(*factories[])())
{
    // 1. Fill _RuntimeTypes

    // Disable building while filling _RuntimeTypes
    U_ASSERT(!_IsBuilding);
    _IsBuilding = true;

    for (size_t i = 0; factories[i]; i++)
        (*factories[i])();

    _IsBuilding = false;

    // 2. ARC/RTTI calculations
    uBuildTypes();

    // Init char & string
    _VoidTypePtr->Init();
    _ObjectTypePtr->Init();
    @{char:TypeOf}->Init();
    @{string:TypeOf}->Init();
}

bool uType::IsClosed()
{
    if (Flags & uTypeFlagsClosedKnown)
        return (Flags & uTypeFlagsClosed) != 0;

    Flags |= uTypeFlagsClosedKnown;

    switch (Type)
    {
    case uTypeTypeByRef:
        if (((uByRefType*)this)->ValueType->IsClosed())
        {
            Flags |= uTypeFlagsClosed;
            return true;
        }
        return false;

    case uTypeTypeArray:
        if (((uArrayType*)this)->ElementType->IsClosed())
        {
            Flags |= uTypeFlagsClosed;
            return true;
        }
        return false;

    case uTypeTypeStruct:
    case uTypeTypeClass:
    case uTypeTypeDelegate:
    case uTypeTypeInterface:
        for (size_t i = 0; i < GenericCount; i++)
            if (!Generics[i]->IsClosed())
                return false;

        Flags |= uTypeFlagsClosed;
        return true;

    case uTypeTypeEnum:
        Flags |= uTypeFlagsClosed;
        return true;

    default:
        return false;
    }
}

bool uType::Is(const uType* type) const
{
    if (!type)
        return false;

    const uType* test = this;

    switch (type->Type)
    {
    case uTypeTypeInterface:
        do
        {
            intptr_t left = 0,
                     right = (intptr_t)test->InterfaceCount - 1;

            while (left <= right)
            {
                intptr_t mid = (left + right) >> 1;
                ptrdiff_t diff = (uint8_t*)type - (uint8_t*)test->Interfaces[mid].Type;

                if (diff > 0)
                    left = mid + 1;
                else if (diff < 0)
                    right = mid - 1;
                else
                    return true;
            }
        }
        while ((test = test->Base));
        break;

    default:
        do
            if (test == type)
                return true;
        while ((test = test->Base));
        break;
    }

    return false;
}

uClassType* uClassType::New(const char* name, uTypeOptions& options)
{
    if (options.ObjectSize)
    {
        if (!options.BaseDefinition)
            options.BaseDefinition = _ObjectTypePtr;
        options.ValueSize = sizeof(uObject*);
        options.Alignment = alignof(uObject*);
    }
    else
    {
        // Assert static class
        U_ASSERT(!options.ValueSize && !options.BaseDefinition);
    }

    uClassType* type = (uClassType*)uNewType(uTypeTypeClass, name, options);
#if @(REFLECTION:Defined)
    uRegisterType(type);
#endif
    return type;
}

uString* uString::Ansi(const char* cstr, size_t length)
{
    uString* string = New(length);

    for (size_t i = 0; i < length; i++)
        string->_ptr[i] = (char16_t)cstr[i];

    return string;
}

uString* uString::Ansi(const char* cstr)
{
    return cstr
        ? Ansi(cstr, strlen(cstr))
        : nullptr;
}

uString* uString::Utf8(const char* mutf8, size_t length)
{
	if (!length)
        return New(0);

    char* src = const_cast<char*>(mutf8);
    size_t null_count = 0;
    
    // Check for modified UTF-8
    for (size_t i = 0; i < length - 1; i++)
    {
        if (src[i + 0] == (char)(unsigned char)0xC0 &&
            src[i + 1] == (char)(unsigned char)0x80)
        {
            null_count++;
            i++;
        }
    }

    // Decode C0 80 -> 0 (modified UTF-8)
    if (null_count)
    {
        char* utf8 = (char*)malloc(length - null_count);
        char* utf8_p = utf8;

        for (size_t i = 0; i < length; i++)
        {
            if (src[i + 0] == (char)(unsigned char)0xC0 && i < length - 1 &&
                src[i + 1] == (char)(unsigned char)0x80)
            {
                *utf8_p++ = 0;
                i++;
            }
            else
                *utf8_p++ = src[i];
        }

        length -= null_count;
        src = utf8;
    }

    // Convert UTF-8 to UTF-16
    uString* string = New(length);
    const UTF8* src_p = (const UTF8*)src;
    UTF16* dst_p = (UTF16*)string->_ptr;

    if (ConvertUTF8toUTF16(&src_p, src_p + length, &dst_p, dst_p + length, lenientConversion) != conversionOK)
    {
        if (src != mutf8)
            free(src);
        U_THROW_IOE("Invalid UTF-8 string");
    }

    if (src != mutf8)
        free(src);

    string->_length = (size_t)(dst_p - (UTF16*)string->_ptr);
    string->_ptr[string->_length] = 0;
    return string;
}

uString* uString::Utf8(const char* mutf8)
{
    return mutf8
        ? Utf8(mutf8, strlen(mutf8))
        : nullptr;
}

uString* uString::Utf16(const char16_t* utf16, size_t length)
{
    uString* string = New(length);
    memcpy(string->_ptr, utf16, sizeof(char16_t) * length);
    return string;
}

uString* uString::Utf16(const char16_t* nullTerminatedUtf16)
{
    if (nullTerminatedUtf16)
    {
        const char16_t* end = nullTerminatedUtf16;
        while (*end)
            ++end;
        ptrdiff_t length = end - nullTerminatedUtf16;
        return Utf16(nullTerminatedUtf16, (size_t)length);
    }

    return nullptr;
}

uString* uString::CharArray(const uArray* array)
{
    if (!array)
        return New(0);

    U_ASSERT(array->GetType() == @{char[]:TypeOf});

    uString* string = New(array->Length());
    memcpy(string->_ptr, array->Ptr(), sizeof(char16_t) * array->Length());
    return string;
}

uString* uString::CharArrayRange(const uArray* array, size_t startIndex, size_t length)
{
    if (!array)
        throw uThrowable(@{Uno.ArgumentNullException(string):New(uString::Utf8("array"))}, __FILE__, __LINE__);

    if (startIndex > array->_length)
        throw uThrowable(@{Uno.ArgumentOutOfRangeException(string):New(uString::Utf8("startIndex"))}, __FILE__, __LINE__);

    if (startIndex + length > array->_length)
        throw uThrowable(@{Uno.ArgumentOutOfRangeException(string):New(uString::Utf8("length"))}, __FILE__, __LINE__);

    U_ASSERT(array->GetType() == @{char[]:TypeOf});

    uString* string = New(length);
    memcpy(string->_ptr, (char16_t*)array->Ptr() + startIndex, sizeof(char16_t) * length);
    return string;
}

uString* uString::Const(const char* mutf8)
{
    std::string key(mutf8);
    std::lock_guard<std::recursive_mutex> lock(_Mutex);
    auto it = _StringConsts->find(key);
    if (it != _StringConsts->end())
        return it->second;

    uString* string = Utf8(key.c_str(), key.size());
    (*_StringConsts)[key] = string;
    uRetain(string);
    return string;
}

static bool uCompareCharStrings(const char16_t* a, const char16_t* b, size_t length, bool ignoreCase)
{
    if (ignoreCase)
    {
        for (size_t i = 0; i < length; i++)
            if (a[i] != b[i] && @{char.ToUpper(char):Call(a[i])} != @{char.ToUpper(char):Call(b[i])})
                return false;

        return true;
    }

    return memcmp(a, b, sizeof(char16_t) * length) == 0;
}

bool uString::Equals(const uString* a, const uString* b, bool ignoreCase)
{
    return a == b || (a && b && a->Length() == b->Length() && uCompareCharStrings(a->Ptr(), b->Ptr(), a->Length(), ignoreCase));
}

char* uAllocCStr(const uString* string, size_t* length)
{
    if (!string)
    {
        if (length)
            *length = 0;
        return nullptr;
    }

    // Convert UTF-16 to UTF-8
    size_t src_len = string->_length;
    const UTF16* src_p = (const UTF16*)string->_ptr;

    size_t dst_len = src_len * 4;
    char* dst = (char*)malloc(dst_len + 1);
    UTF8* dst_p = (UTF8*)dst;

    if (ConvertUTF16toUTF8(&src_p, src_p + src_len, &dst_p, dst_p + dst_len, lenientConversion) != conversionOK)
    {
        free(dst);
        U_THROW_IOE("Invalid UTF-16 string");
    }

    dst_len = (size_t)(dst_p - (UTF8*)dst);
    size_t null_count = 0;

    // Check for modified UTF-8
    for (size_t i = 0; i < dst_len; i++)
        if (!dst[i])
            null_count++;

    // Encode 0 -> C0 80 (modified UTF-8)
    if (null_count)
    {
        char* mutf8 = (char*)malloc(dst_len + null_count + 1);
        char* mutf8_p = mutf8;

        for (size_t i = 0; i < dst_len; i++)
        {
            char c = dst[i];
            if (c)
                *mutf8_p++ = c;
            else
            {
                *mutf8_p++ = (char)(unsigned char)0xC0;
                *mutf8_p++ = (char)(unsigned char)0x80;
            }
        }

        free(dst);
        dst_len += null_count;
        dst = mutf8;
    }

    if (length)
        *length = dst_len;

    dst[dst_len] = 0;
    return dst;
}

void uFreeCStr(const char* cstr)
{
    free((void*)cstr);
}

uEnumType* uEnumType::New(const char* name, uType* base, size_t literalCount)
{
    U_ASSERT(base);
    uTypeOptions options;
    options.ObjectSize = base->ObjectSize;
    options.TypeSize = sizeof(uEnumType) + literalCount * sizeof(uEnumLiteral);
    options.ValueSize = base->ValueSize;
    options.Alignment = base->Alignment;
    options.BaseDefinition = _ObjectTypePtr; // We want to copy vtable from object

    uEnumType* type = (uEnumType*)uNewType(uTypeTypeEnum, name, options);
    type->Base = base;
    type->LiteralCount = literalCount;
    type->Literals = (uEnumLiteral*)((uint8_t*)type + sizeof(uEnumType));
    type->fp_GetHashCode = @{Uno.ValueType.GetHashCode():Function};
    type->fp_Equals = @{Uno.ValueType.Equals(object):Function};
    type->fp_ToString = @{Uno.Enum.ToString():Function};

#if @(REFLECTION:Defined)
    uRegisterType(type);
#endif
    return type;
}

// Uno.Long (int64_t) can represent all enum values
void uEnumType::SetLiterals(const char* name, int64_t value, ...)
{
    Literals[0].Name = uString::Const(name);
    Literals[0].Value = value;

    va_list ap;
    va_start(ap, value);

    for (size_t i = 1; i < LiteralCount; i++)
    {
        Literals[i].Name = uString::Const(va_arg(ap, const char*));
        Literals[i].Value = va_arg(ap, int64_t);
    }

    va_end(ap);
}

static int64_t uLoadEnum(uEnumType* type, void* value)
{
    // See if value is unsigned (Uno.Byte|Uno.U*)
    char16_t fifth = type->Base->FullName[4];
    bool isUnsigned = fifth == 'B' || fifth == 'U';

    switch (type->ValueSize)
    {
    case 1: return isUnsigned
        ? (int64_t)*(uint8_t*)value
        : (int64_t)*(int8_t*)value;
    case 2: return isUnsigned
        ? (int64_t)*(uint16_t*)value
        : (int64_t)*(int16_t*)value;
    case 4: return isUnsigned
        ? (int64_t)*(uint32_t*)value
        : (int64_t)*(int32_t*)value;
    case 8: return isUnsigned
        ? (int64_t)*(uint64_t*)value
        : *(int64_t*)value;
    default:
        U_FATAL();
    }
}

static bool uStoreEnum(uEnumType* type, int64_t value, void* result)
{
    // See if value is unsigned (Uno.Byte|Uno.U*)
    char16_t fifth = type->Base->FullName[4];
    bool isUnsigned = fifth == 'B' || fifth == 'U';

    switch (type->ValueSize)
    {
    case 1: isUnsigned
        ? *(uint8_t*)result = (uint8_t)value
        : *(int8_t*)result = (int8_t)value;
        return true;
    case 2: isUnsigned
        ? *(int16_t*)result = (int16_t)value
        : *(uint16_t*)result = (uint16_t)value;
        return true;
    case 4: isUnsigned
        ? *(int32_t*)result = (int32_t)value
        : *(uint32_t*)result = (uint32_t)value;
        return true;
    case 8: isUnsigned
        ? *(uint64_t*)result = (uint64_t)value
        : *(int64_t*)result = value;
        return true;
    default:
        memset(result, 0, type->ValueSize);
        return false;
    }
}

bool uEnum::TryParse(uType* type, uString* value, bool ignoreCase, uTRef result)
{
    if (!type || !value || type->Type != uTypeTypeEnum)
        return false;

    uEnumType* enumType = (uEnumType*)type;
    for (size_t i = 0; i < enumType->LiteralCount; i++)
        if (uString::Equals(enumType->Literals[i].Name, value, ignoreCase))
            return uStoreEnum(enumType, enumType->Literals[i].Value, result._address);

    memset(result._address, 0, type->ValueSize);
    return false;
}

uString* uEnum::GetString(uType* type, void* value)
{
    if (!value || !type)
        U_THROW_NRE();
    if (type->Type != uTypeTypeEnum || !type->Base)
        U_THROW_ICE();

    uEnumType* enumType = (uEnumType*)type;
    int64_t litVal = uLoadEnum(enumType, value);

    for (size_t i = 0; i < enumType->LiteralCount; i++)
        if (litVal == enumType->Literals[i].Value)
            return enumType->Literals[i].Name;

    // Call base.ToString()
    uString* result;
    uStructType* base = (uStructType*)type->Base;
    U_ASSERT(base && base->fp_ToString_struct);
    return (*base->fp_ToString_struct)(value, base, &result), result;
}

static void uStruct_GetHashCode(uObject* object, int32_t* result)
{
    U_ASSERT(object);
    uStructType* type = (uStructType*)object->__type;
    type->fp_GetHashCode_struct
        ? (*type->fp_GetHashCode_struct)((uint8_t*)object + sizeof(uObject), type, result)
        : @{Uno.ValueType.GetHashCode():Function}(object, result);
}

static void uStruct_Equals(uObject* obj1, uObject* obj2, bool* result)
{
    U_ASSERT(obj1);
    uStructType* type = (uStructType*)obj1->__type;
    type->fp_Equals_struct
        ? (*type->fp_Equals_struct)((uint8_t*)obj1 + sizeof(uObject), type, obj2, result)
        : @{Uno.ValueType.Equals(object):Function}(obj1, obj2, result);
}

static void uStruct_ToString(uObject* object, uString** result)
{
    U_ASSERT(object);
    uStructType* type = (uStructType*)object->__type;
    type->fp_ToString_struct
        ? (*type->fp_ToString_struct)((uint8_t*)object + sizeof(uObject), type, result)
        : @{object.ToString():Function}(object, result);
}

uStructType* uStructType::New(const char* name, uTypeOptions& options)
{
    options.ObjectSize = options.ValueSize + sizeof(uObject);
    options.BaseDefinition = _ObjectTypePtr;
    uStructType* type = (uStructType*)uNewType(uTypeTypeStruct, name, options);
    type->fp_GetHashCode = uStruct_GetHashCode;
    type->fp_Equals = uStruct_Equals;
    type->fp_ToString = uStruct_ToString;
#if @(REFLECTION:Defined)
    uRegisterType(type);
#endif
    return type;
}

#define INLINE_MEMCPY(DST, SRC, SIZE) \
    switch (SIZE) \
    { \
    case 0: \
        break; \
    case 1: \
        *(uint8_t*)(DST) = *(const uint8_t*)(SRC); \
        break; \
    case 2: \
        *(uint16_t*)(DST) = *(const uint16_t*)(SRC); \
        break; \
    case 4: \
        *(uint32_t*)(DST) = *(const uint32_t*)(SRC); \
        break; \
    case 8: \
        *(uint64_t*)(DST) = *(const uint64_t*)(SRC); \
        break; \
    default: \
        memcpy(DST, SRC, SIZE); \
        break; \
    }

uObject* uBoxPtr(uType* type, const void* src, void* stack, bool ref)
{
    switch (type->Type)
    {
    case uTypeTypeEnum:
    case uTypeTypeStruct:
    {
        uObject* object;
        void* ptr;

        if (stack)
        {
            object = (uObject*)stack;
            ptr = (uint8_t*)stack + sizeof(uObject);
            memset(stack, 0, sizeof(uObject));
            object->__type = type;
        }
        else
        {
            object = uNew(type);
            ptr = (uint8_t*)object + sizeof(uObject);

            if (type->Flags & uTypeFlagsRetainStruct)
                uRetainStruct(type, const_cast<void*>(src));
        }

        INLINE_MEMCPY(ptr, src, type->ValueSize);
        return object;
    }
    case uTypeTypeVoid:
        return nullptr;
    default:
        return ref
            ? *U_ASSERT_PTR((uObject**)src)
            : (uObject*)src;
    }
}

void uUnboxPtr(uType* type, uObject* object, void* dst)
{
    switch (type->Type)
    {
    case uTypeTypeEnum:
    case uTypeTypeStruct:
        if (uPtr(object)->__type != type)
            U_THROW_ICE();
        INLINE_MEMCPY(dst, (const uint8_t*)object + sizeof(uObject), type->ValueSize);
        if (type->Flags & uTypeFlagsRetainStruct)
            uRetainStruct(type, dst);
        break;
    case uTypeTypeVoid:
        break;
    default:
        U_ASSERT(dst);
        *(uObject**)dst = object;
        break;
    }
}

uInterfaceType* uInterfaceType::New(const char* name, size_t genericCount, size_t methodCount)
{
    uTypeOptions options;
    options.GenericCount = genericCount;
    options.MethodTypeCount = methodCount;
    options.TypeSize = sizeof(uInterfaceType);
    options.ObjectSize = sizeof(uObject);
    options.ValueSize = sizeof(uObject*);
    options.Alignment = alignof(uObject*);
    options.BaseDefinition = _ObjectTypePtr;
    uInterfaceType* type = (uInterfaceType*)uNewType(uTypeTypeInterface, name, options);
#if @(REFLECTION:Defined)
    uRegisterType(type);
#endif
    return type;
}

const void* uType::InterfacePtr(const uObject* object)
{
    uType* type = object->__type;
    do
    {
        intptr_t left = 0,
                 right = (intptr_t)type->InterfaceCount - 1;

        while (left <= right)
        {
            intptr_t mid = (left + right) >> 1;
            ptrdiff_t diff = (uint8_t*)this - (uint8_t*)type->Interfaces[mid].Type;

            if (diff > 0)
                left = mid + 1;
            else if (diff < 0)
                right = mid - 1;
            else
                return (const uint8_t*)type + type->Interfaces[mid].Offset;
        }
    }
    while ((type = type->Base));
    U_FATAL();
}

uDelegateType* uDelegateType::New(const char* name, size_t paramCount, size_t genericCount)
{
    uTypeOptions options;
    options.GenericCount = genericCount;
    options.TypeSize = sizeof(uDelegateType) + paramCount * sizeof(uType*);
    options.ObjectSize = sizeof(uDelegate);
    options.ValueSize = sizeof(uDelegate*);
    options.Alignment = alignof(uDelegate*);
    options.BaseDefinition = @{Uno.Delegate:TypeOf};

    uDelegateType* type = (uDelegateType*)uNewType(uTypeTypeDelegate, name, options);
    type->ParameterCount = paramCount;
    type->ParameterTypes = (uType**)((uint8_t*)type + sizeof(uDelegateType));
#if @(REFLECTION:Defined)
    uRegisterType(type);
#endif
    return type;
}

void uDelegateType::SetSignature(uType* returnType, ...)
{
    ReturnType = returnType;

    va_list ap;
    va_start(ap, returnType);

    for (size_t i = 0; i < ParameterCount; i++)
        ParameterTypes[i] = U_ASSERT_PTR(va_arg(ap, uType*));

    va_end(ap);
}

void uDelegate::InvokeVoid()
{
    if (_prev != nullptr)
        _prev->InvokeVoid();

    uDelegateType* type = (uDelegateType*)__type;
    if (type->ParameterCount != 0)
        U_THROW_IOORE();
    if (!U_IS_VOID(type->ReturnType))
        U_THROW_ICE();

    _this && _generic
        ? ((void(*)(void*, void*))_func)(_this, _generic) :
    _this
        ? ((void(*)(void*))_func)(_this) :
    _generic
        ? ((void(*)(void*))_func)(_generic)
        : ((void(*)())_func)();
}

void uDelegate::InvokeVoid(void* arg)
{
    if (_prev != nullptr)
        _prev->InvokeVoid(arg);

    uDelegateType* type = (uDelegateType*)__type;
    if (type->ParameterCount != 1)
        U_THROW_IOORE();
    if (!U_IS_VOID(type->ReturnType))
        U_THROW_ICE();

    _this && _generic
        ? ((void(*)(void*, void*, void*))_func)(_this, _generic, arg) :
    _this
        ? ((void(*)(void*, void*))_func)(_this, arg) :
    _generic
        ? ((void(*)(void*, void*))_func)(_generic, arg)
        : ((void(*)(void*))_func)(arg);
}

void uDelegate::Invoke(uTRef retval, void** args, size_t count)
{
    if (_prev != nullptr)
        _prev->Invoke(retval, args, count);

    uDelegateType* type = (uDelegateType*)__type;
    size_t parameterCount = type->ParameterCount;

    if (count != parameterCount)
        U_THROW_IOORE();

    if (_this)
        count++;
    if (_generic)
        count++;
    if (retval._address)
        count++;

    void** stack = (void**)alloca(count * sizeof(void*));
    void** ptr = stack;

    if (_this)
        *ptr++ = _this;
    if (_generic)
        *ptr++ = _generic;

    memcpy(ptr, args, parameterCount * sizeof(void*));
    ptr += parameterCount;

    if (retval._address)
        *ptr++ = retval._address;

    uInvoke(_func, stack, count);
}

void uDelegate::Invoke(uTRef retval, size_t count, ...)
{
    va_list ap;
    va_start(ap, count);
    void** args = count > 0
        ? (void**)alloca(count * sizeof(void*))
        : nullptr;

    for (size_t i = 0; i < count; i++)
        args[i] = va_arg(ap, void*);

    va_end(ap);
    Invoke(retval, args, count);
}

uObject* uDelegate::Invoke(uArray* array)
{
    uDelegateType* type = (uDelegateType*)GetType();
    size_t count = type->ParameterCount;
    uType** params = type->ParameterTypes;
    void** args = nullptr;

    if (array)
    {
        if (!U_IS_OBJECT(((uArrayType*)array->GetType())->ElementType))
            U_THROW_ICE();
        if (count != array->Length())
            U_THROW_IOORE();

        uObject** objects = (uObject**)array->Ptr();
        void** ptr = args = count > 0
            ? (void**)alloca(count * sizeof(void*))
            : nullptr;

        for (size_t i = 0; i < count; i++)
        {
            uType* param = *params++;
            uObject* object = *objects++;

            switch (param->Type)
            {
            case uTypeTypeEnum:
            case uTypeTypeStruct:
                *ptr++ = (uint8_t*)object + sizeof(uObject);
                break;
            case uTypeTypeByRef:
                *ptr++ = U_IS_VALUE(((uByRefType*)param)->ValueType)
                        ? (uint8_t*)object + sizeof(uObject)
                        : (void*)&object;
                break;
            case uTypeTypeClass:
            case uTypeTypeDelegate:
            case uTypeTypeInterface:
            case uTypeTypeArray:
                *ptr++ = object;
                break;
            default:
                U_FATAL();
            }
        }
    }

    uType* returnType = type->ReturnType;
    void* retval = !U_IS_VOID(returnType)
        ? alloca(returnType->ValueSize)
        : nullptr;

    Invoke(retval, args, count);
    return uBoxPtr(returnType, retval, nullptr, true);
}

uObject* uDelegate::Invoke(size_t count, ...)
{
    va_list ap;
    va_start(ap, count);
    void** args = count > 0
        ? (void**)alloca(count * sizeof(void*))
        : nullptr;

    for (size_t i = 0; i < count; i++)
        args[i] = va_arg(ap, void*);

    va_end(ap);
    uType* returnType = ((uDelegateType*)__type)->ReturnType;
    void* retval = !U_IS_VOID(returnType)
        ? alloca(returnType->ValueSize)
        : nullptr;

    Invoke(retval, args, count);
    return uBoxPtr(returnType, retval, nullptr, true);
}

uDelegate* uDelegate::Copy()
{
    uDelegate* delegate = (uDelegate*)uNew(__type);
    delegate->_this = _this;
    delegate->_func = _func;
    delegate->_object = _object;
    delegate->_generic = _generic;
    return delegate;
}

uDelegate* uDelegate::New(uType* type, const void* func, uObject* object, uType* generic)
{
    U_ASSERT(func);
    uDelegate* delegate = (uDelegate*)uNew(type);
    delegate->_func = func;
    delegate->_generic = generic;
    delegate->_object = object;
    delegate->_this = object && U_IS_VALUE(object->GetType())
                    ? (uint8_t*)object + sizeof(uObject)
                    : (void*)object;
    return delegate;
}

uDelegate* uDelegate::New(uType* type, uObject* object, size_t offset, uType* generic)
{
    U_ASSERT(offset && offset < 0xffff); // Max ‭8191‬ virtual functions is enough for now
    uDelegate* result = New(type, *(void**)((uint8_t*)uPtr(object)->GetType() + offset), object, generic);
    result->_this = object;
    return result;
}

uDelegate* uDelegate::New(uType* type, const uInterface& iface, size_t offset, uType* generic)
{
    U_ASSERT(iface._object && iface._vtable);
    return New(type, iface._object, (size_t)((uint8_t*)iface._vtable - (uint8_t*)iface._object->__type) + offset, generic);
}

void uArray::MarshalPtr(size_t index, const void* value, size_t size)
{
    uType* type = ((uArrayType*)__type)->ElementType;
    void* item = (uint8_t*)_ptr + type->ValueSize * index;

    if (type->ValueSize == size)
    {
        INLINE_MEMCPY(item, value, size);

        if (U_IS_OBJECT(type))
            uRetain(*(uObject**)item);
    }
    else
    {
        // Cast value back to correct type (or throw exception)
        // * small ints are promoted to 'int' when passed through '...'
        // * floats are promoted to 'double' when passed through '...'
        switch (size)
        {
        case sizeof(int):
            switch (type->ValueSize)
            {
            case 1:
                if (type == _ByteTypePtr)
                    return *(uint8_t*)item = (uint8_t)*(int*)value, void();
                else if (type == _SByteTypePtr)
                    return *(int8_t*)item = (int8_t)*(int*)value, void();
                else if (type == _BoolTypePtr)
                    return *(bool*)item = *(int*)value != 0, void();
                break;

            case 2:
                if (type == _ShortTypePtr)
                    return *(int16_t*)item = (int16_t)*(int*)value, void();
                else if (type == _UShortTypePtr)
                    return *(uint16_t*)item = (uint16_t)*(int*)value, void();
                else if (type == _CharTypePtr)
                    return *(char16_t*)item = (char16_t)*(int*)value, void();
                break;
            }

        case sizeof(double):
            if (type == _FloatTypePtr)
                return *(float*)item = (float)*(double*)value, void();
            break;
        }

        U_THROW_ICE();
    }
}

uArray* uArray::InitT(uType* type, size_t length, ...)
{
    va_list ap;
    va_start(ap, length);
    uArray* array = New(type, length);

    for (size_t i = 0; i < length; i++)
    {
        const void* src = va_arg(ap, const void*);
        array->TUnsafe(i) = src;
    }

    va_end(ap);
    return array;
}

uArray* uArray::New(uType* type, size_t length, const void* optionalData)
{
    U_ASSERT(type && type->Type == uTypeTypeArray);
    uArrayType* arrayType = (uArrayType*)type;
    uType* elementType = arrayType->ElementType;
    size_t elementSize = elementType->ValueSize;
    uArray* array = (uArray*)uNew(type, sizeof(uArray) + elementSize * length);
    array->_ptr = (uint8_t*)array + sizeof(uArray);
    array->_length = length;

    if (optionalData)
    {
        memcpy(array->Ptr(), optionalData, elementSize * length);

        if (U_IS_OBJECT(elementType))
            for (size_t i = 0; i < length; i++)
                uRetain(((uObject**)array->Ptr())[i]);
        else if (elementType->Flags & uTypeFlagsRetainStruct)
            for (size_t i = 0; i < length; i++)
                uRetainStruct(elementType, (uint8_t*)array->Ptr() + elementType->ValueSize * i);
    }

    return array;
}

const char* uThrowable::what() const throw()
{
    return Exception->__type->FullName;
}

void uThrowable::ThrowNullReference(const char* file, int line)
{
    throw uThrowable(@{Uno.NullReferenceException():New()}, file, line);
}

void uThrowable::ThrowInvalidCast(const char* file, int line)
{
    throw uThrowable(@{Uno.InvalidCastException():New()}, file, line);
}

void uThrowable::ThrowInvalidCast(const uType* from, const uType* to, const char* file, int line)
{
    std::string fromType = from ? from->FullName : "<Unknown type>";
    std::string toType = to ? to->FullName : "<Unknown type>";
    std::string message = "Unable to cast object of type '" + fromType + "' to type '" + toType + "'.";
    throw uThrowable(@{Uno.InvalidCastException(string):New(uString::Utf8(message.c_str(), message.length()))}, file, line);
}

void uThrowable::ThrowInvalidOperation(const char* message, const char* file, int line)
{
    throw uThrowable(@{Uno.InvalidOperationException(string):New(uString::Utf8(message))}, file, line);
}

void uThrowable::ThrowIndexOutOfRange(const char* file, int line)
{
    throw uThrowable(@{Uno.IndexOutOfRangeException():New()}, file, line);
}

// Type specific operators

static void uObject_StorePtr(uType* type, const void* src, void* dst)
{
    U_ASSERT(dst && src);
    *(uObject**)dst = *(uObject**)src;
    U_ASSERT(!*(uObject**)dst || uIs(*(uObject**)dst, type));
}

static void uObject_StoreValue(uType* type, const void* src, void* dst)
{
    U_ASSERT(dst);
    *(uObject**)dst = (uObject*)src;
    U_ASSERT(!*(uObject**)dst || uIs(*(uObject**)dst, type));
}

static void uObject_StoreStrong(uType* type, const void* src, void* dst)
{
    U_ASSERT(dst && src);
    *(uStrong<uObject*>*)dst = *(uObject**)src;
    U_ASSERT(!*(uObject**)dst || uIs(*(uObject**)dst, type));
}

static void uObject_StoreStrongValue(uType* type, const void* src, void* dst)
{
    U_ASSERT(dst);
    *(uStrong<uObject*>*)dst = (uObject*)src;
    U_ASSERT(!*(uObject**)dst || uIs(*(uObject**)dst, type));
}

static void uStruct_StoreN(uType* type, const void* src, void* dst)
{
    U_ASSERT(dst && src && type);
    memcpy(dst, src, type->ValueSize);
}

static void uStruct_StoreStrong(uType* type, const void* src, void* dst)
{
    U_ASSERT(dst && src && type);
    uAutoReleaseStruct(type, dst);
    memcpy(dst, src, type->ValueSize);
    uRetainStruct(type, dst);
}

template<int size>
static void uStruct_Store(uType* type, const void* src, void* dst)
{
    U_ASSERT(dst && src && type && type->ValueSize == size);
    memcpy(dst, src, size);
}

template<int size>
uOperatorTable* uCreateFixedSizeOperators()
{
    uOperatorTable* result = new uOperatorTable;
    result->fp_StorePtr = uStruct_Store<size>;
    result->fp_StoreValue = uStruct_Store<size>;
    result->fp_StoreStrong = uStruct_Store<size>;
    result->fp_StoreStrongValue = uStruct_Store<size>;
    return result;
}

static uOperatorTable* _Object_Operators;
static uOperatorTable* _Struct_Operators1;
static uOperatorTable* _Struct_Operators2;
static uOperatorTable* _Struct_Operators4;
static uOperatorTable* _Struct_Operators8;
static uOperatorTable* _Struct_Operators12;
static uOperatorTable* _Struct_Operators16;
static uOperatorTable* _Struct_OperatorsN;
static uOperatorTable* _Struct_OperatorsStrong;

static void uInitOperators()
{
    _Object_Operators = new uOperatorTable;
    _Object_Operators->fp_StorePtr = uObject_StorePtr;
    _Object_Operators->fp_StoreValue = uObject_StoreValue;
    _Object_Operators->fp_StoreStrong = uObject_StoreStrong;
    _Object_Operators->fp_StoreStrongValue = uObject_StoreStrongValue;

    _Struct_Operators1 = uCreateFixedSizeOperators<1>();
    _Struct_Operators2 = uCreateFixedSizeOperators<2>();
    _Struct_Operators4 = uCreateFixedSizeOperators<4>();
    _Struct_Operators8 = uCreateFixedSizeOperators<8>();
    _Struct_Operators12 = uCreateFixedSizeOperators<12>();
    _Struct_Operators16 = uCreateFixedSizeOperators<16>();

    _Struct_OperatorsN = new uOperatorTable;
    _Struct_OperatorsN->fp_StorePtr = uStruct_StoreN;
    _Struct_OperatorsN->fp_StoreValue = uStruct_StoreN;
    _Struct_OperatorsN->fp_StoreStrong = uStruct_StoreN;
    _Struct_OperatorsN->fp_StoreStrongValue = uStruct_StoreN;

    _Struct_OperatorsStrong = new uOperatorTable;
    _Struct_OperatorsStrong->fp_StorePtr = uStruct_StoreStrong;
    _Struct_OperatorsStrong->fp_StoreValue = uStruct_StoreStrong;
    _Struct_OperatorsStrong->fp_StoreStrong = uStruct_StoreStrong;
    _Struct_OperatorsStrong->fp_StoreStrongValue = uStruct_StoreStrong;
}

static uOperatorTable* uGetOperators(uType* type)
{
    if (U_IS_OBJECT(type))
        return _Object_Operators;
    if (type->Flags & uTypeFlagsRetainStruct)
        return _Struct_OperatorsStrong;

    switch (type->ValueSize)
    {
    case 1:
        return _Struct_Operators1;
    case 2:
        return _Struct_Operators2;
    case 4:
        return _Struct_Operators4;
    case 8:
        return _Struct_Operators8;
    case 12:
        return _Struct_Operators12;
    case 16:
        return _Struct_Operators16;
    default:
        return _Struct_OperatorsN;
    }
}

static void uBuildOperators(uType* type)
{
    if (type->Type == uTypeTypeStruct && type->Refs.StrongCount)
        type->Flags |= uTypeFlagsRetainStruct;

    type->Operators = uGetOperators(type);
}
