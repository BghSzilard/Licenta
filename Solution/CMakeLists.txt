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

# Add your source files here
set(SOURCES
    src/main.cpp
    # Add your source files here
)

# Create the executable
add_executable(AutoCorrector ${SOURCES})

# Link against LLVM and Clang libraries
target_link_libraries(AutoCorrector ${LLVM_LIBRARIES} ${CLANG_LIBRARIES})