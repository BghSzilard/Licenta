#pragma once

#include <clang-c/Index.h>

class ASTTraverser
{

public:

	ASTTraverser(const char* inputFile);

	void traverseAST();

private:

	CXChildVisitResult Visitor(CXCursor current_cursor, CXCursor parent, CXClientData client_data);
	void HandleVariable(CXCursor current_cursor);

private:

	const char* m_inputFile;

};