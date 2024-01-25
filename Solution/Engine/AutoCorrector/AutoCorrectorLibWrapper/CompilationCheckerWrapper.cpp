#include "CompilationCheckerWrapper.h"

bool CompilationCheckerWrapper::Compiles(String^ translationUnit)
{
    std::string nativeTranslationUnit = msclr::interop::marshal_as<std::string>(translationUnit);
    return m_compilationChecker->compiles(nativeTranslationUnit);
}