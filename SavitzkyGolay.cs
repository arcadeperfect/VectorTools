using System;
using UnityEngine;

namespace VectorTools
{
    public class SavitzkyGolay
    {
        public static Vector2[] ApplySavitzkyGolayFilter(Vector2[] path, int windowSize, int polynomialOrder)
        {
            if (path.Length < 2 || windowSize < 1 || polynomialOrder < 0 || windowSize % 2 == 0) return path;

            var newPath = new Vector2[path.Length];

            for (var i = 0; i < path.Length; i++)
                newPath[i] = ApplySavitzkyGolayFilterToPoint(path, i, windowSize, polynomialOrder);

            return newPath;
        }

        private static Vector2 ApplySavitzkyGolayFilterToPoint(Vector2[] path, int index, int windowSize,
            int polynomialOrder)
        {
            var halfWindowSize = windowSize / 2;
            var start = Mathf.Max(0, index - halfWindowSize);
            var end = Mathf.Min(path.Length - 1, index + halfWindowSize);

            var coeffs = SavitzkyGolayCoefficients(windowSize, polynomialOrder);

            float x = 0;
            float y = 0;

            for (int i = start, j = 0; i <= end; i++, j++)
            {
                var coeffIndex = j - (index - start);

                if (i < 0 || i >= path.Length || coeffIndex < 0 || coeffIndex >= coeffs.Length) continue;

                x += path[i].x * coeffs[coeffIndex];
                y += path[i].y * coeffs[coeffIndex];
            }

            return new Vector2(x, y);
        }

        private static float[] SavitzkyGolayCoefficients(int windowSize, int polynomialOrder)
        {
            var m = (windowSize - 1) / 2;
            var matrixA = new float[windowSize, polynomialOrder + 1];
            var matrixB = new float[polynomialOrder + 1, polynomialOrder + 1];

            for (var i = -m; i <= m; i++)
            for (var j = 0; j <= polynomialOrder; j++)
                matrixA[i + m, j] = Mathf.Pow(i, j);

            for (var i = 0; i <= polynomialOrder; i++)
            for (var j = 0; j <= polynomialOrder; j++)
            {
                float sum = 0;

                for (var k = -m; k <= m; k++) sum += Mathf.Pow(k, i + j);

                matrixB[i, j] = sum;
            }

            var inverseMatrixB = MatrixInverse(matrixB);
            var coeffsMatrix = MatrixMultiply(matrixA, inverseMatrixB);

            var coeffs = new float[windowSize];

            for (var i = 0; i < windowSize; i++) coeffs[i] = coeffsMatrix[i, 0];

            return coeffs;
        }

        private static float[,] MatrixInverse(float[,] matrix)
        {
            var n = matrix.GetLength(0);
            var result = new float[n, n];
            var temp = new float[n, n];

            // Initialize the result matrix to the identity matrix
            for (var i = 0; i < n; i++)
            for (var j = 0; j < n; j++)
                result[i, j] = i == j ? 1 : 0;

            // Copy the input matrix to a temporary matrix
            Array.Copy(matrix, temp, matrix.Length);

            // Perform Gaussian elimination
            for (var i = 0; i < n; i++)
            {
                // Find the pivot row
                var pivotRow = i;
                var maxValue = Mathf.Abs(temp[i, i]);

                for (var j = i + 1; j < n; j++)
                {
                    var currentValue = Mathf.Abs(temp[j, i]);
                    if (currentValue > maxValue)
                    {
                        maxValue = currentValue;
                        pivotRow = j;
                    }
                }

                // Swap rows i and pivotRow
                for (var j = 0; j < n; j++)
                {
                    var swap = temp[i, j];
                    temp[i, j] = temp[pivotRow, j];
                    temp[pivotRow, j] = swap;

                    swap = result[i, j];
                    result[i, j] = result[pivotRow, j];
                    result[pivotRow, j] = swap;
                }

                // Normalize row i
                var scale = temp[i, i];
                for (var j = 0; j < n; j++)
                {
                    temp[i, j] /= scale;
                    result[i, j] /= scale;
                }

                // Eliminate other rows
                for (var j = 0; j < n; j++)
                    if (j != i)
                    {
                        var ratio = temp[j, i];

                        for (var k = 0; k < n; k++)
                        {
                            temp[j, k] -= ratio * temp[i, k];
                            result[j, k] -= ratio * result[i, k];
                        }
                    }
            }

            return result;
        }

        private static float[,] MatrixMultiply(float[,] matrixA, float[,] matrixB)
        {
            var rowsA = matrixA.GetLength(0);
            var colsA = matrixA.GetLength(1);
            var rowsB = matrixB.GetLength(0);
            var colsB = matrixB.GetLength(1);

            if (colsA != rowsB) throw new ArgumentException("Invalid matrix dimensions for multiplication.");

            var result = new float[rowsA, colsB];

            for (var i = 0; i < rowsA; i++)
            for (var j = 0; j < colsB; j++)
            {
                float sum = 0;

                for (var k = 0; k < colsA; k++) sum += matrixA[i, k] * matrixB[k, j];

                result[i, j] = sum;
            }

            return result;
        }
    }
}