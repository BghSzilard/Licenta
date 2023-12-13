#pragma once

#include "ICursorChecker.h"
#include <clang-c/Index.h>

class IsGlobalVariableChecker: public ICursorChecker
{
public:

	IsGlobalVariableChecker(const char* translationUnit);

	bool correct(CXCursor cursor) override;

private:

	CXCursor m_variableCursor;
	const char* m_translationUnit;
};