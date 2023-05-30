// @(MSG_ORIGIN)
// @(MSG_EDIT_WARNING)

#include <uno/ObjectModel.h>

// Used by Uno.Environment.GetCommandLineArgs()
int uArgc = 0;
char** uArgv = nullptr;

@(TypeObjects.Declaration:JoinSorted())
void uInitTypes(uType*(*factories[])());

void uInitTypes()
{
    static uType*(*factories[])() =
    {
        @(TypeObjects.FunctionPointer:JoinSorted('\n        ', '', ','))
        nullptr
    };

    uInitTypes(factories);
}

@(Main.Include:JoinSorted('\n', '#include <', '>'))

void uStartApp()
{
    @(Main.Body:Indent(' ', 4))
}
