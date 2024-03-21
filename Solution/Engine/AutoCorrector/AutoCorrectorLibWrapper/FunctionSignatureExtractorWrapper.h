#pragma once

#include "../AutoCorrectorLib/FunctionSignatureExtractor.h"
#include <msclr/marshal_cppstd.h>

#include <string>
#include <vector>

using namespace System;
using namespace System::Collections::Generic;
using namespace System::Runtime::InteropServices;

public ref class FunctionSignatureExtractorWrapper
{
private:
	FunctionSignatureExtractor* m_functionSignatureExtractor;

public:
	FunctionSignatureExtractorWrapper()
	{
		
	}

	List<String^>^ GetSignatures(String^ translationUnit);
};