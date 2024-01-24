#include "CompilationCheckerWrapper.h"

#include <msclr/marshal_cppstd.h>
using namespace System::Runtime::InteropServices;


CompilationCheckerWrapper::CompilationCheckerWrapper()
{
	m_compilationChecker = new CompilationChecker();

}

bool CompilationCheckerWrapper::compiles(String^ translationUnit)
{
    // Convert System::String to wchar_t* (C-style string)
    const wchar_t* wcharString = reinterpret_cast<const wchar_t*>((System::Runtime::InteropServices::Marshal::StringToHGlobalUni(translationUnit)).ToPointer());

    // Convert wchar_t* to std::wstring
    std::wstring translationUnitW(wcharString);

    // Free the allocated memory for the wcharString
    System::Runtime::InteropServices::Marshal::FreeHGlobal(IntPtr((void*)wcharString));

    // Convert std::wstring to std::string
    std::string translationUnitA(translationUnitW.begin(), translationUnitW.end());

    return m_compilationChecker->compiles(translationUnitA);
}