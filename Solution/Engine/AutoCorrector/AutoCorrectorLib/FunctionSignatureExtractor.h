//#pragma once
//#include <clang-c/Index.h>
//#include <string>
//#include <vector>
//
//class FunctionSignatureExtractor
//{
//
//public:
//	FunctionSignatureExtractor(std::string translationUnit);
//
//	void traverseAST();
//
//	std::vector<std::string> getSignatures() const;
//
//private:
//
//	CXChildVisitResult mainVisitor(CXCursor current_cursor, CXCursor parent, CXClientData client_data);
//
//	std::string getFunctionSignature(CXCursor cursor);
//
//private:
//
//	std::string m_inputFile;
//	std::vector<std::string> m_signatures;
//
//	bool isLocationInSystemHeader(CXSourceLocation location);
//};

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