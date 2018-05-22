// @(MSG_ORIGIN)
// @(MSG_EDIT_WARNING)

#include <Uno/ObjectModel.h>

@(TypeObjects.Declaration:Join())
void uInitRtti(uType*(*factories[])());

void uInitRtti()
{
    static uType*(*factories[])() =
    {
        @(TypeObjects.FunctionPointer:Join('\n        ', '', ','))
        NULL
    };

    uInitRtti(factories);
}

@(Main.Include:Join('\n', '#include <', '>'))

void uStartApp()
{
    @(Main.Body:Indent(' ', 4))
}
