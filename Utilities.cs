using System.Collections.Generic;
using System.Runtime.InteropServices.WindowsRuntime;
using ProceduralToolkit;
using UnityEngine;
using VectorTerrain.Scripts.Utils;

namespace VectorTools
{
    public static class Utilities
    {
        // return a vector2 that reprsesents a point along a path according to value t, in absolute units
        public static Vector2 PointAlongPathAbsolute(Vector2[] input, float t)
        {
            float totalLength = TotalLength(input);

            float targetLength = t;
            float currentLength = 0;
            for (int i = 0; i < input.Length - 1; i++)
            {
                float segmentLength = Vector2.Distance(input[i], input[i + 1]);
                if (currentLength + segmentLength > targetLength)
                {
                    float t2 = (targetLength - currentLength) / segmentLength;
                    return Vector2.Lerp(input[i], input[i + 1], t2);
                }

                currentLength += segmentLength;
            }

            return input[input.Length - 1];
        }
        
        public static Vector2 PointAlongPathAbsolute(List<Vector2> input, float t)
        {
            return PointAlongPathAbsolute(input.ToArray(), t);
        }
        
        // return a vector2 that represents a point along a path according to normalised value t
        public static Vector2 PointAlongPathNormalised(Vector2[] input, float t)
        {
            float totalLength = TotalLength(input);

            float targetLength = totalLength * t;
            float currentLength = 0;
            for (int i = 0; i < input.Length - 1; i++)
            {
                float segmentLength = Vector2.Distance(input[i], input[i + 1]);
                if (currentLength + segmentLength > targetLength)
                {
                    float t2 = (targetLength - currentLength) / segmentLength;
                    return Vector2.Lerp(input[i], input[i + 1], t2);
                }

                currentLength += segmentLength;
            }

            return input[input.Length - 1];
        }
        // return a vector2[] that represents a point along a path according to normalised value t, and a vector representing the normal at that point
        public static (Vector2, Vector2) PointAndNormalAlongPathNormalised(Vector2[] input, float t)
        {
            float totalLength = TotalLength(input);

            float targetLength = totalLength * t;
            float currentLength = 0;
            for (int i = 0; i < input.Length - 1; i++)
            {
                float segmentLength = Vector2.Distance(input[i], input[i + 1]);
                if (currentLength + segmentLength > targetLength)
                {
                    float t2 = (targetLength - currentLength) / segmentLength;
                    Vector2 point = Vector2.Lerp(input[i], input[i + 1], t2);
                    Vector2 normal = (input[i + 1] - input[i]).RotateCW90().normalized;
                    return (point, normal);
                }

                currentLength += segmentLength;
            }

            return (input[input.Length - 1], (input[input.Length - 1] - input[input.Length - 2]).RotateNormalized(90).normalized);
        }

        public static float TotalLength(Vector2[] path)
        {
            float totalLength = 0;
            for (int i = 0; i < path.Length - 1; i++)
            {
                totalLength += Vector2.Distance(path[i], path[i + 1]);
            }

            return totalLength;
        }
        public static float TValueOfPointAtIndex(Vector2[] input, int index)
        {
            float totalLength = TotalLength(input);
            float currentLength = 0;
            for (int i = 0; i < index; i++) currentLength += Vector2.Distance(input[i], input[i + 1]);

            return currentLength / totalLength;
        }
        public static float TvalueOfPointAtIndex(Vector2[] input, int index)
        {
            float totalLength = TotalLength(input);
            float currentLength = 0;
            for (int i = 0; i < index; i++) currentLength += Vector2.Distance(input[i], input[i + 1]);

            return currentLength / totalLength;
        }
        public static int IndexOfPointAtT(Vector2[] input, float t, RoundingMode roundingMode = RoundingMode.Closest)
        {
            if (t == 0) return 0;
            if (t == 1) return input.Length - 1;

            int foundIndex = 0;
            float minDifference = float.MaxValue;

            for (int i = 0; i < input.Length; i++)
            {
                float d = TValueOfPointAtIndex(input, i);
                float difference = Mathf.Abs(t - d);

                if (roundingMode == RoundingMode.Closest)
                {
                    if (difference > minDifference)break;
                }
                else if (roundingMode == RoundingMode.RoundUp && d >= t)
                {
                    foundIndex = i;
                    break;
                }
                else if (roundingMode == RoundingMode.RoundDown && d > t)
                {
                    foundIndex = i - 1;
                    break;
                }
                minDifference = difference;
                foundIndex = i;
            }

            return foundIndex;
        }
        public static Vector2[] PointAtT(Vector2[] input, float t, RoundingMode roundingMode = RoundingMode.Closest)
        {
            return new[] {input[IndexOfPointAtT(input, t, roundingMode)]};
        }
        public static Vector2[] PointsBeyondT(Vector2[] input, float t)
        {
            List<Vector2> output = new List<Vector2>();

            int c = 0;
            foreach (var vector2 in input)
            {
                if (Utilities.TValueOfPointAtIndex(input, c) > t) output.Add(vector2);
                c++;
            }
            return output.ToArray();
        }
        // rotate path around a pivot by an angle
        public static Vector2[] RotatePath(Vector2[] input, Vector2 pivot, float angle)
        {
            Vector2[] output = new Vector2[input.Length];
            for (int i = 0; i < input.Length; i++)
            {
                output[i] = RotatePointAround(input[i], pivot, angle);
            }

            return output;
        }
        public static Vector2 RotatePointAround(Vector2 point, Vector2 pivot, float degrees)
        {
            float angleInRadians = degrees * Mathf.Deg2Rad;
            Vector2 translatedPoint = point - pivot;
            float x = translatedPoint.x * Mathf.Cos(angleInRadians) - translatedPoint.y * Mathf.Sin(angleInRadians);
            float y = translatedPoint.x * Mathf.Sin(angleInRadians) + translatedPoint.y * Mathf.Cos(angleInRadians);
            Vector2 rotatedPoint = new Vector2(x, y) + pivot;

            return rotatedPoint;
        }
        public enum RoundingMode
        {
            RoundUp,
            RoundDown,
            Closest
        }
    }
}