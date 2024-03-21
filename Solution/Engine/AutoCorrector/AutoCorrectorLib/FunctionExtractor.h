#pragma once

#ifdef FUNCTION_EXPORTS
#define MYNATIVELIB_APIFUNCB __declspec(dllexport)
#else
#define MYNATIVELIB_APIFUNCB __declspec(dllimport)
#endif

#include <string>

class MYNATIVELIB_APIFUNCB FunctionExtractor
{
public:

	std::string extractFunction(std::string translationUnit, std::string functionName);
};