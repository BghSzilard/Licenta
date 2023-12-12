#include "CompilationChecker.h"

#include <fstream>
#include <format>
#include <cstdio>

bool CompilationChecker::compiles(const std::string& translationUnit)
{
	std::string command = std::format("clang++ {} 2> NUL 1> NUL", translationUnit);
	int result = std::system(command.c_str());
    
    return result == 0;
}

std::optional<std::string> CompilationChecker::getCompilationErrorMessage(const std::string& translationUnit)
{
    std::string tempFile = "error.txt";
    std::string command = std::format("clang++ {} 2> {} 1> NUL", translationUnit, tempFile);
    std::system(command.c_str());

    std::ifstream fin(tempFile);
    if (!fin.is_open())
    {
        return std::nullopt;
    }

    std::string errorMessage((std::istreambuf_iterator<char>(fin)), std::istreambuf_iterator<char>());
    fin.close();

    std::remove(tempFile.c_str());

    return errorMessage.empty() ? std::nullopt : std::make_optional(errorMessage);
}
