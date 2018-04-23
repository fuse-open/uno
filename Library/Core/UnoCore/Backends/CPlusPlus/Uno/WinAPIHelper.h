#pragma once

#define WIN32_LEAN_AND_MEAN
#include <windows.h>
// Needed for UuidCreate()
#include <Rpc.h>
// Needed for SHGetKnownFolderPath()
#include <ShlObj.h>

// windows.h is mean, and define pre-processor symbols that cause problems :(

#undef ChangeDirectory
#undef CopyFile
#undef CreateDirectory
#undef CreateMutex
#undef DeleteFile
#undef FindFirstFile
#undef GetCurrentDirectory
#undef GetEnvironmentVariable
#undef GetMessage
#undef GetSystemDirectory
#undef GetTempPath
#undef MessageBox
#undef MoveFile
#undef SetCurrentDirectory
