#include "Configurations.h"
#include "CompilationChecker.h"
#include "CleanCodeChecker.h"

#include <iostream>
#include <vector>



int main()
{
    //CompilationChecker compilationChecker;
    CleanCodeChecker cleanCodeChecker(Configurations::inputFile);
    cleanCodeChecker.traverseAST();
    return 0;
}