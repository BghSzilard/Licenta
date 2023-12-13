#pragma once

#include <clang-c/Index.h>

class ICursorChecker
{
public:

	virtual bool correct(CXCursor cursor) = 0;
};