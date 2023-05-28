// @(MSG_ORIGIN)
// @(MSG_EDIT_WARNING)

#include <uno/ObjectModel.h>
@(Main.IncludeDirective)
@(TypeObjects.Declaration:JoinSorted())
void uInitTypes(uType*(*factories[])());

// See @{Uno.Environment.GetCommandLineArgs():Call()}
int uArgc = 0;
char** uArgv = nullptr;

int main(int argc, char* argv[])
{
    uArgc = argc;
    uArgv = argv;

    uRuntime uno;
    uAutoReleasePool pool;

    uType* (*factories[])() = {
        @(TypeObjects.FunctionPointer:JoinSorted('\n        ', '', ','))
        nullptr
    };

    uInitTypes(factories);
    return @(Main.Entrypoint);
}
