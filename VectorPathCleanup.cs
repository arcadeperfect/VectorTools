using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace VectorTools
{
    public class VectorPathCleanup
    {
        public static Vector2[] RemoveCollinearPoints(Vector2[] path, float tolerance = 1e-6f)
        {
            if (path == null || path.Length < 3)
            {
                return path;
            }

            List<Vector2> newPath = new List<Vector2>();

            newPath.Add(path[0]);

            for (int i = 1; i < path.Length - 1; i++)
            {
                Vector2 prev = path[i - 1];
                Vector2 current = path[i];
                Vector2 next = path[i + 1];

                Vector2 v1 = (current - prev).normalized;
                Vector2 v2 = (next - current).normalized;

                float crossProduct = Vector2Extensions.Cross(v1, v2);

                if (Mathf.Abs(crossProduct) > tolerance)
                {
                    newPath.Add(path[i]);
                }
            }

            newPath.Add(path[path.Length - 1]);

            return newPath.ToArray();
        }
    
    
        public static Vector2[] RemoveCloseVertices(Vector2[] path, float tolerance)
        {
            List<Vector2> newPath = new List<Vector2>();

            newPath.Add(path[0]);

            for (int i = 1; i < path.Length - 1; i++)
            {
                Vector2 prev = newPath[newPath.Count - 1];
                Vector2 current = path[i];
                Vector2 next = path[i + 1];

                bool tooCloseToNeighbors = Vector2.Distance(prev, current) < tolerance || Vector2.Distance(current, next) < tolerance;
                bool tooCloseToSegment = false;

                for (int j = 0; j < newPath.Count - 1; j++)
                {
                    float distance = DistancePointToSegment(current, newPath[j], newPath[j + 1]);

                    if (distance < tolerance)
                    {
                        tooCloseToSegment = true;
                        break;
                    }
                }

                if (!tooCloseToNeighbors && !tooCloseToSegment)
                {
                    newPath.Add(current);
                }
            }

            newPath.Add(path[path.Length - 1]);

            return newPath.ToArray();
        }



        public static Vector2[] RemoveSelfIntersections(Vector2[] path)
        {
            List<Vector2> newPath = new List<Vector2>();

            for (int i = 0; i < path.Length - 1; i++)
            {
                Vector2 intersection;
                bool hasIntersection = false;

                for (int j = i + 2; j < path.Length - 1; j++)
                {
                    if (FindIntersection((path[i], path[i + 1]), (path[j], path[j + 1]), out intersection))
                    {
                        newPath.Add(intersection);
                        i = j - 1;
                        hasIntersection = true;
                        break;
                    }
                }

                if (!hasIntersection)
                {
                    newPath.Add(path[i]);
                }
            }

            newPath.Add(path[path.Length - 1]);

            return newPath.ToArray();
        }
    
    

        private static float DistancePointToSegment(Vector2 point, Vector2 start, Vector2 end)
        {
            Vector2 line = end - start;
            Vector2 pointToStart = point - start;

            float lineLengthSquared = line.sqrMagnitude;
            float dotProduct = Vector2.Dot(pointToStart, line);

            float t = Mathf.Clamp01(dotProduct / lineLengthSquared);

            Vector2 projection = start + t * line;
            return Vector2.Distance(point, projection);
        }

        private static bool FindIntersection((Vector2, Vector2) seg1, (Vector2, Vector2) seg2, out Vector2 intersection)
        {
            Vector2 p = seg1.Item1;
            Vector2 r = seg1.Item2 - seg1.Item1;
            Vector2 q = seg2.Item1;
            Vector2 s = seg2.Item2 - seg2.Item1;

            float rCrossS = Vector2Extensions.Cross(r, s);
            if (Mathf.Approximately(rCrossS, 0f))
            {
                intersection = Vector2.zero;
                return false;
            }

            float t = Vector2Extensions.Cross((q - p), s) / rCrossS;
            float u = Vector2Extensions.Cross((q - p), r) / rCrossS;

            if (0f <= t && t <= 1f && 0f <= u && u <= 1f)
            {
                intersection = p + t * r;
                return true;
            }

            intersection = Vector2.zero;
            return false;
        }
    
    

    }

    public static class Vector2Extensions
    {
        public static float Cross(this Vector2 a, Vector2 b)
        {
            return a.x * b.y - a.y * b.x;
        }
    }
}