#pragma once

#include "CompilationChecker.h"

using namespace System;

public ref class CompilationCheckerWrapper
{
private:

	CompilationChecker* m_compilationChecker;

public:

	bool compiles(String^ translationUnit);
	CompilationCheckerWrapper();
};