#include "VariableHandler.h"
#include "Configurations.h"
#include "IsGlobalVariableChecker.h"

#include <memory>

VariableHandler::VariableHandler(const char* translationUnit)
{
	m_translationUnit = translationUnit;

	//temporary fill

	IsGlobalVariableChecker globalVariableChecker(m_translationUnit);
	std::unique_ptr<ICursorChecker> pGlobalVariableChecker = std::make_unique<IsGlobalVariableChecker>(globalVariableChecker);
	m_variableCheckers.push_back(std::move(pGlobalVariableChecker));
}

void VariableHandler::checkVariable(CXCursor variableCursor)
{
	for (int i = 0; i < m_variableCheckers.size(); ++i)
	{
		m_variableCheckers[i]->correct(variableCursor);
	}
}