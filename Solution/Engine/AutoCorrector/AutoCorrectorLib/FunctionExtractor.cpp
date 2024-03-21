#include "FunctionExtractor.h"
#include "ASTTraverser.h"

std::string FunctionExtractor::extractFunction(std::string translationUnit, std::string functionName)
{
	ASTTraverser traverser(translationUnit.c_str());
	std::string function = traverser.getFunction(functionName);
	return function;
}