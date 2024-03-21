#include "FunctionExtractorWrapper.h"

String^ FunctionExtractorWrapper::GetFunction(String^ translationUnit, String^ functionName)
{
	std::string nativeTranslationUnit = msclr::interop::marshal_as<std::string>(translationUnit);
	std::string nativeFunctionName = msclr::interop::marshal_as<std::string>(functionName);
	std::string functionBody = m_functionExtractor->extractFunction(nativeTranslationUnit, nativeFunctionName);
	String^ functionBodyManaged = msclr::interop::marshal_as<System::String^>(functionBody);
	return functionBodyManaged;
}