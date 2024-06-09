#include <vector>
#include <algorithm>
#include <iostream>
#include <deque>
#include <utility>
#include <climits>

std::vector<int> mySort(std::vector<int> myVec)
{
    std::sort(myVec.begin(), myVec.end(), std::greater<int>());
    return myVec;
}

struct CellInfo {
    int row, col, distance;
};

bool isCellValid(int row, int col, int numRows, int numCols) {
    return (row >= 0 && row < numRows && col >= 0 && col < numCols);
}

int minPathLength (const std::vector<std::vector<int>>& grid, int numRows, int numCols,
                     int startRow, int startCol, int endRow, int endCol,
                     const std::vector<std::pair<int, int>>& obstacles) {
    int dx[] = {-1, 1, 0, 0};
    int dy[] = {0, 0, -1, 1};

    std::vector<std::vector<bool>> visited(numRows, std::vector<bool>(numCols, false));
    std::deque<CellInfo> dq;

    visited[startRow][startCol] = true;
    dq.push_back({startRow, startCol, 0});

    for (const auto& obstacle : obstacles) {
        int obstacleRow = obstacle.first;
        int obstacleCol = obstacle.second;
        visited[obstacleRow][obstacleCol] = true;
    }

    while (!dq.empty()) {
        CellInfo currentCell = dq.front();
        dq.pop_front();

        if (currentCell.row == endRow && currentCell.col == endCol)
            return currentCell.distance;

        for (int i = 0; i < 4; ++i) {
            int newRow = currentCell.row + dx[i];
            int newCol = currentCell.col + dy[i];

            if (isCellValid(newRow, newCol, numRows, numCols) && !visited[newRow][newCol]) {
                visited[newRow][newCol] = true;
                dq.push_back({newRow, newCol, currentCell.distance + 1});
            }
        }
    }

    return INT_MAX;
}

void printOrderedNumbers(std::vector<int>& numbers) {
    std::stable_sort(numbers.begin(), numbers.end(),  { return i % 2 != j % 2 && i % 2 != 0; });

    for (const auto& number : numbers) {
        std::cout << number << " ";
    }
}

int main()
{
    
}