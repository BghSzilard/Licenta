#pragma once

#include <clang-c/Index.h>

class ASTTraverser
{

public:

	ASTTraverser(const char* inputFile);

	void traverseAST();

	bool foundMain();

private:


	CXChildVisitResult MainVisitor(CXCursor current_cursor, CXCursor parent, CXClientData client_data);
	

private:

	const char* m_inputFile;

	bool m_foundMain = false;
};