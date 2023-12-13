#include "FunctionIdentifier.h"

FunctionIdentifier::FunctionIdentifier(std::string_view translationUnit) : ASTTraverser(translationUnit)
{
	m_translationUnit = translationUnit;
}

CXCursor FunctionIdentifier::findFunction(std::string_view functionName)
{
	traverseAST();
}

CXChildVisitResult FunctionIdentifier::Visitor(CXCursor currentCursor, CXCursor parent, CXClientData clientData)
{
	CXCursorKind cursorKind = clang_getCursorKind(currentCursor);

	CXSourceLocation location = clang_getCursorLocation(currentCursor);

	if (clang_Location_isInSystemHeader(location) == 0)
	{
		return CXChildVisit_Recurse;
	}

	if (cursorKind != CXCursorKind::CXCursor_FunctionDecl)
	{
		return CXChildVisit_Recurse;
	}

	CXString functionDisplayName = clang_getCursorDisplayName(currentCursor);
	std::string functionName = clang_getCString(functionDisplayName);

	clang_disposeString(functionDisplayName);

	return CXChildVisit_Recurse;
}
