using System.Collections.Generic;
using UnityEngine;

public class PolylineOffset
{
    public static List<Vector2> OffsetPolyline(List<Vector2> input, float offset, float angleThreshold = 1e-6f)
    {
        List<Vector2> preprocessedInput = PreprocessPolyline(input, angleThreshold);
        List<Vector2> offsetPolyline = CalculateOffset(preprocessedInput, offset);
        List<Vector2> postprocessedOutput = PostprocessPolyline(offsetPolyline, angleThreshold);

        return postprocessedOutput;
    }

    private static List<Vector2> PreprocessPolyline(List<Vector2> input, float angleThreshold)
    {
        int n = input.Count;
        List<Vector2> result = new List<Vector2>(n);

        for (int i = 0; i < n; ++i)
        {
            Vector2 prev = input[i == 0 ? n - 1 : i - 1];
            Vector2 current = input[i];
            Vector2 next = input[(i + 1) % n];

            Vector2 edge1 = (current - prev).normalized;
            Vector2 edge2 = (next - current).normalized;

            float dotProduct = Vector2.Dot(edge1, edge2);

            if (Mathf.Abs(dotProduct + 1) > angleThreshold)
            {
                result.Add(current);
            }
        }

        return result;
    }

    private static List<Vector2> CalculateOffset(List<Vector2> input, float offset)
    {
        int n = input.Count;
        List<Vector2> result = new List<Vector2>(n);

        for (int i = 0; i < n; ++i)
        {
            Vector2 prev = input[i == 0 ? n - 1 : i - 1];
            Vector2 current = input[i];
            Vector2 next = input[(i + 1) % n];

            Vector2 edge1 = (current - prev).normalized;
            Vector2 edge2 = (next - current).normalized;

            Vector2 bisector = (edge1 + edge2).normalized;
            float offsetLength = offset / Mathf.Sin(Mathf.Acos(Vector2.Dot(edge1, edge2)) / 2);

            result.Add(current + bisector * offsetLength);
        }

        return result;
    }

    private static List<Vector2> PostprocessPolyline(List<Vector2> input, float angleThreshold)
    {
        int n = input.Count;
        List<Vector2> result = new List<Vector2>(n);

        for (int i = 0; i < n; ++i)
        {
            Vector2 prev = input[i == 0 ? n - 1 : i - 1];
            Vector2 current = input[i];
            Vector2 next = input[(i + 1) % n];

            Vector2 edge1 = (current - prev).normalized;
            Vector2 edge2 = (next - current).normalized;

            float dotProduct = Vector2.Dot(edge1, edge2);

            if (Mathf.Abs(dotProduct + 1) > angleThreshold)
            {
                result.Add(current);
            }
        }

        return result;
    }
}


// using System.Collections.Generic;
// using UnityEngine;
//
// public class PolylineOffset
// {
//     public static List<Vector2> OffsetPolyline(List<Vector2> input, float offset, float angleThreshold = 30f)
//     {
//         int n = input.Count;
//         List<Vector2> output = new List<Vector2>(n);
//         float cosThreshold = Mathf.Cos(angleThreshold * Mathf.Deg2Rad);
//
//         for (int i = 0; i < n - 1; ++i)
//         {
//             Vector2 current = input[i];
//             Vector2 next = input[i + 1];
//
//             Vector2 edge = (next - current).normalized;
//             Vector2 normal = new Vector2(-edge.y, edge.x);
//
//             Vector2 currentOffset = current + normal * offset;
//             Vector2 nextOffset = next + normal * offset;
//
//             if (i == 0)
//             {
//                 output.Add(currentOffset);
//             }
//             else
//             {
//                 Vector2 prev = input[i - 1];
//                 Vector2 prevEdge = (current - prev).normalized;
//                 Vector2 prevNormal = new Vector2(-prevEdge.y, prevEdge.x);
//                 Vector2 prevOffset = current + prevNormal * offset;
//
//                 float cosAngle = Vector2.Dot(prevEdge, edge);
//                 if (cosAngle < cosThreshold)
//                 {
//                     Vector2 intersection;
//                     if (LineIntersection(prevOffset, prevEdge, currentOffset, edge, out intersection))
//                     {
//                         output.Add(intersection);
//                     }
//                 }
//                 else
//                 {
//                     output.Add(prevOffset);
//                     output.Add(currentOffset);
//                 }
//             }
//
//             if (i == n - 2)
//             {
//                 output.Add(nextOffset);
//             }
//         }
//
//         return output;
//     }
//
//     private static bool LineIntersection(Vector2 p1, Vector2 d1, Vector2 p2, Vector2 d2, out Vector2 intersection)
//     {
//         float determinant = d1.x * d2.y - d1.y * d2.x;
//
//         if (Mathf.Abs(determinant) < 1e-6f)
//         {
//             intersection = Vector2.zero;
//             return false; // parallel or coincident
//         }
//
//         float t = ((p2.x - p1.x) * d2.y - (p2.y - p1.y) * d2.x) / determinant;
//         intersection = p1 + d1 * t;
//         return true;
//     }
// }


// using System.Collections.Generic;
// using UnityEngine;
//
// public class PolylineOffset
// {
//     public static List<Vector2> OffsetPolyline(List<Vector2> input, float offset)
//     {
//         int n = input.Count;
//         List<Vector2> output = new List<Vector2>(n);
//
//         for (int i = 0; i < n - 1; ++i)
//         {
//             Vector2 current = input[i];
//             Vector2 next = input[i + 1];
//
//             Vector2 edge = (next - current).normalized;
//             Vector2 normal = new Vector2(-edge.y, edge.x);
//
//             Vector2 currentOffset = current + normal * offset;
//             Vector2 nextOffset = next + normal * offset;
//
//             if (i == 0)
//             {
//                 output.Add(currentOffset);
//             }
//             else
//             {
//                 Vector2 prev = input[i - 1];
//                 Vector2 prevEdge = (current - prev).normalized;
//                 Vector2 prevNormal = new Vector2(-prevEdge.y, prevEdge.x);
//                 Vector2 prevOffset = current + prevNormal * offset;
//
//                 Vector2 intersection;
//                 if (LineIntersection(prevOffset, prevEdge, currentOffset, edge, out intersection))
//                 {
//                     output.Add(intersection);
//                 }
//             }
//
//             if (i == n - 2)
//             {
//                 output.Add(nextOffset);
//             }
//         }
//
//         return output;
//     }
//
//     private static bool LineIntersection(Vector2 p1, Vector2 d1, Vector2 p2, Vector2 d2, out Vector2 intersection)
//     {
//         float determinant = d1.x * d2.y - d1.y * d2.x;
//
//         if (Mathf.Abs(determinant) < 1e-6f)
//         {
//             intersection = Vector2.zero;
//             return false; // parallel or coincident
//         }
//
//         float t = ((p2.x - p1.x) * d2.y - (p2.y - p1.y) * d2.x) / determinant;
//         intersection = p1 + d1 * t;
//         return true;
//     }
// }