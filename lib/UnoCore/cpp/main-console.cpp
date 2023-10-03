// @(MSG_ORIGIN)
// @(MSG_EDIT_WARNING)

#include <uno/ObjectModel.h>
@(main.includeDirective)
@(typeObjects.declaration:joinSorted())
void uInitTypes(uType*(*factories[])());

// Used by Uno.Environment.GetCommandLineArgs()
int uArgc = 0;
char** uArgv = nullptr;

int main(int argc, char* argv[])
{
    uArgc = argc;
    uArgv = argv;

    uRuntime uno;
    uAutoReleasePool pool;

    uType* (*factories[])() = {
        @(typeObjects.functionPointer:joinSorted('\n        ', '', ','))
        nullptr
    };

    uInitTypes(factories);
    return @(main.entrypoint);
}
