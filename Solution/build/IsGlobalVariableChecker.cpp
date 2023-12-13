#include "IsGlobalVariableChecker.h"
#include <string>

IsGlobalVariableChecker::IsGlobalVariableChecker(const char* translationUnit)
{
	m_translationUnit = translationUnit;
}

bool IsGlobalVariableChecker::correct(CXCursor cursor)
{
	CXCursor functionCursor = clang_getCursorSemanticParent(cursor);
	CXString functionDisplayName = clang_getCursorDisplayName(functionCursor);
	std::string functionName = clang_getCString(functionDisplayName);

	clang_disposeString(functionDisplayName);

	return m_translationUnit == functionName;
}