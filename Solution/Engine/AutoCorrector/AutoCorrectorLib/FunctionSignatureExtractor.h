#pragma once

#ifdef FUNCTION_SIGNATURE_EXPORTS
#define MYNATIVELIB_APIFUNC __declspec(dllexport)
#else
#define MYNATIVELIB_APIFUNC __declspec(dllimport)
#endif

#include "ASTTraverser.h"

#include <clang-c/Index.h>
#include <string>
#include <vector>

class MYNATIVELIB_APIFUNC FunctionSignatureExtractor
{
public:
	std::vector<std::string> getSignatures(std::string);

private:

	std::string m_inputFile;
};