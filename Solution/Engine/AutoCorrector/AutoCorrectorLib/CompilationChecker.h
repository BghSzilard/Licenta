#pragma once

// Define MYNATIVELIB_EXPORTS when building the DLL
#ifdef MYNATIVELIB_EXPORTS
#define MYNATIVELIB_API __declspec(dllexport)
#else
#define MYNATIVELIB_API __declspec(dllimport)
#endif

#include <string>
#include <optional>

class MYNATIVELIB_API CompilationChecker
{
public:

	bool compiles(const std::string& translationUnit);
	std::optional<std::string> getCompilationErrorMessage(const std::string& translationUnit);
};