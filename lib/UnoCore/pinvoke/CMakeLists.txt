cmake_minimum_required(VERSION 2.6)
project(@(project.name:quoteSpace))
include_directories(@(headerDirectory)@(includeDirectory:quoteSpace:join('', ' ')))
link_directories(@(linkDirectory:quoteSpace:join(' ')))
add_definitions(@(preprocessorDefinition:join(' ', '-D')))

set(EXECUTABLE_OUTPUT_PATH ${PROJECT_SOURCE_DIR}/@(binaryDirectory))
set(CMAKE_RUNTIME_OUTPUT_DIRECTORY_RELEASE ${PROJECT_SOURCE_DIR}/@(binaryDirectory))
set(CMAKE_RUNTIME_OUTPUT_DIRECTORY_DEBUG ${PROJECT_SOURCE_DIR}/@(binaryDirectory))
set(CMAKE_ARCHIVE_OUTPUT_DIRECTORY_RELEASE ${PROJECT_SOURCE_DIR}/@(binaryDirectory))
set(CMAKE_ARCHIVE_OUTPUT_DIRECTORY_DEBUG ${PROJECT_SOURCE_DIR}/@(binaryDirectory))

if (UNIX)
    set(CMAKE_CXX_FLAGS "${CMAKE_CXX_FLAGS} -Wno-invalid-offsetof -Wno-unused-value -Wno-dangling-else --std=@(cppStandard)")
    set(CMAKE_C_FLAGS "${CMAKE_C_FLAGS} --std=@(cStandard)")
endif()

if (APPLE)
    set(CMAKE_MACOSX_RPATH 1)
    set(CMAKE_OSX_ARCHITECTURES "x86_64;i386")
    set(CMAKE_SHARED_LINKER_FLAGS "${CMAKE_SHARED_LINKER_FLAGS}@(xcode.framework:join('', ' -Wl,-framework,'))")
elseif (MSVC)
    set(CMAKE_CXX_FLAGS "${CMAKE_CXX_FLAGS} /GR-")
    set(CMAKE_CONFIGURATION_TYPES "Debug;Release" CACHE STRING "Build Configurations (forced)" FORCE)
endif()

@(cmake.sourceGroups)

add_library(@(project.identifier)-PInvoke SHARED
    @(headerFile:join('\n    ', '"@(headerDirectory)/', '"'))
    @(sourceFile:join('\n    ', '"@(sourceDirectory)/', '"')))

target_link_libraries(@(project.identifier)-PInvoke @(linkLibrary:join(' ')))
