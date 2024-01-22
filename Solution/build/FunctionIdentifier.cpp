#include "FunctionIdentifier.h"

FunctionIdentifier::FunctionIdentifier(std::string_view translationUnit) : ASTTraverser(translationUnit)
{
	m_translationUnit = translationUnit;
}

std::optional<CXCursor> FunctionIdentifier::findFunction(std::string_view functionName)
{
	CXIndex index = clang_createIndex(0, 0);

	CXTranslationUnit unit = clang_parseTranslationUnit(
		index, std::string(m_translationUnit).c_str(), nullptr, 0,
		nullptr, 0,
		CXTranslationUnit_None);

	CXCursor rootCursor = clang_getTranslationUnitCursor(unit);

	return traverseCursor(rootCursor, functionName);
}

std::optional<CXCursor> FunctionIdentifier::traverseCursor(const CXCursor& cursor, std::string_view searchedFunctionName)
{

	/*if (clang_getCursorKind(cursor) != CXCursor_FunctionDecl)
	{
		return CXChildVisit_Recurse;
	}

	CXString functionDisplayName = clang_getCursorDisplayName(cursor);
	std::string functioneName = clang_getCString(functionDisplayName);

	if (functioneName == searchedFunctionName)
	{
		return cursor;
	}

	clang_visitChildren(cursor, [](CXCursor child, CXCursor parent, CXClientData clientData) -> CXChildVisitResult {
		reinterpret_cast<FunctionIdentifier*>(clientData)->traverseCursor(child, functionName);
		return CXChildVisit_Continue;
		}, this);*/

		// Check if the cursor's kind is CXCursor_FunctionDecl
	if (clang_getCursorKind(cursor) == CXCursor_FunctionDecl) {
		// Check if the cursor's name matches the target
		CXString cursorSpelling = clang_getCursorSpelling(cursor);
		std::string cursorName = clang_getCString(cursorSpelling);
		clang_disposeString(cursorSpelling);

		if (cursorName == searchedFunctionName) {
			return cursor;
		}
	}

	// Recursively traverse child cursors
	std::optional<CXCursor> resultCursor;
	clang_visitChildren(cursor, [&resultCursor, searchedFunctionName](CXCursor child, CXCursor parent, CXClientData clientData) -> CXChildVisitResult {
		resultCursor = reinterpret_cast<ASTVisitor*>(clientData)->TraverseCursor(child, searchedFunctionName);
		if (resultCursor.has_value()) {
			return CXChildVisit_Break;
		}
		else {
			return CXChildVisit_Continue;
		}
		}, this);

	return resultCursor;



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