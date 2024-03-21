#pragma once

#include <clang-c/Index.h>
#include <string>
#include <vector>
#include <queue>

class ASTTraverser
{

public:

	ASTTraverser(const char* inputFile);

	void traverseAST();
	void traverseASTExtractor();

	bool foundMain();

	std::vector<std::string> getSignatures();

	std::string getFunction(const std::string& functionName);

protected:

	CXChildVisitResult signatureExtractorVisitor(CXCursor current_cursor, CXCursor parent, CXClientData client_data);
	CXChildVisitResult functionExtractorVisitor(CXCursor current_cursor, CXCursor parent, CXClientData client_data);
	CXChildVisitResult dependencyFunctionExtractorVisitor(CXCursor current_cursor, CXCursor parent, CXClientData client_data);
	CXChildVisitResult MainVisitor(CXCursor current_cursor, CXCursor parent, CXClientData client_data);
	bool isLocationInSystemHeader(CXSourceLocation location);
	std::string getFunctionSignature(CXCursor cursor);

	std::vector<std::string> m_signatures;

private:

	const char* m_inputFile;

	bool m_foundMain = false;
	std::string m_functionName = "";
	std::string m_functionBody = "";
	std::queue<CXCursor> m_cursorsToTraverse;
};