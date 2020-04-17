// @(MSG_ORIGIN)
// @(MSG_EDIT_WARNING)

#include <Uno/Reflection.h>
#include <string>
#include <unordered_map>
@{int:IncludeDirective}
@{bool:IncludeDirective}
@{string:IncludeDirective}
@{Uno.Exception:IncludeDirective}
@{Uno.Type:IncludeDirective}

uType* uGetParameterized(uType* type, uType* root);
void uInvoke(const void* func, void** args = nullptr, size_t count = 0);

static std::unordered_map<std::string, uType*>* _Types;

void uInitReflection()
{
    _Types = new std::unordered_map<std::string, uType*>();
}

void uFreeReflection()
{
    delete _Types;
}

void uRegisterType(uType* type)
{
    (*_Types)[type->FullName] = type;
}

void uRegisterIntrinsics()
{
    @{object:TypeOf}->Reflection.SetFunctions(3,
        new uFunction("Equals", nullptr, nullptr, offsetof(uType, fp_Equals), false, @{bool:TypeOf}, 1, @{object:TypeOf}),
        new uFunction("GetHashCode", nullptr, nullptr, offsetof(uType, fp_GetHashCode), false, @{int:TypeOf}),
        new uFunction("ToString", nullptr, nullptr, offsetof(uType, fp_ToString), false, @{string:TypeOf}));

    uRegisterType(@{object:TypeOf});
    uRegisterType(@{void:TypeOf});
}

void uBuildReflection(uType* type)
{
    U_ASSERT(type);
    uType* def = type->Definition;

    if (type == def)
        return;

    U_ASSERT(def);
    type->Reflection.FieldCount = def->Reflection.FieldCount;
    type->Reflection.FunctionCount = def->Reflection.FunctionCount;
    type->Reflection.Fields = (uField**)malloc(sizeof(uField*) * def->Reflection.FieldCount); // Leak
    type->Reflection.Functions = (uFunction**)malloc(sizeof(uFunction*) * def->Reflection.FunctionCount); // Leak

    for (size_t i = 0; i < def->Reflection.FieldCount; i++)
    {
        uField* f = def->Reflection.Fields[i];
        type->Reflection.Fields[i] = new uField(type, f->Name, f->_index);
    }

    for (size_t i = 0; i < def->Reflection.FunctionCount; i++)
    {
        uFunction* f = def->Reflection.Functions[i];
        uArray* paramTypes = @{Uno.Type[]:New(f->ParameterTypes->Length())};

        for (int32_t j = 0; j < paramTypes->Length(); j++)
            paramTypes->UnsafeStrong<uType*>(j) = uGetParameterized(f->ParameterTypes->Unsafe<uType*>(j), type);

        type->Reflection.Functions[i] = new uFunction(
            type,
            f->Name,
            f->Generic
                ? f->Generic->GenericCount > type->GenericCount
                    ? type->NewMethodType(f->Generic->GenericCount - type->GenericCount - 1)
                    : type
                : nullptr,
            f->Flags,
            f->_func,
            uGetParameterized(f->ReturnType, type),
            paramTypes);
    }
}

uType* uReflection::GetType(uString* name)
{
    uCString cstr(name);
    auto it = _Types->find(cstr.Ptr);
    return it != _Types->end()
        ? it->second
        : nullptr;
}

uArray* uReflection::GetTypes()
{
    uArray* result = @{Uno.Type[]:New(_Types->size())};
    int32_t i = 0;

    for (auto it = _Types->begin(); it != _Types->end(); ++it)
        result->Strong<uType*>(i++) = it->second;

    return result;
}

#define TYPE_PTR ((uType*)((uint8_t*)this - offsetof(uType, Reflection)))

uField* uReflection::GetField(uString* name)
{
    if (!name)
        return nullptr;

    uType* type = TYPE_PTR;
    U_ASSERT(type);

    for (size_t i = 0; i < type->Reflection.FieldCount; i++)
    {
        uField* f = type->Reflection.Fields[i];
        if (uString::Equals(f->Name, name))
            return f;
    }

    return type->Base
        ? type->Base->Reflection.GetField(name)
        : nullptr;
}

uFunction* uReflection::GetFunction(uString* name, uArray* parameterTypes)
{
    if (!name)
        return nullptr;

    uType* type = TYPE_PTR;
    U_ASSERT(type);

    int32_t paramCount = parameterTypes
        ? parameterTypes->Length()
        : 0;

    for (size_t i = 0; i < type->Reflection.FunctionCount; i++)
    {
        uFunction* f = type->Reflection.Functions[i];
        if (f->ParameterTypes->Length() == paramCount &&
            (paramCount == 0 || memcmp(f->ParameterTypes->Ptr(), parameterTypes->Ptr(), paramCount * sizeof(uType*)) == 0) &&
            uString::Equals(f->Name, name))
            return f;
    }

    return type->Base
        ? type->Base->Reflection.GetFunction(name, parameterTypes)
        : nullptr;
}

void uReflection::SetFields(size_t count, uField* first, ...)
{
    uType* type = TYPE_PTR;
    Fields = (uField**)malloc(sizeof(uField*) * count); // Leak
    FieldCount = count;
    Fields[0] = U_ASSERT_PTR(first);
    Fields[0]->DeclaringType = type;

    va_list ap;
    va_start(ap, first);

    for (size_t i = 1; i < count; i++)
    {
        Fields[i] = U_ASSERT_PTR(va_arg(ap, uField*));
        Fields[i]->DeclaringType = type;
    }

    va_end(ap);
}

void uReflection::SetFunctions(size_t count, uFunction* first, ...)
{
    uType* type = TYPE_PTR;
    Functions = (uFunction**)malloc(sizeof(uFunction*) * count); // Leak
    FunctionCount = count;
    Functions[0] = U_ASSERT_PTR(first);
    Functions[0]->DeclaringType = type;

    va_list ap;
    va_start(ap, first);

    for (size_t i = 1; i < count; i++)
    {
        Functions[i] = U_ASSERT_PTR(va_arg(ap, uFunction*));
        Functions[i]->DeclaringType = type;
    }

    va_end(ap);
}

#undef TYPE_PTR

uField::uField(const char* name, size_t index)
{
    DeclaringType = nullptr; // Assigned by uReflection::SetFunctions()
    Name = uString::Const(name);
    _index = index;
}

uField::uField(uType* declType, uString* name, size_t index)
{
    DeclaringType = declType;
    Name = name;
    _index = index;
}

const uFieldInfo& uField::Info() {
    return DeclaringType->Fields[_index];
}

#define FIELD_PTR ( \
    field.Flags & uFieldFlagsStatic \
        ? field.Address \
        : (uint8_t*)object + field.Offset \
    )

uObject* uField::GetValue(uObject* object)
{
    U_ASSERT(DeclaringType);
    DeclaringType->Init();
    const uFieldInfo& field = Info();
    return uBoxPtr(field.Type, FIELD_PTR, nullptr, true);
}

void uField::SetValue(uObject* object, uObject* value)
{
    U_ASSERT(DeclaringType);
    DeclaringType->Init();
    const uFieldInfo& field = Info();
    if (value && !uIs(value, field.Type))
        U_THROW_ICE();

    uUnboxPtr(field.Type, value, FIELD_PTR);
}

#undef FIELD_PTR

uFunction::uFunction(const char* name, uType* generic, const void* func, size_t offset, bool isStatic, uType* returnType, size_t paramCount, ...)
{
    U_ASSERT(name && returnType);
    DeclaringType = nullptr; // Assigned by uReflection::SetFunctions()
    Name = uString::Const(name);
    Generic = generic;
    ReturnType = returnType;
    ParameterTypes = @{Uno.Type[]:New(paramCount)};
    Flags = 0;

    if (func)
    {
        U_ASSERT(!offset);
        _func = func;

        if (isStatic)
            Flags |= uFunctionFlagsStatic;
    }
    else
    {
        U_ASSERT(!isStatic);
        _offset = offset;
        Flags |= uFunctionFlagsVirtual;
    }

    va_list ap;
    va_start(ap, paramCount);

    for (size_t i = 0; i < paramCount; i++)
        ParameterTypes->Strong<uType*>(i) = U_ASSERT_PTR(va_arg(ap, uType*));

    va_end(ap);
}

uFunction::uFunction(uType* declType, uString* name, uType* generic, size_t flags, const void* funcOrOffset, uType* returnType, uArray* paramTypes)
{
    U_ASSERT(declType && name && returnType && paramTypes);
    DeclaringType = declType;
    Name = name;
    Generic = generic;
    Flags = flags;
    _func = funcOrOffset;
    ReturnType = returnType;
    ParameterTypes = paramTypes;
}

static bool uIsDelegateType(uType* type, uType* returnType, uArray* parameterTypes)
{
    if (!type || type->Type != uTypeTypeDelegate)
        return false;

    uDelegateType* delegateType = (uDelegateType*)type;

    if (!parameterTypes ||
        delegateType->ReturnType != returnType ||
        delegateType->ParameterCount != parameterTypes->Length())
        return false;

    for (size_t i = 0; i < delegateType->ParameterCount; i++)
        if (delegateType->ParameterTypes[i] != parameterTypes->Item<uType*>(i))
            return false;

    return true;
}

uDelegate* uFunction::CreateDelegate(uType* type, uObject* object)
{
    if (!type || (!object && !(Flags & uFunctionFlagsStatic)))
        U_THROW_NRE();

    /* Uncommented for now, or createdelegate wont work with the current simulator bytecode
    if (!uIsDelegateType(type, ReturnType, ParameterTypes))
        U_THROW_ICE();
    */

    U_ASSERT(DeclaringType);
    DeclaringType->Init();
    return DeclaringType->Type == uTypeTypeInterface
            ? uDelegate::New(type, uInterface(object, DeclaringType), _offset, Generic) :
        Flags & uFunctionFlagsVirtual
            ? uDelegate::New(type, object, _offset, Generic)
            : uDelegate::New(type, _func, Flags & uFunctionFlagsStatic ? nullptr : uPtr(object), Generic);
}

uObject* uFunction::Invoke(uObject* object, uArray* args)
{
    size_t count = ParameterTypes->Length();

    void* retval = nullptr;
    if (!U_IS_VOID(ReturnType))
    {
        retval = alloca(ReturnType->ValueSize);
        if (U_IS_VALUE(ReturnType))
            memset(retval, 0, ReturnType->ValueSize);
    }

    if (!(Flags & uFunctionFlagsStatic))
        count++;
    if (Generic)
        count++;
    if (retval)
        count++;

    void** stack;
    void** ptr = stack = count > 0
            ? (void**)alloca(count * sizeof(void*))
            : nullptr;

    if (!(Flags & uFunctionFlagsStatic))
        *ptr++ = !(Flags & uFunctionFlagsVirtual) && U_IS_VALUE(DeclaringType)
            ? (uint8_t*)uPtr(object) + sizeof(uObject)
            : (void*)uPtr(object);
    if (Generic)
        *ptr++ = Generic;

    if (args)
    {
        if (args->Length() != ParameterTypes->Length())
            U_THROW_IOORE();
        if (U_IS_VALUE(((uArrayType*)args->GetType())->ElementType))
            U_THROW_ICE();

        uType** params = (uType**)ParameterTypes->Ptr();
        uObject** objects = (uObject**)args->Ptr();

        for (int32_t i = 0; i < args->Length(); i++)
        {
            uType*& param = *params++;
            uObject*& arg = *objects++;

            switch (param->Type)
            {
            case uTypeTypeEnum:
            case uTypeTypeStruct:
                *ptr++ = (uint8_t*)arg + sizeof(uObject);
                break;
            case uTypeTypeByRef:
                *ptr++ = U_IS_VALUE(((uByRefType*)param)->ValueType)
                        ? (uint8_t*)arg + sizeof(uObject)
                        : (void*)&arg;
                break;
            case uTypeTypeClass:
            case uTypeTypeDelegate:
            case uTypeTypeInterface:
            case uTypeTypeArray:
                *ptr++ = arg;
                break;
            default:
                U_FATAL();
            }
        }
    }
    else if (ParameterTypes->Length() != 0)
        U_THROW_IOORE();

    if (retval)
        *ptr++ = retval;

    U_ASSERT(DeclaringType);
    DeclaringType->Init();
    uInvoke(Flags & uFunctionFlagsVirtual
            ? *(void**)((uint8_t*)object->GetType() + _offset)
            : _func,
        stack,
        count);
    return uBoxPtr(ReturnType, retval, nullptr, true);
}
