#pragma once
#include <clang-c/Index.h>
#include <string>
#include <vector>

class FunctionSignatureExtractor
{

public:
	FunctionSignatureExtractor(std::string translationUnit);

	void traverseAST();

	std::vector<std::string> getSignatures() const;

private:

	CXChildVisitResult mainVisitor(CXCursor current_cursor, CXCursor parent, CXClientData client_data);

	std::string getFunctionSignature(CXCursor cursor);

private:

	std::string m_inputFile;
	std::vector<std::string> m_signatures;

	bool isLocationInSystemHeader(CXSourceLocation location);
};