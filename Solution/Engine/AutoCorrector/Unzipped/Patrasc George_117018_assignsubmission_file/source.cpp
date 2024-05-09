#include <vector>
#include <algorithm>

std::vector<int> sort(std::vector<int> myVec)
{
    std::sort(myVec.begin(), myVec.end(), std::greater<int>());
    return myVec;
}

int main()
{
    
}