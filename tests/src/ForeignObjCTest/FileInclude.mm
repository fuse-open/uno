#include <FileInclude.h>
#include <uObjC.Foreign.h>

NSString* string_from_file_include()
{
    NSString* str1 = @"lollercoaster tycoon";
    NSString* str2 = @{string:Of(str1).ToString():Call()};

    return str2;
}
