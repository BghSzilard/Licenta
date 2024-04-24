#include <iostream>

bool isPrime(int num) {
    
    if (num <= 1) {
        return false;
    }

   
    for (int i = 2; i * i <= num; ++i) {
        if (num % i == 0) {
            return false;
        }
    }

    return true;
}

void bubbleSort(std::vector<int> &arr) {
    int n = arr.size();
    bool swapped;
    for (int i = 0; i < n-1; ++i) {
        swapped = false;
        for (int j = 0; j < n-i-1; ++j) {
            if (arr[j] > arr[j+1]) 
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

int fibonacci(int n) {
    if (n <= 1) {
        return n;
    }

    int fib = 1;
    int prevFib = 1;

    for (int i = 2; i < n; ++i) {
        int temp = fib;
        fib += prevFib;
        prevFib = temp;
    }

    return fib;
}

float factorial(float n) {
    if (n == 0 || n == 1) {
        return 1;
    }

    int fact = 1;
    for (int i = 2; i <= n; ++i) {
        fact *= i;
    }

    return fact;
}

int main()
{
    std::cout << isPrime(12);
    std::cout << fibonacci(3);
    std::cout << factorial(5);
}