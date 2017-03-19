// @(MSG_ORIGIN)
// @(MSG_EDIT_WARNING)

#include <Uno/ObjectModel.h>
#include <unordered_map>
#include <vector>

struct uAutoReleaseFrame
{
    size_t StartIndex;

#ifdef DEBUG_ARC
    size_t AllocCount;
    size_t FreeCount;
    size_t AllocSize;
    size_t FreeSize;
#endif
};

struct uCallStackFrame
{
    const char* Type;
    const char* Function;
};

struct uThreadData
{
    std::vector<uObject*> AutoReleaseList;
    uAutoReleaseFrame AutoReleaseStack[0x800];
    uAutoReleaseFrame* AutoReleasePtr;
    uAutoReleaseFrame* AutoReleaseEnd;
    uCallStackFrame CallStack[0xffff];
    uCallStackFrame* CallStackPtr;
    uCallStackFrame* CallStackEnd;
    uType* CurrentType;

    uThreadData()
        : AutoReleasePtr(AutoReleaseStack - 1)
        , AutoReleaseEnd(AutoReleaseStack + sizeof(AutoReleaseStack) / sizeof(AutoReleaseStack[0]))
        , CallStackPtr(CallStack - 1)
        , CallStackEnd(CallStack + sizeof(CallStack) / sizeof(CallStack[0]))
        , CurrentType(nullptr)
    {
    }
};

struct uThreadDataAccessor
{
    uThreadData* _data;
    uThreadDataAccessor(): _data(new uThreadData()) { }
    ~uThreadDataAccessor() { delete _data; }
    uThreadData* operator &() { return _data; }
};

struct uWeakObject
{
    uObject* Object;
    std::atomic_int RefCount;

    enum ObjectState
    {
        Dead = -1,
        Healthy,
        Infected,
        Zombie
    };

    ObjectState ZombieState;
    uWeakStateIntercept::Callback ZombieStateIntercept;
};

struct uTypeKey
{
    uType* Definition;
    uType** Arguments;

    uTypeKey()
        : Definition(nullptr)
        , Arguments(nullptr)
    {
    }
    uTypeKey(uType* def)
        : Definition(def->Definition)
        , Arguments((uType**)malloc(def->GenericCount * sizeof(uType*)))
    {
        U_ASSERT(def && def->Definition && def->GenericCount);
    }
    uTypeKey(uType* def, uType** args)
        : Definition(def->Definition)
        , Arguments((uType**)malloc(def->GenericCount * sizeof(uType*)))
    {
        U_ASSERT(def && def->Definition && def->GenericCount && args);
        memcpy(Arguments, args, def->GenericCount * sizeof(uType*));
    }
    uTypeKey(const uTypeKey& copy)
        : Definition(copy.Definition)
    {
        if (!Definition)
            Arguments = nullptr;
        else
        {
            U_ASSERT(copy.Arguments);
            Arguments = (uType**)malloc(Definition->GenericCount * sizeof(uType*));
            memcpy(Arguments, copy.Arguments, Definition->GenericCount * sizeof(uType*));
        }
    }
    ~uTypeKey()
    {
        if (Arguments)
            free(Arguments);
    }
    uTypeKey& operator =(const uTypeKey& copy)
    {
        if (Arguments)
            free(Arguments);

        Definition = copy.Definition;

        if (!Definition)
            Arguments = nullptr;
        else
        {
            Arguments = (uType**)malloc(Definition->GenericCount * sizeof(uType*));
            memcpy(Arguments, copy.Arguments, Definition->GenericCount * sizeof(uType*));
        }

        return *this;
    }
    bool operator ==(const uTypeKey& other) const
    {
        if (Definition != other.Definition)
            return false;

        return !Definition ||
            memcmp(Arguments, other.Arguments, Definition->GenericCount * sizeof(uType*)) == 0;
    }
};

struct uTypeKeyHash
{
    size_t operator ()(const uTypeKey& key) const
    {
        if (!key.Definition)
            return 0;

        size_t hash = *(size_t*) &key.Definition;
        for (size_t i = 0; i < key.Definition->GenericCount; i++)
            hash = ((hash << 5) + hash) ^ *(size_t*) &key.Arguments[i];

        return hash;
    }
};
