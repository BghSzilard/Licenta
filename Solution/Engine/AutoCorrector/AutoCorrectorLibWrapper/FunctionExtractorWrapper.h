#include "../AutoCorrectorLib/FunctionExtractor.h"

#include <msclr/marshal_cppstd.h>

#include <string>
#include <vector>

using namespace System;
using namespace System::Collections::Generic;
using namespace System::Runtime::InteropServices;

public ref class FunctionExtractorWrapper
{
private:
	FunctionExtractor* m_functionExtractor;

public:
	FunctionExtractorWrapper()
	{

	}

	String^ GetFunction(String^ translationUnit, String^ functionName);
};