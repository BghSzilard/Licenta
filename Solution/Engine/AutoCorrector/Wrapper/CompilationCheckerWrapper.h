#pragma once

#include "D:\UNITBV\Anul 3\LIC\Licenta\Licenta\Solution\build\CompilationChecker.h"

using namespace System;

public ref class CompilationCheckerWrapper
{
private:

	CompilationChecker* m_compilationChecker;

public:

	CompilationCheckerWrapper();

	bool compiles(String^ translationUnit);
};