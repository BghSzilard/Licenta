#pragma once

#include <string>
#include <optional>

class CompilationChecker
{
public:

	bool compiles(const std::string& translationUnit);
	std::optional<std::string> getCompilationErrorMessage(const std::string& translationUnit);
};