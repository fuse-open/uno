cmake_minimum_required(VERSION 3.6)
project(@(Project.Name:QuoteSpace))
include_directories(@(HeaderDirectory)@(IncludeDirectory:QuoteSpace:Join('', ' ')))
link_directories(@(LinkDirectory:QuoteSpace:Join(' ')))
add_definitions(@(PreprocessorDefinition:Join(' ', '-D')))

set(EXECUTABLE_OUTPUT_PATH ${PROJECT_SOURCE_DIR}/@(BinaryDirectory))
set(CMAKE_RUNTIME_OUTPUT_DIRECTORY_RELEASE ${PROJECT_SOURCE_DIR}/@(BinaryDirectory))
set(CMAKE_RUNTIME_OUTPUT_DIRECTORY_DEBUG ${PROJECT_SOURCE_DIR}/@(BinaryDirectory))
set(CMAKE_ARCHIVE_OUTPUT_DIRECTORY_RELEASE ${PROJECT_SOURCE_DIR}/@(BinaryDirectory))
set(CMAKE_ARCHIVE_OUTPUT_DIRECTORY_DEBUG ${PROJECT_SOURCE_DIR}/@(BinaryDirectory))

if (UNIX)
    set(CMAKE_C_FLAGS "${CMAKE_C_FLAGS} --std=@(CStandard)")
    set(CMAKE_CXX_FLAGS "${CMAKE_CXX_FLAGS} --std=@(CppStandard)")
    set(CMAKE_CXX_FLAGS "${CMAKE_CXX_FLAGS} -Wno-invalid-offsetof -Wno-unused-value -Wno-unused-variable -Wno-dangling-else -Wno-switch")
endif()

if (APPLE)
    set(CMAKE_XCODE_ATTRIBUTE_CLANG_CXX_LIBRARY "libc++")
    set(CMAKE_CXX_FLAGS "${CMAKE_CXX_FLAGS} -stdlib=libc++ -fobjc-arc")
    set(CMAKE_EXE_LINKER_FLAGS "${CMAKE_EXE_LINKER_FLAGS}@(Xcode.Framework:Join('', ' -Wl,-framework,')) -w")
    set(CMAKE_MACOSX_RPATH 1)
elseif (MSVC)
    add_definitions(-DUNICODE -D_UNICODE -D_CRT_SECURE_NO_WARNINGS)
    set(CMAKE_C_FLAGS "${CMAKE_C_FLAGS} /MP")
    set(CMAKE_CXX_FLAGS "${CMAKE_CXX_FLAGS} /GR- /MP /wd4065 /wd4101 /wd4390 /wd4674")
    set(CMAKE_CONFIGURATION_TYPES "Debug;Release" CACHE STRING "Build Configurations (forced)" FORCE)
    set(EXTRA_FILES "natvis/uno.natstepfilter" "natvis/uno.natvis"@('!LIBRARY':Defined:Test(' "rc/icon.rc"', '')))
elseif (CMAKE_COMPILER_IS_GNUCC)
    set(CMAKE_EXE_LINKER_FLAGS "${CMAKE_EXE_LINKER_FLAGS} -Wl,--start-group")
endif()

@(CMake.SourceGroups)

#if @(LIBRARY:Defined)
add_library(@(Project.Name:QuoteSpace) SHARED
#else
add_executable(@(Project.Name:QuoteSpace)@(WIN32:Defined:Test(' WIN32', ''))
#endif
    @(HeaderFile:JoinSorted('\n    ', '"@(HeaderDirectory)/', '"'))
    @(SourceFile:JoinSorted('\n    ', '"@(SourceDirectory)/', '"'))
    ${EXTRA_FILES})

target_link_libraries(@(Project.Name:QuoteSpace) @(LinkLibrary:Join(' '))
    @(LinkLibrary.Debug:Join(' ', 'debug '))
    @(LinkLibrary.Release:Join(' ', 'optimized ')))

@(SharedLibrary:Join('\n', 'file(COPY "', '" DESTINATION ${PROJECT_SOURCE_DIR}/@(BinaryDirectory))'))
#if @(X64:Defined)
@(SharedLibrary.x64:Join('\n', 'file(COPY "', '" DESTINATION ${PROJECT_SOURCE_DIR}/@(BinaryDirectory))'))
#elif @(X86:Defined)
@(SharedLibrary.x86:Join('\n', 'file(COPY "', '" DESTINATION ${PROJECT_SOURCE_DIR}/@(BinaryDirectory))'))
#endif

if (MSVC)
    set_property(DIRECTORY ${PROJECT_SOURCE_DIR} PROPERTY VS_STARTUP_PROJECT @(Project.Name:QuoteSpace))
endif()
