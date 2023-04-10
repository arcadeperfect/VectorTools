using System.Collections.Generic;
using UnityEngine;

public class PolylineOffset2
{
    public static List<Vector2> OffsetPolyline(List<Vector2> input, float offset)
    {
        int n = input.Count;
        List<Vector2> offsetVertices = new List<Vector2>(n);

        for (int i = 0; i < n; ++i)
        {
            if (i == 0)
            {
                Vector2 edge = (input[1] - input[0]).normalized;
                Vector2 normal = new Vector2(-edge.y, edge.x);
                offsetVertices.Add(input[0] + normal * offset);
            }
            else if (i == n - 1)
            {
                Vector2 edge = (input[n - 1] - input[n - 2]).normalized;
                Vector2 normal = new Vector2(-edge.y, edge.x);
                offsetVertices.Add(input[n - 1] + normal * offset);
            }
            else
            {
                Vector2 prev = input[i - 1];
                Vector2 current = input[i];
                Vector2 next = input[i + 1];

                Vector2 edge1 = (current - prev).normalized;
                Vector2 edge2 = (next - current).normalized;

                Vector2 bisector = (edge1 + edge2).normalized;
                float offsetLength = offset / Mathf.Sin(Mathf.Acos(Vector2.Dot(edge1, edge2)) / 2);

                offsetVertices.Add(current + bisector * offsetLength);
            }
        }

        return offsetVertices;
    }
}