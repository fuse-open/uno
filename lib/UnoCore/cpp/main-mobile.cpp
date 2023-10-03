// @(MSG_ORIGIN)
// @(MSG_EDIT_WARNING)

#include <uno/ObjectModel.h>

// Used by Uno.Environment.GetCommandLineArgs()
int uArgc = 0;
char** uArgv = nullptr;

@(typeObjects.declaration:joinSorted())
void uInitTypes(uType*(*factories[])());

void uInitTypes()
{
    static uType*(*factories[])() =
    {
        @(typeObjects.functionPointer:joinSorted('\n        ', '', ','))
        nullptr
    };

    uInitTypes(factories);
}

@(main.include:joinSorted('\n', '#include <', '>'))

void uStartApp()
{
    @(main.body:indent(' ', 4))
}
