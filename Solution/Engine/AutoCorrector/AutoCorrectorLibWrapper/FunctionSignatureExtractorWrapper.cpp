#include "FunctionSignatureExtractorWrapper.h"

List<String^>^ FunctionSignatureExtractorWrapper::GetSignatures(String^ translationUnit)
{
    //// Get the vector of strings

    std::string nativeTranslationUnit = msclr::interop::marshal_as<std::string>(translationUnit);


    std::vector<std::string> vec = m_functionSignatureExtractor->getSignatures(nativeTranslationUnit);

    //// Create a new CLI list
    List<String^>^ list = gcnew List<String^>(5);
    
    //// Iterate over the vector and add each element to the list
    for (const std::string& str : vec)
    {
        // Convert std::string to System::String^
        System::String^ cliString = gcnew System::String(str.c_str());

        // Add to the list
        list->Add(cliString);
    }

    return list;
}