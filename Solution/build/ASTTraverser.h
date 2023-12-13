#pragma once

#include <clang-c/Index.h>

#include <string_view>

class ASTTraverser
{

public:

	ASTTraverser(std::string_view translationUnit);

	void traverseAST();

protected:

	unsigned getLineNumber(CXCursor cursor);

private:

	virtual CXChildVisitResult Visitor(CXCursor currentCursor, CXCursor parent, CXClientData clientData) = 0;

private:

	std::string_view m_translationUnit;

};