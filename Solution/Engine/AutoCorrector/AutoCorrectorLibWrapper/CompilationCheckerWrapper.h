#pragma once

#include "../AutoCorrectorLib/CompilationChecker.h"
#include <msclr/marshal_cppstd.h>

using namespace System;
using namespace System::Runtime::InteropServices; // For string conversion

public ref class CompilationCheckerWrapper
{
private:
	CompilationChecker* m_compilationChecker;

public:
	CompilationCheckerWrapper()
	{
		m_compilationChecker = new CompilationChecker();
	}

	bool Compiles(String^ translationUnit);

	bool ContainsMain(String^ translatinoUnit);
};