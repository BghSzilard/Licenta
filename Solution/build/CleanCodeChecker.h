#pragma once

#include "ASTTraverser.h"

#include <string_view>

class CleanCodeChecker : public ASTTraverser
{

public:

	CleanCodeChecker(std::string_view translationUnit);

private:

	CXChildVisitResult Visitor(CXCursor current_cursor, CXCursor parent, CXClientData client_data);
};