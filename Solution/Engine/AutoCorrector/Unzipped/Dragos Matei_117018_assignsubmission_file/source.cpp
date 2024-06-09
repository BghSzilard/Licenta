#include <iostream>
#include <vector>
#include <queue>
#include <climits>

using namespace std;

// Define a structure to represent a cell in the matrix
struct Cell {
    int x, y, dist;
};

// Function to check if a cell is valid (within bounds of matrix)
bool isValid(int x, int y, int n, int m) {
    return (x >= 0 && x < n && y >= 0 && y < m);
}

// Function to find the minimum length path
int minPathLength(vector<vector<int>>& matrix, int n, int m,
                  int i1, int j1, int i2, int j2, vector<pair<int, int>>& positions) {
    // Define the directions: up, down, left, right
    int dx[] = {-1, 1, 0, 0};
    int dy[] = {0, 0, -1, 1};
    
    // Initialize visited array and distance array
    vector<vector<bool>> visited(n, vector<bool>(m, false));
    
    // Initialize queue for BFS
    queue<Cell> q;
    
    // Mark the start position as visited and enqueue it
    visited[i1][j1] = true;
    q.push({i1, j1, 0});
    
    // Iterate through each given position and mark them as visited
    for (auto& pos : positions) {
        int x = pos.first;
        int y = pos.second;
        visited[x][y] = true;
    }
    
    // BFS
    while (!q.empty()) {
        Cell cell = q.front();
        q.pop();
        
        // If we reach the destination position, return the distance
        if (cell.x == i2 && cell.y == j2)
            return cell.dist;
        
        // Explore neighboring cells
        for (int i = 0; i < 4; ++i) {
            int newX = cell.x + dx[i];
            int newY = cell.y + dy[i];
            
            // Check if the new cell is valid and not visited
            if (isValid(newX, newY, n, m) && !visited[newX][newY]) {
                visited[newX][newY] = true;
                q.push({newX, newY, cell.dist + 1});
            }
        }
    }
    
    // If no path is found
    return INT_MAX;
}

void printOrderedNumbers(const vector<int>& numbers) {
    // Sort the numbers in ascending order
    vector<int> sortedNumbers = numbers;
    sort(sortedNumbers.begin(), sortedNumbers.end());

    vector<int> oddNumbers; // Vector for storing odd numbers
    vector<int> evenNumbers; // Vector for storing even numbers

    // Separate odd and even numbers into separate vectors
    for (int n : sortedNumbers) {
        if (n % 2 == 0) {
            evenNumbers.push_back(n);
        } else {
            oddNumbers.push_back(n);
        }
    }

    // Print odd numbers followed by even numbers
    for (int n : oddNumbers) {
        cout << n << " ";
    }
    for (int n : evenNumbers) {
        cout << n << " ";
    }
}

// Function to merge two subarrays of arr[]
void merge(std::vector<int>& arr, int left, int mid, int right) {
    int n1 = mid - left + 1;
    int n2 = right - mid;

    std::vector<int> L(n1), R(n2);

    for (int i = 0; i < n1; i++)
        L[i] = arr[left + i];
    for (int j = 0; j < n2; j++)
        R[j] = arr[mid + 1 + j];

    int i = 0, j = 0, k = left;
    while (i < n1 && j < n2) {
        if (L[i] >= R[j]) {
            arr[k] = L[i];
            i++;
        } else {
            arr[k] = R[j];
            j++;
        }
        k++;
    }

    while (i < n1) {
        arr[k] = L[i];
        i++;
        k++;
    }

    while (j < n2) {
        arr[k] = R[j];
        j++;
        k++;
    }
}

// Iterative Merge Sort function to sort arr in descending order
void mergeSortDescending(std::vector<int>& arr) {
    int n = arr.size();
    for (int curr_size = 1; curr_size <= n-1; curr_size = 2*curr_size) {
        for (int left_start = 0; left_start < n-1; left_start += 2*curr_size) {
            int mid = std::min(left_start + curr_size - 1, n-1);
            int right_end = std::min(left_start + 2*curr_size - 1, n-1);

            merge(arr, left_start, mid, right_end);
        }
    }
}

int main() {
    std::vector<int> numbers = {1, 2, 3, 4, 5, 6, 7, 8, 9, 10};
    printOrderedNumbers(numbers);
    return 0;
}