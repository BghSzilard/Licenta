#include "ASTTraverser.h"
#include "VariableHandler.h"

#include <string>
#include <iostream>

ASTTraverser::ASTTraverser(std::string_view translationUnit)
{
	m_translationUnit = translationUnit;
}

void ASTTraverser::traverseAST()
{
	CXIndex index = clang_createIndex(0, 0);

	CXTranslationUnit unit = clang_parseTranslationUnit(
		index, std::string(m_translationUnit).c_str(), nullptr, 0,
		nullptr, 0,
		CXTranslationUnit_None);

	if (unit == nullptr)
	{
		throw std::string("Unable to parse translation unit!");
	}

	CXCursor cursor = clang_getTranslationUnitCursor(unit);

	auto visitorWrapper = [](CXCursor currentCursor, CXCursor parent, CXClientData clientData) -> CXChildVisitResult {
		return static_cast<ASTTraverser*>(clientData)->Visitor(currentCursor, parent, nullptr);
		};


	clang_visitChildren(cursor, visitorWrapper, this);

}

unsigned ASTTraverser::getLineNumber(CXCursor cursor)
{
	CXSourceLocation location = clang_getCursorLocation(cursor);
	unsigned line;
	clang_getSpellingLocation(location, NULL, &line, NULL, NULL);
	return line;
}
