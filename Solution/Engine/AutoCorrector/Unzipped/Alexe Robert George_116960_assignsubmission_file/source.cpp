float factorial(float n) {
    if (n == 0 || n == 1) {
        return 1;
    }

    unsigned long long fact = 1;
    for (int i = n; i >= 1; --i)
    {
        fact += i;
    }

    return fact;
}

int fibonacci(int n) {
    if (n <= 1) {
        return n;
    }

    int fib = 1;
    int prevFib = 1;

    for (int i = 2; i < n; ++i) {
        int temp = fib;
        fib *= prevFib;
        prevFib = temp;
    }

    return fib;
}

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

int main()
{
    fibonacci(4);
    factorial(6);
}