cmake_minimum_required(VERSION 2.6)
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
    set(CMAKE_CXX_FLAGS "${CMAKE_CXX_FLAGS} -Wno-invalid-offsetof -Wno-unused-value -Wno-dangling-else --std=@(CppStandard)")
    set(CMAKE_C_FLAGS "${CMAKE_C_FLAGS} --std=@(CStandard)")
endif()

if (APPLE)
    set(CMAKE_MACOSX_RPATH 1)
    set(CMAKE_OSX_ARCHITECTURES "x86_64;i386")
    set(CMAKE_SHARED_LINKER_FLAGS "${CMAKE_SHARED_LINKER_FLAGS}@(Xcode.Framework:Join('', ' -Wl,-framework,'))")
elseif (MSVC)
    set(CMAKE_CXX_FLAGS "${CMAKE_CXX_FLAGS} /GR-")
    set(CMAKE_CONFIGURATION_TYPES "Debug;Release" CACHE STRING "Build Configurations (forced)" FORCE)
endif()

@(CMake.SourceGroups)

add_library(@(Project.Identifier)-PInvoke SHARED
    @(HeaderFile:Join('\n    ', '"@(HeaderDirectory)/', '"'))
    @(SourceFile:Join('\n    ', '"@(SourceDirectory)/', '"')))

target_link_libraries(@(Project.Identifier)-PInvoke @(LinkLibrary:Join(' ')))
