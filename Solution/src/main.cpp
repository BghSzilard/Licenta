#include <iostream>

int main()
{
    const char* inputFile = "FileToCorrect.cpp";

    std::string command = "clang++ " + std::string(inputFile); +" -o test";

    int result = std::system(command.c_str());

    if (result == 0) {
        std::cout << "Compilation successful.\n";
    }
    else {
        std::cout << "Compilation failed.\n";
    }

    return 0;
}