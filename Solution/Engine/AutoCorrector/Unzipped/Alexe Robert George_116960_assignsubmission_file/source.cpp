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

int main()
{
    fibonacci(4);
    factorial(6);
}