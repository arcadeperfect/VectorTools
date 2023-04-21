using System.Collections.Generic;
using ProceduralToolkit;
using UnityEngine;

namespace VectorTools
{
    public class PathUtils
    {
        public static Vector2[] RemoveSelfIntersections(Vector2[] path)
        {
            // Convert the input path into an array of segments
            List<Segment> segments = new List<Segment>();
            for (int i = 0; i < path.Length - 1; i++)
            {
                Segment segment = new Segment(path[i], path[i + 1]);
                segments.Add(segment);
            }

            // Check each segment for intersections with previously processed segments
            for (int i = 0; i < segments.Count; i++)
            {
                Segment currentSegment = segments[i];

                // Check for intersections with previously processed segments
                for (int j = 0; j < i - 1; j++)
                {
                    Segment otherSegment = segments[j];

                    Vector2 intersection;
                    if (Intersect(currentSegment, otherSegment, out intersection))
                    {
                        // Split the current segment into two segments
                        Segment newSegment = new Segment(intersection, currentSegment.b);
                        currentSegment.b = intersection;

                        // Add the new segment to the list of segments without intersections
                        segments.Add(newSegment);

                        // Remove the processed segments from the list
                        segments.RemoveAt(i);
                        segments.RemoveAt(j);

                        // Restart the loop with the new list of segments
                        i = -1;
                        break;
                    }
                }
            }

            // Convert the list of segments without intersections back into an array of points
            List<Vector2> newPath = new List<Vector2>();
            foreach (Segment segment in segments)
            {
                newPath.Add(segment.a);
            }
            newPath.Add(segments[segments.Count - 1].b);

            return newPath.ToArray();
        }
        private struct Segment
        {
            public Vector2 a;
            public Vector2 b;

            public Segment(Vector2 a, Vector2 b)
            {
                this.a = a;
                this.b = b;
            }
        }

        private static bool Intersect(Segment a, Segment b, out Vector2 intersection)
        {
            intersection = Vector2.zero;

            float x1 = a.a.x;
            float y1 = a.a.y;
            float x2 = a.b.x;
            float y2 = a.b.y;
            float x3 = b.a.x;
            float y3 = b.a.y;
            float x4 = b.b.x;
            float y4 = b.b.y;

            float denom = (y4 - y3) * (x2 - x1) - (x4 - x3) * (y2 - y1);
            if (denom == 0f)
            {
                return false;
            }

            float ua = ((x4 - x3) * (y1 - y3) - (y4 - y3) * (x1 - x3)) / denom;
            float ub = ((x2 - x1) * (y1 - y3) - (y2 - y1) * (x1 - x3)) / denom;

            if (ua >= 0f && ua <= 1f && ub >= 0f && ub <= 1f)
            {
                intersection.x = x1 + ua * (x2 - x1);
                intersection.y = y1 + ua * (y2 - y1);
                return true;
            }

            return false;
        }
    }
}