#include "FunctionSignatureExtractor.h"

std::vector<std::string> FunctionSignatureExtractor::getSignatures(std::string translationUnit)
{
	ASTTraverser traverser(translationUnit.c_str());
	traverser.traverseASTExtractor();
	return traverser.getSignatures();
}