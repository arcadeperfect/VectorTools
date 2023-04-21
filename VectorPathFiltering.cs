using System;
using System.Collections.Generic;
using UnityEngine;

namespace VectorTools
{
    public static class VectorPathFiltering
    {
        /// <summary>
        /// Resamples a path to a new point count.
        /// </summary>
        /// <param name="path"></param>
        /// <param name="pointCount"></param>
        /// <returns></returns>
        public static Vector2[] ResamplePathByPointCount(Vector2[] path, int pointCount)
        {
            if (path.Length < 2 || pointCount <= 1) return path;

            return ResamplePathInternal(path, pointCount);
        }

        /// <summary>
        /// Resamples a path to a new spacing between points.
        /// </summary>
        /// <param name="path"></param>
        /// <param name="spacing"></param>
        /// <returns></returns>
        public static Vector2[] ResamplePathBySpacing(Vector2[] path, float spacing)
        {
            if (path.Length < 2 || spacing <= 0) return path;

            return ResamplePathInternal(path, spacing: spacing);
        }

        /// <summary>
        /// Resamples a path to a new point count based on a fraction of the original point count.
        /// </summary>
        /// <param name="path"></param>
        /// <param name="fraction"></param>
        /// <returns></returns>
        public static Vector2[] ResamplePathByFraction(Vector2[] path, float fraction)
        {
            if (path.Length < 2 || fraction <= 0) return path;

            var newPointCount = Mathf.Max(2, Mathf.RoundToInt(path.Length * fraction));
            return ResamplePathInternal(path, newPointCount);
        }

        /// <summary>
        /// Path resampling
        /// </summary>
        /// <param name="path"></param>
        /// <param name="pointCount"></param>
        /// <param name="spacing"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentException"></exception>
        private static Vector2[] ResamplePathInternal(Vector2[] path, int? pointCount = null, float? spacing = null)
        {
            if (pointCount == null && spacing == null)
                throw new ArgumentException("Either pointCount or spacing must be specified.");

            var newPath = new List<Vector2>();
            newPath.Add(path[0]);

            var totalLength = 0f;
            for (var i = 0; i < path.Length - 1; i++)
            {
                var distance = Vector2.Distance(path[i], path[i + 1]);
                if (distance > Mathf.Epsilon) totalLength += distance;
            }

            if (pointCount != null) spacing = totalLength / (pointCount.Value - 1);

            var currentSpacing = spacing.Value;
            var currentPointIndex = 0;

            for (var i = 0; i < path.Length - 1; i++)
            {
                var segmentLength = Vector2.Distance(path[i], path[i + 1]);

                while (currentSpacing <= segmentLength && segmentLength > Mathf.Epsilon)
                {
                    var t = currentSpacing / segmentLength;
                    var point = Vector2.Lerp(path[i], path[i + 1], t);
                    newPath.Add(point);
                    currentSpacing += spacing.Value;
                }

                currentSpacing -= segmentLength;
            }

            newPath.Add(path[path.Length - 1]);

            return newPath.ToArray();
        }

        /// <summary>
        /// Smooths a path using a moving average filter.
        /// </summary>
        /// <param name="inputLine"></param>
        /// <param name="windowSize"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentException"></exception>
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

        /// <summary>
        /// Smooths a path using a Gaussian filter.
        /// </summary>
        /// <param name="inputLine"></param>
        /// <param name="windowSize"></param>
        /// <param name="sigma"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentException"></exception>
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

        /// <summary>
        /// Generates a Gaussian kernel.
        /// </summary>
        /// <param name="windowSize"></param>
        /// <param name="sigma"></param>
        /// <returns></returns>
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

        public static Vector2[] SmoothLineFaster(Vector2[] inputLine, int windowSize, float sigma, float lerp)
        {
            Vector2[] originalLine = inputLine.Clone() as Vector2[];
            var smoothedLine = SmoothLineFaster(inputLine, windowSize, sigma);

            return originalLine.Lerp(smoothedLine, lerp);

        }


        /// <summary>
        /// Smooths a path using a Gaussian filter, but with a faster implementation maybe?
        /// </summary>
        /// <param name="inputLine"></param>
        /// <param name="windowSize"></param>
        /// <param name="sigma"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentException"></exception>
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

        /// <summary>
        /// Applies a Gaussian filter to a 1D array.
        /// </summary>
        /// <param name="inputData"></param>
        /// <param name="gaussianKernel"></param>
        /// <returns></returns>
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
         
        /// <summary>
        /// Generates a Gaussian kernel.
        /// </summary>
        /// <param name="windowSize"></param>
        /// <param name="sigma"></param>
        /// <returns></returns>
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

        /// <summary>
        /// curve-smoothing algorithm that cuts the corners of a polyline to create a smooth curve. It can be applied iteratively to obtain smoother curves. This algorithm is particularly useful for generating smooth paths from a set of input points.
        /// </summary>
        /// <param name="path"></param>
        /// <param name="iterations"></param>
        /// <returns></returns>
        public static Vector2[] ChaikinsCornerCutting(Vector2[] path, int iterations = 1)
        {
            iterations = Mathf.Max(1, iterations);
            iterations = Mathf.Min(10, iterations);

            if (path.Length < 2) return path;

            var newPath = path;

            for (var k = 0; k < iterations; k++)
            {
                var smoothedPath = new List<Vector2>();

                // Add the first point
                smoothedPath.Add(path[0]);

                // Apply Chaikin's algorithm
                for (var i = 0; i < path.Length - 1; i++)
                {
                    var point1 = Vector2.Lerp(path[i], path[i + 1], 0.25f);
                    var point2 = Vector2.Lerp(path[i], path[i + 1], 0.75f);

                    smoothedPath.Add(point1);
                    smoothedPath.Add(point2);
                }

                // Add the last point
                smoothedPath.Add(path[path.Length - 1]);

                newPath = smoothedPath.ToArray();
                path = newPath;
            }

            return newPath;
        }
        
        /// <summary>
        /// Path simplification method that reduces the number of points in a path while retaining its overall shape.
        /// </summary>
        /// <param name="path"></param>
        /// <param name="epsilon"></param>
        /// <returns></returns>

        public static Vector2[] ApplyRamerDouglasPeucker(Vector2[] path, float epsilon)
        {
            if (path.Length < 3 || epsilon <= 0) return path;

            var simplifiedPath = RamerDouglasPeuckerRecursive(path, 0, path.Length - 1, epsilon);
            return simplifiedPath.ToArray();
        }

        /// <summary>
        /// Recursive implementation of the Ramer-Douglas-Peucker algorithm.
        /// </summary>
        /// <param name="path"></param>
        /// <param name="startIndex"></param>
        /// <param name="endIndex"></param>
        /// <param name="epsilon"></param>
        /// <returns></returns>
        private static List<Vector2> RamerDouglasPeuckerRecursive(Vector2[] path, int startIndex, int endIndex,
            float epsilon)
        {
            float maxDistance = 0;
            var maxIndex = startIndex;

            for (var i = startIndex + 1; i < endIndex; i++)
            {
                var distance = PointToLineSegmentDistance(path[i], path[startIndex], path[endIndex]);
                if (distance > maxDistance)
                {
                    maxDistance = distance;
                    maxIndex = i;
                }
            }

            if (maxDistance > epsilon)
            {
                var leftPath = RamerDouglasPeuckerRecursive(path, startIndex, maxIndex, epsilon);
                var rightPath = RamerDouglasPeuckerRecursive(path, maxIndex, endIndex, epsilon);

                var combinedPath = new List<Vector2>(leftPath);
                combinedPath.AddRange(rightPath.GetRange(1, rightPath.Count - 1));
                return combinedPath;
            }

            return new List<Vector2> {path[startIndex], path[endIndex]};
        }

        /// <summary>
        /// edge-preserving smoothing filter that combines both spatial and intensity information to smooth images while preserving edges
        /// </summary>
        /// <param name="path"></param>
        /// <param name="windowSize"></param>
        /// <param name="spatialSigma"></param>
        /// <param name="intensitySigma"></param>
        /// <returns></returns>
        public static Vector2[] BilateralFilter(Vector2[] path, int windowSize, float spatialSigma,
            float intensitySigma)
        {
            if (path.Length < 2 || windowSize < 1) return path;

            var newPath = new Vector2[path.Length];

            for (var i = 0; i < path.Length; i++)
                newPath[i] = BilateralFilterToPoint(path, i, windowSize, spatialSigma, intensitySigma);

            return newPath;
        }
        
        /// <summary>
        /// BilateralFilterToPoint
        /// </summary>
        /// <param name="path"></param>
        /// <param name="index"></param>
        /// <param name="windowSize"></param>
        /// <param name="spatialSigma"></param>
        /// <param name="intensitySigma"></param>
        /// <returns></returns>
        private static Vector2 BilateralFilterToPoint(Vector2[] path, int index, int windowSize,
            float spatialSigma, float intensitySigma)
        {
            var halfWindowSize = windowSize / 2;
            var start = Mathf.Max(0, index - halfWindowSize);
            var end = Mathf.Min(path.Length - 1, index + halfWindowSize);

            var filteredPoint = Vector2.zero;
            float normalizationFactor = 0;

            for (var i = start; i <= end; i++)
            {
                var spatialDistance = Vector2.Distance(path[index], path[i]);
                var intensityDistance = ComputeIntensityDistance(path, index, i);

                var spatialWeight = Gaussian(spatialDistance, spatialSigma);
                var intensityWeight = Gaussian(intensityDistance, intensitySigma);

                var weight = spatialWeight * intensityWeight;

                filteredPoint += weight * path[i];
                normalizationFactor += weight;
            }

            return filteredPoint / normalizationFactor;
        }

        /// <summary>
        /// ComputeIntensityDistance
        /// </summary>
        /// <param name="path"></param>
        /// <param name="index1"></param>
        /// <param name="index2"></param>
        /// <returns></returns>
        private static float ComputeIntensityDistance(Vector2[] path, int index1, int index2)
        {
            var angle1 = ComputeAngle(path, index1);
            var angle2 = ComputeAngle(path, index2);

            return Mathf.Abs(angle1 - angle2);
        }

        /// <summary>
        /// ComputeAngle
        /// </summary>
        /// <param name="path"></param>
        /// <param name="index"></param>
        /// <returns></returns>
        private static float ComputeAngle(Vector2[] path, int index)
        {
            if (index <= 0 || index >= path.Length - 1) return 0;

            var prev = path[index - 1];
            var current = path[index];
            var next = path[index + 1];

            var direction1 = (current - prev).normalized;
            var direction2 = (next - current).normalized;

            var angle = Mathf.Acos(Vector2.Dot(direction1, direction2)) * Mathf.Rad2Deg;
            return angle;
        }

        private static float Gaussian(float x, float sigma)
        {
            var a = 1 / (Mathf.Sqrt(2 * Mathf.PI) * sigma);
            var b = -1 * (x * x) / (2 * sigma * sigma);
            return a * Mathf.Exp(b);
        }


        private static float PointToLineSegmentDistance(Vector2 point, Vector2 start, Vector2 end)
        {
            var lengthSquared = (end - start).sqrMagnitude;

            if (lengthSquared == 0) return Vector2.Distance(point, start);

            var t = Mathf.Clamp01(Vector2.Dot(point - start, end - start) / lengthSquared);
            var projection = start + t * (end - start);
            return Vector2.Distance(point, projection);
        }
    }
}