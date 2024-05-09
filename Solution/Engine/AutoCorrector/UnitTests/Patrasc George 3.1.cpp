
#include <vector>
#include <algorithm>
#include <iostream>

void sort(std::vector<int> myVec)
{
    std::sort(myVec.begin(), myVec.end(), std::greater<int>());
}

void testSort()
{
    std::vector<int> input = {12, 25, 9, 77};
    std::vector<int> expected = {77, 25, 12, 9};
    std::vector<int> realOutput = input;
    sort(realOutput);
    if (realOutput == expected)
        std::cout << "All unit tests passed" << std::endl;
    else
        std::cout << "Input {" << input[0] << ", " << input[1] << ", " << input[2] << ", " << input[3] << "} Expected output {" << expected[0] << ", " << expected[1] << ", " << expected[2] << ", " << expected[3] << "} Real Output {" << realOutput[0] << ", " << realOutput[1] << ", " << realOutput[2] << ", " << realOutput[3] << "}" << std::endl;
}

int main()
{
    testSort();
    return 0;
}
