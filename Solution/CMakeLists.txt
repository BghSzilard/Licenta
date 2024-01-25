cmake_minimum_required(VERSION 3.12)

project(AutoCorrector)

# Set the path to LLVM and Clang
set(LLVM_PATH "C:/Users/sziba/Desktop/llvm/llvm/llvm")
set(CLANG_PATH "C:/Users/sziba/Desktop/clang/clang/clang")

# Include directories for LLVM and Clang headers
include_directories(${LLVM_PATH}/include)
include_directories(${CLANG_PATH}/include)

# Find LLVM and Clang libraries
file(GLOB LLVM_LIBRARIES ${LLVM_PATH}/lib/*.lib)
file(GLOB CLANG_LIBRARIES ${CLANG_PATH}/lib/*.lib)

# Add your source files for the library
set(LIBRARY_SOURCES
    src/library.cpp
    # Add your source files here
)

# Create the library
add_library(AutoCorrectorLib ${LIBRARY_SOURCES})

# Link against LLVM and Clang libraries
target_link_libraries(AutoCorrectorLib ${LLVM_LIBRARIES} ${CLANG_LIBRARIES})

# Optionally, you can set the library's version and visibility
# set_target_properties(AutoCorrectorLib PROPERTIES VERSION 1.0 SOVERSION 1)
# target_compile_definitions(AutoCorrectorLib PRIVATE -DAutoCorrectorLib_EXPORTS)

# Add your source files for the executable
set(EXECUTABLE_SOURCES
    src/main.cpp
    # Add your source files here
)

# Create the executable
add_executable(AutoCorrector ${EXECUTABLE_SOURCES})

# Link against the library and LLVM/Clang libraries
target_link_libraries(AutoCorrector AutoCorrectorLib ${LLVM_LIBRARIES} ${CLANG_LIBRARIES})

# Add your source files for the CLR project (C++/CLI)
set(CLR_SOURCES
    src/clr_project.cpp
    # Add your C++/CLI source files here
)

# Create the C++/CLI project (DLL)
add_library(AutoCorrectorCLR SHARED ${CLR_SOURCES})

# Set CLR support for the C++/CLI project
set_target_properties(AutoCorrectorCLR PROPERTIES COMMON_LANGUAGE_RUNTIME "")

# Link against the library and LLVM/Clang libraries
target_link_libraries(AutoCorrectorCLR AutoCorrectorLib ${LLVM_LIBRARIES} ${CLANG_LIBRARIES})
