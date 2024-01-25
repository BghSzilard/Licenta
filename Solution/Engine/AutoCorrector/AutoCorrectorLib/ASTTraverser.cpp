#include "ASTTraverser.h"

#include <string>
#include <iostream>

ASTTraverser::ASTTraverser(const char* inputFile)
{
	m_inputFile = inputFile;
}

void ASTTraverser::traverseAST()
{
    m_foundMain = false;
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
		return static_cast<ASTTraverser*>(client_data)->MainVisitor(current_cursor, parent, nullptr);
		};

	clang_visitChildren(cursor, visitorWrapper, nullptr);
}

bool ASTTraverser::foundMain()
{
    traverseAST();
    return m_foundMain;
}

CXChildVisitResult ASTTraverser::MainVisitor(CXCursor current_cursor, CXCursor parent, CXClientData client_data)
{
    CXCursorKind cursorKind = clang_getCursorKind(current_cursor);

    CXSourceLocation location = clang_getCursorLocation(current_cursor);

    if (clang_Location_isInSystemHeader(location) == 0)
    {
        return CXChildVisit_Recurse;
    }

    if (cursorKind == CXCursor_FunctionDecl)
    {
        CXString function_name = clang_getCursorSpelling(current_cursor);
        if (clang_getCString(function_name) && strcmp(clang_getCString(function_name), "main") == 0)
        {
            // Check if it's the correct signature for the main function
            CXType function_type = clang_getCursorType(current_cursor);
            CXType canonical_function_type = clang_getCanonicalType(function_type);

            // Check if it's either int main(void) or int main()
            if (clang_getNumArgTypes(canonical_function_type) == 0)
            {
                m_foundMain = true;
            }
            else if (clang_getNumArgTypes(canonical_function_type) == 2)
            {
                CXType arg1_type = clang_getArgType(canonical_function_type, 0);
                CXType arg2_type = clang_getArgType(canonical_function_type, 1);
                if (clang_getCanonicalType(arg1_type).kind == CXType_Int && clang_getCanonicalType(arg2_type).kind == CXType_Pointer)
                {
                    m_foundMain = true;
                }
            }
        }
        clang_disposeString(function_name);
    }

    return CXChildVisit_Continue;
}