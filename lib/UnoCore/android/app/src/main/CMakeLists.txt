cmake_minimum_required(VERSION 3.4.1)
project(@(project.name:quoteSpace))

include(AndroidNdkModules)
include_directories(include@(includeDirectory:quoteSpace:join('', ' ')))
link_directories(@(linkDirectory:quoteSpace:join(' ')))
add_definitions(@(preprocessorDefinition:join(' ', '-D')))

set(CMAKE_C_FLAGS "${CMAKE_C_FLAGS} -std=@(cStandard)")
set(CMAKE_CXX_FLAGS "${CMAKE_CXX_FLAGS} -std=@(cppStandard) -Wno-unused-value -Wno-dangling-else -Wno-switch -Wno-invalid-offsetof -fexceptions -frtti")

# Strip release builds to reduce file size.
if (CMAKE_BUILD_TYPE STREQUAL "Release")
    set(CMAKE_SHARED_LINKER_FLAGS "${CMAKE_SHARED_LINKER_FLAGS} -s")
endif()

# Using response file as workaround for long paths on Windows.
set(CMAKE_C_USE_RESPONSE_FILE_FOR_OBJECTS 1)
set(CMAKE_CXX_USE_RESPONSE_FILE_FOR_OBJECTS 1)
set(CMAKE_C_RESPONSE_FILE_LINK_FLAG "@")
set(CMAKE_CXX_RESPONSE_FILE_LINK_FLAG "@")
set(CMAKE_NINJA_FORCE_RESPONSE_FILE 1 CACHE INTERNAL "")

# Find libraries extracted from AARs, if applicable.
#if @(gradle.dependency.nativeImplementation:isRequired)
link_directories("@(outputDirectory:path)/app/build/native/jni/${ANDROID_ABI}")
#endif

add_library(@(activity.name) SHARED
    @(headerFile:joinSorted('\n    ', '"include/', '"'))
    @(sourceFile:joinSorted('\n    ', '"jni/', '"')))

target_link_libraries(@(activity.name)
    @(linkLibrary:join('\n    ', '"', '"'))
    @(sharedLibrary:join('\n    ', '"', '"'))
    @(linkOrderedStaticLibraries:join('\n    ', '"', '"')))

@(sharedLibrary:quoteSpace:join('\n', 'file(COPY ', ' DESTINATION "@(outputDirectory:path)/app/src/main/jniLibs/${ANDROID_ABI}")'))
