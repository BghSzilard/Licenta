#include "FunctionSignatureExtractor.h"

FunctionSignatureExtractor::FunctionSignatureExtractor(std::string inputFile)
{
	m_inputFile = inputFile;
}

void FunctionSignatureExtractor::traverseAST()
{
    m_signatures.clear();
    CXIndex index = clang_createIndex(0, 0);
    CXTranslationUnit unit = clang_parseTranslationUnit(
        index, m_inputFile.c_str(), nullptr, 0,
        nullptr, 0,
        CXTranslationUnit_None);

    if (unit == nullptr)
    {
        throw std::string("Unable to parse translation unit!");
    }

    CXCursor cursor = clang_getTranslationUnitCursor(unit);

    auto visitorWrapper = [](CXCursor current_cursor, CXCursor parent, CXClientData client_data) -> CXChildVisitResult
        {
            // Cast client_data to ASTTraverser* and call MainVisitor
            return static_cast<FunctionSignatureExtractor*>(client_data)->mainVisitor(current_cursor, parent, nullptr);
        };

    // Pass 'this' as client_data
    clang_visitChildren(cursor, visitorWrapper, this);
}

std::vector<std::string> FunctionSignatureExtractor::getSignatures() const
{
    return m_signatures;
}

CXChildVisitResult FunctionSignatureExtractor::mainVisitor(CXCursor current_cursor, CXCursor parent, CXClientData client_data)
{
    CXSourceLocation location = clang_getCursorLocation(current_cursor);

    if (isLocationInSystemHeader(location))
    {
        return CXChildVisit_Continue;
    }

    if (clang_getCursorKind(current_cursor) == CXCursor_FunctionDecl)
    {
        std::string signature = getFunctionSignature(current_cursor);
        m_signatures.push_back(signature);
    }

    return CXChildVisit_Recurse;
}

std::string FunctionSignatureExtractor::getFunctionSignature(CXCursor cursor)
{
    std::string signature;
    CXType functionType = clang_getCursorType(cursor);

    // Get the return type
    CXType returnType = clang_getResultType(functionType);
    CXString returnTypeSpelling = clang_getTypeSpelling(returnType);
    signature += clang_getCString(returnTypeSpelling);
    clang_disposeString(returnTypeSpelling);

    signature += " ";

    // Get the function name
    CXString functionName = clang_getCursorSpelling(cursor);
    signature += clang_getCString(functionName);
    clang_disposeString(functionName);

    signature += "(";

    // Get the function parameters
    int numArgs = clang_Cursor_getNumArguments(cursor);
    for (int i = 0; i < numArgs; ++i) {
        if (i != 0) {
            signature += ", ";
        }
        CXCursor argCursor = clang_Cursor_getArgument(cursor, i);
        CXType argType = clang_getCursorType(argCursor);
        CXString argTypeSpelling = clang_getTypeSpelling(argType);
        signature += clang_getCString(argTypeSpelling);
        clang_disposeString(argTypeSpelling);
    }

    signature += ")";

    return signature;
}

bool FunctionSignatureExtractor::isLocationInSystemHeader(CXSourceLocation location)
{
    return clang_Location_isInSystemHeader(location) != 0;
}