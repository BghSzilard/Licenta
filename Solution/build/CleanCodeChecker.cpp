#include "CleanCodeChecker.h"

CleanCodeChecker::CleanCodeChecker(std::string_view translationUnit): ASTTraverser(translationUnit)
{
}

CXChildVisitResult CleanCodeChecker::Visitor(CXCursor current_cursor, CXCursor parent, CXClientData client_data)
{
	CXCursorKind cursorKind = clang_getCursorKind(current_cursor);

	CXSourceLocation location = clang_getCursorLocation(current_cursor);

	if (clang_Location_isInSystemHeader(location) == 0)
	{
		CXChildVisit_Recurse;
	}

	/*switch (cursorKind)
	{
	case CXCursor_VarDecl:
		HandleVariable(current_cursor);
	}*/

	return CXChildVisit_Recurse;
}

void CleanCodeChecker::traverseCursor(const CXCursor& cursor)
{
}