#include "Configurations.h"
#include "CompilationChecker.h"

#include <iostream>

int main()
{
    CompilationChecker compilationChecker;
    auto asd = compilationChecker.getCompilationErrorMessage(Configurations::inputFile);
    return 0;
}