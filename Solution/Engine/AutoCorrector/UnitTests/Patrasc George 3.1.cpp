
#include <vector>
#include <algorithm>

void sortVector(std::vector<int>& vec) {
    std::sort(vec.begin(), vec.end(), std::greater<int>());
}

void checkOutput(std::vector<int> input, std::vector<int> expectedOutput) {
    sortVector(input);
    if (input == expectedOutput) {
        std::cout << "All unit tests passed" << std::endl;
    } else {
        std::cout << "Input {";
        for (int i : input) {
            std::cout << i << " ";
        }
        std::cout << "} Expected output {";
        for (int i : expectedOutput) {
            std::cout << i << " ";
        }
        std::cout << "} Real Output {";
        for (int i : input) {
            std::cout << i << " ";
        }
        std::cout << "}" << std::endl;
    }
}

int main() {
    std::vector<int> input = {12, 25, 9, 77};
    std::vector<int> expectedOutput = {77, 25, 12, 9};
    checkOutput(input, expectedOutput);
    return 0;
}
