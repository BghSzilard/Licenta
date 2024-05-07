
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
            if (arr[j] > arr[j+1]) {
                // Swap arr[j] and arr[j+1]
                int temp = arr[j];
                arr[j] = arr[j+1];
                arr[j+1] = temp;
                swapped = true;
            }
        }
        // If no two elements were swapped in the inner loop, then the array is already sorted
        if (!swapped)
            break;
    }
}

void testBubbleSort() {
    std::vector<int> input = {12, 25, 9, 77};
    std::vector<int> expectedOutput = {77, 25, 12, 9};
    bubbleSort(input);
    if (input == expectedOutput) {
        std::cout << "All unit tests passed" << std::endl;
    } else {
        std::cout << "Input {12, 25, 9, 77} Expected output {77, 25, 12, 9} Real Output {";
        for (int i : input) {
            std::cout << i << " ";
        }
        std::cout << "}" << std::endl;
    }
}

int main() {
    testBubbleSort();
    return 0;
}
