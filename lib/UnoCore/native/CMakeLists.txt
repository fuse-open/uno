cmake_minimum_required(VERSION 3.6)
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
    set(CMAKE_C_FLAGS "${CMAKE_C_FLAGS} --std=@(cStandard)")
    set(CMAKE_CXX_FLAGS "${CMAKE_CXX_FLAGS} --std=@(cppStandard)")
    set(CMAKE_CXX_FLAGS "${CMAKE_CXX_FLAGS} -Wno-invalid-offsetof -Wno-unused-value -Wno-unused-variable -Wno-dangling-else -Wno-switch")
endif()

if (APPLE)
    set(CMAKE_XCODE_ATTRIBUTE_CLANG_CXX_LIBRARY "libc++")
    set(CMAKE_CXX_FLAGS "${CMAKE_CXX_FLAGS} -stdlib=libc++ -fobjc-arc")
    set(CMAKE_EXE_LINKER_FLAGS "${CMAKE_EXE_LINKER_FLAGS}@(xcode.framework:join('', ' -Wl,-framework,')) -w")
    set(CMAKE_MACOSX_RPATH 1)
elseif (MSVC)
    add_definitions(-DUNICODE -D_UNICODE -D_CRT_SECURE_NO_WARNINGS)
    set(CMAKE_C_FLAGS "${CMAKE_C_FLAGS} /MP")
    set(CMAKE_CXX_FLAGS "${CMAKE_CXX_FLAGS} /GR- /MP /wd4065 /wd4101 /wd4390 /wd4674")
    set(CMAKE_CONFIGURATION_TYPES "Debug;Release" CACHE STRING "Build Configurations (forced)" FORCE)
    set(EXTRA_FILES "natvis/uno.natstepfilter" "natvis/uno.natvis"@('!LIBRARY':defined:test(' "rc/icon.rc"', '')))
elseif (CMAKE_COMPILER_IS_GNUCC)
    set(CMAKE_EXE_LINKER_FLAGS "${CMAKE_EXE_LINKER_FLAGS} -Wl,--start-group")
endif()

@(cmake.sourceGroups)

#if @(LIBRARY:defined)
add_library(@(project.name:quoteSpace) SHARED
#else
add_executable(@(project.name:quoteSpace)@(WIN32:defined:test(' WIN32', ''))
#endif
    @(headerFile:joinSorted('\n    ', '"@(headerDirectory)/', '"'))
    @(sourceFile:joinSorted('\n    ', '"@(sourceDirectory)/', '"'))
    ${EXTRA_FILES})

target_link_libraries(@(project.name:quoteSpace) @(linkLibrary:join(' '))
    @(linkLibrary.debug:join(' ', 'debug '))
    @(linkLibrary.release:join(' ', 'optimized ')))

@(sharedLibrary:join('\n', 'file(COPY "', '" DESTINATION ${PROJECT_SOURCE_DIR}/@(binaryDirectory))'))
#if @(X64:defined)
@(sharedLibrary.x64:join('\n', 'file(COPY "', '" DESTINATION ${PROJECT_SOURCE_DIR}/@(binaryDirectory))'))
#elif @(X86:defined)
@(sharedLibrary.x86:join('\n', 'file(COPY "', '" DESTINATION ${PROJECT_SOURCE_DIR}/@(binaryDirectory))'))
#endif

if (MSVC)
    set_property(DIRECTORY ${PROJECT_SOURCE_DIR} PROPERTY VS_STARTUP_PROJECT @(project.name:quoteSpace))
endif()
