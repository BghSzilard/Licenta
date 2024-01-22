#pragma once

#include "ASTTraverser.h"

#include <clang-c/Index.h>

#include <string_view>
#include <optional>

class FunctionIdentifier : public ASTTraverser
{
public:

	FunctionIdentifier(std::string_view translationUnit);

	std::optional<CXCursor> findFunction(std::string_view functionName);

private:

	std::string_view m_translationUnit;

	CXChildVisitResult Visitor(CXCursor currentCursor, CXCursor parent, CXClientData clientData) override;

	// Inherited via ASTTraverser
	std::optional<CXCursor> traverseCursor(const CXCursor& cursor, std::string_view functionName);
};