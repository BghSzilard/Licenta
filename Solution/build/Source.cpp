#include "Configurations.h"
#include "CompilationChecker.h"
#include "ASTTraverser.h"

#include <iostream>
#include <vector>



int main()
{
    //CompilationChecker compilationChecker;
   
    ASTTraverser traverser(Configurations::inputFile);
    traverser.traverseAST();

    return 0;
}