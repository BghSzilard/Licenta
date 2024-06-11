#include <iostream>
#include <vector>
#include <queue>
#include <climits>

#include "header.h"

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

int main()
{
   
}