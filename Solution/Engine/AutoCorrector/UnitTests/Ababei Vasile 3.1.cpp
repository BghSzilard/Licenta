
#include <iostream>
#include <vector>
#include <queue>
#include <climits>

void bubbleSort(std::vector<int> &arr) {
    int n = arr.size();
    bool swapped;
    for (int i = 0; i < n-1; ++i) {
        swapped = false;
        for (int j = 0; j < n-i-1; ++j) {
            if (arr[j] < arr[j+1]) 
            {
                int temp = arr[j];
                arr[j] = arr[j+1];
                arr[j+1] = temp;
                swapped = true;
            }
        }
        if (!swapped)
            break;
    }
}

void checkOutput(std::vector<int> input, std::vector<int> expectedOutput) {
    bubbleSort(input);
    bool passed = true;
    for (int i = 0; i < input.size(); ++i) {
        if (input[i] != expectedOutput[i]) {
            passed = false;
            break;
        }
    }
    if (passed) {
        std::cout << "All unit tests passed" << std::endl;
    } else {
        std::cout << "Input {";
        for (int i = 0; i < input.size(); ++i) {
            std::cout << input[i] << " ";
        }
        std::cout << "} Expected output {";
        for (int i = 0; i < expectedOutput.size(); ++i) {
            std::cout << expectedOutput[i] << " ";
        }
        std::cout << "} Real Output {";
        for (int i = 0; i < input.size(); ++i) {
            std::cout << input[i] << " ";
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
