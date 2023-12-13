#pragma once

#include "ICursorChecker.h"

#include <clang-c/Index.h>

#include <string_view>
#include <vector>
#include <memory>

class VariableHandler
{

public:

	VariableHandler(const char* translationUnit);
	void checkVariable(CXCursor variableCursor);

private:

	const char* m_translationUnit;
	std::vector<std::unique_ptr<ICursorChecker>> m_variableCheckers;
};