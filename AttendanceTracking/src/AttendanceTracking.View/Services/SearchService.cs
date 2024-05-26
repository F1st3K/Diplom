using System;
using System.Collections.Generic;

namespace AttendanceTracking.View.Services
{
    public class SearchService
    {

        public int GetLevenshteinDistance(string a, string b)
        {
            if (string.IsNullOrEmpty(a)) return b?.Length ?? 0;
            if (string.IsNullOrEmpty(b)) return a.Length;

            if (a.Contains(b) || b.Contains(a))
                return 0;

            int lenA = a.Length, lenB = b.Length;

            // Create and initialize the matrix
            int[,] matrix = new int[lenA + 1, lenB + 1];
            for (int i = 0; i <= lenA; i++) matrix[i, 0] = i;
            for (int j = 0; j <= lenB; j++) matrix[0, j] = j;

            // Initialize a dictionary to keep track of the last occurence
            // of each character in string a
            Dictionary<char, int> lastRow = new Dictionary<char, int>();

            // Iterate over the elements of the matrix
            for (int i = 1; i <= lenA; i++)
            {
                int lastCol = 0;
                for (int j = 1; j <= lenB; j++)
                {
                    // Calculate the cost of the current operation
                    int cost = a[i - 1] == b[j - 1] ? 0 : 1;

                    // Check if we can apply a transposition
                    if (i > 1 && j > 1 && a[i - 1] == b[j - 2] && a[i - 2] == b[j - 1])
                    {
                        matrix[i, j] = Math.Min(matrix[i, j], matrix[i - 2, j - 2] + cost);
                    }

                    // Calculate the minimum cost
                    matrix[i, j] = Math.Min(matrix[i - 1, j] + 1, Math.Min(matrix[i, j - 1] + 1, matrix[i - 1, j - 1] + cost));

                    // Check if we can apply a forbidden transposition
                    if (lastRow.ContainsKey(b[j - 1]) && lastCol > 0)
                    {
                        int row = lastRow[b[j - 1]];
                        int col = lastCol;
                        if (i > row && j > col)
                        {
                            matrix[i, j] = Math.Min(matrix[i, j], matrix[row, col] + (i - row - 1) + 1 + (j - col - 1));
                        }
                    }

                    // Update the last occurence of the current character in string a
                    if (a[i - 1] == b[j - 1])
                    {
                        lastCol = j;
                    }
                }

                // Add the last occurence of the current character in string a to the dictionary
                lastRow[a[i - 1]] = i;
            }

            // Return the final cost
            return matrix[lenA, lenB];
        }
    }
}