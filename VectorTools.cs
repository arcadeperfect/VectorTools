using System;
using UnityEngine;

namespace VectorTools
{
    public static class VectorTools
    {
        
        // method that takes a vector2 array and removes points that are too close to each other within tolerance
        public static Vector2[] RemoveClosePoints(Vector2[] inputLine, float tolerance)
        {
            if (inputLine == null || inputLine.Length < 2)
                throw new ArgumentException("Input line must contain at least two points.");

            if (tolerance < 0) throw new ArgumentException("Tolerance must be non-negative.");

            var n = inputLine.Length;
            var outputLine = new Vector2[n];
            var outputLineLength = 0;

            outputLine[outputLineLength++] = inputLine[0];

            for (var i = 1; i < n; i++)
            {
                var lastPoint = outputLine[outputLineLength - 1];
                var currentPoint = inputLine[i];

                if (Vector2.Distance(lastPoint, currentPoint) > tolerance)
                    outputLine[outputLineLength++] = currentPoint;
            }

            if (outputLineLength == n) return outputLine;

            var trimmedOutputLine = new Vector2[outputLineLength];
            Array.Copy(outputLine, trimmedOutputLine, outputLineLength);
            return trimmedOutputLine;
        }
        

        public static Vector2[] SmoothLineAverage(Vector2[] inputLine, int windowSize)
        {
            if (inputLine == null || inputLine.Length < 2)
                throw new ArgumentException("Input line must contain at least two points.");

            if (windowSize < 1) throw new ArgumentException("Window size must be at least 1.");

            var n = inputLine.Length;
            var smoothedLine = new Vector2[n];

            smoothedLine[0] = inputLine[0]; // Maintain the beginning point
            smoothedLine[n - 1] = inputLine[n - 1]; // Maintain the end point

            for (var i = 1; i < n - 1; i++)
            {
                var halfWindowSize = windowSize / 2;
                var start = Math.Max(0, i - halfWindowSize);
                var end = Math.Min(n - 1, i + halfWindowSize);
                var actualWindowSize = end - start + 1;

                var sum = Vector2.zero;
                for (var j = start; j <= end; j++) sum += inputLine[j];

                smoothedLine[i] = sum / actualWindowSize;
            }

            return smoothedLine;
        }
        public static Vector2[] SmoothLineGaussian(Vector2[] inputLine, int windowSize, float sigma)
        {
            if (inputLine == null || inputLine.Length < 2)
                throw new ArgumentException("Input line must contain at least two points.");

            if (windowSize < 1) throw new ArgumentException("Window size must be at least 1.");

            var n = inputLine.Length;
            var smoothedLine = new Vector2[n];

            smoothedLine[0] = inputLine[0]; // Maintain the beginning point
            smoothedLine[n - 1] = inputLine[n - 1]; // Maintain the end point

            var gaussianKernel = GenerateGaussianKernel1(windowSize, sigma);

            for (var i = 1; i < n - 1; i++)
            {
                var halfWindowSize = windowSize / 2;
                var sum = Vector2.zero;

                for (var j = -halfWindowSize; j <= halfWindowSize; j++)
                {
                    var index = Mathf.Clamp(i + j, 0, n - 1);
                    sum += inputLine[index] * gaussianKernel[j + halfWindowSize];
                }

                smoothedLine[i] = sum;
            }

            return smoothedLine;
        }
        private static float[] GenerateGaussianKernel1(int windowSize, float sigma)
        {
            var halfWindowSize = windowSize / 2;
            var kernel = new float[2 * halfWindowSize + 1];
            float sum = 0;

            for (var i = -halfWindowSize; i <= halfWindowSize; i++)
            {
                float x = i;
                var weight = 1.0f / (Mathf.Sqrt(2 * Mathf.PI) * sigma) * Mathf.Exp(-(x * x) / (2 * sigma * sigma));
                kernel[i + halfWindowSize] = weight;
                sum += weight;
            }

            // Normalize the kernel
            for (var i = 0; i < 2 * halfWindowSize + 1; i++) kernel[i] /= sum;

            return kernel;
        }
        public static Vector2[] SmoothLineFaster(Vector2[] inputLine, int windowSize, float sigma)
        {
            if (inputLine == null || inputLine.Length < 2)
                throw new ArgumentException("Input line must contain at least two points.");

            if (windowSize < 1) throw new ArgumentException("Window size must be at least 1.");

            var n = inputLine.Length;
            var smoothedLine = new Vector2[n];

            var gaussianKernel = GenerateGaussianKernel(windowSize, sigma);

            // Apply the 1D Gaussian filter separately for x and y
            var xInput = new float[n];
            var yInput = new float[n];
            for (var i = 0; i < n; i++)
            {
                xInput[i] = inputLine[i].x;
                yInput[i] = inputLine[i].y;
            }

            var xSmoothed = ApplyGaussianFilter(xInput, gaussianKernel);
            var ySmoothed = ApplyGaussianFilter(yInput, gaussianKernel);

            for (var i = 0; i < n; i++) smoothedLine[i] = new Vector2(xSmoothed[i], ySmoothed[i]);

            // Maintain the beginning and end points
            smoothedLine[0] = inputLine[0];
            smoothedLine[n - 1] = inputLine[n - 1];

            return smoothedLine;
        }
        private static float[] ApplyGaussianFilter(float[] inputData, float[] gaussianKernel)
        {
            var n = inputData.Length;
            var halfWindowSize = gaussianKernel.Length / 2;
            var outputData = new float[n];

            for (var i = 0; i < n; i++)
            {
                float sum = 0;

                for (var j = -halfWindowSize; j <= halfWindowSize; j++)
                {
                    var index = Mathf.Clamp(i + j, 0, n - 1);
                    sum += inputData[index] * gaussianKernel[j + halfWindowSize];
                }

                outputData[i] = sum;
            }

            return outputData;
        }
        private static float[] GenerateGaussianKernel(int windowSize, float sigma)
        {
            var halfWindowSize = windowSize / 2;
            var kernel = new float[2 * halfWindowSize + 1];
            float sum = 0;

            for (var i = -halfWindowSize; i <= halfWindowSize; i++)
            {
                float x = i;
                var weight = 1.0f / (Mathf.Sqrt(2 * Mathf.PI) * sigma) * Mathf.Exp(-(x * x) / (2 * sigma * sigma));
                kernel[i + halfWindowSize] = weight;
                sum += weight;
            }

            // Normalize the kernel
            for (var i = 0; i < 2 * halfWindowSize + 1; i++) kernel[i] /= sum;

            return kernel;
        }
    }
}