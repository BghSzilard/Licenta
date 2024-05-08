namespace AutoCorrectorEngine;

using System.Collections.Generic;


public class Cluster
{
    public List<List<int>> CreateClusters(double[,] similarityMatrix, double threshold)
    {
        // Initialize clusters
        List<List<int>> clusters = new List<List<int>>();
        for (int i = 0; i < similarityMatrix.GetLength(0); i++)
        {
            clusters.Add(new List<int> { i });
        }

        while (true)
        {
            // Find the maximum similarity
            double maxSimilarity = 0;
            int clusterA = 0, clusterB = 0;

            for (int i = 0; i < clusters.Count; i++)
            {
                for (int j = i + 1; j < clusters.Count; j++)
                {
                    double similarity = CalculateSimilarity(clusters[i], clusters[j], similarityMatrix);
                    if (similarity > maxSimilarity)
                    {
                        maxSimilarity = similarity;
                        clusterA = i;
                        clusterB = j;
                    }
                }
            }

            // Break if the maximum similarity is below the threshold
            if (maxSimilarity < threshold)
            {
                break;
            }

            // Merge the two clusters with the maximum similarity
            clusters[clusterA].AddRange(clusters[clusterB]);
            clusters.RemoveAt(clusterB);
        }

        return clusters;
    }

    static double CalculateSimilarity(List<int> clusterA, List<int> clusterB, double[,] similarityMatrix)
    {
        double totalSimilarity = 0;

        foreach (int studentA in clusterA)
        {
            foreach (int studentB in clusterB)
            {
                totalSimilarity += similarityMatrix[studentA, studentB];
            }
        }

        return totalSimilarity / (clusterA.Count * clusterB.Count);
    }
}