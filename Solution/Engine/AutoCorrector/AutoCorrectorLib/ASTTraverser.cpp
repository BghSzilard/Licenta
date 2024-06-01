#include "ASTTraverser.h"

#include <string>
#include <iostream>
#include <fstream>
#include <queue>

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

    auto visitorWrapper = [](CXCursor current_cursor, CXCursor parent, CXClientData client_data) -> CXChildVisitResult 
        {
        // Cast client_data to ASTTraverser* and call MainVisitor
        return static_cast<ASTTraverser*>(client_data)->MainVisitor(current_cursor, parent, nullptr);
        };

    // Pass 'this' as client_data
    clang_visitChildren(cursor, visitorWrapper, this);
}

void ASTTraverser::traverseASTExtractor()
{
    m_signatures.clear();
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

    auto visitorWrapper = [](CXCursor current_cursor, CXCursor parent, CXClientData client_data) -> CXChildVisitResult
        {
            // Cast client_data to ASTTraverser* and call MainVisitor
            return static_cast<ASTTraverser*>(client_data)->signatureExtractorVisitor(current_cursor, parent, nullptr);
        };

    // Pass 'this' as client_data
    clang_visitChildren(cursor, visitorWrapper, this);
}


bool ASTTraverser::foundMain()
{
    traverseAST();
    return m_foundMain;
}

std::vector<std::string> ASTTraverser::getSignatures()
{
    return m_signatures;
}

std::string ASTTraverser::getFunction(const std::string& functionName)
{
    m_functionName = functionName;
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

    auto visitorWrapper = [](CXCursor current_cursor, CXCursor parent, CXClientData client_data) -> CXChildVisitResult
        {
            // Cast client_data to ASTTraverser* and call MainVisitor
            return static_cast<ASTTraverser*>(client_data)->functionExtractorVisitor(current_cursor, parent, nullptr);
        };

    auto auxVisitorWrapper = [](CXCursor current_cursor, CXCursor parent, CXClientData client_data) -> CXChildVisitResult
        {
            // Cast client_data to ASTTraverser* and call MainVisitor
            return static_cast<ASTTraverser*>(client_data)->dependencyFunctionExtractorVisitor(current_cursor, parent, nullptr);
        };
    
    clang_visitChildren(cursor, visitorWrapper, this);
    
    while (!m_cursorsToTraverse.empty())
    {
        clang_visitChildren(m_cursorsToTraverse.front(), auxVisitorWrapper, this);
        m_cursorsToTraverse.pop();
    }

    return m_functionBody;
}

CXChildVisitResult ASTTraverser::signatureExtractorVisitor(CXCursor current_cursor, CXCursor parent, CXClientData client_data)
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

CXChildVisitResult ASTTraverser::functionExtractorVisitor(CXCursor current_cursor, CXCursor parent, CXClientData client_data)
{
    CXCursorKind cursorKind = clang_getCursorKind(current_cursor);

    CXSourceLocation location = clang_getCursorLocation(current_cursor);

    if (cursorKind == CXCursor_FunctionDecl)
    {
        CXString function_name = clang_getCursorSpelling(current_cursor);
        if (clang_getCString(function_name) && strcmp(clang_getCString(function_name), m_functionName.c_str()) == 0)
        {
            m_cursorsToTraverse.push(current_cursor);

            CXSourceRange range = clang_getCursorExtent(current_cursor);
            CXSourceLocation startLoc = clang_getRangeStart(range);
            CXSourceLocation endLoc = clang_getRangeEnd(range);
            unsigned startLine, endLine;
            clang_getSpellingLocation(startLoc, nullptr, &startLine, nullptr, nullptr);
            clang_getSpellingLocation(endLoc, nullptr, &endLine, nullptr, nullptr);

            std::ifstream inputFile(m_inputFile);
            std::string line;
            for (unsigned i = 1; i <= endLine; ++i) 
            {
                std::getline(inputFile, line);
                if (i >= startLine) {
                    m_functionBody += line;
                    m_functionBody += '\n';
                }
            }
            inputFile.close();

            return CXChildVisit_Break;
        }
        clang_disposeString(function_name);
    }

    return CXChildVisit_Continue;
}

CXChildVisitResult ASTTraverser::dependencyFunctionExtractorVisitor(CXCursor current_cursor, CXCursor parent, CXClientData client_data)
{
    CXCursorKind cursorKind = clang_getCursorKind(current_cursor);

    CXSourceLocation location = clang_getCursorLocation(current_cursor);

    if (clang_getCursorKind(current_cursor) == CXCursor_DeclRefExpr && clang_getCursorKind(clang_getCursorReferenced(current_cursor)) == CXCursor_FunctionDecl)
    {
        m_cursorsToTraverse.push(clang_getCursorReferenced(current_cursor));

        CXSourceRange range = clang_getCursorExtent(m_cursorsToTraverse.back());
        CXSourceLocation startLoc = clang_getRangeStart(range);
        CXSourceLocation endLoc = clang_getRangeEnd(range);
        unsigned startLine, endLine;
        clang_getSpellingLocation(startLoc, nullptr, &startLine, nullptr, nullptr);
        clang_getSpellingLocation(endLoc, nullptr, &endLine, nullptr, nullptr);

        std::ifstream inputFile(m_inputFile);
        std::string line;
        std::string auxiliaryFunction = "";
        for (unsigned i = 1; i <= endLine; ++i)
        {
            std::getline(inputFile, line);
            if (i >= startLine) {
                auxiliaryFunction += line;
                auxiliaryFunction += '\n';
            }
        }
        m_functionBody.insert(0, auxiliaryFunction);
        inputFile.close();

        return CXChildVisit_Break;
        
    }

    return CXChildVisit_Recurse;
}

CXChildVisitResult ASTTraverser::MainVisitor(CXCursor current_cursor, CXCursor parent, CXClientData client_data)
{
    CXCursorKind cursorKind = clang_getCursorKind(current_cursor);

    CXSourceLocation location = clang_getCursorLocation(current_cursor);

    /*if (clang_Location_isInSystemHeader(location) == 0)
    {
        return CXChildVisit_Recurse;
    }*/

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

bool ASTTraverser::isLocationInSystemHeader(CXSourceLocation location)
{
    return clang_Location_isInSystemHeader(location) != 0;
}

std::string ASTTraverser::getFunctionSignature(CXCursor cursor)
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