// @(MSG_ORIGIN)
// @(MSG_EDIT_WARNING)

#include <uno/ObjectModel.h>

// See @{Uno.Environment.GetCommandLineArgs():Call()}
int uArgc = 0;
char** uArgv = nullptr;

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
