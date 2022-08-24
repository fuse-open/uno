// @(MSG_ORIGIN)
// @(MSG_EDIT_WARNING)

#include <Uno/ObjectModel.h>

@(TypeObjects.Declaration:JoinSorted())
void uInitRtti(uType*(*factories[])());

void uInitRtti()
{
    static uType*(*factories[])() =
    {
        @(TypeObjects.FunctionPointer:JoinSorted('\n        ', '', ','))
        nullptr
    };

    uInitRtti(factories);
}

@(Main.Include:JoinSorted('\n', '#include <', '>'))

void uStartApp()
{
    @(Main.Body:Indent(' ', 4))
}
