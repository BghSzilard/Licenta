#include "ASTTraverser.h"

#include <string>
#include <iostream>

ASTTraverser::ASTTraverser(const char* inputFile)
{
	m_inputFile = inputFile;
}

void ASTTraverser::traverseAST()
{
	CXIndex index = clang_createIndex(0, 0);
	CXTranslationUnit unit = clang_parseTranslationUnit(
		index, m_inputFile, nullptr, 0,
		nullptr, 0,
		CXTranslationUnit_None);

	if (unit == nullptr)
	{
		throw std::string("Unable to parse translation unit!");
	}

	CXCursor cursor = clang_getTranslationUnitCursor(unit);

	auto visitorWrapper = [](CXCursor current_cursor, CXCursor parent, CXClientData client_data) -> CXChildVisitResult {
		return static_cast<ASTTraverser*>(client_data)->Visitor(current_cursor, parent, nullptr);
		};

	clang_visitChildren(cursor, visitorWrapper, nullptr);
}

CXChildVisitResult ASTTraverser::Visitor(CXCursor current_cursor, CXCursor parent, CXClientData client_data)
{
	CXCursorKind cursorKind = clang_getCursorKind(current_cursor);

	CXSourceLocation location = clang_getCursorLocation(current_cursor);

	if (clang_Location_isInSystemHeader(location) == 0)
	{
		CXChildVisit_Recurse;
	}

	switch (cursorKind)
	{
	case CXCursor_VarDecl:
		HandleVariable(current_cursor);
	}

	return CXChildVisit_Recurse;
}

void ASTTraverser::HandleVariable(CXCursor current_cursor)
{

	CXString variableDisplayName = clang_getCursorDisplayName(current_cursor);
	std::string variableName = clang_getCString(variableDisplayName);

	CXCursor functionCursor = clang_getCursorSemanticParent(current_cursor);
	CXString functionDisplayName = clang_getCursorDisplayName(functionCursor);
	std::string functionName = clang_getCString(functionDisplayName);

	
	std::cout << variableName << std::endl;;
	

	clang_disposeString(variableDisplayName);
	clang_disposeString(functionDisplayName);
}